using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectsActive : MonoBehaviour
{
    GameObject[] childObjects;
    ScreenRangeScript screenRangeScript;

	// Use this for initialization
	void Start ()
    {
        screenRangeScript = GameObject.Find("ScreenManager").GetComponent<ScreenRangeScript>();

        childObjects = new GameObject[transform.childCount];

        int count = 0;
        foreach (Transform child in transform)
        {
            childObjects[count] = child.transform.gameObject;
            childObjects[count].SetActive(false);               // 最初全部止める
            count++;
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        for (int i = 0; i < childObjects.Length; i++)
        {
            // オブジェクトが画面内に入ったら
            if (screenRangeScript.isContains(childObjects[i].transform.position))
                childObjects[i].SetActive(true);

            // 停止が画面端にいったら終了
            if (childObjects[i].transform.Find("Stop").transform.position.x < screenRangeScript.GetLeftWorldPosition())
            {
                childObjects[i].GetComponent<DangerLineManagerScript>().SetMyActive(false);
                Debug.Log(1);
            }
        }
	}
}
