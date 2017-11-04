using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerDeathJump
{

	//死んだときの飛び出す速度
	public Vector3 startSpeed = new Vector3(0.0f, 16.0f, 0.0f);

	//死んだときに飛び出すまでの時間
	public float startDelay = 0.5f;

	//死んだときの加速度
	public Vector3 axcel = new Vector3(0.0f, -64.0f, 0.0f);


	//位置の移動を始める
	public void Start(Vector3 aNowPosition)
	{
		nowPosition = aNowPosition;
		nowVelocity = startSpeed;
		takeTime = 0.0f;
	}

	//時間を進める
	public void ForwardTime(float deltaTime)
	{
		takeTime += deltaTime;


		if (takeTime <= 0.5f)
		{
			//何もしない
		}
		else
		{
			//速度の変化
			nowVelocity += axcel * deltaTime;

			//位置の変化
			nowPosition += nowVelocity * deltaTime;
		}
	}

	public Vector3 UpdatePosition()
	{
		//時間を進める
		ForwardTime(Time.deltaTime);

		return nowPosition;
	}


	//計算後の現在地点
	Vector3 nowPosition;

	//現在の速度
	Vector3 nowVelocity;

	//経過時間
	float takeTime;
}
