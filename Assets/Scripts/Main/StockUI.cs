using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StockUI : MonoBehaviour {

	int oldStockNum = -1;
	public int stockNum = 2;
	
	[System.Serializable]
	public class SpriteData
	{
		public Sprite sprite;
		public float offset = 0.0f;
	}

	public List<SpriteData> num;

	public GameObject imageObject;
	

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		//ストックが変更されているなら
		if(oldStockNum != stockNum)
		{
			//ストックが変更したときのイベント
			ChangeStock(stockNum);
		}
	}


	public void ChangeStock(int aStockNum)
	{
		//ストック値の変更
		oldStockNum = stockNum;
		stockNum = aStockNum;

		//数字に対応するゲームオブジェクトを作成する
		CreateSpriteObjects();
	}

	void CreateSpriteObjects()
	{
		//全てのスプライトオブジェクトを削除する
		for(int i = 0; i < transform.Find("Num").childCount; i++)
		{
			Destroy(transform.Find("Num").GetChild(i).gameObject);
		}


		//テキストの作成開始位置を求める
		Vector3 tPos = transform.Find("Num").GetComponent<RectTransform>().position;

		List<SpriteData> sp = GetSpriteData(stockNum.ToString());

		foreach (var s in sp)
		{
			tPos = CreateSpriteObject(s, tPos);
		}
	}

	Vector3 CreateSpriteObject(SpriteData s, Vector3 pos)
	{
		//ゲームオブジェクトを作成
		GameObject g = Instantiate(imageObject, transform.Find("Num"));

		//位置を移動
		g.GetComponent<RectTransform>().position = pos;

		//スプライトをセット
		g.GetComponent<Image>().sprite = s.sprite;

		//描画した画像のサイズと、オフセット値分をずらした座標を返す
		pos.x += s.offset + g.GetComponent<RectTransform>().sizeDelta.x;
		return pos;
	}

	List<SpriteData> GetSpriteData(string s)
	{
		List<SpriteData> sprite = new List<SpriteData>();

		foreach(var c in s) {
			switch (c)
			{
				case '0':
					sprite.Add(num[0]);
					break;
				case '1':
					sprite.Add(num[1]);
					break;
				case '2':
					sprite.Add(num[2]);
					break;
				case '3':
					sprite.Add(num[3]);
					break;
				case '4':
					sprite.Add(num[4]);
					break;
				case '5':
					sprite.Add(num[5]);
					break;
				case '6':
					sprite.Add(num[6]);
					break;
				case '7':
					sprite.Add(num[7]);
					break;
				case '8':
					sprite.Add(num[8]);
					break;
				case '9':
					sprite.Add(num[9]);
					break;
			}
		}

		return sprite;
	}
}
