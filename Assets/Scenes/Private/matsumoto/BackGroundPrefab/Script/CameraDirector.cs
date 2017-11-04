using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラの速度を動的に管理するためのクラス
/// </summary>
public class CameraDirector : MonoBehaviour {

	CameraScroll cameraScroll = null;       //この中のスクロールスピードをいじる
	GameObject[] characters = new GameObject[3];            //キツネの実態達
	float OffsetX = 0.0f;          //カメラの中心からどれだけ右に行ったら早くなるか？
	float NormalSpeed = 1.0f;
	float UpSpeed = 2.0f;
	bool LoadFlag = false;

	// Use this for initialization
	void Start() {
		cameraScroll = FindObjectOfType<CameraScroll>();
	}

	// Update is called once per frame
	void Update() {
		//プレイヤーを取得するまで探索を続ける
		if (LoadFlag == false) {
			LoadFoxObj();		//まだ読み込まれてなかったらキツネオブジェクトを取得しに行く
		}
		else {
			cameraScroll.scrollSpeed = CameraSpeedControl();		//カメラの速度を調整する
		}
	}

	void LoadFoxObj() {
		PlayerController[] Fox = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
		bool flag = true;
		for (int i = 0; i < 3; i++) {
			if (Fox[i].gameObject != null) {
				characters[i] = Fox[i].gameObject;
			}
			else
				flag = false;       //一人でも見つからなかったらフラグをオフにする
		}
		if (flag) {
			LoadFlag = true;
		}
	}

	float CameraSpeedControl() {
		//３匹が指定ラインを超えていたら速度を上げる
		for(int i = 0; i < 3; i++) {
			//キャラの位置が指定ラインを超えていなければ普通の速度とする
			if (transform.position.x + OffsetX > characters[i].transform.position.x)
				return NormalSpeed;
		}
		//すべてのキャラが指定ラインを超えていれば速度を上げる
		return UpSpeed;
	}
}
