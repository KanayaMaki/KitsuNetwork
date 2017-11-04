using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIconSprite : MonoBehaviour {

	[System.Serializable]
	public struct SpriteData
	{
		public string name;
		public Sprite sprite;
	}
	public List<SpriteData> sprite;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public Sprite FindSprite(string name)
	{
		foreach (var s in sprite)
		{
			if (s.name == name)
			{
				return s.sprite;
			}
		}
		return null;
	}
}
