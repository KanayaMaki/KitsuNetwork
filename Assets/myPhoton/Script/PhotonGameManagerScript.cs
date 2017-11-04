using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonGameManagerScript : MonoBehaviour
{
    // 1秒に何回データ送信するか
    [SerializeField]
    private float sendNumPerSecond = 60.0f;

    private float gameTime;
    private float sendTimer;

    void Awake()
    {
        PhotonNetwork.isMessageQueueRunning = true;
    }

    void Start()
    {
        gameTime = 0.0f;
        sendTimer = 0.0f;
    }

    void Update()
    {
        this.gameTime += Time.deltaTime;     

        sendTimer += Time.deltaTime;
        if (sendTimer > 1.0f / sendNumPerSecond)
        {
            //データの送信
            if (PhotonNetwork.isMasterClient)
            {
                GetComponent<PhotonView>().RPC("SendGameTime", PhotonTargets.Others, this.gameTime);
            }
            sendTimer = 0.0f;
        }
    }

    [PunRPC]
    public void SendGameTime(float time)
    {
        this.gameTime = time;
    }

    public void GameStart(int r0, int r1, int r2)
    {
        GetComponent<PhotonView>().RPC("GameStartRPC", PhotonTargets.AllViaServer, r0, r1, r2);
    }
	public void GameStart()
	{
		GetComponent<PhotonView>().RPC("GameStartRPC", PhotonTargets.AllViaServer, 0, 1, 2);
	}

	[PunRPC]
    public void GameStartRPC(int r0, int r1, int r2)
    {
        this.gameTime = 0;
        CreatePlayer(r0, r1, r2);
    }

    public float GetGameTime()
    {
        return this.gameTime;
    }

    private void CreatePlayer(int r0, int r1, int r2)
    {
        // Roomに参加しているプレイヤー情報を配列で取得.
        PhotonPlayer[] player = PhotonNetwork.playerList;

        // IDを小さい順に並べる
        int[] idList = new int[3];
        for (int i = 0; i < player.Length; i++)
            idList[i] = player[i].ID;

        // ソート
        for (int i = 0; i < player.Length; i++)
        {
            for (int j = (player.Length - 1); j > i; j--)
            {
                if (idList[j - 1] > idList[j])
                {
                    int temp = idList[j - 1];
                    idList[j - 1] = idList[j];
                    idList[j] = temp;
                }
            }
        }

        // 自分が何番目に小さいか取得
        int number = -1;
        for (int i = 0; i < player.Length; i++)
        {
            if (idList[i] == PhotonNetwork.player.ID)
            {
                number = i;
                break;
            }
        }

		float locY = 10.0f;
		switch(FindObjectOfType<GameManager>().GetStageNum())
		{
			case GameManager.CStageNum.cTutorial:
				locY = 10.0f;
				break;
			case GameManager.CStageNum.cStage1:
				locY = 10.0f;
				break;
			case GameManager.CStageNum.cStage2:
				locY = 10.0f;
				break;
			case GameManager.CStageNum.cStage3:
				locY = 10.0f;
				break;
		}


		var randomList = new List<int> { r0, r1, r2 };

        // 自分の番号に応じて生成
        GameObject go = null;
        switch (randomList[number])
        {
            case 0:
                go = PhotonNetwork.Instantiate("Player/PlayerEar", new Vector3(-4, locY, 0), Quaternion.identity, 0);
                GameObject.Find("GameManager").GetComponent<GameManager>().SetType(GameManager.CPlayerType.cEar);
                break;

            case 1:
                go = PhotonNetwork.Instantiate("Player/PlayerEye", new Vector3(0, locY, 0), Quaternion.identity, 0);
                GameObject.Find("GameManager").GetComponent<GameManager>().SetType(GameManager.CPlayerType.cEye);
                break;

            case 2:
                go = PhotonNetwork.Instantiate("Player/PlayerNose", new Vector3(4, locY, 0), Quaternion.identity, 0);
                GameObject.Find("GameManager").GetComponent<GameManager>().SetType(GameManager.CPlayerType.cNose);
                break;

            default:
                Debug.Log(number);
                break;
        }
		GameObject.Find("GameManager").GetComponent<GameManager>().SetLocalPlayer(go);

		//キャラクターを移動不可に
		go.GetComponent<PlayerController>().SetImmobile();
	}
}
