using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magadama : MonoBehaviour {

    // フォトン追加
    PhotonView photonView;
	SoundManager soundManager;

	// Use this for initialization
	void Start () {
        photonView = GetComponent<PhotonView>();
		soundManager = FindObjectOfType<SoundManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnTriggerEnter(Collider other)
	{
        if (!photonView.isMine)
            return;

        //もしプレイヤーなら
        if (other.tag == "Player")
        {
            //取られる
            photonView.RPC("Getting", PhotonTargets.AllViaServer);
        }
	}

    //取られたときの処理
    [PunRPC]
    void Getting()
    {
        //ストックを増やす
        FindObjectOfType<MagadamaManager>().Add(1,this.gameObject.GetInstanceID());

		soundManager.PlaySE("Magadama");

		//自身を消去
		Destroy(gameObject);
    }
}
