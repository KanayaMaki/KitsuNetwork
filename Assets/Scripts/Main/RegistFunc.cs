using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistFunc : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Button>().onClick.AddListener(GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManagerScript>().GameStart);
		GetComponent<Button>().onClick.AddListener(GameObject.Find("GameManager").GetComponent<GameManager>().OnGameStart);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
