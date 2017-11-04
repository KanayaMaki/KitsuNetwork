using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownDirection : MonoBehaviour {

	public float offsetZ = 0.0f;

	// Use this for initialization
	void Start () {

		Transform scale = transform.parent.Find("Collider");

		if(scale)
		{
			transform.localPosition = new Vector3(scale.localScale.x / 2.0f, -scale.localScale.y / 2.0f, offsetZ);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
