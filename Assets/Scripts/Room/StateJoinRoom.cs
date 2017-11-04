using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomScene_State;
using UnityEngine.UI;

//  ルーム入室画面
public class StateJoinRoom : RoomSceneState
{
	// コールバック関数
	public OnCallBack NextState;
	public OnCallBack BackState;

	public delegate void OnEventSystemActive (bool _flag);

	public OnEventSystemActive EventSystem;

	private Button m_pushNNextState;
	private GUIEvent m_pushBackState;
	private Button m_pushInputName;
	private Text m_inputText;

	private GUIEvent m_focuscreate;
	private GUIEvent m_focusback;
	private GUIEvent m_focusname;

	private GameObject m_prefabKeybord;
	private GameObject m_virtualKeybord;

	// 詳細アンダーバー(選んでるボタンの説明)
	private Image m_underBar;
	private Image m_create;
	private Image m_back;
	private Image m_name;

	// 部屋入室失敗時エラーポップ
	private GameObject m_warningPop;
	private Image m_warningPopImage;

	// 画像切り替え
	[SerializeField]
	private Sprite m_noRoomPop;
	[SerializeField]
	private Sprite m_noNamePop;
	[SerializeField]
	private Sprite m_fullHousePop;

	[SerializeField]
	private Sprite m_create_art_push;
	[SerializeField]
	private Sprite m_back_art_push;
	[SerializeField]
	private Sprite m_create_art;
	[SerializeField]
	private Sprite m_back_art;

	// エラーポップのサイズ、位置
	[SerializeField]
	private Vector3 m_sizePop;
	[SerializeField]
	private Vector3 m_posPop;

	[SerializeField]
	private Sprite m_name_art;
	[SerializeField]
	private Sprite m_name_art_push;

	private string roomName;
    private bool initFlag = false;

	public override string GetStateName ()
	{
		return "RoomScene_JoinRoom";
	}


	// ------------------------ 初期化 ------------------------
	public override void Init ()
	{
		Debug.Log ("RoomSceneStateJoinRoom開始");
		if (!gameObject.GetActive ())
			gameObject.SetActive (true);

        soundMng = GameObject.Find("SoundManagerRoom").GetComponent<SoundManager>();

        InitPostion ();

		SetParentOtherObj ();

		SetButtonExec ();
		SetFocusExec ();

		StartFocus ();
	}

	void Update ()
	{
		if (m_warningPop == null)
			return;

		if (InputController.Instance.OnDecision ()) {
			ReleaseWarningPop ();
		}
	}

	protected override void SetParentOtherObj ()
	{
		// TODO:
		m_pushNNextState = gameObject.transform.GetChild (1).GetComponent<Button> ();
		m_pushBackState = gameObject.transform.GetChild (2).GetComponent<GUIEvent> ();
		m_pushInputName = gameObject.transform.GetChild (3).GetComponent<Button> ();
		m_inputText = m_pushInputName.transform.GetChild (0).GetComponent<Text> ();
		m_focuscreate = gameObject.transform.GetChild (1).GetComponent<GUIEvent> ();
		m_focusback = gameObject.transform.GetChild (2).GetComponent<GUIEvent> ();
		m_focusname = gameObject.transform.GetChild (3).GetComponent<GUIEvent> ();
       
		m_underBar = gameObject.transform.GetChild (4).GetComponent<Image> ();
		m_create = gameObject.transform.GetChild (1).GetComponent<Image> ();
		m_back = gameObject.transform.GetChild (2).GetComponent<Image> ();
		m_name = gameObject.transform.GetChild (3).GetComponent<Image> ();

		var canvas = GameObject.FindObjectOfType<Canvas> ();
		m_prefabKeybord = Resources.Load<GameObject> ("Prefab/VirtualKeyBorad");
		m_virtualKeybord = GameObject.Instantiate (m_prefabKeybord, canvas.transform);
		m_virtualKeybord.SetActive (false);
	}

	protected override void SetButtonExec ()
	{
        // TODO:
        m_pushNNextState.onClick.RemoveAllListeners();
        m_pushBackState.onLongPress.RemoveAllListeners();
        m_pushInputName.onClick.RemoveAllListeners();

        m_pushNNextState.onClick.AddListener (OnButton_Next);
		m_pushBackState.onLongPress.AddListener (OnButton_Back);
		m_pushInputName.onClick.AddListener (OnButton_InputName);
	}

