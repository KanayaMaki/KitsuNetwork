using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RoomScene_State;
using UnityEngine.EventSystems;

//  ルームシーン最初の画面
public class StateSelect : RoomSceneState
{
	// コールバック関数
	public OnCallBack NextState_CreateRoom;
	public OnCallBack NextState_JoinRoom;

    
	private Button m_pushRoomCreate;
	private Button m_pushRoomJoin;
	private GUIEvent m_focusRoomCreate;
	//選ばれている
	private GUIEvent m_focusRoomJoin;
	//選ばれている

	// 詳細アンダーバー(選んでるボタンの説明)
	private Image m_underBar;
	private Image m_createRoom;
	private Image m_joinRoom;
	[SerializeField]
	private Sprite m_createRoomBar;
	[SerializeField]
	private Sprite m_joinRoomBar;
	[SerializeField]
	private Sprite m_createRoom_art_push;
	[SerializeField]
	private Sprite m_joinRoom_art_push;
	[SerializeField]
	private Sprite m_createRoom_art;
	[SerializeField]
	private Sprite m_joinRoom_art;

    private bool initFlag = false;

	public override string GetStateName ()
	{
		return "RoomScene_Select";
	}

	// ------------------------ 初期化 ------------------------
	public override void Init ()
	{
		Debug.Log ("RoomSceneStateSelect開始");
		if (!gameObject.GetActive ())
			gameObject.SetActive (true);

        soundMng = GameObject.Find("SoundManagerRoom").GetComponent<SoundManager>();
		//PhotonController.Instance.NetworkConnect();

		InitPostion ();

		SetParentOtherObj ();

		SetButtonExec ();
		SetFocusExec ();

		StartFocus ();
	}


	protected override void SetParentOtherObj ()
	{
		// TODO:
		m_pushRoomCreate = gameObject.transform.GetChild (1).GetComponent<Button> ();
		m_pushRoomJoin = gameObject.transform.GetChild (2).GetComponent<Button> ();
		m_focusRoomCreate = gameObject.transform.GetChild (1).GetComponent<GUIEvent> ();
		m_focusRoomJoin = gameObject.transform.GetChild (2).GetComponent<GUIEvent> ();

		m_underBar = gameObject.transform.GetChild (3).GetComponent<Image> ();
		m_createRoom = gameObject.transform.GetChild (1).GetComponent<Image> ();
		m_joinRoom = gameObject.transform.GetChild (2).GetComponent<Image> ();
	}

	// ボタンが押されたとき実行されるメソッドをセット
	protected override void SetButtonExec ()
	{
		Debug.Log("SetButtonExec");
        m_pushRoomCreate.onClick.RemoveAllListeners();
        m_pushRoomJoin.onClick.RemoveAllListeners();
        m_pushRoomCreate.onClick.AddListener (OnButton_Create);
		m_pushRoomJoin.onClick.AddListener (OnButton_Join);
	}

	// ボタンにフォーカスが合わさった時実行されるメソッドをセット
	private void SetFocusExec ()
	{
		Debug.Log("SetFocusExec");
		m_focusRoomCreate.onSelect.RemoveAllListeners();
        m_focusRoomJoin.onSelect.RemoveAllListeners();
        m_focusRoomCreate.onSelect.AddListener (OnFocus_Create);
		m_focusRoomJoin.onSelect.AddListener (OnFocus_Join);
	}

	protected override void StartFocus ()
	{
		m_pushRoomCreate.Select ();
	}
	// --------------------------------------------------------


	// ルーム作成ボタンが押された時、実行
	void OnButton_Create ()
	{
        // TODO:
        if (soundMng == null) return;
        soundMng.PlaySE("DecisionSE");
		NextState_CreateRoom ();
	}

	// ルーム入室ボタンが押された時、実行
	void OnButton_Join ()
	{
        // TODO:
        if (soundMng == null) return;
        soundMng.PlaySE("DecisionSE");
        NextState_JoinRoom ();
	}

	// ルーム作成ボタンが選択された時、実行
	void OnFocus_Create ()
	{
        if (!gameObject.GetActive()) return;
        if (soundMng == null) return;
        // TODO:
        if (initFlag)
        {
            soundMng.PlaySE("CursorSE");
        }
        m_underBar.sprite = m_createRoomBar;//画像切り替え 
		m_createRoom.sprite = m_createRoom_art_push;
		m_joinRoom.sprite = m_joinRoom_art;

	}

	// ルーム入室ボタンが選択された時、実行
	void OnFocus_Join ()
	{
        if (!gameObject.GetActive()) return;
        if (soundMng == null) return;
        // TODO:
        initFlag = true;
        soundMng.PlaySE("CursorSE");
        m_underBar.sprite = m_joinRoomBar;
		m_joinRoom.sprite = m_joinRoom_art_push;
		m_createRoom.sprite = m_createRoom_art;
	}

	public override void Uninit ()
	{
        soundMng = null;
        initFlag = false;
		Debug.Log ("RoomSceneStateSelect終了");
		gameObject.SetActive (false);
	}

}