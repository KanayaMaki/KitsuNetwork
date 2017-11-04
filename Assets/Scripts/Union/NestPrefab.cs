using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NestPrefab : MonoBehaviour {

	[System.Serializable]
	public class Data
	{
		public string name;
		public GameObject prefab;
		public Vector3 offset;
		public Vector3 rotation;
		internal GameObject instance;
	}
	public List<Data> data;


	// Use this for initialization
	public virtual void Awake () {
		foreach(var d in data)
		{
			if (d.prefab == null) continue;

			d.instance = Instantiate(d.prefab, transform);
			d.instance.name = d.name;
			d.instance.transform.localPosition += d.offset;
			d.instance.transform.localRotation *= Quaternion.Euler(d.rotation);
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public GameObject FindData(string name)
	{
		foreach (var d in data)
		{
			if(name == d.name)
			{
				return d.instance;
			}
		}
		return null;
	}
}
