using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class leaf_drop_In : MonoBehaviour
{

	Animator animator;

	// Use this for initialization
	void Start ()
	{
		animator = this.GetComponent<Animator> ();
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public void Drop ()
	{
		animator.SetTrigger ("Fade_In");
	}

}