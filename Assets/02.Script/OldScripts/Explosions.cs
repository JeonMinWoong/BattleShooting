using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Explosions : MonoBehaviourPun
{
    public GameObject player;

    private void Start()
    {
        ObjectSec();
    }

    IEnumerator DamageSec()
    {
        yield return new WaitForSecondsRealtime(0.01f);
        gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }

    [PunRPC]
    public void ObjectSec()
    {
        StartCoroutine("DestroyExplosions");
        StartCoroutine("DamageSec");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if (player != null && other.gameObject.name != player.gameObject.name)
            {
                float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                if (other.GetComponent<TestHealth>().pV.IsMine)
                {
                    TestHealth ph = other.GetComponent<TestHealth>();
                    other.GetComponent<TestHealth>().pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                    
                    if (ph.currentHealth <= 0 && !ph.isDeath)
                    {
                        other.gameObject.GetComponent<TestHealth>().TakeDamageMurderN(player);
                        player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                        ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                    }
                    ph.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                }
            }
        }
        if (other.tag == "Enemy")
        {
            if (TrainingController.instance.training == true)
            {
                float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                other.gameObject.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                //if (other.gameObject.GetComponent<EnemyHealthTest>().currentHealth <= 0)
                //    player.GetComponent<TestShoot>().KillCountPlus();
                other.gameObject.GetComponent<EnemyHealthTest>().ExPushEnd(transform.position, 5f);
            }
            else
            {
                if (other.GetComponent<EnemyHealthTest>().pV.IsMine)
                {
                    EnemyHealthTest eh = other.gameObject.GetComponent<EnemyHealthTest>();
                    float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                    eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                    eh.TakeDamageMurderN(player);
                    if (eh.currentHealth <= 0 && !eh.isDeath)
                    {
                        player.GetComponent<TestShoot>().KillCountPlus();
                        eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                    }
                    eh.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                }
            }
        }
    }

    IEnumerator DestroyExplosions()
    {
        yield return new WaitForSecondsRealtime(4);
        Destroy(gameObject);
    }

}
