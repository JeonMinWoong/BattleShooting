using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPos : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (transform.parent.GetComponent<EnemyHealthTest>().isDeath)
            return;
        if (TrainingController.instance.training != true)
        {
            if (collision.tag == "Player" || (collision.tag == "Enemy" && collision.GetComponent<EnemyHealthTest>().enemyName != transform.parent.GetComponent<EnemyHealthTest>().enemyName))
            {
                transform.parent.GetComponent<EnemyAttackTest>().targetTr = collision.transform;
            }
        }
        else
        {
            if(collision.tag == "Player")
            {
                transform.parent.GetComponent<EnemyAttackTest>().targetTr = collision.transform;
            }
        }
    }

    private void OnTriggerStay(Collider collision)
    {
        if (transform.parent.GetComponent<EnemyHealthTest>().isDeath)
            return;

        if (TrainingController.instance.training != true)
        {
            if (collision.tag == "Player" || (collision.tag == "Enemy" && collision.GetComponent<EnemyHealthTest>().enemyName != transform.parent.GetComponent<EnemyHealthTest>().enemyName))
            {
                if (collision.tag == "Player" && !collision.GetComponent<TestHealth>().isDeath)
                    transform.parent.GetComponent<EnemyAttackTest>().targetTr = null;
                if (transform.parent.GetComponent<EnemyAttackTest>().targetTr == null)
                    transform.parent.GetComponent<EnemyAttackTest>().targetTr = collision.transform;
            }
        }
        else
        {
            if (collision.tag == "Player")
            {
                transform.parent.GetComponent<EnemyAttackTest>().targetTr = collision.transform;
            }
        }
    }

    void OnTriggerExit(Collider collision)
    {
        if (transform.parent.GetComponent<EnemyHealthTest>().isDeath)
            return;
        if (TrainingController.instance.training != true)
        {
            if (collision.tag == "Player" || (collision.tag == "Enemy" && collision.GetComponent<EnemyHealthTest>().enemyName != transform.parent.GetComponent<EnemyHealthTest>().enemyName))
            {
                transform.parent.GetComponent<EnemyAttackTest>().targetTr = null;
            }
        }
        else
        {
            if (collision.tag == "Player")
            {
                transform.parent.GetComponent<EnemyAttackTest>().targetTr = null;
            }
        }
    }

}
