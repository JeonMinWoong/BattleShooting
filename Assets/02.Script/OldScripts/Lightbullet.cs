using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Lightbullet : MonoBehaviour
{
    public float bulletSpeed = 75f;
    public Rigidbody rigid;
    public float bulletDamage = 20;
    public GameObject player;
    public float pushBullet;
    public float pushSize;
    public LayerMask layermask;
    public GameObject ex;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        ex = transform.GetChild(1).gameObject;
    }

    private void FixedUpdate()
    {
        if(!ex.activeSelf)
            rigid.AddForce(transform.forward * bulletSpeed);

        if (transform.position.magnitude > 500.0f)
        {
            StartCoroutine(DestroyBullet());
        }

        if (player != null)
        {
            if (player.GetComponent<TestShoot>().itemType == 10)
            {
                float bulletSpeed = 75 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 75f + bulletSpeed;
                float AttackDmg = 25 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 25 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(1.5f + bulletSize, 1.5f + bulletSize, 1.5f + bulletSize);
                pushSize = 10f;

            }
            else
                return;
        }   
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (player != null && other.gameObject.name != player.gameObject.name)
            {
                AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                ex.SetActive(true);
                RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
                foreach (RaycastHit hitObj in rayhists)
                {
                    if (hitObj.collider.tag == "Player" && other.gameObject != player)
                    {
                        if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                        {
                            TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(player);
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            ph.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                        }
                    }
                    else if (hitObj.collider.tag == "Enemy")
                    {
                        if (TrainingController.instance.training != true)
                        {
                            if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                            {
                                EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                                eh.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                                eh.TakeDamageMurderN(player);
                                if (eh.currentHealth <= 0 && !eh.isDeath)
                                {
                                    player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                    eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                                }
                                eh.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                            }
                        }
                        else
                        {
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                            hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                            hitObj.transform.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize);
                        }
                    }
                }
                StartCoroutine(DestroyBullet());
            }
        }
        if (other.tag == "Enemy" && player != null)
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                        
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                        hitObj.transform.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Environment")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                        {
                            TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(player);
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            ph.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                        }
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                        hitObj.transform.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Item")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                        
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                        hitObj.transform.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
    }

    public void SetBullet(GameObject host, Vector3 pos, Quaternion quater)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.SetParent(transform.parent.parent);
        transform.position = pos;
        transform.rotation = quater;
        player = host;
    }

    IEnumerator DestroyBullet()
    {
        rigid.velocity = Vector2.zero;
        transform.GetChild(0).gameObject.SetActive(false);

        while (ex.activeSelf)
        {
            //Debug.Log("?");
            yield return null;
        }
        //Debug.Log("!");
        transform.SetParent(ObjectManager.instance.poolingGroup[4]);
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
