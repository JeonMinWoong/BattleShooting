using Cinemachine;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerShoot : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Stats")]
    public string pName;
    public int win;
    public int kill;
    public int ing;
    public int los;
    private int gold;
    private int medal;
    private bool playerSave;
    private PlayerScore playerScore;
    public bool playerOnZon;
    public Image onZonUi;
    public Animator canvasAnimator;
    public GameObject mapPlayer;
    public TextMeshProUGUI playerKillText;
    public PhotonView PV;

    [Header("Shooting")]
    public Transform gunBarrelEnd;      //총구
    public Transform UziGunEnd;
    public Transform UziGunEnd2;
    public Transform gatlingEnd;
    public GameObject bullet;
    public GameObject bulletM;
    public bool attackKing;
    private Animator animator;
    public LayerMask laserLayer;
    public ParticleSystem[] playerParticle;
    public LineRenderer lineRenderer;

    [Header("Bullet&Count")]
    public int gunMax = 0;   // 총알 최대 개수
    public int currentBulletCount; // 지금 총알 개수
    public ItemSlot itemslot;

    public int itemType = 0;
    public TextMeshProUGUI bulletText;
    private Camera mainCamera;
    private Vector3 targetPos;
    private Rigidbody rigid;
    public LayerMask layerMask;
    private int bulletUp;
    public bool isDeath = false;

    [Header("Kill")]
    public int killCount;
    public int ranking;
    public GameObject murder;

    public int CurrentblletCount
    {
        get
        {
            return currentBulletCount;
        }
        set
        {
            currentBulletCount = value;

        }
    }


    bool isReroading = false;            // 리로드 중인가?
    public float reroadtime = 0.5f;     // 리로드 시간


    void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            killCount = 0;
            lineRenderer = GetComponentInChildren<LineRenderer>();
            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody>();
            playerOnZon = true;
            playerScore = GetComponent<PlayerScore>();
            canvasAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
            playerKillText = GameObject.Find("Kill Text (TMP)").GetComponent<TextMeshProUGUI>();
            onZonUi = canvasAnimator.gameObject.transform.Find("OnZonUi").GetComponent<Image>();
            itemslot = GameObject.Find("BulletSlot").GetComponent<ItemSlot>();
            bulletText = itemslot.gameObject.transform.Find("BulletText (TMP)").GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        if (PV.IsMine)
        {
            CurrentblletCount = gunMax;
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            if (TrainingController.instance.training == false)
                StartCoroutine("OnZon");
            playerSave = false;
            mapPlayer.GetComponent<MeshRenderer>().material.color = Color.green;

        }
        else
            mapPlayer.GetComponent<MeshRenderer>().material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (PV.IsMine)
        {
            playerKillText.text = killCount.ToString();
            //공격 키를 누르면 총 쏘도록
            if (Input.GetButton("Fire1") && isReroading == false && attackKing == false)
            {
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 10000f))
                {
                    targetPos = hit.point;
                }
                Vector3 dir = targetPos - transform.position;
                Vector3 dirXZ = new Vector3(dir.x, 0f, dir.z);
                Quaternion targetRot = Quaternion.LookRotation(dirXZ);
                rigid.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 550.0f);

                StartCoroutine(Shoot());
                PV.RPC("ShootRPC", RpcTarget.AllBuffered);
            }

            if (CurrentblletCount <= 0)
            {
                itemslot.itemImage.color = new Color(1, 1, 1, 0);
                bulletText.gameObject.SetActive(false);
                GetComponent<PlayerController>().SetArsenal("Empty");
            }
            else
            {
                itemslot.itemImage.color = new Color(1, 1, 1, 1);
                bulletText.gameObject.SetActive(true);
                bulletText.text = (CurrentblletCount.ToString() + "");
                GetComponent<PhotonView>().RPC("GunChange", RpcTarget.All);
            }
        }
    }
    
    [PunRPC]
    public void GunChange()
    {
        if (itemType == 1)
            GetComponent<PhotonView>().RPC("SetArsenal", RpcTarget.All, "M4");
        //GetComponent<PlayerController>().SetArsenal("M4");
        else if (itemType == 2)
            GetComponent<PlayerController>().SetArsenal("Pistol");
        else if (itemType == 3)
            GetComponent<PlayerController>().SetArsenal("Uzi");
        else if (itemType == 4)
            GetComponent<PlayerController>().SetArsenal("Smaw");
        else if (itemType == 5)
            GetComponent<PlayerController>().SetArsenal("Shotgun");
        else if (itemType == 6)
            GetComponent<PlayerController>().SetArsenal("Sniper");
        else if (itemType == 7)
            GetComponent<PlayerController>().SetArsenal("Gatling");
        else if (itemType == 8)
            GetComponent<PlayerController>().SetArsenal("Ak");
        else if (itemType == 9)
            GetComponent<PhotonView>().RPC("SetArsenal", RpcTarget.All, "Laser");
        //GetComponent<PlayerController>().SetArsenal("Laser");
        else
            return;
    }


    [PunRPC]
    public void ShootRPC()
    {
        StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        if (PV.IsMine)
        {
            attackKing = true;
            if (itemType == 0)  //주먹
            {

                animator.SetTrigger("Attack");
                RaycastHit attack;
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward * 2f, Color.blue, 0.3f);
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward, out attack, 2f, layerMask))
                {
                    print("Hit : " + attack.collider.gameObject.name);
                    if (attack.collider != null && attack.collider.tag == "Enemy")
                        attack.collider.GetComponent<EnemyHealthTest>().TakeDamage(5);
                    else if (attack.collider.tag == "Player")
                        attack.collider.GetComponent<PlayerHealth>().TakeDamage(5, gameObject);
                    else
                        yield return null;
                    Debug.Log("적충돌");

                }
                yield return new WaitForSeconds(0.5f);
                attackKing = false;
            }

            if (itemType == 1)  //M4
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }

                animator.SetTrigger("Shoot");
                playerParticle[0].gameObject.SetActive(true);
                playerParticle[0].Play();
                AudioManager.Instance.PlaySound("M4", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                for (int i = 0; i < 3; i++)
                {
                    GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject; 
                    
                    bullet.SetActive(true);
                    bullet.transform.SetParent(Camera.main.transform);
                    bullet.transform.position = gunBarrelEnd.transform.position;
                    bullet.transform.rotation = gunBarrelEnd.transform.rotation;

                    yield return new WaitForSeconds(0.1f);
                }
                if (CurrentblletCount > 0)

                    for (int i = 0; i < 3; i++)
                    {
                        CurrentblletCount--;
                    }

                yield return new WaitForSeconds(0.3f);
                playerParticle[0].gameObject.SetActive(false);
                playerParticle[0].Stop();
                attackKing = false;
            }
            else if (itemType == 2) //리볼버
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[3].gameObject.SetActive(true);
                playerParticle[3].Play();
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("Pistol", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                bullet.SetActive(true);
                bullet.transform.SetParent(Camera.main.transform);
                bullet.transform.position = gunBarrelEnd.transform.position;
                bullet.transform.rotation = gunBarrelEnd.transform.rotation;

                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(0.6f);
                playerParticle[3].gameObject.SetActive(false);
                playerParticle[3].Stop();
                attackKing = false;
            }
            else if (itemType == 3)  //핸드건
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }

                animator.SetTrigger("Shoot");
                playerParticle[7].gameObject.SetActive(true);
                playerParticle[7].Play();
                playerParticle[8].gameObject.SetActive(true);
                playerParticle[8].Play();
                AudioManager.Instance.PlaySound("Uzi", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                
                for (int i = 0; i < 2; i++)
                {
                    GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                    bullet.SetActive(true);
                    bullet.transform.SetParent(Camera.main.transform);
                    bullet.transform.position = i == 0 ? UziGunEnd.transform.position : UziGunEnd2.transform.position;
                    bullet.transform.rotation = i == 0 ? UziGunEnd.transform.rotation : UziGunEnd2.transform.rotation;
                    int random = Random.Range(-2, 3);
                    bullet.transform.Rotate(new Vector3(0, random, 0));
                }
                if (CurrentblletCount > 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        CurrentblletCount--;
                    }

                }
                yield return new WaitForSeconds(0.2f);
                playerParticle[7].gameObject.SetActive(false);
                playerParticle[7].Stop();
                playerParticle[8].gameObject.SetActive(false);
                playerParticle[8].Stop();
                attackKing = false;
            }
            else if (itemType == 4)  //로켓런처
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[4].gameObject.SetActive(true);
                playerParticle[4].Play();
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("SMAW", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                Instantiate(bulletM, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation);
                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(2.5f);
                playerParticle[4].gameObject.SetActive(false);
                playerParticle[4].Stop();
                attackKing = false;
            }
            else if (itemType == 5)  // 샷건
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[5].gameObject.SetActive(true);
                playerParticle[5].Play();
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("Shotgun", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                for (int i = 0; i < 5; i++)
                {
                    GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                    bullet.SetActive(true);
                    bullet.transform.SetParent(Camera.main.transform);
                    bullet.transform.position = gunBarrelEnd.transform.position;
                    bullet.transform.rotation = gunBarrelEnd.transform.rotation;
                    bullet.transform.Rotate(new Vector3(0, i * 3, 0));
                }
                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(2f);
                playerParticle[5].gameObject.SetActive(false);
                playerParticle[5].Stop();
                attackKing = false;
            }
            else if (itemType == 6)    //저격총
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[6].gameObject.SetActive(true);
                playerParticle[6].Play();
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("Sniper", transform.position, 1f + Random.Range(-0.1f, 0.1f));

                GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                bullet.SetActive(true);
                bullet.transform.SetParent(Camera.main.transform);
                bullet.transform.position = gunBarrelEnd.transform.position;
                bullet.transform.rotation = gunBarrelEnd.transform.rotation;

                bullet.GetComponent<bullet>().StartCoroutine(bullet.GetComponent<bullet>().SniperBullet());

                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(3f);
                playerParticle[6].gameObject.SetActive(false);
                playerParticle[6].Stop();
                attackKing = false;
            }
            else if (itemType == 7)  //게틀링건
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[1].gameObject.SetActive(true);
                playerParticle[1].Play();
                AudioManager.Instance.PlaySound("Gatling", transform.position, 1f + Random.Range(-0.1f, 0.1f));

                animator.SetTrigger("Shoot"); 
                GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                bullet.SetActive(true);
                bullet.transform.SetParent(Camera.main.transform);
                bullet.transform.position = gatlingEnd.transform.position;
                bullet.transform.rotation = gatlingEnd.transform.rotation;

                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(0.1f);
                playerParticle[1].gameObject.SetActive(false);
                playerParticle[1].Stop();
                attackKing = false;
            }
            else if (itemType == 8)  //AK
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                playerParticle[2].gameObject.SetActive(true);
                playerParticle[2].Play();
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("AK", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                for (int i = 0; i < 2; i++)
                {
                    GameObject bullet = ObjectManager.instance.poolingGroup[1].GetChild(0).gameObject;

                    bullet.SetActive(true);
                    bullet.transform.SetParent(Camera.main.transform);
                    bullet.transform.position = gatlingEnd.transform.position;
                    bullet.transform.rotation = gatlingEnd.transform.rotation;
                    yield return new WaitForSeconds(0.1f);
                }
                if (CurrentblletCount > 0)
                    for (int i = 0; i < 2; i++)
                    {
                        CurrentblletCount--;
                    }
                yield return new WaitForSeconds(0.5f);
                playerParticle[2].gameObject.SetActive(false);
                playerParticle[2].Stop();
                attackKing = false;
            }
            else if (itemType == 9)  //레이저
            {

                if (CurrentblletCount <= 0)
                {
                    AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    itemType = 0;
                    attackKing = false;
                    yield break;
                }
                Ray shootRay = new Ray(gunBarrelEnd.position, gunBarrelEnd.forward);
                lineRenderer.enabled = true;
                playerParticle[9].gameObject.SetActive(true);
                playerParticle[9].Play();
                playerParticle[10].gameObject.SetActive(true);
                playerParticle[10].Play();
                playerParticle[11].gameObject.SetActive(true);
                playerParticle[11].Play();

                lineRenderer.SetPosition(0, shootRay.origin);
                if (Physics.Raycast(shootRay, out RaycastHit hit, 50f, laserLayer))
                {
                    if (hit.collider.tag == "Player")
                    {
                        hit.collider.gameObject.GetComponent<PlayerHealth>().TakeDamage(3, gameObject);

                    }
                    else if (hit.collider.tag == "Enemy")
                    {
                        hit.collider.gameObject.GetComponent<EnemyHealthTest>().TakeDamage(3);

                    }
                    lineRenderer.SetPosition(1, hit.point);
                    playerParticle[12].gameObject.transform.position = hit.point;
                    yield return new WaitForSeconds(0.01f);
                }
                else
                {
                    lineRenderer.SetPosition(1, shootRay.origin + shootRay.direction * 50f);
                    yield return new WaitForSeconds(0.01f);

                }
                animator.SetTrigger("Shoot");
                AudioManager.Instance.PlaySound("Laser", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                if (CurrentblletCount > 0)
                    CurrentblletCount--;
                yield return new WaitForSeconds(0.01f);
                playerParticle[9].gameObject.SetActive(false);
                playerParticle[9].Stop();
                playerParticle[10].gameObject.SetActive(false);
                playerParticle[10].Stop();
                playerParticle[11].gameObject.SetActive(false);
                playerParticle[11].Stop();
                lineRenderer.enabled = false;
                attackKing = false;
            }

            else
                yield break;

        }

    }





    IEnumerator Reroad(int GunMax)
    {
        isReroading = true;

        yield return new WaitForSeconds(reroadtime); // 장전시간

        CurrentblletCount = GunMax;                 // 장전

        isReroading = false; //재장전 완료
    }

    public void BulletUp()
    {
        if (CurrentblletCount != 0)
        {
            if (itemType != 0)
            {
                if (itemType == 1)
                    bulletUp = Random.Range(10, 31);
                if (itemType == 2)
                    bulletUp = Random.Range(3, 11);
                if (itemType == 3)
                    bulletUp = Random.Range(20, 41);
                if (itemType == 4)
                    bulletUp = Random.Range(3, 10);
                if (itemType == 5)
                    bulletUp = Random.Range(10, 21);
                if (itemType == 6)
                    bulletUp = Random.Range(5, 12);
                if (itemType == 7)
                    bulletUp = Random.Range(100, 201);
                if (itemType == 8)
                    bulletUp = Random.Range(30, 61);
                if (itemType == 9)
                    bulletUp = Random.Range(80, 151);
                CurrentblletCount = CurrentblletCount + bulletUp;
            }
        }
    }

    IEnumerator OnZon()
    {
        if (GameManager.instance.gameEnd == false)
        {
            if (playerOnZon == true)
            {
                onZonUi.gameObject.SetActive(false);
            }
            if (playerOnZon == false)
            {
                gameObject.GetComponent<PlayerHealth>().TakeDamage(5 , GameManager.instance.deathZon.gameObject);
                canvasAnimator.SetTrigger("OutZon");
            }
            yield return new WaitForSeconds(1f);
            StartCoroutine("OnZon");
        }
    }
 
    public void Stats()
    {
        if (playerSave == false)
        {
            playerScore.playerNaming = pName;
            playerScore.playerKillCount += kill;
            playerScore.playerWinCount += win;
            playerScore.playerIngCount += ing;
            playerScore.playerLosCount += los;
            playerScore.playerGold += gold;
            playerScore.playerMedal += medal;
            playerSave = true;
        }
        else
            return;
    }

    public void KillUp()
    {
        kill++;
    }

    public void WinUp()
    {
        win++;
    }

    public void IngUp()
    {
        ing++;
    }

    public void LosUp()
    {
        los++;
    }
    
    public void GoldUp()
    {
        if (ranking == 0)
            gold = gold + 100;
        else if (ranking == 2)
            gold = gold + 75;
        else if (ranking == 3)
            gold = gold + 50;
        else if (ranking == 4 || ranking == 5)
            gold = gold + 40;
        else if (ranking == 6 || ranking == 7)
            gold = gold + 30;
        else if (ranking == 8 || ranking == 9 || ranking == 10 || ranking == 11)
            gold = gold + 25;
        else if (ranking == 12 || ranking == 13 || ranking == 14 || ranking == 15)
            gold = gold + 20;
        else
            gold = gold + 10;
    }

    public void MedalUp()
    {
        if (ranking == 0)
            medal = medal + 3;
        else if (ranking == 2)
            medal = medal + 2;
        else if (ranking == 3)
            medal = medal + 1;
        else
            return;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemType);
            
        }
        else
        {
            itemType = (int)stream.ReceiveNext();
        }
    }
}
