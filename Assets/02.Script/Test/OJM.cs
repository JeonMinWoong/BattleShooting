using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OJM : MonoBehaviourPun
{ 
    public static OJM instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<OJM>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static OJM m_instance;

    PhotonView pV;
    [SerializeField]
    GameObject photonCanvas;
    [SerializeField]
    GameObject photonBullet;

    public Transform[] poolingGroup;

    void Start()
    {
        pV = GetComponent<PhotonView>();
        ObjectInit();
        //if (pV.IsMine)
        //    pV.RPC("ObjectInit",RpcTarget.All);
    }

    void ObjectInit()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject damageCanvas = Instantiate(photonCanvas, transform.position, transform.rotation);
            damageCanvas.transform.SetParent(poolingGroup[0]);
            damageCanvas.name = "damageCanvas";
            damageCanvas.SetActive(false);
            GameObject bullet = Instantiate(photonBullet, transform.position, transform.rotation);
            bullet.transform.SetParent(poolingGroup[1]);
            bullet.name = "bullet";
            bullet.SetActive(false);
        }
    }

    public GameObject PoolGet(int childNumber)
    {
        GameObject pool = poolingGroup[childNumber].transform.GetChild(0).gameObject;
        pool.SetActive(true);
        return pool;
    }
}
