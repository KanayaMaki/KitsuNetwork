using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugPhoton : MonoBehaviour
{
    public string roomName;
    public Text text;
    public GameObject inputField;
    private InputField inputFieldScript;

    public void ConnectPhotonEnter()
    {
        // Photonを利用するための初期設定ロビーに入る
        PhotonNetwork.ConnectUsingSettings("v1.0");
        PhotonNetwork.sendRate = 15;
        PhotonNetwork.sendRateOnSerialize = 15;
    }
    public void LobbyInEnter()
    {
        // ロビーに入ります
        PhotonNetwork.JoinLobby();
    }
    public void InputLogger()
    {
        inputFieldScript = inputField.GetComponent<InputField>();
        roomName = inputFieldScript.text;
        
    }
    public void InputJoinLogger()
    {
        InputField inputfield = GameObject.Find("JoinRoomInput").GetComponent<InputField>();
        roomName = inputfield.text;
    }
    public void CreateRoomEnter()
    {
        // 部屋のオプション
        RoomOptions roomOp = new RoomOptions();
        roomOp.IsVisible = true;
        roomOp.IsOpen = true;
        roomOp.MaxPlayers = 3;

        // ロビー内に部屋を作ります。
        if (PhotonNetwork.CreateRoom(roomName,roomOp,null))
        {
            Debug.Log(roomName + "名の部屋を作成しました。");
        }
        else
        {
            Debug.Log("部屋作成に失敗しました。");
        }
    }
    public void JoinRoomEnter()
    {
        // ルームに入ります。
        if (PhotonNetwork.JoinRoom(roomName))
        {
            Debug.Log("入室");
        }
        else
        {
            Debug.Log("入室失敗");
        }
    }
    public void LeftRoomEnter()
    {
        // ルームから退出
        PhotonNetwork.LeaveRoom();
    }
    public void DisconnectEnter()
    {
        // 通信を切断
        PhotonNetwork.Disconnect();
    }

    // Photonに接続（Auto-Join Lobby無効）
    public void OnConnectedToMaster()
    {
        Debug.Log("Photon接続状態");
    }
    // マスターサーバー上のロビーに入った際に呼ばれる（Auto-Join Lobby有効）
    public void OnJoinedLobby()
    {
        Debug.Log("Photonロビーに入った");
    }

    public void OnJoinedRoom()
    {
        Debug.Log(roomName + "ルームに入りました。");
        text.text = "ルーム名：" + roomName;
    }

    public void OnPhotonPlayerConnected()
    {
        Debug.Log("誰かがルーム内に入った。");
    }
}
