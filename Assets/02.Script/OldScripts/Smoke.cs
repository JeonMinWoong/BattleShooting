using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Smoke : MonoBehaviourPun
{
    public PhotonView PV;
    public GameObject[] enemyAi;
    public int enemys;

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
        StartCoroutine("Destroy");
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSecondsRealtime(10f);
        gameObject.SetActive(false);
        for (int i = 0; i < enemys; i++)
        {
            enemyAi[i].gameObject.GetComponent<CapsuleCollider>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            if (other.GetComponent<EnemyAttackTest>() == null)
                return;

            other.GetComponent<EnemyAttackTest>().targetTr = null;
        }

        if (other.tag == "EnemyAi")
        {
            other.GetComponent<CapsuleCollider>().enabled = false;
            enemyAi[enemys] = other.gameObject;
            enemys++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "EnemyAi")
        {
            other.GetComponent<CapsuleCollider>().enabled = true;
        }
    }
}
