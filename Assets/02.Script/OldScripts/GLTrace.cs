using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GLTrace : MonoBehaviourPun
{
    public LayerMask layermask;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != transform.parent.parent.GetComponent<GLbullet>().player)
        {
            transform.parent.parent.GetComponent<GLbullet>().targetTr = other.transform;
            Debug.Log(other);
        }
        else if (other.tag == "Enemy")
        {
            transform.parent.parent. GetComponent<GLbullet>().targetTr = other.transform;
            Debug.Log(other);
        }
        
    }
}
