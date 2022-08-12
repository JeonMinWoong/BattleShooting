using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingStart : MonoBehaviour
{
    LobbyManager lobbyManager;

    private void Start()
    {
        lobbyManager = FindObjectOfType<LobbyManager>();
    }

    public void OnClick()
    {
        lobbyManager.isTraining = true;
        PhotonNetwork.Disconnect();
        //PhotonNetwork.OfflineMode = true;
        AuthManager.instance.userTraining = true;
        AuthManager.instance.OnClickUpdateChildren();
        StartCoroutine(TrainingGo());
    }

    IEnumerator TrainingGo()
    {
        yield return new WaitUntil(()=> !PhotonNetwork.IsConnected);
        PhotonNetwork.OfflineMode = true;
        SceneManager.LoadScene("Training");
    }
}
