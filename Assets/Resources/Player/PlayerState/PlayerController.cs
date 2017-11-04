/*=================================================
//								FILE : PlayerController.cs
//		ファイル説明 :
//		キャラクターの動作を記述するスクリプト
//
//							HAL大阪 : 松本 雄之介
=================================================*/
using PlayerState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの動きを取り扱うクラス
/// </summary>
public class PlayerController : MonoBehaviour {

	//======================== 変数 ===============================

	// プレイヤーのステートを管理するマネージャ
	PlayerStateManager AliveState = new PlayerStateManager();               //プレイヤーが生存しているかどうかで処理を分岐する
	PlayerStateManager SideMoveState = new PlayerStateManager();      //プレイヤーの現状態を管理するマネージャ
	PlayerStateManager JumpState = new PlayerStateManager();      //プレイヤーの現状態を管理するマネージャ
	JumpState m_JumpState = new JumpState();
	OnGroundState m_OnGround = new OnGroundState();
	WaitState m_WaitState = new WaitState();
	WalkState m_WalkState = new WalkState();
	RunState m_RunState = new RunState();
	IvyState m_IvyState = new IvyState();
	SurvivalState m_Survival = new SurvivalState();

	// 各種移動パラメータ
	public float MaxWalkSpeed = 0.3f;               //歩く最大速度
	public float MaxRunSpeed = 0.5f;                //走る最大速度
	public float fallMaxSpeed = 0.5f;               //落ちる最大速度
	public float accelerationTime = 3.0f;       //最高速度に達するまでの時間
												// TODO 単位秒数にしてプランナーに分かりやすいように施す
	public float DecelerationWalk = 0.8f;       //歩いている時に減速する割合
	public float DecelerationRun = 0.5f;            //走っている時に減速する割合
	public float JumpMinSpeed = 1.0f;              //ジャンプの最初にかかるY方向初速の最低値(ここに最大入力時間の間、ジャンプ加速がかかり続ける)
	public float JunpAccele = 1.1f;                 //ジャンプ中にボタンを押し続けたときにかかる加速度(1.0以上を指定すること。じゃないと逆に減速する)
	public float Gravity = 9.8f;                        //重力値
	public float MaxJumpButtonTime;                //ジャンプボタンの入力を受け付ける最大時間
												   // TODO 時間を取得してジャンプ中の時間を調整できるようにする
	public float HitWallDecelera = 0.5f;            //壁に激突したときに減速値とともに速度にかけられる(1.0fで減速なし)
	public float HitCeilingDecelera = 0.5f;         //天井に激突したときにジャンプ速度にかけられる(1.0fで減速なし)

	float EnemyStepJump = 8.0f;         //敵を踏んだ時にかかるジャンプ補間

	public Vector3 velocity = Vector3.zero;
	public float m_JumpVelocity = 0.0f;
	Vector3 MoveFloorVec = Vector3.zero;
	bool EnemyJumpFlag = false;

	// ツタを利用する時のパラメータ


	CharacterController characterController;
	IvyManager ivyManager;                       //キャラがやられた時の復帰処理を管理するマネージャ
	AnimationChange animeChange;                //アニメーションの切り替えを管理
	public InputManager inputManager;       //入力マネージャ
	GameObject PlayerObj;       //自分自身のオブジェクト
	GameObject ChildObj;            //子のオブジェクト
	public int PlayerNo = 0;                //コントローラーで取得する番号	
											// TODO このプレイヤー番号はa版のみで今後、クライアントが別れれば不要

	float m_JumpTimeCnt = 0.0f; //ジャンプ時間を計測する
	GameManager gameManager;

	// Photon追加
	private PhotonView photonView = null;
	private Vector3 receiveVelocity = Vector3.zero;

	//サウンドマネージャー
	private SoundManager soundManager;


	/// <summary>
	/// プレイヤーのタイプ
	/// </summary>
	public enum Type {
		Eye = 0,
		Ear,
		Nose
	}

