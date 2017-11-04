using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObject : MonoBehaviour {


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/// <summary>
	/// キツネさんたちの初期位置を設定する
	/// </summary>
	/// <param name="PlayerType"></param>
	public void SetPos(PlayerController.Type PlayerType,Vector3 Pos) {
		//自分の子供情報を取得する
		string ChildName = "FOX";
		transform.position = new Vector3(Pos.x, Pos.y, 0);		//キャラクターコントローラの座標を変更
		GameObject Child = transform.Find(ChildName).gameObject;
		int No = (int)GetComponent<PlayerController>().m_Type;
		GetComponent<PlayerController>().PlayerNo = No;       //プレイヤーの種類を設定する
		Child.transform.position = Pos;        //プレイヤーが操作するキャラなら一番前に押し出す
	}
}