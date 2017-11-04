using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartFade : MonoBehaviour {

	[System.Serializable]
	public class TitleSprite
	{
		public Sprite title1;
	}
	[SerializeField]
	List<TitleSprite> titleSprite;

	
	// Use this for initialization
	void Start () {

		GameManager g = FindObjectOfType<GameManager>();

		//スプライト１を変更
		ChangeSprite(transform.Find("Title1"), titleSprite[(int)(g.GetStageNum())].title1);
	}
	
	// Update is called once per frame
	void Update () {
		
		//一番最後に描画する
		transform.SetAsLastSibling();
	}

	//テキストを透明に
	public void FadeText(float alpha)
	{
		//アルファを適正な値に収める
		alpha = Mathf.Clamp01(alpha);

		//透明値を変更
		TransSprite(transform.Find("Title1"), alpha);
	}

	//背景を透明に
	public void FadeBack(float alpha)
	{
		//アルファを適正な値に収める
		alpha = Mathf.Clamp01(alpha);

		//透明値を変更
		TransSprite(transform.Find("Back"), alpha);
	}

	//全て透明に
	public void FadeAll(float alpha)
	{
		FadeText(alpha);
		FadeBack(alpha);
	}


	//スプライトを変更
	public void ChangeSprite(Transform t, Sprite sprite)
	{
		t.GetComponent<UnityEngine.UI.Image>().sprite = sprite;
	}
		 
	//Transformに関連付けられているGameObjectの持つスプライトの透明値を変更
	void TransSprite(Transform t,  float alpha)
	{
		var m = t.GetComponent<UnityEngine.UI.Image>().material;
		m.color = new Color(m.color.r, m.color.g, m.color.b, alpha);
	}
}