	/// <summary>
	/// プレイヤーが持っている当たり判定に利用するカプセルの情報
	/// </summary>
	struct FoxCapsule {
		public Vector3 p1, p2;     //始点、終点
		public float radius;        //半径
	}

	FoxCapsule CapsuleData;

	public Type m_Type;
	bool operationFlag = false; //操作してるキャラ？

	//========================================================================================================↓
	// ADD
	PlayerDeathJump DeathJumpData = new PlayerDeathJump();      //死んだときのジャンプを管理してくれるクラス
	ToBeKillState m_KillState = new ToBeKillState();
	ImmobileState m_NotMove = new ImmobileState();          //キャラを移動不可にする

	StateBase bufState = null;      //以前のステートを保持するバッファ
	ParticleSystem particle = null;     // ADD
										//========================================================================================================↑

	//======================== 関数 ===============================
	// Use this for initialization
	void Start() {
		ivyManager = FindObjectOfType<IvyManager>();
		ChildObj = transform.Find("FOX").gameObject;
		animeChange = ChildObj.GetComponent<AnimationChange>();
		PlayerObj = gameObject;
		gameManager = FindObjectOfType<GameManager>();      //ゲームオブジェクトを取得する
		var no = gameManager.localPlayer.Get();
		if ((int)no == (int)m_Type) operationFlag = true;       //操作してるキャラなら操作キャラフラグを立てる
		operationFlag = true;
		m_KillState.execDelegate = ToBeKillState;       // ADD
		m_NotMove.execDelegate = ImmobileState;         // ADD

		//ここでは基本的に各状態クラスのデリゲートに関数を登録していく
		AliveState.SetState(m_Survival);
		//生存時処理
		m_Survival.execDelegate = SurvivalExe;
		// 復帰処理状態
		m_IvyState.execDelegate = IvyExe;

		SideMoveState.SetState(m_WaitState);
		//待機状態の処理
		m_WaitState.execDelegate = WaitExe;
		//歩き状態の処理
		m_WalkState.execDelegate = WalkExe;
		//走り状態の処理
		m_RunState.execDelegate = RunExe;

		JumpState.SetState(m_OnGround);
		//着地状態
		m_OnGround.execDelegate = OnGroundExe;
		//ジャンプ状態
		m_JumpState.execDelegate = JumpExe;


		//自身のキャラクターコントローラを取得しておく
		characterController = GetComponent<CharacterController>();

		// Photon追加
		// インプットマネージャ取得

		//Transform t = GameObject.Find("GameManager").transform;
		//Transform t2 = GameObject.Find("GameManager").transform.Find("InputManager");
		//var t3 = GameObject.Find("GameManager").transform.Find("InputManager").GetComponent<InputManager>();

		inputManager = GameObject.Find("GameCEO/InputManager").GetComponent<InputManager>();
		photonView = GetComponent<PhotonView>();
		soundManager = FindObjectOfType<SoundManager>();

		// ADD
		particle = ChildObj.GetComponent<ParticleSystem>();
		particle.Stop();
	}

	// Update is called once per frame
	void Update() {
		if (!photonView.isMine) {
			characterController.Move(new Vector3(0.0f, -0.01f, 0.0f));

			//速度によって左右を向かせる
			if (receiveVelocity.x > 0.0f)
				ChildObj.transform.rotation = Quaternion.Euler(0, 140, 0);
			else if (receiveVelocity.x < 0.0f)
				ChildObj.transform.rotation = Quaternion.Euler(0, -140, 0);
			return;
		}

		particle.Stop();
		transform.tag = "Untagged";     // ADD
		SideMoveState.StateUpdate();
		JumpState.StateUpdate();
		AliveState.StateUpdate();
		AliveState.Execute();
	}

	Vector3 CharacterMove(Vector3 nowSpeed, Vector3 vect, float MaxSpeed) {
		Vector3 vec = vect * MaxSpeed;      //ベクトル方向の最大長	

		//補間処理
		return Vector3.Lerp(nowSpeed, vec, Mathf.Min(Time.deltaTime * accelerationTime, 1.0f));
	}

