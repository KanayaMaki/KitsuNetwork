using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RoomScene;

public class RoomSceneController : MonoBehaviour
{
    [SerializeField]
    private SoundManager soundMng;

	// 現在のステート（これを操作）
	private StateProcessor m_stateProcessor;

	// 各ステート
	public StateSelect m_selectState;
	public StateCreateRoom m_createState;
	public StateJoinRoom m_joinState;
	public StateWait m_waitState;

	private EventSystem m_eventSystem;

    void Awake()
    {
        soundMng.PlayBGM("RoomBGM");
    }

	// 初期化
	void Start ()
	{

		GameObject.Find ("Cherry_Blossom_front").GetComponent<leaf_drop_In> ().Drop ();

		m_eventSystem = GetComponent<EventSystem> ();

		m_stateProcessor = new StateProcessor ();

		EachStateOfInstance ();
		TransitionState_Select ();
		SetDelegate ();

		PhotonController.Instance.NetworkConnect ();
	}

	// 各ステート実体化
	void EachStateOfInstance ()
	{
		var canvas = GameObject.FindObjectOfType<Canvas> ();
		var prefab = Resources.Load<GameObject> ("Prefab/RoomScene/Room_Select");
		m_selectState = GameObject.Instantiate (prefab, canvas.transform).GetComponent<StateSelect> ();
		m_selectState.gameObject.SetActive (true);

		prefab = Resources.Load<GameObject> ("Prefab/RoomScene/Room_NameSet");
		m_createState = GameObject.Instantiate (prefab, canvas.transform).GetComponent<StateCreateRoom> ();
		m_createState.gameObject.SetActive (false);

		prefab = Resources.Load<GameObject> ("Prefab/RoomScene/Room_NameEnter");
		m_joinState = GameObject.Instantiate (prefab, canvas.transform).GetComponent<StateJoinRoom> ();
		m_joinState.gameObject.SetActive (false);

		prefab = Resources.Load<GameObject> ("Prefab/RoomScene/Room_Wait");
		m_waitState = GameObject.Instantiate (prefab, canvas.transform).GetComponent<StateWait> ();
		m_waitState.gameObject.SetActive (false);
	}

	// 各ステートデリゲート登録（遷移通知）
	void SetDelegate ()
	{
		m_selectState.NextState_CreateRoom = TransitionState_CreateRoom;
		m_selectState.NextState_JoinRoom = TransitionState_JoinRoom;

		m_createState.NextState = TransitionState_Wait;
		m_createState.BackState = TransitionState_Select;
		m_createState.EventSystem = EventSystemActive;

		m_joinState.NextState = TransitionState_Wait;
		m_joinState.BackState = TransitionState_Select;
		m_joinState.EventSystem = EventSystemActive;

		m_waitState.NextState = StageSelectLoadScene;
		m_waitState.BackState = TransitionState_Select;
	}

	/// <summary>
	/// 選択シーンへ遷移
	/// </summary>
	public void TransitionState_Select ()
	{
		m_stateProcessor.Uninit ();
		m_stateProcessor.State = m_selectState;
		m_stateProcessor.Init ();
	}

	/// <summary>
	/// ルーム作成シーンへ遷移
	/// </summary>
	public void TransitionState_CreateRoom ()
	{
		m_stateProcessor.Uninit ();
		m_stateProcessor.State = m_createState;
		m_stateProcessor.Init ();
	}

	/// <summary>
	/// ルーム入室シーンへ遷移
	/// </summary>
	public void TransitionState_JoinRoom ()
	{
		m_stateProcessor.Uninit ();
		m_stateProcessor.State = m_joinState;
		m_stateProcessor.Init ();
	}

	/// <summary>
	/// ルーム待機シーンへ遷移
	/// </summary>
	public void TransitionState_Wait ()
	{
		m_stateProcessor.Uninit ();
		m_stateProcessor.State = m_waitState;
		m_stateProcessor.Init ();
	}

	/// <summary>
	/// イベントシステム有効化、無効化
	/// </summary>
	/// <param name="_flag"></param>
	public void EventSystemActive (bool _flag)
	{
		m_eventSystem.enabled = _flag;
	}


	public void StageSelectLoadScene ()
	{

		PhotonController.Instance.LoadSceneRPC ("StageSelectScene");
	}
}
