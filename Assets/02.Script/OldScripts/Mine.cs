using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Mine : MonoBehaviourPun
{
    public GameObject player;
    public LayerMask layerMask;
    public PhotonView PV;
    public MeshRenderer mesh;
    public GameObject mineEx;

    public void Start()
    {
        if (player == null)
            mesh.material.color = new Color(0, 0, 0, 0.03f);
        else
            mesh.material.color = new Color(0, 0, 0, 0.5f);
        mineEx = transform.GetChild(1).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player != null && other.gameObject.name != player.gameObject.name)
            {
                AudioManager.Instance.PlaySound("Mine", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                mineEx.SetActive(true);
                RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 5, Vector3.up, 0, layerMask);
                foreach (RaycastHit hitObj in rayhists)
                {
                    if (hitObj.collider.tag == "Player")
                    {
                        TestHealth ph = hitObj.transform.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 30f);
                        
                        ph.pV.RPC("ExPushEnd", RpcTarget.All, transform.position, 3f);
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                    }
                    else if (hitObj.collider.tag == "Enemy")
                    {
                        if (TrainingController.instance.training != true)
                        {
                            EnemyHealthTest eh = hitObj.transform.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 30f);
                            eh.TakeDamageMurderN(player);
                            //hitObj.transform.GetComponent<EnemyHealthTest>().StartCoroutine("ExTakePush", 2f);
                            eh.pV.RPC("ExPushEnd", RpcTarget.All, transform.position, 3f);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else
                        {
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(30f);
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                            hitObj.transform.GetComponent<EnemyHealthTest>().ExPushEnd(transform.position, 3f);
                        }
                    }
                }
                StartCoroutine(Destroy());
            }
        }
        else if (other.tag == "Enemy")
        {
            AudioManager.Instance.PlaySound("Mine", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            mineEx.SetActive(true);
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 5, Vector3.up, 0, layerMask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player")
                {
                    TestHealth ph = hitObj.transform.GetComponent<TestHealth>();
                    ph.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 30f);
                    
                    ph.pV.RPC("ExPushEnd", RpcTarget.All, transform.position, 5);
                    if (ph.currentHealth <= 0 && !ph.isDeath)
                    {
                        ph.TakeDamageMurderN(player);
                        player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                        ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        EnemyHealthTest eh = hitObj.transform.GetComponent<EnemyHealthTest>();
                        eh.pV.RPC("TakeDamage", RpcTarget.AllBuffered, 30f);
                        eh.TakeDamageMurderN(player);
                        eh.pV.RPC("ExPushEnd", RpcTarget.All, transform.position, 5);
                        if (eh.currentHealth <= 0 && !eh.isDeath)
                        {
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(30f);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        //hitObj.transform.GetComponent<EnemyHealthTest>().StartCoroutine("ExTakePush", 2f);
                        hitObj.transform.GetComponent<EnemyHealthTest>().ExPushEnd(transform.position, 3f);
                    }
                    
                }
            }
            StartCoroutine(Destroy());
        }
        else if (other.tag == "Environment")
        {
            AudioManager.Instance.PlaySound("MineSet", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        }
    }

    IEnumerator Destroy()
    {
        transform.GetChild(0).gameObject.SetActive(false);

        while (mineEx.activeSelf)
        {
            //Debug.Log("?");
            yield return null;
        }
        //Debug.Log("!");
        transform.GetChild(0).gameObject.SetActive(true);
        Destroy(gameObject);
    }
}