	void Collision() {
		//当たった方向を判定して
		CollisionFlags flag = characterController.collisionFlags;
		//横方向に当たった
		if ((flag & CollisionFlags.Sides) != 0) {
			velocity.x *= DecelerationRun * HitWallDecelera;
		}
		//上方向に当たった
		if ((flag & CollisionFlags.Above) != 0) {
			velocity.y *= HitCeilingDecelera;
		}
	}

	//====================== 状態別処理 =============================

	//ここにはキャラクターが各状態の場合のそれぞれの処理を書く

	void SurvivalExe() {
		//死亡したらAliveStateをm_IvyStateに変更する
		transform.tag = "Player";       // ADD

		//===移動ベクトルを求める処理===
		SideMoveState.Execute();
		JumpState.Execute();

		//=======================

		Collision();

		//ベクトルを元にして実際に移動させる
		IsToPress();        //地面に押し付ける処理
		if (m_JumpVelocity != 0)
			velocity.y = m_JumpVelocity;        //ジャンプしてたらジャンプしてね！
		characterController.Move((velocity * Time.deltaTime) + MoveFloorVec);       //移動床のベクトルも足しこんでいる。移動床のベクトルはデルタタイム済み
		MoveFloorVec = Vector3.zero;        //とりあえず移動床のベクトルは毎度ゼロに


		//速度によって左右を向かせる
		if (velocity.x > 0.0f) {
			ChildObj.transform.rotation = Quaternion.Euler(0, 140, 0);
		}
		else if (velocity.x < 0.0f) {
			ChildObj.transform.rotation = Quaternion.Euler(0, -140, 0);
		}

		if (transform.position.y < -20.0f) {
			soundManager.PlaySE("Rakka");   //サウンド　落下したときの音
			SetDeathData();
		}

		// ADD こいつらをUpdateから持ってきた
		//情報更新系
		UpdateCapsule();        //当たり判定を取るためのカプセル情報の更新

		HitObjectCheck();       //当たり判定を行う
	}

	void WaitExe() {
		animeChange.SetAnime(AnimationChange.AnimeState.WAIT);
		if (inputManager.GetButtonPress(InputManager.KeyData.RIGHT) || inputManager.GetButtonPress(InputManager.KeyData.LEFT)) {
			SideMoveState.ChangeState(m_WalkState, ref PlayerObj);
		}
	}

	void WalkExe() {
		animeChange.SetAnime(AnimationChange.AnimeState.WALK);
		//右方向処理
		if (inputManager.GetButtonPress(InputManager.KeyData.RIGHT)) {
			velocity = CharacterMove(velocity, Vector3.right, MaxWalkSpeed);
		}
		//左方向処理
		else if (inputManager.GetButtonPress(InputManager.KeyData.LEFT)) {
			velocity = CharacterMove(velocity, Vector3.left, MaxWalkSpeed);
		}
		else {
			velocity *= DecelerationWalk;
		}

		//速度が落ちたら待機
		if (Mathf.Abs(velocity.x) < 0.001f) {
			velocity = Vector3.zero;
			SideMoveState.ChangeState(m_WaitState, ref PlayerObj);
			return;
		}

		if (inputManager.GetButtonPress(InputManager.KeyData.X))
			SideMoveState.ChangeState(m_RunState, ref PlayerObj);
	}

	void RunExe() {
		particle.Play();
		animeChange.SetAnime(AnimationChange.AnimeState.HASIRU);
		//右方向処理
		if (inputManager.GetButtonPress(InputManager.KeyData.RIGHT)) {
			velocity = CharacterMove(velocity, Vector3.right, MaxRunSpeed);
		}
		//左方向処理
		else if (inputManager.GetButtonPress(InputManager.KeyData.LEFT)) {
			velocity = CharacterMove(velocity, Vector3.left, MaxRunSpeed);
		}
		else {
			velocity *= DecelerationRun;
		}

		//速度が落ちたら待機
		if (Mathf.Abs(velocity.x) < 0.001f) {
			velocity = Vector3.zero;
			SideMoveState.ChangeState(m_WaitState, ref PlayerObj);
			return;
		}

		if (!inputManager.GetButtonPress(InputManager.KeyData.X))
			SideMoveState.ChangeState(m_WalkState, ref PlayerObj);
	}

