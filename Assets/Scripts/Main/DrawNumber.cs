using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DrawNumber : MonoBehaviour {

	public int num = 0;

	[System.Serializable]
	public class SpriteData
	{
		public Sprite sprite;
		public float offset = 0.0f;
		public Vector2 imageSize = new Vector2(0.0f, 0.0f);
	}

	public List<SpriteData> numSprite;

	public Vector2 imageSize = new Vector2(50.0f, 50.0f);
	public float scale = 1.0f;
	public Vector2 basePos;


	private void OnGUI()
	{
		//テキストの描画位置を求める
		List<SpriteData> sp = GetSpriteData(num.ToString());

		foreach (var s in sp)
		{
			basePos = DrawSprite(s, basePos);
		}
	}


	Vector2 DrawSprite(SpriteData s, Vector2 drawPos)
	{
		//drawPosが左端になるように画像を描画
		Vector2 size = s.imageSize * scale;
		if (size.x <= 0.001f)
		{
			size = imageSize * scale;
		}

		GUI.DrawTexture(new Rect(drawPos - new Vector2(0.0f, size.y / 2.0f), size), s.sprite.texture, ScaleMode.ScaleToFit, true, 0.0F);

		//描画した画像のサイズと、オフセット値分をずらした座標を返す
		drawPos.x += (s.offset + size.x) * scale;
		return drawPos;
	}

	List<SpriteData> GetSpriteData(string s)
	{
		List<SpriteData> sprite = new List<SpriteData>();

		foreach (var c in s)
		{
			switch (c)
			{
				case '0':
					sprite.Add(numSprite[0]);
					break;
				case '1':
					sprite.Add(numSprite[1]);
					break;
				case '2':
					sprite.Add(numSprite[2]);
					break;
				case '3':
					sprite.Add(numSprite[3]);
					break;
				case '4':
					sprite.Add(numSprite[4]);
					break;
				case '5':
					sprite.Add(numSprite[5]);
					break;
				case '6':
					sprite.Add(numSprite[6]);
					break;
				case '7':
					sprite.Add(numSprite[7]);
					break;
				case '8':
					sprite.Add(numSprite[8]);
					break;
				case '9':
					sprite.Add(numSprite[9]);
					break;
			}
		}

		return sprite;
	}
}
