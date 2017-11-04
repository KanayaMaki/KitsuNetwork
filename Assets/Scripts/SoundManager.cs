using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public GameObject BGMPrefab;
	public GameObject SEPrefab;

	[System.Serializable]
	public class SoundData
	{
		public string name;
		public AudioClip clip;

		[Tooltip("音の大きさ。最小が0.0、最大が1.0")]
		public float volume = 1.0f;

		[Tooltip("音を再生するまでの遅延")]
		public float delayTime = 0.0f;
	}

	public List<SoundData> soundData;

	// Use this for initialization
	void Start () {
		if (FindObjectOfType<SoundManager>() != this)
		{
			Debug.LogError("SoundManagerが二つ作られています");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}



	public GameObject PlayBGM(string name)
	{
		return PlaySound(name, BGMPrefab);
	}

	public GameObject PlaySE(string name)
	{
		return PlaySound(name, SEPrefab);
	}
	
	GameObject PlaySound(string name, GameObject prefab)
	{
		SoundData s = FindSoundData(name);
		var g = Instantiate(prefab, transform);
		g.name = name;
		g.GetComponent<AudioSource>().clip = s.clip;
		g.GetComponent<AudioSource>().volume = s.volume;
		g.GetComponent<Sound>().delayTime = s.delayTime;

		return g;
	}


	public GameObject PlayBGM(string name, float delayTime)
	{
		return PlaySound(name, BGMPrefab, delayTime);
	}

	
	public GameObject PlaySE(string name, float delayTime)
	{
		return PlaySound(name, SEPrefab, delayTime);
	}

	GameObject PlaySound(string name, GameObject prefab, float delayTime)
	{
		SoundData s = FindSoundData(name);
		var g = Instantiate(prefab, transform);
		g.name = name;
		g.GetComponent<AudioSource>().clip = s.clip;
		g.GetComponent<AudioSource>().volume = s.volume;
		g.GetComponent<Sound>().delayTime = delayTime;

		return g;
	}


	public void StopBGM(string name)
	{
		StopSound(name);
	}
	public void StopBGM(GameObject gameObject)
	{
		StopSound(gameObject);
	}
	public void StopSE(string name)
	{
		StopSound(name);
	}
	public void StopSE(GameObject gameObject)
	{
		StopSound(gameObject);
	}

	void StopSound(string name)
	{
		Transform t = transform.Find(name);
		if (t)
		{
			Destroy(t.gameObject);
		}
	}
	void StopSound(GameObject gameObject)
	{
		Destroy(gameObject);
	}


	SoundData FindSoundData(string name)
	{
		foreach(var c in soundData)
		{
			if(c.name == name)
			{
				return c;
			}
		}
		return null;
	}
}