	void OnGroundExe() {
		//地上にいる間の処理
		if (inputManager.GetButtonTrigger(InputManager.KeyData.A)) {
			// ==============================================================================================
			if (operationFlag)
				inputManager.SetRightVibration(10.0f);
			// ==============================================================================================
			m_JumpVelocity = JumpMinSpeed;
			JumpState.ChangeState(m_JumpState, ref PlayerObj);
			particle.Play();    //一瞬パーティクルを出す
		}

		//もし地上じゃなくなっても空中処理になる
		if (!characterController.isGrounded) {
			JumpState.ChangeState(m_JumpState, ref PlayerObj);
		}
	}

	void JumpExe() {
		particle.Stop();        //ジャンプ中はキャラの走ったときのパーティクルを強制OFFに
								//まずは追加のジャンプボタンを受け付けるか判定を行い、処理する
		float delta = Time.deltaTime;
		if (inputManager.GetButtonTrigger(InputManager.KeyData.A)) {
			m_JumpTimeCnt += delta;
			//まだジャンプボタンを受け付けられる
			if (m_JumpTimeCnt < MaxJumpButtonTime) {
				//m_JumpVelocity += JunpAccele * delta;       //加速処理
			}
		}
		else {
			//ジャンプボタンが離されたらもう追加のジャンプはしない
			m_JumpTimeCnt = MaxJumpButtonTime + 1;  //これでm_JumpTimeCntがMaxJumpButtonTimeを下回ることはない
		}

		m_JumpVelocity -= Gravity * Time.deltaTime;

		//アニメーションの変更
		if (m_JumpVelocity > 0.0f) {
			animeChange.SetAnime(AnimationChange.AnimeState.JUMP);
		}
		else {
			animeChange.SetAnime(AnimationChange.AnimeState.OTIHAJIME);
			EnemyJumpFlag = false;      //敵を踏めるフラグ
		}

		//もし地面についたら
		if (characterController.isGrounded && !EnemyJumpFlag) {
			m_JumpVelocity = 0.0f;  //上下移動を0にする
			JumpEntry();
			animeChange.SetAnime(AnimationChange.AnimeState.TYAKUTI);
			JumpState.ChangeState(m_OnGround, ref PlayerObj);
		}
	}

	public void JumpEntry() {
		//ジャンプ処理に入るときの初期化処理
		m_JumpTimeCnt = 0.0f;
	}

