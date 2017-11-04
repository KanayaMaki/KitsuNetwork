using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEnemy : MonoBehaviour {

	GameObject ui;
	void ActiveWarning()
	{
		//もし見えるプレイヤーなら
		if (visibleWarning.Get() == true)
		{
			//見えるようにする
			ui.SetActive(true);
		}
		else
		{
			ui.SetActive(false);
		}
	}

	public BufferProperty<bool> visibleWarning;

	// Use this for initialization
	void Start () {
		ui = GetComponent<NestPrefab>().FindData("Warning");
		ChangeState(StartCoroutine(StateHide()));
		ActiveWarning();
	}
	
	// Update is called once per frame
	void Update () {
		if(nextState != null)
		{
			//もし現在のシーンがすでにあるなら
			if (nowState != null)
			{
				//止めておく
				StopCoroutine(nowState);
			}

			nowState = nextState;
			nextState = null;
		}
		ActiveWarning();
	}

	private void OnTriggerStay(Collider other)
	{
		playerHit = true;

		if(other.transform.position.x < transform.position.x)
		{
			playerHitDir = -1.0f;
		}
		else
		{
			playerHitDir = 1.0f;
		}
	}
	

	Coroutine nowState;
	Coroutine nextState;

	void ChangeState(Coroutine aNextState)
	{
		//もし次のシーンがすでにあるなら
		if(nextState != null)
		{
			//止めておく
			StopCoroutine(nextState);
		}
		nextState = aNextState;
	}

	IEnumerator StateHide()
	{
		//1フレーム待機
		yield return null;

		//初期化
		playerHit = false;


		//処理
		while (true)
		{
			if (playerHit)
			{
				ChangeState(StartCoroutine(StateAttackStart()));
			}
			yield return null;
		}
	}
	IEnumerator StateAttackStart()
	{
		//1フレーム待機
		yield return null;


		//初期化


		//移動開始地点
		moveStartPos = transform.position;
		moveTargetPos = moveStartPos + Vector3.right * attackDistance * playerHitDir;  //移動で目指す位置

		float moveTime = 0.0f;


		while (true)
		{
			moveTime += Time.deltaTime;

			transform.position = Vector3.Lerp(moveStartPos, moveTargetPos, Mathf.Pow(Mathf.Clamp(moveTime / attackStartTime, 0.0f, 1.0f), 2));

			if (moveTime >= attackStartTime)
			{
				ChangeState(StartCoroutine(StateAttackKeep()));
			}
			yield return null;
		}
	}

	IEnumerator StateAttackKeep()
	{
		//1フレーム待機
		yield return null;


		//初期化
		float moveTime = 0.0f;


		while (true)
		{
			moveTime += Time.deltaTime;

			if (moveTime >= attackKeepTime)
			{
				ChangeState(StartCoroutine(StateAttackEnd()));
			}
			yield return null;
		}
	}

	
	IEnumerator StateAttackEnd()
	{
		//1フレーム待機
		yield return null;


		//初期化
		float moveTime = 0.0f;

		while (true)
		{
			moveTime += Time.deltaTime;

			transform.position = Vector3.Lerp(moveStartPos, moveTargetPos, 1.0f - Mathf.Pow(Mathf.Clamp(moveTime / attackStartTime, 0.0f, 1.0f), 2));

			if (moveTime >= attackEndTime)
			{
				ChangeState(StartCoroutine(StateHide()));
			}
			yield return null;
		}
	}


	public float attackStartTime = 1.0f;
	public float attackKeepTime = 1.0f;
	public float attackEndTime = 1.0f;
	public float attackDistance = 2.0f;
	bool playerHit;
	float playerHitDir;


	Vector3 moveStartPos;   //移動開始地点
	Vector3 moveTargetPos;  //移動で目指す位置
}
