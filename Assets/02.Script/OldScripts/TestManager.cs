using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.SceneManagement;

public class TestManager : MonoBehaviourPunCallbacks
{
    private readonly string gameVersion = "Test";

    public TextMeshProUGUI connentionInforText;
    public Button JoinButton;
    public Toggle modeToggle;
    public Toggle modeToggle2;
    public bool isMap;
    public bool isText;
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
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connet()
    {
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

    public void Toggle()
    {
        isMap = modeToggle.isOn;
    }
    public void Toggle2()
    {
        isText = modeToggle2.isOn;
    }
    public override void OnJoinedRoom()
    {
        connentionInforText.text = "Connected with Room";
        Debug.Log("룸으로");
        if(!isMap && !isText)
            PhotonNetwork.LoadLevel("Photon");
        else if(isText && !isMap)
            PhotonNetwork.LoadLevel("Room");
        else
            PhotonNetwork.LoadLevel("BattleTest");
    }
}
