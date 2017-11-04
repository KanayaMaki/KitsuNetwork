using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpDownBlock : MonoBehaviour {

	public enum CDirection
	{
		cUp,
		cDown
	}

	enum CState
	{
		cStop,
		cMove,
	}

	public float speed = 1.0f;
	public CDirection direction;
	CState state;
	bool stateChanged = true;
	bool stateChangedBuffer;
	
	bool passUp = false;    //上を通り過ぎた
	bool passDown = false;  //下を通り過ぎた
	bool ridePlatform = false;  //足場に乗った

	[SerializeField]
	GameObject hit;	//コリジョンのオブジェクト

    // フォトン追加
    PhotonView photonView;

	// Use this for initialization
	void Start()
	{
        // フォトン追加
        photonView = GetComponent<PhotonView>();
	}

	void Update()
	{
		//状態が変化しているかのバッファから読み出す
		stateChanged = stateChangedBuffer;
		stateChangedBuffer = false;	//リセット

		switch (state)
		{
			case CState.cStop:
				UpdateStop();
				break;

			case CState.cMove:
				UpdateMove();
				break;
		}

		passDown = false;
		passUp = false;
		ridePlatform = false;
	}

	void UpdateStop()
	{
		//初期化
		if (stateChanged)
		{
			
		}

		if (MoveStart())
		{
            // フォトン追加
            photonView.RPC("ChangeStateRPC", PhotonTargets.AllViaServer, CState.cMove);
		}
	}
	void UpdateMove()
	{
		//初期化
		if (stateChanged)
		{

		}

		Vector3 dir = new Vector3();
		if(direction == CDirection.cUp)
		{
			dir = Vector3.up;
		}
		else if (direction == CDirection.cDown)
		{
			dir = Vector3.down;
		}
		transform.position += dir * speed * Time.deltaTime;
	}

	void ChangeState(CState aState)
	{
		state = aState;
		stateChangedBuffer = true;
	}

    [PunRPC]
    void ChangeStateRPC(CState aState)
    {
        ChangeState(aState);
    }

	bool MoveStart()
	{
		switch (direction)
		{
			case CDirection.cUp:
				if(passUp | passDown | ridePlatform)
				{
					return true;
				}
				break;

			case CDirection.cDown:
				if (passDown | ridePlatform)
				{
					return true;
				}
				break;
		}
		return false;
	}

	private void OnTriggerStay(Collider other)
	{
		//もし当たったのがプレイヤーで
		if (other.tag == "Player")
        {
            if (!other.GetComponent<PhotonView>().isMine)
                return;

			//もしプレイヤーが上なら
			if (transform.position.y <= other.transform.position.y)
			{
				//プレイヤーとの間に何もなければ
				if(NoObjectToPlayer(other.gameObject, Vector3.up))
				{
					passUp = true;  //上を通り過ぎた

					//上に乗っているかの判定
					CharacterController c = other.GetComponent<CharacterController>();

					//プレイヤーが上から当たっているなら
					if (c.isGrounded)
					{
						ridePlatform = true;    //足場の上に乗っている
					}
				}
			}
			//下なら
			if (transform.position.y >= other.transform.position.y)
			{
				//プレイヤーとの間に何もなければ
                if (NoObjectToPlayer(other.gameObject, Vector3.down))
                {
                    passDown = true;    //下を通り過ぎた
                }
			}
		}
	}

	bool NoObjectToPlayer(GameObject player, Vector3 toPlayer)
	{
		//プレイヤーからレイを飛ばす
		Vector3 toBlock = -toPlayer;
		Vector3 playerPos = player.transform.position - toBlock.normalized * 0.5f;

		//もしプレイヤーとの間に障害物がなければ
		Vector3 playerBox = new Vector3(1.0f, 0.1f, 1.0f);

		RaycastHit c;

		if(Physics.BoxCast(playerPos, playerBox / 2,  toBlock, out c, Quaternion.identity, 100.0f, LayerMask.GetMask("Platform")))
		{
			//プレイヤーからレイキャストしたときに、一番近いのが自分だったら
			if(c.collider.gameObject == hit)
			{
				return true;    //障害物がない
			}
		}

		return false;
	}
}
