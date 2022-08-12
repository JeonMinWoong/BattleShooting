using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Start : MonoBehaviourPun
{
    public InputField playerName;
    public GameObject nameBg;

    public void OnClickStart()
    {
        if (playerName.text != "")
        {
            PlayerPrefs.SetString("PlayerName", playerName.text);
            PlayerPrefs.Save();
            AudioManager.Instance.PlaySound("Start", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            PhotonNetwork.LoadLevel("Lobby");
        }
        else
            nameBg.SetActive(true);
    }
}
