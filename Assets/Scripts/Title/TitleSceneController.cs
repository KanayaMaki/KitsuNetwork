using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneController : MonoBehaviour
{
	[SerializeField]
	private SoundManager soundMng;
	[SerializeField]

	void Awake()
	{
		Debug.Log("TitleScene Awake");
		soundMng.PlayBGM("TitleBGM");
	}
	// Use this for initialization
	void Start()
	{
		Debug.Log("TitleScene Start");
	}

	// Update is called once per frame
	void Update()
	{
		if (InputController.Instance.GetAnyButton())
		{
			// シーン遷移
			soundMng.PlaySE("ButtonSE");

			SceneController.Instance.LoadScene("RoomScene");
		}
	}
}
