using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RoomScene_State;
using UnityEngine.UI;

//  ルーム作成画面
public class StateCreateRoom : RoomSceneState
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

	private GameObject m_prefabKeybord;
	private GameObject m_virtualKeybord;

	private GUIEvent m_focuscreate;
	private GUIEvent m_focusname;

	// 詳細アンダーバー(選んでるボタンの説明)
	private Image m_underBar;
	private Image m_name;
	private Image m_create;
	private Image m_back;


	[SerializeField]
	private Sprite m_create_art_push;
	[SerializeField]
	private Sprite m_back_art_push;
	[SerializeField]
	private Sprite m_create_art;
	[SerializeField]
	private Sprite m_back_art;
	[SerializeField]
	private Sprite m_name_art;
	[SerializeField]
	private Sprite m_name_art_push;

    private bool initFlag = false;

	public override string GetStateName ()
	{
		return "RoomScene_CreateRoom";
	}


	// ------------------------ 初期化 ------------------------
	public override void Init ()
	{
		Debug.Log ("RoomSceneCreateRoom開始");


		if (!gameObject.GetActive ())
			gameObject.SetActive (true);

        soundMng = GameObject.Find("SoundManagerRoom").GetComponent<SoundManager>();

		InitPostion ();

		SetParentOtherObj ();

		SetButtonExec ();
		SetFocusExec ();

		StartFocus ();
	}

	protected override void SetParentOtherObj ()
	{
		// TODO:

		m_pushNNextState = gameObject.transform.GetChild (1).GetComponent<Button> ();
		m_pushBackState = gameObject.transform.GetChild (2).GetComponent<GUIEvent> ();
		m_pushInputName = gameObject.transform.GetChild (3).GetComponent<Button> ();
		m_inputText = m_pushInputName.transform.GetChild (0).GetComponent<Text> ();
		m_focuscreate = gameObject.transform.GetChild (1).GetComponent<GUIEvent> ();
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
        m_pushNNextState.enabled = false;
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

   
	// 作成ボタンが押された時、実行
	void OnButton_Next ()
	{
        // TODO:ルーム作成
        if (soundMng == null) return;
        PhotonController.Instance.CreateRoom (m_inputText.text);
        soundMng.PlaySE("DecisionSE");

        NextState ();
	}
	// 戻るが押された時、実行
	void OnButton_Back ()
	{
        // TODO:
        if (soundMng == null) return;
        Debug.Log ("選択画面に戻る");
        soundMng.PlaySE("CancelSE");
		BackState ();
	}

	// ルーム作成ボタンが選択された時、実行
	void OnFocus_Create ()
	{
        // TODO:
        if (soundMng == null) return;
        soundMng.PlaySE("CursorSE");
		m_create.sprite = m_create_art_push;
		m_back.sprite = m_back_art;
		m_name.sprite = m_name_art;

	}

	// 戻るが選択された時、実行
	void OnFocus_Back ()
	{
        // TODO:
        if (!gameObject.GetActive()) return;
        if (soundMng == null) return;
        initFlag = true;
        soundMng.PlaySE("CursorSE");
		m_create.sprite = m_create_art;
		m_back.sprite = m_back_art_push;
		m_name.sprite = m_name_art;
	}

	// 名前入力が選択された時、実行
	void OnFocus_Name ()
	{
        // TODO:
        if (!gameObject.GetActive()) return;
        if (soundMng == null) return;
        if (initFlag)
        {
            soundMng.PlaySE("CursorSE");
        }
		m_create.sprite = m_create_art;
		m_back.sprite = m_back_art;
		m_name.sprite = m_name_art_push;
	}


	// 名前入力ボタンが押された時、実行
	void OnButton_InputName ()
	{
        // 仮想キーボード生成
        // その他入力を止める
        if (soundMng == null) return;
        if (!m_virtualKeybord.GetActive ()) {
            initFlag = false;
            soundMng.PlaySE("DecisionSE");
			m_virtualKeybord.SetActive (true);
			m_virtualKeybord.GetComponent<VirtualKeybord> ().Str = InputEnd;

			EventSystem (false);
		}

	}


	// 入力された文字列を受け取り選択イベントをアクティブにする
	void InputEnd (string _str)
	{
		m_inputText.text = _str;

        if(!string.IsNullOrEmpty(m_inputText.text) && m_inputText.text.Length != 0)
        {
            m_pushNNextState.enabled = true;
        }
        else
        {
            m_pushNNextState.enabled = false;
        }
		EventSystem (true);
		StartFocus ();
	}


	public override void Uninit ()
	{
        initFlag = false;
        soundMng = null;
		Debug.Log ("RoomSceneStateCreateRoom終了");
		m_inputText.text = "";
		gameObject.SetActive (false);
	}

}
