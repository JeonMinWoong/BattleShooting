using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class bulletCrash : MonoBehaviourPunCallbacks
{
    void Start()
    {
        transform.Rotate(new Vector3(180, 0, 0));
        StartCoroutine("DestroyCrash");
    }

    IEnumerator DestroyCrash()
    {
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }
}
