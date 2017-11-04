using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Move : MonoBehaviour
{
    Vector3 startPosition;
    Vector3 endPosition;

    [Tooltip("終了地点")]
    public GameObject endObject;

    [Tooltip("移動量。endObjectが設定されていないなら有効です")]
    public Vector3 moveDelta;

    [Tooltip("一秒間の移動量")]
    public float speed = 1.0f;
    float takeTimeWay;

	Vector3 beforePosition;

    PhotonGameManagerScript photonGameManagerScript;

    // Use this for initialization
    void Start()
    {

        //スタート地点は、現在の位置
        startPosition = transform.position;

        //もしエンド地点を指すオブジェクトがあるなら
        if (endObject)
        {
            //そのオブジェクトの位置をエンド地点にする
            endPosition = endObject.transform.position;
        }
        //ないなら
        else
        {
            //moveDeltaを使う
            endPosition = startPosition + moveDelta;
        }

        takeTimeWay = (endPosition - startPosition).magnitude / speed;

        photonGameManagerScript = GameObject.Find("PhotonGameManager").GetComponent<PhotonGameManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float cos = -Mathf.Cos(photonGameManagerScript.GetGameTime() * 2.0f * Mathf.PI / takeTimeWay);
        float rate = (cos + 1.0f) / 2.0f;

		beforePosition = transform.position;

        transform.position = Vector3.Lerp(startPosition, endPosition, rate);
    }

	public Vector3 GetPositionDelta()
	{
		return beforePosition - transform.position;
	}
}
