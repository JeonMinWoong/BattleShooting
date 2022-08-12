using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonBullet : MonoBehaviourPun
{
    public Rigidbody rigid;
    public GameObject player;
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (!gameObject.activeSelf)
            return;

        rigid.AddForce(transform.forward * 20);

        if (transform.position.magnitude > 500.0f)
        {
            DestroyBullet();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject != player)
        {
            //AudioManager.Instance.PlaySound("MetalEnemy", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (other.GetComponent<PhotonPlayer>().pV.IsMine)
            {
                other.GetComponent<PhotonPlayer>().pV.RPC("TakeDamage", RpcTarget.All, 5, transform.position);
                player.GetComponent<PhotonPlayer>().pV.RPC("KillCount",RpcTarget.All);
            }
            DestroyBullet();
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

    void DestroyBullet()
    {
        rigid.velocity = Vector2.zero;
        transform.SetParent(OJM.instance.poolingGroup[1]);
        gameObject.SetActive(false);
    }
}
