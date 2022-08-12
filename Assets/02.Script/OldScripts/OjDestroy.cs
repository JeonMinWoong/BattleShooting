using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class OjDestroy : MonoBehaviourPun
{
    public float deathTime;
    public PhotonView PV;

    public void Start()
    {
        if (TrainingController.instance.training != true)
            PV.RPC("DestroyRPC", RpcTarget.AllBuffered);
        else
            DestroyRPC();
    }

    [PunRPC]
    public void DestroyRPC()
    {
        StartCoroutine("ObjectDestroy");
    }

    IEnumerator ObjectDestroy()
    {
        yield return new WaitForSecondsRealtime(deathTime);
        Destroy(gameObject);
    }
}
