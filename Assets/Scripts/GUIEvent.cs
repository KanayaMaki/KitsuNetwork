using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


/// <summary>
/// 長押しのせいで通常のキー入力が反映されてるので分けた方がいい
/// </summary>
public class GUIEvent : MonoBehaviour, ISelectHandler, IMoveHandler
{
	[SerializeField]
	private UnityEvent eventSelect;
	public UnityEvent onSelect
	{
		get { return eventSelect; }
	}
	[SerializeField]
	private UnityEvent eventMove;
	public UnityEvent onMove
	{
		get { return eventMove; }
	}
	[SerializeField]
	private UnityEvent eventLongPress;
	public UnityEvent onLongPress
	{
		get { return eventLongPress; }
	}

	[SerializeField]
	private float pressTimeLimit = 1.0f;
	private float pressTime = 0.0f;
	private bool isPress = false;

	void Awake()
	{
		EventTrigger.Entry pressDown = new EventTrigger.Entry();
		pressDown.eventID = EventTriggerType.PointerDown;
		pressDown.callback.AddListener(value => PressDown());

		//イベントトリガーを追加し、イベントを登録
		EventTrigger trigger = gameObject.AddComponent<EventTrigger>();
		trigger.triggers.Add(pressDown);
	}

	void Update()
	{
		/*
        if (!InputController.Instance.OnDecisionPress()) {
            isPress = false;
            pressTime = 0.0f;
            return;
        }
        */
		if (InputController.Instance.GetButtonPress(InputController.KeyData.B))
		{
			pressTime += Time.deltaTime;
			if (pressTime >= pressTimeLimit)
			{
				eventLongPress.Invoke();
				pressTime = 0.0f;
			}
		}

		if (InputController.Instance.GetButtonRelease(InputController.KeyData.B))
		{
			pressTime = 0.0f;
		}

		/// pressTime += Time.deltaTime;
		/*
		 if(pressTime >= pressTimeLimit)
		 {
			 eventLongPress.Invoke();
			 pressTime = 0.0f;
		 }
		 */
	}

	public void Select()
	{
		EventSystem.current.SetSelectedGameObject(gameObject);
	}

	void PressDown()
	{
		isPress = true;
	}

	// フォーカスが合っている時
	public void OnSelect(BaseEventData data)
	{
		eventSelect.Invoke();
	}

	// フォーカスが移動した時
	public void OnMove(AxisEventData data)
	{
		eventMove.Invoke();
	}

}