	/// <summary>
	/// 死亡してからの復帰処理を行う
	/// </summary>
	public void IvyExe() {
		//まずはキャラが一度画面を少し駆け上がってそのまま落ちていく
		// TODO 現在、死ぬといきなり上からツタが下りてくるようになってるけど将来的には一度ダメージ演出を入れたい
		//=====処理順=====
		//まずはツタが伸びるのを待つ必要があるのでツタが伸び終わったかどうかを判定するために
		//bool値をツタマネージャから取得して判定する
		//ツタが伸び切っていたらキャラクターが移動可能かどうかの判定をbool値でもらう.
		//キャラ移動可がついている間はキャラをマネージャの移動速度を参照して移動させ、
		//移動後にツタマネージャーに値を渡しておく。そのときにツタマネージャーは
		//これ以降も移動させるかどうかの判定を行っておく。
		//最後にジャンプ可能かどうかの判定を見て、可能状態であればジャンプボタンが押されたタイミングで
		//上方向へ力を与えて遷移をジャンプに移す。
		//この状態でジャンプ可能が解除されれば指定時間経過とみなし、力を与えずに
		//ジャンプ状態へ状態を戻す
		//復帰するときは生存フラグを返してキャラ側の処理は終了
		//===============

		//ツタが伸び切っているか？
		Vector3 IvyPos = ivyManager.GetIvyPos();
		IvyPos.x += 0.6f;       //キャラがツタをつかんでいるように見せる
		ChildObj.transform.rotation = Quaternion.Euler(0, 180, 0);      //正面を向かせる処理
		IvyPos.z = transform.position.z;
		animeChange.SetAnime(AnimationChange.AnimeState.BURABURA);
		float offX = IvyPos.x - transform.position.x;
		if (ivyManager.GetIvyMaxLenFlag()) {
			//ツタ中にキャラがなにかにぶつかった=================================================================================================================↓
			// ADD
			CollisionFlags flag = characterController.collisionFlags;
			if (flag != CollisionFlags.None) {
				AliveState.ChangeState(m_Survival, ref PlayerObj);
				m_JumpVelocity = 0.0f;
				JumpState.ChangeState(m_JumpState, ref PlayerObj);
				ivyManager.CharacterHasReturned(m_Type, true);
			}
			//==========================================================================================================================================↑
			//まだキャラ移動してる？
			if (ivyManager.GetReturnMoveFlag(m_Type)) {
				//キャラクターが現在移動中
				characterController.Move(new Vector3(offX, -ivyManager.PlayerDownSpeed * Time.deltaTime, 0.0f));
				ivyManager.SetCharaVector3(m_Type, transform.position);     //マネージャに座標を保持させる
			}
			else {
				characterController.Move(new Vector3(offX, 0.0f, 0.0f));
				//復帰できる状態になり、その状態でジャンプボタンが押された
				if (ivyManager.GetBuraBuraTimeExit(m_Type)) {       //ここの判定をまだジャンプ出来る時間なら、にする
					if (inputManager.GetButtonTrigger(InputManager.KeyData.A)) {
						if (operationFlag)
							inputManager.SetRightVibration(10.0f);
						AliveState.ChangeState(m_Survival, ref PlayerObj);
						m_JumpVelocity = JumpMinSpeed;
						JumpState.ChangeState(m_JumpState, ref PlayerObj);
						ivyManager.CharacterHasReturned(m_Type, true);
					}
				}
				else {
					//ジャンプ可能時間が来てしまった
					AliveState.ChangeState(m_Survival, ref PlayerObj);
					m_JumpVelocity = 0.0f;
					JumpState.ChangeState(m_JumpState, ref PlayerObj);
					ivyManager.CharacterHasReturned(m_Type, true);
				}
			}
		}
		else {
			//まだツタが伸びきっていない場合は画面上部で待っていただく
			IvyPos.y = ivyManager.CharaWaitHeight;
			transform.position = IvyPos;
		}

		//キャラクターが移動出来るか？


		//ジャンプ可能かどうか？
		//AliveState.ChangeState(m_Survival, ref PlayerObj);
	}

	/// <summary>
	/// 死んだときに情報をまとめて変更する。これを呼べばツタが出てきてキャラの移動を始める
	/// </summary>
	public void SetDeathData() {
		// ====================================================================================================================================
		// ADD
		//キャラのやられた時のへ遷移を行ってそっちでキャラの移動が終わったらこいつを呼ぶ
		AliveState.ChangeState(m_KillState, ref PlayerObj);
		DeathJumpData.Start(transform.position);
		animeChange.SetAnime(AnimationChange.AnimeState.DAMAGE);
		if (operationFlag)
			inputManager.SetLeftVibration(100.0f);
		//=====================================================================================================================================
	}

