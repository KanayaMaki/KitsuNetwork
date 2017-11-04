using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonController : Photon.MonoBehaviour
{

    private static PhotonController m_Instance;

    public const short ERROR_NOROOM = 0001;
    public const short ERROR_NONAME = 0010;
    public const short ERROR_FULLHOUSE = 0100;
    public const short SUCCESS = 1000;

    public delegate void OnListener();
    public OnListener CreateListener;
    public OnListener OtherConnecedListener;
    public OnListener OtherDisConnecedListener;
    public OnListener OtherUpdateListener;
    public OnListener LeaveListener;

    private PhotonViewController m_photonView;

    private int order = 0;
    public int Order
    {
        get { return order; }
    }

    public static PhotonController Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<PhotonController>();
                if (m_Instance == null)
                {
                    Debug.LogError(typeof(PhotonController) + "がシーン上に存在しません");
                }
            }

            return m_Instance;
        }
    }

    void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            Debug.Log("自分自身を破棄");
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// Photonサーバー接続
    /// </summary>
    public void NetworkConnect()
    {
        PhotonNetwork.ConnectUsingSettings("");
        PhotonNetwork.JoinLobby();

        Debug.Log("Photonサーバー接続開始");
    }
    private void OnJoinedLobby()
    {
        Debug.Log("ロビーに入りました");
        m_photonView = GameObject.Find("PhotonViewController").GetComponent<PhotonViewController>();
    }



    /// <summary>
    /// ルーム作成
    /// </summary>
    /// <param name="_name">ルーム名</param>
    public void CreateRoom(string _name)
    {

        PhotonNetwork.CreateRoom(_name, new RoomOptions() { MaxPlayers = 3 }, null);
        Debug.Log(_name + "部屋作成開始");
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("Master");
        }
    }
    private void OnCreatedRoom()
    {
        RoomInfo roomInfo = PhotonNetwork.room;
        Debug.Log(roomInfo.Name + "部屋作成完了");
        if (PhotonNetwork.isMasterClient)
        {
            CreateListener();
        }
    }

    /// <summary>
    /// ルーム入室
    /// </summary>
    /// <param name="_name">ルーム名</param>
    public short JoinRoom(string _name)
    {
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            return ERROR_NOROOM;
        }

        foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
        {
            if (roomInfo.Name != _name) continue;
            if (roomInfo.PlayerCount == roomInfo.MaxPlayers) return ERROR_FULLHOUSE;

            Debug.Log(_name + "部屋入室開始");
            PhotonNetwork.JoinRoom(_name);
            return SUCCESS;
        }
        return ERROR_NONAME;
    }
    private void OnJoinedRoom()
    {
        RoomInfo roomInfo = PhotonNetwork.room;
        Debug.Log(roomInfo.Name + "部屋入室完了");

        int playerNum = roomInfo.PlayerCount + 1;
        order = playerNum - 1;

        if (!PhotonNetwork.isMasterClient)
        {
            Debug.Log("Client");
            OtherUpdateListener();
        }



    }

    /// <summary>
    /// ルーム退室
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();

    }
    private void OnLeftRoom()
    {
        Debug.Log("ルーム退室");
        LeaveListener();
    }

    /// <summary>
    /// マスターかクライアントか取得
    /// </summary>
    /// <returns></returns>
    public bool IsMaster()
    {
        return PhotonNetwork.isMasterClient;
    }

    /// <summary>
    /// 誰かがルームに入室したときに呼ばれる
    /// </summary>
    /// <param name="_player"></param>
    private void OnPhotonPlayerConnected(PhotonPlayer _player)
    {
        OtherConnecedListener();
    }

    /// <summary>
    /// 誰かがルームに退室したときに呼ばれる
    /// </summary>
    /// <param name="otherPlayer"></param>
    private void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        OtherDisConnecedListener();
    }

    /// <summary>
    /// ルーム内が更新
    /// </summary>
    private void OnReceivedRoomListUpdate()
    {
        //if (!IsMaster()) return;
        if (!PhotonNetwork.inRoom) return;
        if (PhotonNetwork.GetRoomList().Length == 0) return;
        OtherUpdateListener();
    }

    /// <summary>
    /// ルーム取得
    /// </summary>
    /// <returns></returns>
    public Room GetRoomInfo()
    {
        return PhotonNetwork.room;
    }

    public void LoadSceneRPC(string _name)
    {
        m_photonView.LoadScene(_name);
    }
}


