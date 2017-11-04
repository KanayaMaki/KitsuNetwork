using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCameraRoll : MonoBehaviour {

	Vector3 StartPos = Vector3.zero;     //初期位置
	Vector3 CameraStartPos = Vector3.zero;      //カメラの初期位置
	GameObject cameraObj = null;		//カメラの実態

	// Use this for initialization
	void Start () {
		cameraObj = GameObject.Find("Camera/Camera2");      //ステージすべてでこの命名規則
		StartPos = transform.position;
		CameraStartPos = cameraObj.transform.position;
		//スカイの初期位置調整
		transform.position = new Vector3(CameraStartPos.x + 10.0f, CameraStartPos.y - 6.69f, 173.0f);
	}
	
	// Update is called once per frame
	void Update () {
		//カメラがどれだけ進んだかを調べる
		Vector3 vec = cameraObj.transform.position - CameraStartPos;        //移動量
		vec /= 1.04f;       //割合を下げる
		transform.position = StartPos + vec;
	}
}
