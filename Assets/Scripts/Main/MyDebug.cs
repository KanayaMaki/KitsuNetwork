using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MyDebug : MonoBehaviour {
	
	// Use this for initialization
	void Start () {

		//リザルト状態へ飛ぶボタンが押された場合
		{
			Button b = transform.Find("Canvas3/ResultButton").GetComponent<Button>();
			b.onClick.AddListener(()=> { GameObject.Find("GameManager").GetComponent<GameManager>().OnGoal(); });
		}

		//ワープするボタンが押された場合
		{
			Button b = transform.Find("Canvas3/SkipButton").GetComponent<Button>();
			b.onClick.AddListener(SkipPosition);
		}

		//リザルトエンドボタンが押された場合
		{
			Button b = transform.Find("Canvas3/ResultEndButton").GetComponent<Button>();
			b.onClick.AddListener(() => { GameObject.Find("GameManager").GetComponent<GameManager>().OnResultEnd(); });
		}

        /*
		//ストックの加減
		{
			Button b = transform.Find("Canvas3/AddStockButton").GetComponent<Button>();
			b.onClick.AddListener(() => { FindObjectOfType<StockManager>().Add(1); });
		}
		{
			Button b = transform.Find("Canvas3/SubStockButton").GetComponent<Button>();
			b.onClick.AddListener(() => { FindObjectOfType<StockManager>().Add(-1); });
		}


		//まが玉の加減
		{
			Button b = transform.Find("Canvas3/AddMagaButton").GetComponent<Button>();
			b.onClick.AddListener(() => { FindObjectOfType<MagadamaManager>().Add(1); });
		}
		{
			Button b = transform.Find("Canvas3/SubMagaButton").GetComponent<Button>();
			b.onClick.AddListener(() => { FindObjectOfType<MagadamaManager>().Add(-1); });
		}
         * */


		//見えるものの変更
		{
			Toggle t = transform.Find("Canvas3/CanSee/Eye").GetComponent<Toggle>();
			t.onValueChanged.AddListener((bool b) => {
				if (b == true)
					FindObjectOfType<GameManager>().SetType(GameManager.CPlayerType.cEye);
			});
		}
		{
			Toggle t = transform.Find("Canvas3/CanSee/Ear").GetComponent<Toggle>();
			t.onValueChanged.AddListener((bool b) => {
				if (b == true)
					FindObjectOfType<GameManager>().SetType(GameManager.CPlayerType.cEar);
			});
		}
		{
			Toggle t = transform.Find("Canvas3/CanSee/Nose").GetComponent<Toggle>();
			t.onValueChanged.AddListener((bool b) => {
				if (b == true)
					FindObjectOfType<GameManager>().SetType(GameManager.CPlayerType.cNose);
			});
		}
	}

	
	// Update is called once per frame
	void Update () {
		
	}

	void SkipPosition()
	{
		Vector3 loc = transform.Find("WarpLocation").position;


		//プレイヤーの位置の変更
		foreach (var p in GameObject.FindGameObjectsWithTag("Player"))
		{
			p.transform.position = loc;
		}

		//カメラの位置の変更
		{
			var c = GameObject.Find("Camera2");
			Vector3 pos = c.transform.position;
			pos.x = loc.x;
			c.transform.position = pos;
		}
	}

	void ChangeStateResult()
	{
		GameObject.Find("GameManager").GetComponent<GameManager>().OnGoal();
	}
}
