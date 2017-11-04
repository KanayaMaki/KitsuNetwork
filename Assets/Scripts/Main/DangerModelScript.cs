using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DangerModelScript : MonoBehaviour
{
    // マネージャー
    private GameObject dangerManager;

    // 移動量
    private Vector3 velocity;

    // マネージャースクリプト
    DangerLineManagerScript dangerLineManagerScript;

    // スクリーン内外判定用スクリプト
    ScreenRangeScript screenRangeScript;

    [SerializeField]
    float rotationSpeed = 10.0f;

	// Use this for initialization
	void Start ()
    {
        dangerLineManagerScript = this.transform.parent.parent.GetComponent<DangerLineManagerScript>();
        screenRangeScript = GameObject.Find("ScreenManager").GetComponent<ScreenRangeScript>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        this.transform.position += (velocity * Time.deltaTime);
        this.transform.Rotate(Vector3.forward, rotationSpeed);

        // 画面外にいる
        if (Camera.main.transform.position.x - (screenRangeScript.GetWorldWidth() * 0.5f) > this.transform.position.x)
        {
            dangerLineManagerScript.Reconduct();
        }
        if (Camera.main.transform.position.y - (screenRangeScript.GetWorldHeight() * 0.5f) > this.transform.position.y)
        {
            dangerLineManagerScript.Reconduct();
        }
	}

    public void SetVelocity( bool isHorizontal , float moveSpeed )
    {
        if (isHorizontal)
            velocity = new Vector3(0.0f, -moveSpeed, 0.0f);
        else
            velocity = new Vector3(-moveSpeed, 0.0f, 0.0f);
    }

    public void Create(Vector3 position)
    {
        this.transform.position = new Vector3(position.x,position.y, position.z);
    }

    void OnTriggerStay( Collider other )
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PhotonView>().isMine)
                return;

            other.GetComponent<PlayerController>().SetDeathData();
        }
    }
}
