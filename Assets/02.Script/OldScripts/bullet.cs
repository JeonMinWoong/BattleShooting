using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class bullet : MonoBehaviour
{
    public float bulletSpeed = 30f;
    Rigidbody rigid;
    public float bulletDamage = 5;
    public GameObject player;
    LineRenderer lineRenderer;
    //public PhotonView PV;
    public float pushBullet;
    public float pushSize;
    GameObject ex;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        ex = transform.GetChild(1).gameObject;
    }

    private void FixedUpdate()
    {
        if (!gameObject.activeSelf)
            return;

        rigid.AddForce(transform.forward * bulletSpeed);
        if (transform.position.magnitude > 500.0f)
        {
            StartCoroutine(DestroyBullet());

        }
        if (player != null)
        {
            if (player.GetComponent<TestShoot>().itemType == 1)
            {
                float bulletSpeed = 70 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 70f + bulletSpeed;
                float AttackDmg = 10 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 10 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 0.5f;

            }
            else if (player.GetComponent<TestShoot>().itemType == 2)
            {
                float bulletSpeed = 80 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 80f + bulletSpeed;
                float AttackDmg = 20 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 20 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 1;
            }
            else if (player.GetComponent<TestShoot>().itemType == 3)
            {
                float bulletSpeed = 60 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 60f + bulletSpeed;
                float AttackDmg = 7 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 7 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 0f;
            }
            else if (player.GetComponent<TestShoot>().itemType == 5)
            {
                float bulletSpeed = 50 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 50f + bulletSpeed;
                float AttackDmg = 7 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 7 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 6;
            }
            else if (player.GetComponent<TestShoot>().itemType == 6)
            {
                float bulletSpeed = 120 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 120f+ bulletSpeed;
                float AttackDmg = 75 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 75 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                SniperBulletRPC();
                pushSize = 8;
            }
            else if (player.GetComponent<TestShoot>().itemType == 7)
            {
                float bulletSpeed = 90 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 90f + bulletSpeed;
                float AttackDmg = 5 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 5 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 0;
            }
            else if (player.GetComponent<TestShoot>().itemType == 8)
            {
                float bulletSpeed = 100 * player.GetComponent<PlayerSkill_Specificity>().spBulletSpeed;
                bulletSpeed = 100f + bulletSpeed;
                float AttackDmg = 15 * player.GetComponent<PlayerSkill_Specificity>().spAttackPlus;
                bulletDamage = 15 + AttackDmg;
                float bulletSize = player.GetComponent<PlayerSkill_Specificity>().spBulletSize;
                gameObject.transform.localScale = new Vector3(0.5f + bulletSize, 0.5f + bulletSize, 0.5f + bulletSize);
                pushSize = 0.6f;
            }
            else
                return;
        }   
    }
    



    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != player)
        {
            AudioManager.Instance.PlaySound("MetalEnemy", transform.position, 1f + Random.Range(-0.1f, 0.1f));
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
        if (other.tag == "Enemy" && other.gameObject != player)
        {
            AudioManager.Instance.PlaySound("MetalEnemy", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            if (TrainingController.instance.training == true)
            {
                other.gameObject.GetComponent<EnemyHealthTest>().TakeDamage(bulletDamage);
                other.gameObject.GetComponent<EnemyHealthTest>().TakeDamageMurderN(player);
                if (player.GetComponent<TestShoot>().itemType != 7 && player.GetComponent<TestShoot>().itemType != 3)
                {
                    other.gameObject.GetComponent<EnemyHealthTest>().Push(transform.rotation, pushSize);
                }
            }
            else
            {
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
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Environment")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            rigid.velocity = Vector3.zero;
            ex.SetActive(true);
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Item")
        {
            AudioManager.Instance.PlaySound("MetalWell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            rigid.velocity = Vector3.zero;
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
        //Debug.Log("!");
        transform.SetParent(ObjectManager.instance.poolingGroup[1]);
        transform.GetChild(0).gameObject.SetActive(true);
        lineRenderer.enabled = false;
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void SniperBulletRPC()
    {
        if(gameObject.activeSelf)
            StartCoroutine(SniperBullet());
    }


    public IEnumerator SniperBullet()
    {
        yield return new WaitForSecondsRealtime(0.15f);
        lineRenderer.enabled = true;
    }

}
