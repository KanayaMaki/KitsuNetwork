using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameManager : MonoBehaviour {

	Goal goal;
	bool isEnd = false;

	// Use this for initialization
	void Start () {
		goal = FindObjectOfType<Goal>();
	}
	
	// Update is called once per frame
	void Update () {

		//マスタークライアントでないなら処理しない
		if (!IsMasterClient()) return;

		//チュートリアルシーン限定で、プレイヤーが全員ゴール済みなら
		if (goal.IsAllPlayerGoal())
		{
			if(isEnd == false)
			{
				//ゲームマネージャーに、ゴールしたと伝える
				FindObjectOfType<GameManager>().OnTutorialGoal();
			}
			isEnd = true;
		}
	}

	bool IsMasterClient()
	{
		return PhotonNetwork.isMasterClient;
	}

}
