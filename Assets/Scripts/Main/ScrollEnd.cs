using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollEnd : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log("Tanch");

		//侵入してきたオブジェクトがカメラでないなら処理しない
		if (!IsCamera(other)) return;

		//マスタークライアントでないなら処理しない
		if (!IsMasterClient()) return;

		Debug.Log("ScrollEndGoal");
		//ゲームマネージャーに、ゴールしたと伝える
		GameObject.Find("GameManager").GetComponent<GameManager>().OnGoal();
	}

	bool IsCamera(Collider other)
	{
		return other.tag == "Camera";
	}

	bool IsMasterClient()
	{
		return PhotonNetwork.isMasterClient;
	}
}
