using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScroll : MonoBehaviour {

	public float scrollSpeed = 1.0f;
	public bool isScroll = true;

    // フォトン追加
    private PhotonView photonView;

	// Use this for initialization
	void Start ()
    {
        // フォトン追加
        photonView = GetComponent<PhotonView>();
        StopScroll();	
	}
	
	// Update is called once per frame
	void Update () {

		//もしスクロールするなら
		if(isScroll == true)
		{
			//スクロール量を計算
			Vector3 scrollDelta = new Vector3(1.0f, 0.0f, 0.0f) * scrollSpeed * Time.deltaTime;

			//移動
			transform.position = transform.position + scrollDelta;
		}
	}

    //************************************************
    // フォトン追加
    [PunRPC]
    private void StartScrollRPC()
    {
        isScroll = true;
    }

    public void StartScroll()
    {
        photonView.RPC("StartScrollRPC", PhotonTargets.AllViaServer);
    }

    private void StopScroll()
    {
        isScroll = false;
    }
    //************************************************
}
