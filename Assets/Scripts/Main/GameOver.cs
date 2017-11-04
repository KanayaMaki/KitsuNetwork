using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour {

	[Tooltip("ゲームオーバーになってから、フェードが始まるまでの秒数")]
	public float waitStartTime = 1.0f;

	[Tooltip("一色のフェードにかかる秒数")]
	public float backFadeTime = 1.0f;

	[Tooltip("一色のフェード完了後に待機する秒数")]
	public float waitFrontFadeTime = 0.0f;

	[Tooltip("画像のフェードにかかる秒数")]
	public float frontFadeTime = 1.0f;

	[Tooltip("ゲームオーバーからステージセレクトへのフェードにかかる秒数")]
	public float gameoverToSelectFadeTime = 1.0f;

	[Tooltip("フェード完了後、ステージセレクトに移るまでの秒数")]
	public float waitToSelectTime = 1.0f;


	Image back;	//フェードの画像
	Image front;    //表に出す画像

	PhotonView photonView;
	InputManager input;

	bool gameoverEnd = false;	//ゲームオーバーを終えるか


	// Use this for initialization
	void Start () {
		back = transform.Find("Back").GetComponent<Image>();
		front = transform.Find("Front").GetComponent<Image>();

		photonView = GetComponent<PhotonView>();
		input = FindObjectOfType<InputManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator UpdateGameoverCoroutine()
	{
		Fade(front, 0.0f);  //前面の画像を透明に
		Fade(back, 0.0f);	//背面の画像も透明に


		float stateTime = 0.0f;	//経過時間を格納する

		//フェード待機中
		while (stateTime <= waitStartTime)
		{
			stateTime += Time.deltaTime;
			yield return null;
		}


		stateTime = 0.0f;

		//バックのフェード中
		while (stateTime <= backFadeTime)
		{
			Fade(back, Mathf.Clamp01(stateTime / backFadeTime));

			stateTime += Time.deltaTime;
			yield return null;
		}


		Fade(back, 1.0f);

		stateTime = 0.0f;

		//フロントのフェード待機中
		while (stateTime <= waitFrontFadeTime)
		{
			stateTime += Time.deltaTime;
			yield return null;
		}


		stateTime = 0.0f;

		//フロントのフェード中
		while (stateTime <= frontFadeTime)
		{
			Fade(front, Mathf.Clamp01(stateTime / frontFadeTime));

			stateTime += Time.deltaTime;
			yield return null;
		}


		Fade(front, 1.0f);


		stateTime = 0.0f;

		/*
		//Aボタンがどこかのプレイヤーで押されていたらシーン遷移
		while(true)
		{
			if (gameoverEnd == true) break;
			
			if(IsDecideGameoverEnd())
			{
				OnEndGameover();
			}
			
			yield return null;
		}
		*/

		//ボタンを押さなくても遷移するように変更
		while (stateTime <= 2.0f)
		{
			stateTime += Time.deltaTime;
			yield return null;
		}


		stateTime = 0.0f;

		//フロントのフェード待機中
		while (stateTime <= gameoverToSelectFadeTime)
		{
			Fade(front, Mathf.Clamp01(1.0f - stateTime / gameoverToSelectFadeTime));

			stateTime += Time.deltaTime;
			yield return null;
		}

		Fade(front, 0.0f);


		stateTime = 0.0f;

		//待機
		while (stateTime <= waitToSelectTime)
		{
			stateTime += Time.deltaTime;
			yield return null;
		}


		EndGameover();

		yield return null;
	}

	bool IsDecideGameoverEnd()
	{
		if (PhotonNetwork.isMasterClient)
		{
			if (input.GetButtonPress(InputManager.KeyData.B))
			{
				return true;
			}
		}

		return false;
	}

	void OnEndGameover()
	{
		photonView.RPC("OnEndGameoverRPC", PhotonTargets.AllViaServer);
	}
	[PunRPC]
	void OnEndGameoverRPC()
	{
		gameoverEnd = true;
	}


	void EndGameover()
	{
		//シーン遷移
		SceneManager.LoadScene("StageSelectScene");
	}


	public void StartGameover()
	{
		back.gameObject.SetActive(true);
		front.gameObject.SetActive(true);

		StartCoroutine(UpdateGameoverCoroutine());
	}

	void Fade(Image image, float alpha)
	{
		Color c = image.color;
		c.a = alpha;
		image.color = c;
	}
}
