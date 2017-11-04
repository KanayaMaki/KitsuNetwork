using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathJumpTest : MonoBehaviour {

	[SerializeField]
	PlayerDeathJump death;

	bool isDeath;

	// Use this for initialization
	void Start () {
		isDeath = false;
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.GetKeyDown(KeyCode.N))
		{
			isDeath = true;
			death.Start(transform.position);
		}

		if(isDeath)
		{
			transform.position = death.UpdatePosition();
		}
	}
}
