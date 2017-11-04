using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomScene_State;
using UnityEngine.UI;

// ルーム待機画面
public class StateWait : RoomSceneState
{
	// コールバック関数
	public OnCallBack NextState;
	public OnCallBack BackState;

	private Button m_pushNNextState;
	private GUIEvent m_pushNNext;
	private Image m_pushNextStateImage;
	private GUIEvent m_pushBackState;

	//追加---------------------------------
	private GUIEvent m_no_pushBack_art;
	private Image m_back;
	private GUIEvent m_pushBack_art;
	//------------------------------------

	private GameObject m_namePlate;
	private Image[] m_namePlateImage = new Image[3];
	[SerializeField]
	private Sprite m_nameNone;
	[SerializeField]
	private Sprite m_nameMe;
	[SerializeField]
	private Sprite m_nameYour;

	[SerializeField]
	private Sprite m_back_art_push;
	[SerializeField]
	private Sprite m_back_art;

	[SerializeField]
	private Text m_roomName;

	private bool m_isMaster = false;

	private bool initFlag = false;
	private bool inputFlag = false;

	public override string GetStateName()
	{
		return "RoomScene_Wait";
	}


	// ------------------------ 初期化 ------------------------
	public override void Init()
	{
		Debug.Log("RoomSceneStateWait開始");
		if (!gameObject.GetActive())
			gameObject.SetActive(true);

		PhotonController.Instance.CreateListener = OnListenerMaster;

		PhotonController.Instance.OtherConnecedListener = OnRoomConneced;
		PhotonController.Instance.OtherDisConnecedListener = OnRoomConneced;
		PhotonController.Instance.OtherUpdateListener = OnRoomConneced;

		soundMng = GameObject.Find("SoundManagerRoom").GetComponent<SoundManager>();

		InitPostion();

		SetParentOtherObj();

		SetButtonExec();
		SetFocusExec();

		StartFocus();

	}



	protected override void SetParentOtherObj()
	{
		// TODO:
		m_pushNNextState = gameObject.transform.GetChild(1).GetComponent<Button>();
		m_pushNNext = gameObject.transform.GetChild(1).GetComponent<GUIEvent>();
		m_pushBackState = gameObject.transform.GetChild(2).GetComponent<GUIEvent>();
		m_pushNextStateImage = gameObject.transform.GetChild(1).GetComponent<Image>();
		m_no_pushBack_art = gameObject.transform.GetChild(2).GetComponent<GUIEvent>();
		m_back = gameObject.transform.GetChild(2).GetComponent<Image>();

		m_namePlate = gameObject.transform.GetChild(3).gameObject;
		for (int i = 0; i < 3; ++i)
		{
			m_namePlateImage[i] = m_namePlate.transform.GetChild(i).GetComponent<Image>();
		}


		m_pushNNextState.gameObject.SetActive(false);
	}

	protected override void SetButtonExec()
	{
		// TODO:
		m_pushNNextState.onClick.RemoveAllListeners();
		m_pushBackState.onLongPress.RemoveAllListeners();

		m_pushNNextState.onClick.AddListener(OnButton_Next);
		m_pushBackState.onLongPress.AddListener(OnButton_Back);
	}

	// ボタンにフォーカスが合わさった時実行されるメソッドをセット
	private void SetFocusExec()
	{
		m_pushBackState.onSelect.RemoveAllListeners();
		m_pushNNext.onSelect.RemoveAllListeners();

		m_pushBackState.onSelect.AddListener(OnFocus_Back);
		m_pushNNext.onSelect.AddListener(OnFocus_Go);
	}

	protected override void StartFocus()
	{
		// TODO:
		m_pushBackState.Select();
	}

	// --------------------------------------------------------

	// 出発ボタンが押された時、実行
	void OnButton_Next()
	{
		if (inputFlag) return;

		m_pushNNextState.onClick.RemoveAllListeners();

		Debug.Log("部屋に入室し待機画面へ");

		// TODO:ルーム作成
		soundMng.PlaySE("GoSE");
		NextState();
	}
	// 戻るボタンが押された時、実行
	void OnButton_Back()
	{
		if (inputFlag) return;
		// TODO:
		if (soundMng == null) return;
		Debug.Log("選択画面に戻る");
		PhotonController.Instance.LeaveRoom();
		soundMng.PlaySE("CancelSE");
		BackState();
	}

	// もどるボタンが選択された時、実行
	void OnFocus_Back()
	{
		// TODO:
		if (initFlag)
		{
			soundMng.PlaySE("CursorSE");
		}
		m_back.sprite = m_back_art_push;
	}

	// GOボタンが選択された時、実行
	void OnFocus_Go()
	{
		// TODO:
		initFlag = true;
		soundMng.PlaySE("CursorSE");
		m_back.sprite = m_back_art;
	}

	// マスタークライアントになる
	void OnListenerMaster()
	{
		m_isMaster = true;
		m_pushNNextState.gameObject.SetActive(true);
		m_pushNNextState.interactable = false;
		PhotonController.Instance.LeaveListener = OnButton_Back;

		m_namePlateImage[0].sprite = m_nameMe;

		// 追加：
		m_roomName.text = PhotonController.Instance.GetRoomInfo().Name;
	}

	// 他のプレイヤーがルームに入った(抜けた)
	void OnRoomConneced()
	{
		// ネームプレート表示
		NamePlateUpdate();
		// 追加：
		m_roomName.text = PhotonController.Instance.GetRoomInfo().Name;

		soundMng.PlaySE("RoomInSE");
		if (!m_isMaster)
			return;

		ActiveJudge_NextButton();
	}

	void ActiveJudge_NextButton()
	{
		if (PhotonController.Instance.GetRoomInfo().PlayerCount == 3)
		{
			// TODO:ボタンを押せるようにする
			soundMng.PlaySE("GoReady");
			m_pushNNextState.interactable = true;
		}
		else
		{
			// TODO:ボタンを押せないようにする
			m_pushNNextState.interactable = false;
		}
	}

	void NamePlateUpdate()
	{
		// ここの処理雑
		var playerNum = PhotonController.Instance.GetRoomInfo().PlayerCount;
		var playerOrder = PhotonController.Instance.Order;

		// ネームプレート画像初期化
		for (int i = 0; i < 3; ++i)
		{
			m_namePlateImage[i].sprite = m_nameNone;
		}

		// ネームプレート画像代入
		for (int i = 0; i < playerNum; ++i)
		{
			if (playerOrder == i)
			{
				m_namePlateImage[i].sprite = m_nameMe;
				continue;
			}
			m_namePlateImage[i].sprite = m_nameYour;
		}
	}

	public override void Uninit()
	{
		initFlag = false;
		soundMng = null;
		Debug.Log("RoomSceneStateWait終了");
		gameObject.SetActive(false);
	}
}