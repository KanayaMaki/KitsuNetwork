using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour {

	AudioSource s;
	public float delayTime = 0.0f;

	// Use this for initialization
	void Start () {
		s = GetComponent<AudioSource>();
		StartCoroutine(PlayDelay(delayTime));
	}
	
	// Update is called once per frame
	void Update () {
		//処理はコルーチンで行う
	}

	IEnumerator PlayDelay(float delayTime)
	{
		yield return new WaitForSeconds(delayTime);

		s.Play();

		while(true)
		{
			//再生を終えたら
			if (s.isPlaying == false)
			{
				//サウンドオブジェクトを破棄する
				Destroy(gameObject);
			}
			//1フレームに一回
			yield return null;
		}
	}
}
