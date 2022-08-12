using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ID : MonoBehaviourPun , IPunObservable
{
    public PhotonView pV;
    public string playerName;


    private void Start()
    {
        pV = GetComponent<PhotonView>();

        playerName = gameObject.name;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(playerName);
        }
        else
        {
            playerName = (string)stream.ReceiveNext();
            gameObject.name = playerName;
        }
    }
}
