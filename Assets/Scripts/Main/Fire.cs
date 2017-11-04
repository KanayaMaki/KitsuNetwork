using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponentInChildren<ParticleSystem>().Stop();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//炎を出す
	public void PlayFire()
	{
		GetComponentInChildren<ParticleSystem>().Play();
	}

	//炎を止める
	public void StopFire()
	{
		GetComponentInChildren<ParticleSystem>().Stop();
	}
}