	[PunRPC]
	void SetDeathDataRPC() {
		//死んだ情報をツタマネージャに伝える
		ivyManager.SetDeathFlag(m_Type, true);
		AliveState.ChangeState(m_IvyState, ref PlayerObj);
		transform.position = ivyManager.GetCharacterInitPos(m_Type);
		ivyManager.SetCharaVector3(m_Type, transform.position);     //マネージャに座標を保持させる

		// 1機減らす
		GameObject.Find("GameCEO/Item").GetComponent<StockManager>().Add(-1, this.gameObject.GetInstanceID());
	}


	//======================== 当たり判定系===============================

	/// <summary>
	/// 左方向から当たっているオブジェクトのレイヤー名を取得する
	/// </summary>
	/// <returns></returns>
	string GetLeftHitObjectLayerName() {


		return LayerMask.LayerToName(2);
	}

	/// <summary>
	/// 右方向から当たっているオブジェクトのレイヤー名を取得する
	/// </summary>
	/// <returns></returns>
	//string GetRightHitObjectLayerName() {

	//}

	////当たり判定
	//void OnControllerColliderHit(ControllerColliderHit hit) {
	//	//当たり判定を種類別にとっていく

	//}

	GameObject GetHitCapsuleObj(Vector3 vec, float Distance, int layerMask = Physics.DefaultRaycastLayers) {
		RaycastHit hit;

		if (Physics.CapsuleCast(CapsuleData.p1, CapsuleData.p2, CapsuleData.radius, vec, out hit, Distance, layerMask)) {
			return hit.collider.gameObject;     //当たったオブジェクトを返す
		}
		return null;        //何も当たってなかったらnull
	}

	void UpdateCapsule() {
		//必要なデータを計算して取得していく
		//半径
		//始点、終点
		float r = characterController.radius;                   //半径を取得
		float height = characterController.height;          //高さ情報
		Vector3 center = characterController.center;        //中心座標
		Vector3 ObjPos = transform.position;                    //オブジェクトの位置

		//中心座標からカプセルの下限位置と上限位置を求める
		Vector3 UpSide = center;
		Vector3 DownSide = center;
		UpSide.y += height / 2.0f - r;
		DownSide.y -= height / 2.0f + r;

		//ローカル座標をワールド座標へ変換
		UpSide += ObjPos;
		DownSide += ObjPos;

		//求めた座標を保持する
		CapsuleData.radius = r;
		CapsuleData.p1 = UpSide;
		CapsuleData.p2 = DownSide;
	}

