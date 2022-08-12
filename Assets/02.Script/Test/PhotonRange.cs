using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Contains("Player"))
            transform.parent.GetComponent<PhotonPlayer>().target = other.gameObject;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Contains("Player"))
            transform.parent.GetComponent<PhotonPlayer>().target = null;
    }
}
