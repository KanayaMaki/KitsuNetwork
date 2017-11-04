using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NestPrefabUI : NestPrefab {

	// Use this for initialization
	public override void Awake () {
		GameObject canvas = GameObject.Find("Canvas");

		if(canvas)
		{
			foreach (var d in data)
			{
				if (d.prefab == null) continue;

				d.instance = Instantiate(d.prefab, canvas.transform, false);
				d.instance.name = d.name;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
