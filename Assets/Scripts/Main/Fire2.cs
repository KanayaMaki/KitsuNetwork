using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponentInChildren<SpriteRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//炎を出す
	public void PlayFire()
	{
        GetComponentInChildren<SpriteRenderer>().enabled = true;
	}

	//炎を止める
	public void StopFire()
	{
        GetComponentInChildren<SpriteRenderer>().enabled = false;
	}
}
