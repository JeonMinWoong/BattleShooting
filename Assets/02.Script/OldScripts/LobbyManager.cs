using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "Test";

    public TextMeshProUGUI connentionInforText;
    public Button JoinButton;
    public GameObject roomOJ;
    public bool isTraining;
    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        JoinButton.interactable = false;
        connentionInforText.text = "Connection To Master Server...";
    }

    public override void OnConnectedToMaster()
    {
        JoinButton.interactable = true;
        connentionInforText.text = "Online";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        JoinButton.interactable = false;
        connentionInforText.text = $"Offline";
        if(!isTraining)
            PhotonNetwork.ConnectUsingSettings();
    }

    public void Connet()
    {
        if (AuthManager.instance.userTraining == false)
            return;

        JoinButton.interactable = false;

        if(PhotonNetwork.IsConnected)
        {
            connentionInforText.text = "Connecting to...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connentionInforText.text = "Offline - Try....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connentionInforText.text = "There is no empty room, Creating new Room";
        PhotonNetwork.CreateRoom(roomName: null, new RoomOptions { MaxPlayers = 8 });
    }

    public override void OnJoinedRoom()
    {
        connentionInforText.text = "Connected with Room";
        Debug.Log("룸으로");
        PhotonNetwork.LocalPlayer.NickName = AuthManager.instance.usernickName.ToString();
        GameObject room = Instantiate(roomOJ, transform.position,Quaternion.identity);
        room.SetActive(true);
        room.GetPhotonView().ViewID = 50;
    }
}
