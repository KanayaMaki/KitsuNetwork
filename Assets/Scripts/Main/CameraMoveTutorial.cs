using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoveTutorial : MonoBehaviour {

	public float accel = 2.0f;  //加速度
	public float deccel = 1.0f;  //加速度
	public float speedMax = 4.0f;	//最高速度

	//移動を制限する
	public bool limitMoveX = false;
	public bool limitMoveY = false;
	public bool limitMoveZ = false;

	float speed = 0.0f;

	public Vector3 locLimitMin = - new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
	public Vector3 locLimitMax = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);

	public float moveDistance = 10.0f;

	public Vector3 targetLoc;

	public GameObject localPlayer;
	

	// Use this for initialization
	void Start()
	{
		
	}

	// Update is called once per frame
	void Update()
	{
		if(localPlayer == null)
		{
			localPlayer = FindObjectOfType<GameManager>().GetLocalPlayer();
		}

		if (localPlayer != null)
		{
			targetLoc = localPlayer.transform.position;
		}

		//目標との差を計算
		Vector3 toTarget = targetLoc - transform.position;
		if (limitMoveX) toTarget.x = 0.0f;
		if (limitMoveY) toTarget.y = 0.0f;
		if (limitMoveZ) toTarget.z = 0.0f;

		//速度の変化
		if(toTarget.magnitude >= moveDistance)
		{
			speed += accel * Time.deltaTime;
		}
		else
		{
			speed -= deccel * Time.deltaTime;
		}
		speed = Mathf.Clamp(speed, 0.0f, speedMax);


		//スクロール量を計算
		float speedMag = speed * Time.deltaTime;
		if(toTarget.magnitude - moveDistance <= speedMag)
		{
			speed = 0.0f;
			speedMag = Mathf.Clamp(toTarget.magnitude - moveDistance, 0.0f, toTarget.magnitude - moveDistance);
		}

		Vector3 moveDelta = toTarget.normalized * speedMag;

		//移動
		transform.position = transform.position + moveDelta;


		if(transform.position.x < locLimitMin.x)
		{
			Vector3 pos = transform.position;
			pos.x = locLimitMin.x;
			transform.position = pos;
			speed = 0.0f;
		}
		if (transform.position.x > locLimitMax.x)
		{
			Vector3 pos = transform.position;
			pos.x = locLimitMax.x;
			transform.position = pos;
			speed = 0.0f;
		}
	}
}