	// ボタンにフォーカスが合わさった時実行されるメソッドをセット
	private void SetFocusExec ()
	{
        m_focuscreate.onSelect.RemoveAllListeners();
        m_pushBackState.onSelect.RemoveAllListeners();
        m_focusname.onSelect.RemoveAllListeners();

        m_focuscreate.onSelect.AddListener (OnFocus_Create);
		m_pushBackState.onSelect.AddListener (OnFocus_Back);
		m_focusname.onSelect.AddListener (OnFocus_Name);
	}

	protected override void StartFocus ()
	{
		// TODO:
		m_pushInputName.Select ();
	}
	// --------------------------------------------------------

	// 入室ボタンが押された時、実行
	void OnButton_Next ()
	{

		Debug.Log (roomName + "部屋に入室し待機画面へ");

		// TODO:ルーム入室

		var judge = PhotonController.Instance.JoinRoom (m_inputText.text);

		if (judge != PhotonController.SUCCESS) {
			Debug.Log ("入室失敗");
			ErrorExec (judge);
			return;
		}
        soundMng.PlaySE("DecisionSE");
        NextState ();
	}
	// 戻るボタンが押された時、実行
	void OnButton_Back ()
	{
		// TODO:
		Debug.Log ("選択画面に戻る");
        soundMng.PlaySE("CancelSE");
        BackState ();
	}

	// 名前入力ボタンが押された時、実行
	void OnButton_InputName ()
	{
        // 仮想キーボード生成
        // その他入力を止める
        soundMng.PlaySE("DecisionSE");
        if (!m_virtualKeybord.GetActive ()) {
			m_virtualKeybord.SetActive (true);
			m_virtualKeybord.GetComponent<VirtualKeybord> ().Str = InputEnd;

			EventSystem (false);
		}

	}

	// ルーム作成ボタンが選択された時、実行
	void OnFocus_Create ()
	{
        // TODO:
        soundMng.PlaySE("CursorSE");
        initFlag = true;
		m_create.sprite = m_create_art_push;
		m_back.sprite = m_back_art;
		m_name.sprite = m_name_art;

	}

	// 戻るボタンが選択された時、実行
	void OnFocus_Back ()
	{
        // TODO:
        soundMng.PlaySE("CursorSE");
        initFlag = true;
        m_create.sprite = m_create_art;
		m_back.sprite = m_back_art_push;
		m_name.sprite = m_name_art;

	}

	// 名前入力ボタンが選択された時、実行
	void OnFocus_Name ()
	{
        // TODO:
        if (initFlag)
        {
            soundMng.PlaySE("CursorSE");
        }
		m_create.sprite = m_create_art;
		m_back.sprite = m_back_art;
		m_name.sprite = m_name_art_push;
	}

	// 入力された文字列を受け取り選択イベントをアクティブにする
	void InputEnd (string _str)
	{
		m_inputText.text = _str;

		EventSystem (true);
		StartFocus ();
	}


	// ---------------- エラー処理 ----------------
	void ErrorExec (short _judge)
	{

		CreateWarningPop (_judge);

		EventSystem (false);


	}

	void CreateWarningPop (short _judge)
	{
		//var canvas = GameObject.FindObjectOfType<Canvas>();
		if (m_warningPop != null)
			return;
        initFlag = false;
        soundMng.PlaySE("WarningPopUpSE");
        m_warningPop = new GameObject ("WarningPop");

		m_warningPop.transform.SetParent (gameObject.transform);
		// Image,RectTransformコンポーネント追加
		m_warningPopImage = m_warningPop.AddComponent<Image> ();

		RectTransform rect = m_warningPop.GetComponent<RectTransform> ();
		rect.localScale = new Vector3 (m_sizePop.x, m_sizePop.y);
		rect.localPosition = new Vector3 (m_posPop.x, m_posPop.y);

		switch (_judge) {
		case PhotonController.ERROR_NOROOM:
			m_warningPopImage.sprite = m_noRoomPop;
			return;
		case PhotonController.ERROR_NONAME:
			m_warningPopImage.sprite = m_noNamePop;
			return;
		case PhotonController.ERROR_FULLHOUSE:
			m_warningPopImage.sprite = m_fullHousePop;
			return;
		}
	}

	void ReleaseWarningPop ()
	{
        soundMng.PlaySE("WarningPopDownSE");
        Destroy (m_warningPop);
		Destroy (m_warningPopImage);
		m_warningPop = null;
		m_warningPopImage = null;

		EventSystem (true);

		StartFocus ();
	}
	// ------------------------------------------------

	public override void Uninit ()
	{
        initFlag = false;
        soundMng = null;
        Debug.Log ("RoomSceneStateJoinRoom終了");
		m_inputText.text = "";
		gameObject.SetActive (false);
	}

}