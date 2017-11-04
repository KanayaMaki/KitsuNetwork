using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ___________________Scene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Sceneの変更
		if (Input.GetKeyDown(KeyCode.F12)) {
			SceneManager.LoadScene("MainScene");
		}
	}
}
