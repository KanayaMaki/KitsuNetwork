using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

	public enum CPlayerType
	{
		cEye,
		cEar,
		cNose,
	}
	public BufferProperty<CPlayerType> localPlayer;
	GameObject localPlayerObject;

	public PhotonView photonView;
	Ready ready;

	SoundManager soundManager;


	public enum CStageNum
	{
		cTutorial,
		cStage1,
		cStage2,
		cStage3,
		cStageNull
	}
	[SerializeField]
	CStageNum stageNum;

	public CStageNum GetStageNum()
	{
		return stageNum;
	}


	private void Awake()
	{
		//複数のインスタンスが作られていないかのチェック
		if (FindObjectOfType<GameManager>() != this)
		{
			Debug.LogError("GameManagerが二つ作られています");
		}

		localPlayer.Set(CPlayerType.cEye);

		state = CState.cWait;
		stateChanged = true;


		photonView = GetComponent<PhotonView>();

		soundManager = FindObjectOfType<SoundManager>();
	}

	// Use this for initialization
	void Start () {
		
		fade = GameObject.Find("Canvas/Fade").GetComponent<StartFade>();

		ready = FindObjectOfType<Ready>();

		ready.ImReady();
	}




	public enum CState
	{
		cWait,	//最初の待機状態
		cStart,	//開始
		cMain,	//メイン
		cResult,    //リザルト
		cResultEnd, //リザルトが終了したフェードアウト
		cGameover,  //ゲームオーバー
		cTutorialGoal,	//チュートリアルでゴールした時
	}
	public CState state;
	bool stateChanged;
	float stateTime;

	float scrollSpeed;

	StartFade fade;


	// Update is called once per frame
	void Update () {

		
		localPlayer.SetNotChanged();


		//状態が変わっていたら初期化
		if (stateChanged)
		{
			InitState();
		}

		//更新
		UpdateState();
	}

	[SerializeField] [Tooltip("シーン開始時に停止する時間")]
	float startKeepTime = 1.0f;

	[SerializeField] [Tooltip("シーン開始時にフェードする時間")]
	float startFadeAppearTime = 1.0f;

	[SerializeField] [Tooltip("シーン開始時に停止する時間")]
	float startFadeKeepTime = 1.0f;

	[SerializeField] [Tooltip("シーン開始時にフェードする時間")]
	float startFadeOutTime = 1.0f;

	[SerializeField][Tooltip("シーン終了時にフェードする時間")]
	float endFadeOutTime = 1.0f;

	[SerializeField][Tooltip("リザルト終了時に待つ時間")]
	float endWaitTime = 1.0f;

	void UpdateState()
	{
		//共通の処理
		stateTime += Time.deltaTime;

		//状態によって分岐する
		switch (state)
		{
			case CState.cWait:
				UpdateWait();
				break;
			case CState.cStart:
				UpdateStart();
				break;
			case CState.cMain:
				UpdateMain();
				break;
			case CState.cResult:
				UpdateResult();
				break;
			case CState.cTutorialGoal:
				UpdateTutorialGoal();
				break;
			case CState.cGameover:
				UpdateGameover();
				break;
			case CState.cResultEnd:
				UpdateResultEnd();
				break;
		}
	}
	void InitState()
	{
		//共通の初期化
		stateTime = 0.0f;
		stateChanged = false;

		//状態によって分岐する
		switch (state)
		{
			case CState.cWait:
				InitWait();
				break;
			case CState.cStart:
				InitStart();
				break;
			case CState.cMain:
				InitMain();
				break;
			case CState.cResult:
				InitResult();
				break;
			case CState.cTutorialGoal:
				InitTutorialGoal();
				break;
			case CState.cGameover:
				InitGameover();
				break;
			case CState.cResultEnd:
				InitResultEnd();
				break;
		}
	}

	bool gameInit = false;

	//待機状態
	void InitWait()
	{
		//初期設定
		fade.FadeText(0.0f);    //テキストは透明に
		fade.FadeBack(1.0f);    //背景は真っ黒に
	}
	void UpdateWait()
	{
		ready.ImReady();    //自身は準備完了

							//マスターだけ処理をする
		if (!PhotonNetwork.isMasterClient) return;

		//三人そろっていて
		if(ready.ReadyNum() >= 3)
		{
			//初期化されていないなら
			if(gameInit == false)
			{
				var rList = GetRandomList(3);

				//ゲームスタート
				GameObject.Find("GameManager").GetComponent<GameManager>().OnGameStart();
				GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManagerScript>().GameStart(rList[0], rList[1], rList[2]);

				gameInit = true;	//初期化済み
			}
		}
	}

	List<int> GetRandomList(int num)
	{
		//ランダムに並び替えられたリスト。返却用
		var randomList = new List<int>();

		//順番に並んだ数
		var orderList = new List<int> { 0, 1, 2 };


		//orderListからランダムに数字を取り出して、randomListに突っ込んでいく
		for(int i = num; i > 0; i--)
		{
			//0～i-1までの間で乱数を求める
			int rand = Random.Range(0, i);

			//リストに残っているrand番目の要素を追加
			randomList.Add(orderList[rand]);

			//リストから、追加された要素を削除する
			orderList.RemoveAt(rand);
		}
		
		return randomList;
	}


	//開始状態
	void InitStart()
	{
		var c = FindObjectOfType<CameraScroll>();

		//スクロールが存在するなら
		if(c)
		{
			scrollSpeed = c.scrollSpeed;    //カメラスクロールの値を一時保存
			c.scrollSpeed = 0.0f;   //スクロールを止める
			c.isScroll = false;
		}

		StartCoroutine(UpdateStartCoroutine());
	}
	void UpdateStart()
	{
		//動けない状態にする
		localPlayerObject.GetComponent<PlayerController>().SetImmobile();
	}

	IEnumerator UpdateStartCoroutine()
	{
		
		//フェード開始時までの
		while (stateTime <= startKeepTime)
		{
			yield return null;
		}

		//時間を0始まりにする
		stateTime -= startKeepTime;

		
		//フェード中の
		while (stateTime <= startFadeAppearTime)
		{
			float alpha = stateTime / startFadeAppearTime;
			fade.FadeText(alpha);
			yield return null;
		}

		//時間を0始まりにする
		stateTime -= startFadeAppearTime;


		//フェード中の
		while (stateTime <= startFadeKeepTime)
		{
			yield return null;
		}

		//時間を0始まりにする
		stateTime -= startFadeKeepTime;


		//フェード中の
		while (stateTime <= startFadeOutTime)
		{
			float alpha = 1.0f - stateTime / startFadeOutTime;
			fade.FadeAll(alpha);
			yield return null;
		}

		//完全に透過させる
		fade.FadeAll(0.0f);

		//時間を0始まりにする
		stateTime -= startFadeOutTime;


		//メインシーンに遷移
		ChangeState(CState.cMain);


		yield return null;
	}

	//メイン状態
	void InitMain()
	{
		var c = FindObjectOfType<CameraScroll>();

		if(c)
		{
			c.scrollSpeed = scrollSpeed;    //スクロールを再開
			c.isScroll = true;
		}

		PlayStageBGM();

		//動ける状態にする
		localPlayerObject.GetComponent<PlayerController>().SetMoveState();
	}
	void UpdateMain()
	{
		//OnGoalが呼ばれるとシーン遷移する
	}

	ScoreManager scoreManager;

	//リザルト状態
	void InitResult()
	{
		//スクロールの停止
		var c = FindObjectOfType<CameraScroll>();
		if (c)
		{
			c.scrollSpeed = 0.0f;
			c.isScroll = false;
		}

		//リザルトのBGM再生
		StopStageBGM();
		soundManager.PlayBGM("ResultBGM");

		//キャラクターを移動不可に
		localPlayerObject.GetComponent<PlayerController>().SetImmobile();

		scoreManager = FindObjectOfType<ScoreManager>();
		scoreManager.DrawStart();
	}
	void UpdateResult()
	{
		if(scoreManager.IsResultEnd())
		{
			ChangeState(CState.cResultEnd);
		}
	}


	//エンド状態
	void InitTutorialGoal()
	{
		//エンド状態のコルーチンを起動
		StartCoroutine(UpdateTutorialGoalCoroutine());
	}
	void UpdateTutorialGoal()
	{
	}

	IEnumerator UpdateTutorialGoalCoroutine()
	{
		//キャラクターを移動不可に
		localPlayerObject.GetComponent<PlayerController>().SetImmobile();

		//リザルトのBGM再生
		StopStageBGM();
		soundManager.PlayBGM("ResultBGM");


		//待機
		while (stateTime <= 5.0f)
		{
			yield return null;
		}

		ChangeState(CState.cResultEnd);
		
		yield return null;
	}



	//リザルト状態
	void InitGameover()
	{
		//スクロールの停止
		var c = FindObjectOfType<CameraScroll>();
		if (c)
		{
			c.scrollSpeed = 0.0f;
			c.isScroll = false;
		}

		//ローカルのキャラクターを操作不可能にする
		/////
		//  ToDo
		/////

		StopStageBGM();

		//Gameoverを開始する
		FindObjectOfType<GameOver>().StartGameover();

		soundManager.PlaySE("GameOverSE");
		soundManager.PlayBGM("GameOverBGM", 2.0f);

		//キャラクターを移動不可に
		localPlayerObject.GetComponent<PlayerController>().SetImmobile();
	}
	void UpdateGameover()
	{
	}


	//エンド状態
	void InitResultEnd()
	{
		//エンド状態のコルーチンを起動
		StartCoroutine(UpdateResultEndCoroutine());
	}
	void UpdateResultEnd()
	{
	}

	IEnumerator UpdateResultEndCoroutine()
	{
		yield return new WaitForSeconds(endWaitTime);

		//ステートタイムをリセット
		stateTime = 0.0f;

		//フェード中の
		while (stateTime <= endFadeOutTime)
		{
			float alpha = stateTime / endFadeOutTime;
			fade.FadeBack(alpha);
			yield return null;
		}

		//フェードで完全に隠す
		fade.FadeBack(1.0f);

		//シーン遷移
		SceneManager.LoadScene("StageSelectScene");

		yield return null;
	}


	void PlayStageBGM()
	{
		if (stageNum == CStageNum.cTutorial)
		{
			soundManager.PlayBGM("TutorialBGM");
		}
		else
		{
			soundManager.PlayBGM("GameBGM");
		}
	}
	void StopStageBGM()
	{
		soundManager.StopBGM("TutorialBGM");
		soundManager.StopBGM("GameBGM");
	}


	//状態が変わったときのイベント
	void ChangeState(CState aState)
	{
		bool canChange = false;

		switch(state)
		{
			case CState.cWait:
				if(aState == CState.cStart)
				{
					canChange = true;
				}
				break;

			case CState.cStart:
				if (aState == CState.cMain)
				{
					canChange = true;
				}
				break;

			case CState.cMain:
				if (aState == CState.cResult || aState == CState.cGameover || aState == CState.cTutorialGoal)
				{
					canChange = true;
				}
				break;

			case CState.cResult:
				if (aState == CState.cResultEnd)
				{
					canChange = true;
				}
				break;

			case CState.cTutorialGoal:
				if (aState == CState.cResultEnd)
				{
					canChange = true;
				}
				break;

			case CState.cGameover:
				break;

			case CState.cResultEnd:
				break;
		}

		
		if(canChange)
		{
			state = aState;
			stateChanged = true;
		}
	}


	//ゲームオーバーになった時のイベント
	public void OnGameover()
	{
		photonView.RPC("OnGameoverRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnGameoverRPC()
	{
		ChangeState(CState.cGameover);
	}


	//ゴールした時のイベント
	public void OnGoal()
	{
		photonView.RPC("OnGoalRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnGoalRPC()
	{
		ChangeState(CState.cResult);
	}


	//ゲームを終えるときのイベント
	public void OnResultEnd()
	{
		photonView.RPC("OnResultEndRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnResultEndRPC()
	{
		ChangeState(CState.cResultEnd);
	}



	//チュートリアルでゴールするときのイベント
	public void OnTutorialGoal()
	{
		photonView.RPC("OnTutorialGoalRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnTutorialGoalRPC()
	{
		ChangeState(CState.cTutorialGoal);
	}


	//ゲーム開始時のイベント
	public void OnGameStart()
	{
		photonView.RPC("OnGameStartRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnGameStartRPC()
	{
		ChangeState(CState.cStart);

		GameObject camera2 = GameObject.Find("Camera2");
		if(camera2)
		{
			camera2.GetComponent<CameraScroll>().StartScroll();
		}
	}


	// 追加
	public void SetType(CPlayerType type)
	{
		localPlayer.Set(type);
	}

	//ローカルのプレイヤーを設定する
	public void SetLocalPlayer(GameObject aLocalPlayerObject)
	{
		localPlayerObject = aLocalPlayerObject;
	}

	public GameObject GetLocalPlayer()
	{
		return localPlayerObject;
	}
}