	/// <summary>
	/// キャラクターが壁に挟まれたかを判定する関数
	/// </summary>
	/// <returns>true : 当たった</returns>
	bool IsWallSandwiched() {
		//判定するレイヤー番号をする
		int Mask = 0;
		Mask += 1 << 14;        //カメラ外にある強制スクロール壁との当たり判定
		Mask += 1 << 12;            //ステージのオブジェクトとの当たり判定

		float Len = 0.2f;       //挟まれた判定になる距離

		//当たった方向を判定して
		CollisionFlags flag = characterController.collisionFlags;

		//まず右方向の当たり判定を取る
		if (GetHitCapsuleObj(Vector3.right, Len, Mask)) {
			if (GetHitCapsuleObj(Vector3.left, Len, Mask)) {
				soundManager.PlaySE("Damage");  //サウンド　挟まれたときの音
				return true;
			}
		}

		//上下方向の判定を行う
		if (GetHitCapsuleObj(Vector3.up, Len * 2.0f, Mask)) {
			//下方向当たってる
			if ((flag & CollisionFlags.Below) != 0) {
				soundManager.PlaySE("Damage");  //サウンド　挟まれたときの音
				return true;
			}
		}

		return false;
	}       // end bool IsWallSandwiched()

	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if (stream.isWriting) {
			//データの送信
			stream.SendNext(this.velocity);                 // 移動速度
			stream.SendNext(this.transform.position);
			stream.SendNext(animeChange.GetAnimeState());
		}
		else {
			// データの受信
			receiveVelocity = (Vector3)stream.ReceiveNext();
			this.transform.position = (Vector3)stream.ReceiveNext();
			animeChange.SetAnime((AnimationChange.AnimeState)stream.ReceiveNext());
		}
	}

	/// <summary>
	/// ステージ中のオブジェクトとの判定を行う
	/// </summary>
	void HitObjectCheck() {

		//キャラがブロックに挟まれていないかをチェックする
		if (IsWallSandwiched()) {
			SetDeathData();
		}
	}

	GameObject GetHitRayObj(Vector3 origin, Vector3 vec, float Distance, int layerMask = Physics.DefaultRaycastLayers) {
		RaycastHit hit;

		if (Physics.Raycast(origin, vec, out hit, Distance, layerMask)) {
			return hit.collider.gameObject;     //当たったオブジェクトを返す
		}
		return null;        //何も当たってなかったらnull
	}

	/// <summary>
	/// 下方向に押し付ける処理
	/// </summary>
	void IsToPress() {
		velocity.y = -0.5f;        //ジャンプしてなかったら下方向に押し付ける。Unityの特性上、これがないとバグる
								   //まず地面と接触していた場合のみ処理
		if (characterController.isGrounded) {
			int Mask = 0;
			Mask += 1 << 14;        //カメラ外にある強制スクロール壁との当たり判定
			Mask += 1 << 12;            //ステージのオブジェクトとの当たり判定
										//キャラ中央からレイを下に飛ばして当たれば強く押し付ける
			Vector3 origin = transform.position + Vector3.up;       //レイを飛ばす起点を少し上にあげておく
			GameObject hitObj = GetHitRayObj(origin, Vector3.down, 2.0f, Mask); //当たったオブジェクトを取得する
			if (hitObj != null) {
				velocity.y = -100.0f;        //強く押し付ける
			}
		}
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		//ここで移動床の設定
		if (hit.gameObject.tag == "MoveBlock" || hit.gameObject.tag == "InvisibleMoveBlock") {
			//移動床がキャラより下にあれば
			if (transform.position.y > hit.gameObject.transform.position.y) {
				//親の実態を取得する
				GameObject pare = hit.gameObject.transform.parent.gameObject;
				MoveFloorVec = pare.GetComponent<Move>().GetPositionDelta();
			}
		}

		//ここでは敵の踏み判定
		if (!EnemyJumpFlag) {
			if (hit.gameObject.layer == LayerMask.NameToLayer("EnemyMove")) {
				//敵が自分より下にいればそれって…踏んでるやん？
				if (transform.position.y > hit.gameObject.transform.position.y) {

					Debug.Log(3);
					//一度、ジャンプ状態を初期化する
					m_JumpVelocity = 0.0f;  //上下移動を0にする
					JumpEntry();
					m_JumpVelocity = EnemyStepJump;
					JumpState.ChangeState(m_JumpState, ref PlayerObj);
					JumpState.StateUpdate();
					JumpState.Execute();
					EnemyJumpFlag = true;       //敵を踏んだ！

					soundManager.PlaySE("Hunnda");  //サウンド　敵を踏んだ時の音
													// TODO ここで敵を殺す処理をする
				}
			}
		}
	}




	//==============================================================================================↓
	void ToBeKillState() {
		//キャラが死ぬモーションをしている時に呼ばれる
		characterController.enabled = false;
		ChildObj.transform.rotation = Quaternion.Euler(0, 180, 0);      //正面を向かせる処理
		transform.position = DeathJumpData.UpdatePosition();
		if (transform.position.y < -5.0f) {
			characterController.enabled = true;
			photonView.RPC("SetDeathDataRPC", PhotonTargets.All);
		}
	}

	void ImmobileState() {
		//ここに動かない際のステートをおいておく
	}

	// この↓、プレイヤーの動きを制御する
	public void SetImmobile() {
		AliveState.SetState(m_NotMove);
		GetComponent<PlayerUI>().SetCanNotInput();
	}

	public void SetMoveState() {
		AliveState.SetState(m_Survival);
		GetComponent<PlayerUI>().SetCanInput();
	}
	//==============================================================================================↑
}