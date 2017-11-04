using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonViewController : Photon.MonoBehaviour {

    private PhotonView m_photonView;
    public PhotonView PhotonView
    {
        get { return m_photonView; }
    }
	// Use this for initialization
	void Start () {
        //m_photonView = PhotonView.Get(this);
	}

    public void LoadScene(string _name)
    {
       // if (m_photonView.isMine)
        this.photonView.RPC("LoadSceneRPC", PhotonTargets.AllViaServer, _name);
        SceneController scene = SceneController.Instance;
        if(scene.GetActiveNextSceneName(_name) == scene.GetActiveSceneName())
        {
 //           PhotonNetwork.isMessageQueueRunning = true;
        }
    }

    public void Destroy() {
        Destroy(gameObject);
    }


    [PunRPC]
    private void LoadSceneRPC(string _name)
    {
        PhotonNetwork.isMessageQueueRunning = false;
        SceneController.Instance.LoadScene(_name);
    }
}
