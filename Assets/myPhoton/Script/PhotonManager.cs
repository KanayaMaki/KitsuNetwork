using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PhotonManager : Photon.MonoBehaviour
{
    public void Start()
    {
        // Photonを利用するための初期設定ロビーに入る
        PhotonNetwork.sendRate = 15;
        PhotonNetwork.sendRateOnSerialize = 15;
	}
}
