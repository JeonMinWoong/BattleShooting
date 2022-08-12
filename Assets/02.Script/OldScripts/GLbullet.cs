using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GLbullet : MonoBehaviour
{
    public float bulletSpeed = 3f;
    public Rigidbody rigid;
    public float bulletDamage = 15;
    public GameObject player;
    public float pushBullet;
    public float pushSize;
    public LayerMask layermask;
    public Transform targetTr;
    public GameObject range;
    public float speed = 5;
    public GameObject ex;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        ex = transform.GetChild(1).gameObject;
    }

    private void FixedUpdate()
    {
        if (targetTr == null)
        {
            rigid.AddForce(transform.forward * bulletSpeed);
        }
        else
        {
            rigid.velocity = Vector3.zero;
            FollowTarget();
        }
        if (transform.position.magnitude > 500.0f)
        {
            StartCoroutine(DestroyBullet());
        }
        if (player != null)
        {
            if (player.GetComponent<TestShoot>().itemType == 11)
            {
                float bulletSpeed = 3 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 3f + bulletSpeed;
                float AttackDmg = 15 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 15 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(2f + bulletSize, 2f + bulletSize, 2f + bulletSize);
                pushSize = 3f;

            }
            else
                return;
        }   
    }

    public void FollowTarget()
    {
        Vector3 heading = targetTr.position - this.transform.position;
        transform.position = Vector3.Lerp(transform.position, new Vector3(targetTr.position.x, transform.position.y,targetTr.position.z), Time.deltaTime * speed);
        transform.LookAt(targetTr);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != player)
        {
            Debug.Log(other.name);
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            if (other.GetComponent<TestHealth>().pV.IsMine)
            {
                TestHealth ph = other.GetComponent<TestHealth>();
                ph.pV.RPC("TakeDamage", RpcTarget.All, bulletDamage);
                
                if (ph.currentHealth <= 0 && !ph.isDeath)
                {
                    ph.TakeDamageMurderN(player);
                    player.GetComponent<TestShoot>().PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                    ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                }
                ph.pV.RPC("Push", RpcTarget.All, transform.rotation, pushSize);
            }
            StartCoroutine(DestroyBullet());
        }
        else if (other.tag == "Enemy" && player != null)
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training != true)
            {
                ex.SetActive(true);
                if (other.GetComponent<EnemyHealthTest>().pV.IsMine)
                {
                    EnemyHealthTest eh = other.GetComponent<EnemyHealthTest>();
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
                ex.SetActive(true);
                other.gameObject.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                other.gameObject.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                if (player.GetComponent<TestShoot>().itemType != 7 && player.GetComponent<TestShoot>().itemType != 3)
                {
                    other.gameObject.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize * 10);
                }
            }
            StartCoroutine(DestroyBullet());
        }
        else if (other.tag == "Environment")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            StartCoroutine(DestroyBullet());
        }
        else if (other.tag == "Item")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
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
        targetTr = null;
        transform.SetParent(ObjectManager.instance.poolingGroup[5]);
        gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
    }

}
