using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotonItemMake : MonoBehaviourPun
{
    public static PhotonItemMake instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<PhotonItemMake>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static PhotonItemMake m_instance;

    [SerializeField]
    public GameObject itemGroup;
    [SerializeField]
    int itemCount;
    [SerializeField]
    int itemMaxCount;
    [SerializeField]
    PhotonView pV;
    void Start()
    {
        pV = GetComponent<PhotonView>();
        Init();
    }

    void Init()
    {
        PhotonNetwork.InstantiateRoomObject("itemGroup", new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity);
        pV.RPC("ItemGroup", RpcTarget.AllBuffered);
        pV.RPC("ReSpawn", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void ItemGroup()
    {
        if (itemGroup != null)
            return;

        Debug.Log("찾기");
        itemGroup = GameObject.Find("ItemGroup(Clone)");
    }

    [PunRPC]
    void ReSpawn()
    {
        int reSpawnTime = Random.Range(0, 1);
        StartCoroutine(ReSpawnItem(reSpawnTime));
    }
    IEnumerator ReSpawnItem(int _reSpawn)
    {
        int reSpawnType = 0;
        yield return new WaitForSecondsRealtime(_reSpawn);
        float randomX = Random.Range(-30f, 30f);
        float randomZ = Random.Range(-30f, 30f);
        if (itemCount <= itemMaxCount)
        {
            switch (reSpawnType)
            {
                default:
                    break;
                case 0:
                    if (pV.IsMine)
                    {
                        PhotonNetwork.InstantiateRoomObject("M4", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity)
                            .GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered,1);
                        //PhotonNetwork.InstantiateRoomObject("M4", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity)
                        //    .GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered);
                    }
                    break;
            }
            
            itemCount++;
            ReSpawn();
        }
    }
}
