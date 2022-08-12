using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonTest : MonoBehaviourPunCallbacks
{
    public GameObject myPlayer;
    public GameObject playerPrefab;
    public GameObject floor;
    private void Awake()
    {
        myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(floor.transform.position.x, 5.0f, floor.transform.position.x), Quaternion.identity);
        PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
        myPlayer.name = PhotonNetwork.NickName;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Test");
    }
}
