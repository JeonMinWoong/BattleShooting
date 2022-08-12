using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DeathZon : MonoBehaviourPunCallbacks
{



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.tag == "Player")
                other.gameObject.GetComponent<TestHealth>().playerOnZon = true;
        }
        else
        {
            if (other.tag == "Enemy")
            {
                if (TrainingController.instance.training != true)
                    other.gameObject.GetComponent<EnemyHealthTest>().enemyOnZon = true;
                else
                    other.gameObject.GetComponent<EnemyHealthTest>().enemyOnZon = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<TestHealth>().playerOnZon = false;
        }
        else
        {
            if (other.tag == "Enemy")
            {
                if (TrainingController.instance.training != true)
                    other.gameObject.GetComponent<EnemyHealthTest>().enemyOnZon = false;
                else
                    other.gameObject.GetComponent<EnemyHealthTest>().enemyOnZon = false;
            }
        }
    }


}
