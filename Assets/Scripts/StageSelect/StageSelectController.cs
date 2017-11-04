using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// DEBUG: の下の行をコメント化するとデバッグできる

/*
public class StageSelectController : Photon.MonoBehaviour {

public enum Stage
{
	Tutorial,
	Stage_1,
	Stage_2,
	Stage_3,
	NUM
}

private Stage m_stage = Stage.Tutorial;
private InputController input ;

[SerializeField]
private GameObject m_boardObj;
private StageBoard m_borad;

[SerializeField]
private GameObject m_foxObj;
private Fox2DAnimation m_fox;

[SerializeField]
private RotationPivot m_pivot;

[SerializeField]
private PhotonViewController m_viewController;

[SerializeField]
private Stage_Logo m_logoObj;

private PhotonView m_photonView;

private SoundManager soundMng;

Ready startReady;
Ready gameReady;


void Awake()
{
	soundMng = GameObject.Find("SoundManagerStageSelect").GetComponent<SoundManager>();
	soundMng.PlayBGM("StageSelectBGM");
}

// 初期化
void Start()
{
	input = InputController.Instance;

	if (m_boardObj == null)
		Debug.LogError("Boardがありません。");
	if(m_foxObj == null)
		Debug.LogError("Foxがありません。");
	if (m_pivot == null) 
		Debug.LogError("RotationPivotがありません。");
	if (m_viewController == null)
		Debug.LogError("PhotonViewControllerがありません。");

	m_borad = m_boardObj.GetComponent<StageBoard>();
	m_boardObj.SetActive(false);

	m_fox = m_foxObj.GetComponent<Fox2DAnimation>();

	m_pivot.Stop = StopAction;

	m_photonView = GetComponent<PhotonView>();

	PhotonNetwork.isMessageQueueRunning = true;

	startReady = GameObject.Find("StartReady").GetComponent<Ready>();
	gameReady = GameObject.Find("GameReady").GetComponent<Ready>();

	gameReady.SetReadyNum(100);
}

// 入力受付
void Update()
{

	startReady.ImReady();

	// マスタークライアント以外処理しない
	// DEBUG:
	if (!PhotonController.Instance.IsMaster()) return;

	MasterClientExec();
}

// マスタークライアントのみの処理
void MasterClientExec()
{
	//全員準備できるまで、処理しない
	if (startReady.ReadyNum() < 3) return;
	if (gameReady.ReadyNum() < 3) return;

	// キャンセル入力
	if (input.OnCancel())
	{
		if (m_boardObj.GetActive())
		{
			gameReady.Wait();
			m_photonView.RPC("BoardRelease", PhotonTargets.AllViaServer);
		}

	}

	// 決定入力
	if (input.OnDecision())
	{
		if (!m_boardObj.GetActive())
		{
			gameReady.Wait();
			m_photonView.RPC("BoardPop", PhotonTargets.AllViaServer);
		}
		else
		{
			gameReady.Wait();
			NextStageScene();
		}	
	}

	// 左右入力はステージボードが出てるとき処理しない
	if (m_boardObj.GetActive()) return;

	// 左右入力
	if (input.OnRiight())
	{
		gameReady.Wait();
		m_photonView.RPC("RightRotation", PhotonTargets.AllViaServer);
	}
	else if (input.OnLeft())
	{
		gameReady.Wait();
		m_photonView.RPC("LeftRotation", PhotonTargets.AllViaServer);
	}
}



// ステージボード出現
[PunRPC]
void BoardPop()
{
	soundMng.PlaySE("DecisionSE");
	m_boardObj.SetActive(true);
	m_borad.Init(m_stage);

	gameReady.ImReady();
}
void NextStageScene()
{
	//TODO:現在選択中のステージへ遷移
	// DEBUG:
	m_photonView.RPC("LoadNextSceneSound", PhotonTargets.All);
	switch (m_stage)
	{
		case Stage.Tutorial:
			Debug.Log("Tutorial遷移");
			LoadScene("tutorial");
			break;
		case Stage.Stage_1:
			Debug.Log("Stage1遷移");
			LoadScene("Stage1");
			break;
		case Stage.Stage_2:
			Debug.Log("Stage2遷移");
			LoadScene("Stage2");
			break;
		case Stage.Stage_3:
			Debug.Log("Stage3遷移");
			LoadScene("Stage3");
			break;
	}
}

// ステージボード消去
[PunRPC]
void BoardRelease()
{
	m_boardObj.SetActive(false);
	soundMng.PlaySE("CancelSE");

	gameReady.ImReady();
}

// 地面右回転処理、キツネ右歩きスタート
[PunRPC]
public void RightRotation()
{
	soundMng.PlaySE("MoveSE");
	m_pivot.SetRotationRight();
	m_fox.RightWorkAnimationStart();
	m_stage++;
	m_stage = (Stage)((int)m_stage % (int)Stage.NUM);
	m_logoObj.SetLogo(m_stage);
	Debug.Log("通ったよ");
}

// 地面左回転処理、キツネ左歩きスタート
[PunRPC]
void LeftRotation()
{
	soundMng.PlaySE("MoveSE");
	m_pivot.SetRotationLeft();
	m_fox.LeftWorkAnimationStart();
	m_stage--;
	if (m_stage < Stage.Tutorial)
	{
		m_stage += (int)Stage.NUM;
	}
	m_logoObj.SetLogo(m_stage);
}

// RotationPivotの回転が終了したら呼ばれる
void StopAction()
{
	soundMng.StopSE("MoveSE");
	m_fox.StopAnimation();

	gameReady.ImReady();
}

[PunRPC]
void LoadNextSceneSound()
{
	soundMng.PlaySE("GoSE");
}


void LoadScene(string name)
{
	photonView.RPC("LoadSceneRPC", PhotonTargets.AllViaServer, name);
}
[PunRPC]
void LoadSceneRPC(string name)
{
	StartCoroutine(LoadSceneCoroutine(name));
}

IEnumerator LoadSceneCoroutine(string name)
{
	yield return SceneManager.LoadSceneAsync(name);
}
}

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// DEBUG: の下の行をコメント化するとデバッグできる

public class StageSelectController : Photon.MonoBehaviour
{

	public enum Stage
	{
		Tutorial,
		Stage_1,
		Stage_2,
		Stage_3,
		NUM
	}

	private Stage m_stage = Stage.Tutorial;
	private InputController input;

	[SerializeField]
	private GameObject m_boardObj;
	private StageBoard m_borad;

	[SerializeField]
	private GameObject m_foxObj;
	private Fox2DAnimation m_fox;

	[SerializeField]
	private RotationPivot m_pivot;

	[SerializeField]
	private PhotonViewController m_viewController;

	[SerializeField]
	private Stage_Logo m_logoObj;

	private PhotonView m_photonView;

	private bool input_flag = false;

	private SoundManager soundMng;

	Ready startReady;
	Ready gameReady;


	void Awake()
	{
		soundMng = GameObject.Find("SoundManagerStageSelect").GetComponent<SoundManager>();
		soundMng.PlayBGM("StageSelectBGM");
	}

	// 初期化
	void Start()
	{
		input = InputController.Instance;

		if (m_boardObj == null)
			Debug.LogError("Boardがありません。");
		if (m_foxObj == null)
			Debug.LogError("Foxがありません。");
		if (m_pivot == null)
			Debug.LogError("RotationPivotがありません。");
		if (m_viewController == null)
			Debug.LogError("PhotonViewControllerがありません。");

		m_borad = m_boardObj.GetComponent<StageBoard>();
		m_boardObj.SetActive(false);

		m_fox = m_foxObj.GetComponent<Fox2DAnimation>();

		m_pivot.Stop = StopAction;

		m_photonView = GetComponent<PhotonView>();

		PhotonNetwork.isMessageQueueRunning = true;

		startReady = GameObject.Find("StartReady").GetComponent<Ready>();
		gameReady = GameObject.Find("GameReady").GetComponent<Ready>();

		gameReady.SetReadyNum(100);
	}

	// 入力受付
	void Update()
	{
		startReady.ImReady();

		// マスタークライアント以外処理しない
		// DEBUG:
		if (!PhotonController.Instance.IsMaster()) return;

		MasterClientExec();
	}

	// マスタークライアントのみの処理
	void MasterClientExec()
	{
		if (input_flag) return;

		Debug.Log("Master:" + PhotonNetwork.player.ID.ToString());

		//全員準備できるまで、処理しない
		if (startReady.ReadyNum() < 3) return;
		if (gameReady.ReadyNum() < 3) return;

		// キャンセル入力
		if (input.OnCancel())
		{
			gameReady.Wait();
			m_photonView.RPC("BoardRelease", PhotonTargets.AllViaServer);
		}

		// 決定入力
		if (input.OnDecision())
		{
			gameReady.Wait();

			NextStageScene();
			m_photonView.RPC("BoardPop", PhotonTargets.AllViaServer);

		}


		// 左右入力はステージボードが出てるとき処理しない
		if (m_boardObj.GetActive()) return;


		// 左右入力
		if (input.OnRiight())
		{
			gameReady.Wait();
			m_photonView.RPC("RightRotation", PhotonTargets.AllViaServer);
		}
		else if (input.OnLeft())
		{
			gameReady.Wait();
			m_photonView.RPC("LeftRotation", PhotonTargets.AllViaServer);
		}
	}



	// ステージボード出現
	[PunRPC]
	void BoardPop()
	{
		// 既にステージボードがある場合処理しない
		if (m_boardObj.GetActive())
		{
			gameReady.ImReady();
			return;
		}

		gameReady.ImReady();

		soundMng.PlaySE("DecisionSE");
		m_boardObj.SetActive(true);
		m_borad.Init(m_stage);
	}
	void NextStageScene()
	{
		// ステージボードが存在しない場合、処理しない
		if (!m_boardObj.GetActive()) return;

		//TODO:現在選択中のステージへ遷移
		// DEBUG:
		m_photonView.RPC("LoadNextSceneSound", PhotonTargets.All);
		switch (m_stage)
		{
			case Stage.Tutorial:
				Debug.Log("Tutorial遷移");
				LoadScene("tutorial");
				break;
			case Stage.Stage_1:
				Debug.Log("Stage1遷移");
				LoadScene("Stage1");
				break;
			case Stage.Stage_2:
				Debug.Log("Stage2遷移");
				LoadScene("Stage2");
				break;
			case Stage.Stage_3:
				Debug.Log("Stage3遷移");
				LoadScene("Stage3");
				break;
		}
		input_flag = true;
	}

	// ステージボード消去
	[PunRPC]
	void BoardRelease()
	{
		if (m_boardObj.GetActive())
		{
			m_boardObj.SetActive(false);
			soundMng.PlaySE("CancelSE");
		}
		gameReady.ImReady();
	}

	// 地面右回転処理、キツネ右歩きスタート
	[PunRPC]
	public void RightRotation()
	{
		soundMng.PlaySE("MoveSE");
		m_pivot.SetRotationRight();
		m_fox.RightWorkAnimationStart();
		m_stage++;
		m_stage = (Stage)((int)m_stage % (int)Stage.NUM);
		m_logoObj.SetLogo(m_stage);
		Debug.Log("通ったよ");
	}

	// 地面左回転処理、キツネ左歩きスタート
	[PunRPC]
	void LeftRotation()
	{
		soundMng.PlaySE("MoveSE");
		m_pivot.SetRotationLeft();
		m_fox.LeftWorkAnimationStart();
		m_stage--;
		if (m_stage < Stage.Tutorial)
		{
			m_stage += (int)Stage.NUM;
		}
		m_logoObj.SetLogo(m_stage);
	}

	// RotationPivotの回転が終了したら呼ばれる
	void StopAction()
	{
		soundMng.StopSE("MoveSE");
		input_flag = false;
		gameReady.ImReady();
		m_fox.StopAnimation();
	}

	[PunRPC]
	void LoadNextSceneSound()
	{
		soundMng.PlaySE("GoSE");
	}


	void LoadScene(string name)
	{
		photonView.RPC("LoadSceneRPC", PhotonTargets.AllViaServer, name);
	}
	[PunRPC]
	void LoadSceneRPC(string name)
	{
		StartCoroutine(LoadSceneCoroutine(name));
	}

	IEnumerator LoadSceneCoroutine(string name)
	{
		yield return SceneManager.LoadSceneAsync(name);
	}
}
