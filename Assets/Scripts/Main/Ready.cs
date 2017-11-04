using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
public class Ready : MonoBehaviour {

	PhotonView photonView;

	int readyNum;
	List<int> playerID;

	public void ImReady()
	{
		//全員に、準備ができたと伝える
		photonView.RPC("ImReadyRPC", PhotonTargets.AllViaServer, PhotonNetwork.player.ID);
	}
	
	[PunRPC]
	void ImReadyRPC(int pID)
	{
		bool isAdd = true;
		foreach(var i in playerID)
		{
			if(i == pID)
			{
				isAdd = false;
				break;
			}
		}

		if(isAdd)
		{
			playerID.Add(pID);
			readyNum += 1;
		}
	}

	//他のクライアントが準備できるまで待つ状態に
	public void Wait()
	{
		playerID.Clear();
		readyNum = 0;
	}
	

	public int ReadyNum()
	{
		return readyNum;
	}
	public void SetReadyNum(int num)
	{
		readyNum = num;
	}

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		readyNum = 0;
		playerID = new List<int>();
	}

	// Use this for initialization
	void Start () {
		if(FindObjectOfType<Ready>() != this)
		{
			Debug.LogError("Readyコンポーネントが複数存在します");
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
*/


public class Ready : MonoBehaviour
{

	PhotonView photonView;

	int readyNum;
	List<int> playerID;

	public void ImReady()
	{
		//全員に、準備ができたと伝える
		photonView.RPC("ImReadyRPC", PhotonTargets.AllViaServer, PhotonNetwork.player.ID);
	}

	[PunRPC]
	void ImReadyRPC(int pID)
	{
		bool isAdd = true;
		foreach (int p in playerID)
		{
			if (p == pID)
			{
				isAdd = false;
				break;
			}
		}

		if (isAdd)
		{
			playerID.Add(pID);
			readyNum += 1;
			Debug.Log("Ready:" + pID.ToString());
		}
	}

	//他のクライアントが準備できるまで待つ状態に
	public void Wait()
	{
		playerID.Clear();
		readyNum = 0;
	}


	public int ReadyNum()
	{
		return readyNum;
	}
	public void SetReadyNum(int num)
	{
		readyNum = num;
	}

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		readyNum = 0;

		playerID = new List<int>();
	}

	// Use this for initialization
	void Start()
	{
		if (FindObjectOfType<Ready>() != this)
		{
			Debug.LogError("Readyコンポーネントが複数存在します");
		}
	}

	// Update is called once per frame
	void Update()
	{

	}
}
