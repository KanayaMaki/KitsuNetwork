using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleBlock : MonoBehaviour {

	[System.Serializable]
	public class Data
	{
		public string name;
		internal GameObject model;
	}
	public List<Data> data;
	public GameManager.CPlayerType canSeePlayer;

	bool visible;
	GameManager gameManager;
	

	// Use this for initialization
	void Start () {
		
		//モデルを担当しているゲームオブジェクトの取得
		foreach(var d in data)
		{
			d.model = transform.Find(d.name).gameObject;
		}

		gameManager = FindObjectOfType<GameManager>();

		ChangePlayer(gameManager.localPlayer.Get());
	}
	
	// Update is called once per frame
	void Update () {
		ChangePlayer(gameManager.localPlayer.Get());
	}

	void ChangePlayer(GameManager.CPlayerType aPlayer)
	{
		visible = true;
		if(canSeePlayer != aPlayer)
		{
			visible = false;
		}

		foreach (var d in data)
		{
			d.model.SetActive(visible);
		}
	}

	public bool Visible()
	{
		return visible;
	}
}
