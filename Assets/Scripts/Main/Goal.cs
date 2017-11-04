using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour {

	//ゴールしたプレイヤーの数
	int goalPlayerNum = 0;

	//プレイヤーの数
	const int maxPlayerNum = 3;

	PhotonView photonView;
	SoundManager soundManager;

    public GameObject[] particles;


	// Use this for initialization
	void Start () {
		photonView = GetComponent<PhotonView>();
		soundManager = FindObjectOfType<SoundManager>();

        for( int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(false);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	//ゴールのトリガーに、オブジェクトが入ったとき
	private void OnTriggerEnter(Collider other)
	{
		//オブジェクトが、ローカルのプレイヤーでないなら、処理をしない
		if(!IsLocalPlayer(other)) return;

		//ローカルのプレイヤーがゴールに入った
		photonView.RPC("OnPlayerEnterGoalRPC", PhotonTargets.AllViaServer);
	}

	//ローカルのプレイヤーがゴールに入った時のイベント
	[PunRPC]
	void OnPlayerEnterGoalRPC()
	{
		//炎を再生する
		GetFire(goalPlayerNum).PlayFire();
        GetFire2(goalPlayerNum).PlayFire();

		//ゴール音を鳴らす
		soundManager.PlaySE("Goal");

		//ゴールしたプレイヤーの数を増やす
		goalPlayerNum += 1;

		//もし全てのプレイヤーがゴールしていたら
		if (goalPlayerNum == maxPlayerNum)
		{
			//全員がゴール
			OnAllPlayerGoal();
		}
	}


	//ゴールのトリガーから、オブジェクトが出たとき
	private void OnTriggerExit(Collider other)
	{
		//オブジェクトが、ローカルのプレイヤーでないなら、処理をしない
		if (!IsLocalPlayer(other)) return;

		//ローカルのプレイヤーがゴールから出た
		photonView.RPC("OnPlayerExitGoalRPC", PhotonTargets.AllViaServer);
	}

	[PunRPC]
	void OnPlayerExitGoalRPC()
	{
		//ゴールしたプレイヤーの数を減らす
		goalPlayerNum -= 1;

		//炎を消す
		GetFire(goalPlayerNum).StopFire();
        GetFire2(goalPlayerNum).StopFire();
	}


	bool IsLocalPlayer(Collider other)
	{
		//もしプレイヤーで
		if(other.tag == "Player")
		{
			//もし自分のローカルで作成したプレイヤーなら
			if(other.gameObject.GetComponent<PhotonView>().isMine)
			{
				//ローカルのプレイヤーである
				return true;
			}
		}

		//ローカルのプレイヤーではない
		return false;
	}


	//Fireコンポーネントを取得する
	// fireNum : 炎の順番。０～
	Fire GetFire(int fireNum)
	{
		return transform.Find("Fires/Fire" + (fireNum + 1)).GetComponent<Fire>();
	}

    Fire2 GetFire2(int fireNum)
    {
        return transform.Find("Fires2/Fire" + (fireNum + 1)).GetComponent<Fire2>();
    }


	//全員がゴールしたとき
	void OnAllPlayerGoal()
	{
		//壁を有効にする
		transform.Find("Wall").gameObject.SetActive(true);

        for (int i = 0; i < particles.Length; i++)
        {
            particles[i].SetActive(true);
        }
	}



	public bool IsAllPlayerGoal()
	{
		if(transform.Find("Wall").gameObject.GetActive() == true)
		{
			return true;
		}
		return false;
	}
}
