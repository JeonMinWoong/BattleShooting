using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ItemMake : MonoBehaviourPun
{
    public static ItemMake instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<ItemMake>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static ItemMake m_instance;

    public GameObject itemGroup;

    public int reSpawnTime;
    public int reSpawnType;
    public int reSpawncSBItemType;
    public GameObject[] items;
    public GameObject[] conitems;
    public int stage;
    private float randomRange;
    public int itemCount;
    public int itemMaxCount;
    public int cSBItemCount;
    public int cSBItemMaxCount;
    public int specialCount;
    public int maxSpecialCount;
    public PhotonView pV;
    

    // Start is called before the first frame update
    void Start()
    {
        pV = GetComponent<PhotonView>();
        Init();
    }

    void Init()
    {
        if (!TrainingController.instance.training)
        {
            PhotonNetwork.InstantiateRoomObject("itemGroup", new Vector3(transform.position.x, 1f, transform.position.z), Quaternion.identity);
            pV.RPC("ItemGroup", RpcTarget.AllBuffered);
            pV.RPC("ReSpawn", RpcTarget.AllBuffered);
        }
        else
        {
            ReSpawn();
        }
    }

    [PunRPC]
    void ItemGroup()
    {
        if (itemGroup != null)
            return;

        itemGroup = GameObject.Find("ItemGroup(Clone)");
    }

    [PunRPC]
    public void ReSpawn()
    {
        reSpawnTime = Random.Range(0,1);
        StartCoroutine(ReSpawnItem(reSpawnTime));
        StartCoroutine(ReSpawnCSBItem(reSpawnTime));
        StartCoroutine(ReSpawnSpecialItem(reSpawnTime));
        
    }

    IEnumerator ReSpawnItem(int _reSpawn)
    {
        if (stage == 1)
        {
            randomRange = 30;
            itemMaxCount = 25;
        }
        else if (stage == 2)
        {
            randomRange= 120f;
            itemMaxCount = 50;        
        }
        else
            yield break;

        
        while(itemCount <= itemMaxCount)
        {
            int rand = Random.Range(0, 12);
            float randomX = Random.Range(-randomRange, randomRange);
            float randomZ = Random.Range(-randomRange, randomRange);
            if (TrainingController.instance.training != true)
            {
                if (pV.IsMine)
                {
                    switch (rand)
                    {
                        default:
                            break;
                        case 0:
                            PhotonNetwork.InstantiateRoomObject("M4", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 1:
                            PhotonNetwork.InstantiateRoomObject("Bullets", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 2:
                            PhotonNetwork.InstantiateRoomObject("Kit", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                 GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 3:
                            PhotonNetwork.InstantiateRoomObject("Pistol", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 4:
                            PhotonNetwork.InstantiateRoomObject("Shield", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 5:
                            PhotonNetwork.InstantiateRoomObject("Uzi", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 6:
                            PhotonNetwork.InstantiateRoomObject("SMAW", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 7:
                            PhotonNetwork.InstantiateRoomObject("Shotgun", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 8:
                            PhotonNetwork.InstantiateRoomObject("Sniper", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 9:
                            PhotonNetwork.InstantiateRoomObject("Gatling", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 10:
                            PhotonNetwork.InstantiateRoomObject("AK", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 11:
                            PhotonNetwork.InstantiateRoomObject("Laser", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                    }
                }
            }
            else
            {
                switch (rand)
                {
                    default:
                        break;
                    case 0:
                        Instantiate(items[0], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(items[1], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(items[2], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(items[3], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(items[4], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 5:
                        Instantiate(items[5], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 6:
                        Instantiate(items[6], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 7:
                        Instantiate(items[7], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 8:
                        Instantiate(items[8], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 9:
                       Instantiate(items[9], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 10:
                        Instantiate(items[10], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 11:
                        Instantiate(items[11], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                }
            }
            itemCount++;
            yield return null;
        }
    }

    IEnumerator ReSpawnCSBItem(int _reSpawn)
    {
        if (stage == 1)
        {
            cSBItemMaxCount = 10;
        }
        else if (stage == 2)
        {
            cSBItemMaxCount = 20;
        }
        else
            yield break;

        
        while (cSBItemCount <= cSBItemMaxCount)
        {
            int rand = Random.Range(0, 5);
            float randomX = Random.Range(-randomRange, randomRange);
            float randomZ = Random.Range(-randomRange, randomRange);
            if (TrainingController.instance.training != true)
            {
                if (pV.IsMine)
                {
                    switch (rand)
                    {

                        default:
                            break;
                        case 0:
                            PhotonNetwork.InstantiateRoomObject("Grenade (Consumable)", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 1:
                            PhotonNetwork.InstantiateRoomObject("Mine (Consumable)", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 2:
                            PhotonNetwork.InstantiateRoomObject("Smoke (Consumable)", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 3:
                            PhotonNetwork.InstantiateRoomObject("EMP (Consumable)", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 4:
                            PhotonNetwork.InstantiateRoomObject("PainKiller (Consumable)", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                    }
                }
            }
            else
            {
                switch (rand)
                {
                    default:
                        break;
                    case 0:
                        Instantiate(conitems[0], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(conitems[1], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(conitems[2], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 3:
                        Instantiate(conitems[3], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 4:
                        Instantiate(conitems[4], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                }
            }
            cSBItemCount++;
            yield return null;
        }
    }

    IEnumerator ReSpawnSpecialItem(int _reSpawn)
    {
        if (stage == 1)
        {
            maxSpecialCount = 3;
        }
        else if (stage == 2)
        {
            maxSpecialCount = 7;
        }
        else
            yield break;



        while (specialCount <= maxSpecialCount)
        {
            int rand = Random.Range(0, 3);
            float randomX = Random.Range(-randomRange, randomRange);
            float randomZ = Random.Range(-randomRange, randomRange);
            if (TrainingController.instance.training != true)
            {
                if (pV.IsMine)
                {
                    switch (rand)
                    {
                        default:
                            break;
                        case 0:
                            PhotonNetwork.InstantiateRoomObject("LightGun", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 1:
                            PhotonNetwork.InstantiateRoomObject("GL", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                        case 2:
                            PhotonNetwork.InstantiateRoomObject("HellWailer", new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity).
                                 GetComponent<Item>().pV.RPC("SetItemPos", RpcTarget.AllBuffered, 0);
                            break;
                    }
                }
            }
            else
            {
                switch (rand)
                {
                    default:
                        break;
                    case 0:
                        Instantiate(items[12], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 1:
                        Instantiate(items[13], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                    case 2:
                        Instantiate(items[14], new Vector3(transform.position.x + randomX, 1f, transform.position.z + randomZ), Quaternion.identity);
                        break;
                }
            }
            specialCount++;
            yield return null;
        }
    }
}
