using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

	public PlayerController.Type m_PlayerType;      //キャラクターが操作するキツネのタイプ
	public Vector3 InitPos = Vector3.zero;
	public float XOffset = 2.0f;
	public float ZOffset = 2.0f;

	// Use this for initialization
	void Start() {
		//これから必要な機能
		//PlayerManagerが持っている3つのオブジェクトに
		//それぞれのキツネのタイプを持たせる

		//プレイヤーキャラを描画順を前に出すためにキャラクターを認識して
		//そのキャラのモデル座標を前に押し出す

		int Num = transform.childCount;
		int NonPlayCharaCnt = 0;		//操作しないキャラのカウント
		for (int i = 0; i < Num; i++) {
			string ChildName = "PlayerObject" + i;
			GameObject Child = transform.Find(ChildName).gameObject;
			Child.GetComponent<PlayerController>().m_Type = (PlayerController.Type)i;

			// TODO キャラの位置調整する！！
			//プレイヤーの操作キャラと違っていたらカウントする
			float ZPos = 0;
			if ((PlayerController.Type)i != m_PlayerType) {
				ZPos = ZOffset * (float)(NonPlayCharaCnt + 1);		//順番にZ方向を加算する
				NonPlayCharaCnt++;
			}
			else {
				ZPos = ZOffset * 3.0f;											//プレイヤーは一番先頭に表示させる
			}
			Child.GetComponent<PlayerObject>().SetPos(m_PlayerType,new Vector3(InitPos.x - ((float)i * XOffset),InitPos.y, -ZPos));
		}

		Debug.Log("Count=" + Num);
	}

	// Update is called once per frame
	void Update() {

	}
}
