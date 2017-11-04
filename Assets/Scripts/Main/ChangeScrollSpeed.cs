using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeScrollSpeed : MonoBehaviour {

	public float scrollSpeed = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Camera")
		{
			FindObjectOfType<CameraScroll>().scrollSpeed = scrollSpeed;
		}
	}
}
