using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGround : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var canvas = GameObject.FindObjectOfType<Canvas>();
        var rect = gameObject.GetComponent<RectTransform>();

        rect.sizeDelta = canvas.GetComponent<RectTransform>().sizeDelta;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
