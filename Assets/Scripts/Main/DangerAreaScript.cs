using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerAreaScript : MonoBehaviour
{
    public GameObject dangerAreaObject;

    // 各種調整用パラメータ
    [Tooltip("チェックをつけると縦方向")]
    [SerializeField]
    private bool isVertical = true;       // 垂直方向の移動

    [Tooltip("敵オブジェクトの移動速度(小数点以下も可能)")]
    [SerializeField]
    private float moveSpeed = 7.5f;         // 進行速度

    [Tooltip("生成時間(秒数)小数点以下も可能")]
    [SerializeField]
    private float createTimerSecond = 5.0f; // 生成時間（秒）

    PhotonView photonView;

	// Use this for initialization
	void Start ()
    {
        GetComponent<MeshRenderer>().enabled = false;
        photonView = GetComponent<PhotonView>();
	}
	
    void OnTriggerEnter()
    {
        if (!photonView.isMine)
            return;

        photonView.RPC("TriggerEnterRPC", PhotonTargets.AllViaServer);
    }

    [PunRPC]
    void TriggerEnterRPC()
    {
        GameObject go = GameObject.Instantiate(dangerAreaObject, this.transform.position, Quaternion.identity);
        DangerLineManagerScript dangerLineManager = go.GetComponent<DangerLineManagerScript>();

        dangerLineManager.SetVertical(isVertical);
        dangerLineManager.SetMoveSpeed(moveSpeed);
        dangerLineManager.SetCreateTimerSpeed(createTimerSecond);
        dangerLineManager.PlayOnce();

        Destroy(this.gameObject);
    }
}
