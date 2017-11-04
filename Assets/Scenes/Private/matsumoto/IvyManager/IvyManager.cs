/*=================================================
//								FILE : IvyManager.cs
//		ファイル説明 :
//		ツタの情報を管理し、取り扱うクラス
//
//							HAL大阪 : 松本 雄之介
=================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ツタの管理を行うマネージャ
/// </summary>
public class IvyManager : MonoBehaviour {

	//各種パラメータ
	public Vector3 IvyPos = Vector3.up;         //ツタを表示させる位置。カメラからもらう
	public float CharaWaitHeight = 10.0f;           //キャラクターが空中で待期する高さ
	public float IvyBackTime = 3.0f;                //ツタが表れてから戻るまでの時間
	public float IvyMaxJumpTime = 1.5f;         //ツタが出てきてからジャンプを受け付ける時間
	public float IvyMaxLen = 200.0f;                       //ツタが伸びてくる長さ
	public float PlayerDownMaxLen = 180.0f;     //キャラクターが落ちてくる一番下の位置
	public float IvyOffsetPosition = -10.0f;        //ツタを画面の中心からどれだけ離れた位置に表示
	public float IvyDownSpeed = 1.0f;               //ツタが下りてくる速度(1秒間に)
	public float PlayerDownSpeed = 1.0f;            //一秒間にどれだけの座標分だけキャラクターを下に降ろすか？
	public float PlayerOffsetY = 2.5f;              //キャラクターが同時に死んだときにお互いの距離をどの程度離すか？
	float PlayerBuraBuraTime = 3.0f;

	float IvyRetTimeCnt;        //キャラがいなくなってからツタが戻るまでの時間のカウントする

	Vector3 IvyInitPos;     //ツタ画像の初期値
	
	GameObject CameraObj;            //カメラの情報を取得するために使用
	public GameObject IvySprite;

	// ============== キャラクターに使う情報 ==============
	public bool[] PlayerDeathFlag = new bool[3];        //	プレイヤーの死亡しているかどうかの判定
	bool[] OldeDeathFlag = new bool[3];                     //1つ前のフレームの情報を保持する
	public bool LifeFlag = true;                                    //現在生き返られるかのフラグ
	bool[] ReturnMoveFlag = new bool[3];                                            //キャラがツタから降りて復帰している最中か？

	// ============== ツタの移動に利用する情報 ===========
	float IvyNowLen;            //今現在、ツタがどこまで到達しているか？
	bool IvyMaxLenFlag;     //現在ツタが最大長まで伸びきっているか？

    PhotonView photonView;

	/// <summary>
	/// 復帰する際に利用するためのパラメータ
	/// </summary>
	public struct RetParam {
		public bool RetFlag;                        //今、ジャンプボタンを押したら復帰出来るか？
		public float TimeFromDeath;            //死んでからの時間
		public Vector3 NowOffsetPos;             //今現在、どれだけ上からスライドしているか？
		public int OrderNo;                         //降りてきている順番
	}

	RetParam[] param = new RetParam[3];     //各キャラクターが復帰処理をする際に利用するパラメータ

	// TODO ゲームマネージャからライフが残っているかの情報をもらって常に更新し続ける
	// TODO カメラから中央の座標位置を取得してその位置を中心にしてツタの位置を決める

	// Use this for initialization
	void Start() {
		IvyInitPos = IvySprite.transform.position;
		IvyPos = IvySprite.transform.position;
		for (int i = 0; i < 3; i++) {
			PlayerDeathFlag[i] = false;     //初めは誰も死んでいない
			OldeDeathFlag[i] = false;
			ParametersInit(i);          //キツネを下していくときのパラメータ初期化
			ReturnMoveFlag[i] = false;
		}       // end for

		//ツタのパラメータ初期化
		IvyNowLen = 0.0f;
		IvyMaxLenFlag = false;      //初めは誰もツタを使っていない

		//スプライトの実態を取得
		string ChildName = "Ivy";
		IvySprite = transform.Find(ChildName).gameObject;


		//カメラを取得
		CameraObj = GameObject.FindWithTag("MainCamera");

        photonView = GetComponent<PhotonView>();
	}

	// Update is called once per frame
	void Update() {
		IvyPos.x = CameraObj.transform.position.x;  //横の位置を合わせる

		//まずは古い情報を残しておく1
		for (int i = 0; i < 3; i++) {
			OldeDeathFlag[i] = PlayerDeathFlag[i];
		}

		//処理手順としてまずキャラが死んでいるのにツタが一番下に行っていなければ下げる処理を行う
		// まだツタが伸びきっていない
		bool Deceased = false;      //死んでいる人がいるか？
		for (int i = 0; i < 3; i++) {
			if (PlayerDeathFlag[i]) Deceased = true;        //死んでる人いる
		}

		//ツタ伸びてないかつ死んでる人がいる
		//この場合、ツタを伸ばす処理を行う
		if (!IvyMaxLenFlag && Deceased) {
			float Len = IvyDownSpeed * Time.deltaTime;
			IvyNowLen += Len;       //長さを足しこむ
			IvySprite.transform.Translate(0.0f, -Len, 0.0f);
			IvyPos = IvySprite.transform.position;
			//最大長さになった
			if (IvyNowLen >= IvyMaxLen) {
				IvyMaxLenFlag = true;
			}
		}

		//ツタが伸びていてかつキャラクターが死んでいる
		else if (IvyMaxLenFlag && Deceased) {
			//キャラクター三人分をチェックしてそれぞれ処理を行う
			for (int i = 0; i < 3; i++) {
				//死んでるプレイヤーが見つかった
				if (PlayerDeathFlag[i]) {
					//キャラクターの位置が指定位置よりも下に来た
					//プレイヤーの移動量を計算する
					param[i].TimeFromDeath += Time.deltaTime;
					if (param[i].TimeFromDeath > PlayerBuraBuraTime) param[i].RetFlag = true;
					float Len = CharaWaitHeight - param[i].NowOffsetPos.y;
					Len += param[i].OrderNo * PlayerOffsetY;        //キャラクター同時に死んでいる場合、キャラの位置をずらす
					if (Len >= PlayerDownMaxLen) {
						ReturnMoveFlag[i] = true;
					}
				}
			}
		}

		//プレイヤーが一人も死んでいない
		if (!Deceased) {
			//ツタが画面下方向に移動している
			if (IvyInitPos.y > IvySprite.transform.position.y) {
				IvyRetTimeCnt += Time.deltaTime; ;//経過時間を足しこむ
				if (IvyRetTimeCnt >= IvyBackTime) {
					IvySprite.transform.Translate(0.0f, IvyDownSpeed * Time.deltaTime, 0.0f);
					IvyPos = IvySprite.transform.position;
				}

			}
			else {
				IvySprite.transform.position = IvyInitPos;
				IvyMaxLenFlag = false;
				IvyNowLen = 0.0f;
			}
		}
		else {
			IvyRetTimeCnt = 0.0f;
		}
		
		IvySprite.transform.position = IvyPos;
	}

	/// <summary>
	/// プレイヤーオブジェクトから死亡情報を取得して設定する
	/// </summary>
	/// <param name="type">プレイヤーの種類</param>
	/// <param name="Life">プレイヤーが死んだかどうか？trueが死 : falseが生存</param>
	public void SetDeathFlag(PlayerController.Type type, bool Life) {
		//死亡してる
		if (Life) {
			int No = (int)type;                      //死亡したキャラクターの種類を番号に変換する
													 //何人今、死んでいるか？
			int cnt = 0;
			for (int i = 0; i < 3; i++) {
				if (PlayerDeathFlag[i])
					cnt++;      //死んだ人数をカウント
			}
			param[No].OrderNo = cnt;            //死んだ順番を保持する
			PlayerDeathFlag[No] = Life;     //キャラクターの情報を殺す
		}
	}       // SetDeathFlag

	/// <summary>
	/// それぞれのやられた際に利用する際にパラメータの情報をまとめて初期化する関数
	/// </summary>
	public void ParametersInit(int No) {
		param[No].TimeFromDeath = 0.0f;
		param[No].NowOffsetPos = Vector3.zero;
		param[No].RetFlag = false;
		ReturnMoveFlag[No] = false;
		param[No].OrderNo = -1;
	}

	/// <summary>
	/// 現在、降りているキャラの情報を返す
	/// </summary>
	/// <param name="No"></param>
	/// <returns></returns>
	public RetParam GetReturnParam(int No) {
		return param[No];
	}

	/// <summary>
	/// ツタの位置を返す
	/// </summary>
	/// <returns>ツタの位置を返す</returns>
	public Vector3 GetIvyPos() {
		return IvyPos;
	}

	/// <summary>
	/// キャラクターの位置を保持する関数。基本的にはキャラが死んでツタに捕まってるときにしか呼ばない
	/// </summary>
	/// <param name="type"></param>
	/// <param name="pos"></param>
	public void SetCharaVector3(PlayerController.Type type, Vector3 pos) {
		int No = (int)type;
		param[No].NowOffsetPos = pos;       //現在の座標を保持する
	}

	/// <summary>
	/// キャラクターがツタの上で待期する時間を設定する
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public Vector3 GetCharacterInitPos(PlayerController.Type type) {
		int No = (int)type;
		Vector3 Pos = IvyPos;
		Pos.y = CharaWaitHeight;        //キャラクターの高さを調整
		return Pos;
	}

	/// <summary>
	/// 現在、ツタが最大長まで伸びているかを返す
	/// </summary>
	/// <returns></returns>
	public bool GetIvyMaxLenFlag() {
		return IvyMaxLenFlag;
	}

	/// <summary>
	/// キャラがツタを降りて移動中かのフラグをゲッとする
	/// </summary>
	/// <returns></returns>
	public bool GetReturnMoveFlag(PlayerController.Type type) {
		return !ReturnMoveFlag[(int)type];
	}

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="RetFlag">true : 復帰</param>
    public void CharacterHasReturned(PlayerController.Type type, bool RetFlag)
    {
        photonView.RPC("CharacterHasReturnedRPC", PhotonTargets.AllViaServer, type, RetFlag);
    }

    [PunRPC]
    void CharacterHasReturnedRPC(PlayerController.Type type, bool RetFlag)
    {
        // キャラの復帰パラメータを初期化する
        if (RetFlag)
        {
            int i = (int)type;
            PlayerDeathFlag[i] = false;     //初めは誰も死んでいない
            OldeDeathFlag[i] = false;
            ParametersInit(i);          //キツネを下していくときのパラメータ初期化
            ReturnMoveFlag[i] = false;
        }
    }

	public bool GetBuraBuraTimeExit(PlayerController.Type type) {
		return !param[(int)type].RetFlag;
	}

	/// <summary>
	/// キャラが今、ジャンプしてもいい段階かどうかを返す
	/// </summary>
	/// <returns></returns>
	//public bool GetCharacterJumpFlag() {

	//}
}
