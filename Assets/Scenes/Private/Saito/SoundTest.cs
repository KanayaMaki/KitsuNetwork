using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTest : MonoBehaviour {

	SoundManager s;

	// Use this for initialization
	void Start () {
		s = FindObjectOfType<SoundManager>();
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Y))
		{
			s.PlaySE("sound0");
		}
		if (Input.GetKeyDown(KeyCode.U))
		{
			s.PlaySE("sound0", 3.0f);
		}

		if (Input.GetKeyDown(KeyCode.I))
		{
			s.PlayBGM("sound1");
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			s.PlayBGM("sound1", 3.0f);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			s.StopBGM("sound1");
		}
		if (Input.GetKeyDown(KeyCode.L))
		{
			s.StopSE("sound0");
		}
	}
}
