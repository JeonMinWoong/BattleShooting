using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class EnemyBullet : MonoBehaviourPun
{
    public float bulletSpeed;
    public Rigidbody rigid;
    public float bulletDamage = 10;
    public GameObject ex;
    public GameObject host;

    private void Start()
    {
        bulletSpeed = 100;
        rigid = GetComponent<Rigidbody>();
        ex = transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        if (gameObject.activeSelf == true)
        {
            rigid.AddForce(transform.forward * bulletSpeed);
            if (transform.position.magnitude > 500.0f)
            {
                rigid.velocity = Vector3.zero;
                StartCoroutine(DestroyBullet());
            }
        }
        else
            rigid.velocity = Vector3.zero;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            AudioManager.Instance.PlaySound("EnemyBulletEx", transform.position, 1f + Random.Range(-0.1f, 0.1f));

            if (other.tag == "Player")
            {
                if (TrainingController.instance.training != true)
                {
                    if (other.GetComponent<TestHealth>().pV.IsMine)
                    {
                        other.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, bulletDamage);
                        
                        other.gameObject.GetComponent<PhotonView>().RPC("Push", RpcTarget.All, transform.rotation, 5f);
                        if (other.gameObject.GetComponent<TestHealth>().currentHealth <= 0 && !other.gameObject.GetComponent<TestHealth>().isDeath)
                        {
                            other.gameObject.GetComponent<TestHealth>().TakeDamageMurderN(host);
                            other.gameObject.GetComponent<TestHealth>().pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                    }
                }
                else
                {
                    other.gameObject.GetComponent<TestHealth>().TakeDamage(bulletDamage);
                    other.gameObject.GetComponent<TestHealth>().Push(transform.rotation, 5f);

                }

            }
            else if (other.tag == "Enemy")
            {
                if (TrainingController.instance.training != true)
                {
                    if (other.GetComponent<EnemyHealthTest>().pV.IsMine)
                    {
                        EnemyHealthTest eh = other.gameObject.GetComponent<EnemyHealthTest>();
                        eh.pV.RPC("TakeDamage", RpcTarget.AllBuffered, bulletDamage);
                        eh.TakeDamageMurderN(host);
                        if (eh.currentHealth <= 0 && !eh.isDeath)
                        {
                            eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                        }
                        //eh.pV.RPC("Push", RpcTarget.All, transform.rotation, 10f);
                    }
                }
            }
            ex.SetActive(true);
            rigid.velocity = Vector3.zero;
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Environment")
        {
            AudioManager.Instance.PlaySound("EnemyBulletEx", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            rigid.velocity = Vector3.zero;
            StartCoroutine(DestroyBullet());
        }
        if (other.tag == "Item")
        {
            AudioManager.Instance.PlaySound("EnemyBulletEx", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            ex.SetActive(true);
            rigid.velocity = Vector3.zero;
            StartCoroutine(DestroyBullet());
        }
    }

    public void SetBullet(GameObject _host, Vector3 pos, Quaternion quater)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        transform.position = Vector3.zero;
        transform.SetParent(transform.parent.parent);
        transform.position = pos;
        transform.rotation = quater;
        host = _host;
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
        transform.SetParent(ObjectManager.instance.poolingGroup[0]);
        transform.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

}
