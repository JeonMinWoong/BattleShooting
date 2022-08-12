using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class bulletM : MonoBehaviourPunCallbacks
{
    public float bulletSpeed;
    public Rigidbody rigid;
    public bool crash = false;
    public PhotonView PV;
    public GameObject player;
    public GameObject ex;
    public LayerMask layermask;

    private void Start()
    {
        bulletSpeed = 60;
        rigid = GetComponent<Rigidbody>();
        ex = transform.GetChild(1).gameObject;

    }
    private void FixedUpdate()
    {
        if (player.GetComponent<TestShoot>().itemType == 4)
        {
            float bulletSpeed = 60 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
            bulletSpeed = 60f + bulletSpeed;
            float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
            gameObject.transform.localScale = new Vector3(0.075f + bulletSize * 0.1f, 0.075f + bulletSize * 0.1f, 0.075f + bulletSize * 0.1f);
        }

        if (crash != true)
            rigid.AddForce(transform.forward * bulletSpeed);

        if (transform.position.magnitude > 500.0f)
        {
            StartCoroutine(DestroyBullet());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != player)
        {
            AudioManager.Instance.PlaySound("SMAWE", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            crash = true;
            rigid.velocity = Vector3.zero;
            ex.SetActive(true);
            float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                        
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().ExPush(transform.rotation);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
        else if (other.tag == "Enemy" && player != null)
        {
            AudioManager.Instance.PlaySound("SMAWE", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            crash = true;
            rigid.velocity = Vector3.zero;
            ex.SetActive(true);
            float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                       
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().ExPush(transform.rotation);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Environment" && player != null)
        {
            AudioManager.Instance.PlaySound("SMAWE", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            crash = true;
            rigid.velocity = Vector3.zero;
            ex.SetActive(true);
            float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                        
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().ExPush(transform.rotation);
                    }
                }
            }
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Item" && player != null)
        {
            AudioManager.Instance.PlaySound("SMAWE", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            crash = true;
            rigid.velocity = Vector3.zero;
            ex.SetActive(true);
            float AttackDmg = 45 + 45 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
            RaycastHit[] rayhists = Physics.SphereCastAll(transform.position, 4, Vector3.up, 0, layermask);
            foreach (RaycastHit hitObj in rayhists)
            {
                if (hitObj.collider.tag == "Player" && other.gameObject != player)
                {
                    if (hitObj.collider.GetComponent<TestHealth>().pV.IsMine)
                    {
                        TestHealth ph = hitObj.collider.GetComponent<TestHealth>();
                        ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                       
                        if (ph.currentHealth <= 0 && !ph.isDeath)
                        {
                            ph.TakeDamageMurderN(player);
                            player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                            ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        ph.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                    }
                }
                else if (hitObj.collider.tag == "Enemy")
                {
                    if (TrainingController.instance.training != true)
                    {
                        if (hitObj.collider.GetComponent<EnemyHealthTest>().pV.IsMine)
                        {
                            EnemyHealthTest eh = hitObj.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(player);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                            eh.pV.RPC("ExPush", RpcTarget.All, transform.rotation);
                        }
                    }
                    else
                    {
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hitObj.transform.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                        hitObj.transform.GetComponent<EnemyHealthTest>().ExPush(transform.rotation);
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
        transform.SetParent(ObjectManager.instance.poolingGroup[3]);
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
