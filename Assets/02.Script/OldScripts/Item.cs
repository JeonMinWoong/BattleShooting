using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;

public class Item : MonoBehaviourPun, IPunObservable
{
    public Image ItemTimeUi;
    [SerializeField]
    private bool isPickUp;
    public GameObject itemTimeUi;
    public int itemNumber;
    public GameObject player;
    [SerializeField]
    private float maxItemBar;
    [SerializeField]
    private float currentItemBar;
    public PhotonView pV;
    private bool isSupplies;
    public Rigidbody rigid;
    public Transform itemGroup;

    void OnEnable()
    {
        pV = GetComponent<PhotonView>();
    }

    public void Start()
    {
        
        maxItemBar = 1f;
        currentItemBar = 0;
        if (itemNumber == 12)
            transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }

    private void FixedUpdate()
    {
        if (itemNumber != 12)
            transform.Rotate(new Vector3(0, 0.5f, 0));
        if (isPickUp == true)
        {
            ItemPickUp();
        }
        if (isSupplies == true)
            StartCoroutine(SuppliesPickUp());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && player == null)
        {
            player = other.gameObject;
            itemTimeUi = player.transform.Find("ItemCanvas").gameObject.transform.Find("ItemTime").gameObject;
            ItemTimeUi = itemTimeUi.GetComponent<Image>();
            if (itemNumber == 12)
                isSupplies = true;
            else
                isPickUp = true;
        }
        if (other.tag == "Environment")
        {
            float randomX = Random.Range(-30f, 30f);
            float randomZ = Random.Range(-30f, 30f);
            gameObject.transform.position = new Vector3(transform.position.x + randomX, 1, transform.position.z + randomZ);
        }
        if (other.tag == "Floar")
        {
            if (TrainingController.instance.training != true)
                pV.RPC("ItemMoveBan", RpcTarget.All);
            else
                ItemMoveBan();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            currentItemBar = 0;
            ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
            isPickUp = false;
            player = null;
            isSupplies = false;

        }
    }

    void ItemPickUp()
    {
        StartCoroutine(CoItemPickUp());
    }

    IEnumerator CoItemPickUp()
    {
        if (isPickUp == true)
        {
            if (currentItemBar < maxItemBar)
            {
                currentItemBar = currentItemBar + 0.01f;
                yield return null;
                if (!TrainingController.instance.training)
                {
                    if (player != null && player.GetComponent<PhotonView>().IsMine)
                        ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
                }
                else
                {
                    ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
                }
                ItemTimeUi.gameObject.SetActive(true);
            }
            else if (currentItemBar >= maxItemBar)
            {
                if (!TrainingController.instance.training)
                {
                    if (player.GetComponent<PhotonView>().IsMine)
                    {
                        pV.RPC("ItemPlayerUp", RpcTarget.AllViaServer);
                        pV.RPC("ItemPickUpRPC", RpcTarget.AllViaServer);
                        Debug.Log("픽업완료");
                    }
                }
                else
                {
                    ItemPlayerUp();
                    ItemPickUpRPC();
                }
                isPickUp = false;
                yield break;
            }
        }
        else
        {
            if (!TrainingController.instance.training)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                    pV.RPC("ItemPickUpCancal", RpcTarget.AllViaServer);
            }
            else
            {
                ItemPickUpCancal();
            }
            yield break;
        }
    }

    [PunRPC]
    public void ItemMoveBan()
    {
         rigid.useGravity = false;
         rigid.isKinematic = true;
    }

    IEnumerator SuppliesPickUp()
    {
        if (isSupplies == true)
        {

            if (currentItemBar < maxItemBar)
            {
                currentItemBar = currentItemBar + 0.005f;
                yield return null;
                if (player != null && player.GetComponent<PhotonView>().IsMine)
                    ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
                ItemTimeUi.gameObject.SetActive(true);
            }
            else if (currentItemBar >= maxItemBar)
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    pV.RPC("ItemPlayerUp", RpcTarget.AllViaServer);
                    pV.RPC("SuppliesBoxRPC", RpcTarget.AllViaServer);
                }
                isSupplies = false;
                yield break;
            }
        }
        else
        {
            if (player.GetComponent<PhotonView>().IsMine)
                pV.RPC("ItemPickUpCancal", RpcTarget.AllViaServer);
            yield break;
        }

    }

    [PunRPC]
    public void ItemPickUpCancal()
    {
        isPickUp = false;
        isSupplies = false;
        currentItemBar = 0;
        ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
        ItemTimeUi.gameObject.SetActive(false);
    }

    [PunRPC]
    public void ItemPickUpRPC()
    {
        currentItemBar = 0;
        ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
        ItemTimeUi.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void SuppliesBoxRPC()
    {
        AudioManager.Instance.PlaySound("SuppliesOpen", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        gameObject.GetComponent<Animation>().Play();
        gameObject.transform.Find("PickUp").gameObject.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0);
        currentItemBar = 0;
        ItemTimeUi.fillAmount = currentItemBar / maxItemBar;
        ItemTimeUi.gameObject.SetActive(false);
        gameObject.GetComponent<Item>().enabled = false;
    }

    [PunRPC]
    public void ItemPlayerUp()
    {
        if (player == null)
            return;

        Debug.Log("픽업완료 = 일반");
        if (itemNumber == 0)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 30 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(30 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 1;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 1)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 15 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(15 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 2;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 2)
        {
            AudioManager.Instance.PlaySound("Bullet", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            player.GetComponent<TestShoot>().BulletUp();
        }
        if (itemNumber == 3)
        {
            AudioManager.Instance.PlaySound("Health", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            player.GetComponent<TestHealth>().RestoreHealth(30);
        }
        if (itemNumber == 4)
        {
            AudioManager.Instance.PlaySound("Shield", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            player.GetComponent<TestHealth>().RestoreShield(25);
        }
        if (itemNumber == 5)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 60 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(60 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 3;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 6)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 5 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(5 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 4;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 7)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 12 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(12 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 5;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 8)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 10 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(10 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 6;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 9)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 100 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;

            player.GetComponent<TestShoot>().Reroad(100 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 7;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 10)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 45 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(45 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 8;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 11)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 150 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(150 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 9;
            player.GetComponent<TestShoot>().GunChange();
        }
        if(itemNumber == 12)
        {
            AudioManager.Instance.PlaySound("Supplies", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            player.GetComponent<TestHealth>().RestoreHealth(50);
            player.GetComponent<TestHealth>().RestoreShield(50);
            int randomItem = Random.Range(0, 3);
            if (randomItem == 0)
                PhotonNetwork.InstantiateRoomObject("LightGun", new Vector3(transform.position.x + 3, 1f, transform.position.z), Quaternion.identity);
            else if (randomItem == 1)
                PhotonNetwork.InstantiateRoomObject("GL", new Vector3(transform.position.x + 3, 1f, transform.position.z), Quaternion.identity);
            else if (randomItem == 2)
                PhotonNetwork.InstantiateRoomObject("HellWailer", new Vector3(transform.position.x + 3, 1f, transform.position.z), Quaternion.identity);
            else
                return;
        }
        if(itemNumber == 13)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 20 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(20 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 10;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 14)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 20 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(20 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 11;
            player.GetComponent<TestShoot>().GunChange();
        }
        if (itemNumber == 15)
        {
            AudioManager.Instance.PlaySound("GunPickUp", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float bulletPlus = 125 * player.GetComponent<PlayerSkill_Specificity>().spBulletPlus;
            player.GetComponent<TestShoot>().Reroad(125 + (int)bulletPlus);
            player.GetComponent<TestShoot>().itemType = 12;
            player.GetComponent<TestShoot>().GunChange();
        }

        //소비 아이템
        if (itemNumber == 21) // 수류탄
        {
            AudioManager.Instance.PlaySound("Consumable", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float conSumPlus = 3 * player.GetComponent<PlayerSkill_Specificity>().spConsumPlus;
            player.GetComponent<TestShoot>().ConsumableCount(3 + (int)conSumPlus);
            player.GetComponent<TestShoot>().consumableType = 1;
        }
        if (itemNumber == 22) //마인
        {
            AudioManager.Instance.PlaySound("Consumable", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float conSumPlus = 4 * player.GetComponent<PlayerSkill_Specificity>().spConsumPlus;
            player.GetComponent<TestShoot>().ConsumableCount(4 + (int)conSumPlus);
            player.GetComponent<TestShoot>().consumableType = 2;
        }
        if (itemNumber == 23) //연막탄
        {
            AudioManager.Instance.PlaySound("Consumable", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float conSumPlus = 2 * player.GetComponent<PlayerSkill_Specificity>().spConsumPlus;
            player.GetComponent<TestShoot>().ConsumableCount(2+(int)conSumPlus);
            player.GetComponent<TestShoot>().consumableType = 3;
        }
        if (itemNumber == 24) //섬광탄
        {
            AudioManager.Instance.PlaySound("Consumable", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float conSumPlus = 2 * player.GetComponent<PlayerSkill_Specificity>().spConsumPlus;
            player.GetComponent<TestShoot>().ConsumableCount(2+(int)conSumPlus);
            player.GetComponent<TestShoot>().consumableType = 4;
        }
        if (itemNumber == 25) //진통제
        {
            AudioManager.Instance.PlaySound("Consumable", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            float conSumPlus = 3 * player.GetComponent<PlayerSkill_Specificity>().spConsumPlus;
            player.GetComponent<TestShoot>().ConsumableCount(3+(int)conSumPlus);
            player.GetComponent<TestShoot>().consumableType = 5;
        }


    }


    [PunRPC]
    public void SetItemPos(int count)
    {
        if(count == 0)
            transform.SetParent(ItemMake.instance.itemGroup.transform);
        else
            transform.SetParent(PhotonItemMake.instance.itemGroup.transform);
        //Debug.Log("자리찾기");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(currentItemBar);
            stream.SendNext(maxItemBar);

        }
        else
        {
            currentItemBar = (float)stream.ReceiveNext();
            maxItemBar = (float)stream.ReceiveNext();

        }
    }
}
