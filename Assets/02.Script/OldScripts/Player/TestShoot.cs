using Cinemachine;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class TestShoot : MonoBehaviourPun, IPunObservable
{
    TestHealth testHealth;
    PlayerMove playermove;
    PlayerController playerController;
    [Header("Stats")]
    private PlayerScore playerScore;
    public TextMeshProUGUI playerKillText;
    public PhotonView PV;
    public TextMeshProUGUI gunName;
    public bool shootEyes;

    [Header("Shooting")]
    public Transform gunBarrelEnd;      //총구
    public Transform UziGunEnd;
    public Transform UziGunEnd2;
    public Transform gatlingEnd;
    public GameObject bullet;
    public GameObject bulletM;
    public GameObject lightbullet;
    public GameObject glbullet;
    public bool attackKing;
    private Animator animator;
    public LayerMask laserLayer;
    public ParticleSystem[] playerParticle;
    public LineRenderer lineRenderer;
    public Image reroadUi;
    public float currentReroad;
    public float maxReroad;
    public LineRenderer playerAttackLine;
    public bool shootLineReady;
    public float maxDistance = 15f;
    public RaycastHit[] hits;
    public bool helling;

    [Header("Consumable")]
    public Image consumableItemUi;
    public ConsumableItem consumableItem;
    public TextMeshProUGUI consumableText;
    public TextMeshProUGUI consumableCount;
    public int consumableType;
    public int consumableItemCount;
    public GameObject targetPoint;
    public bool boom;
    public GameObject canvas;
    public GameObject[] conItem;

    [Header("Specificity")]
    public PlayerSkill_Specificity playerSp;

    [Header("Bullet&Count")]
    public int gunMax = 0;   // 총알 최대 개수
    public int currentBulletCount; // 지금 총알 개수
    public ItemSlot itemslot;
    public DamageText bulletInGameText;

    public int itemType = 0;
    public TextMeshProUGUI bulletText;
    private Camera mainCamera;
    private Vector3 targetPos;
    private Rigidbody rigid;
    public LayerMask layerMask;
    private int bulletUp;


    [Header("Kill")]
    public int killCount;
    public GameObject murder;

    [Header("Ob")]
    public Vector3 pos;
    public Quaternion rot;
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

    public int ConsumableItemCount
    {
        get
        {
            return consumableItemCount;
        }
        set
        {
            consumableItemCount = value;

        }
    }

    private void Start()
    {
        if (!TrainingController.instance.training)
        {
            PV = GetComponent<PhotonView>();

            killCount = 0;
            CurrentblletCount = gunMax;
            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody>();
            playerScore = GetComponent<PlayerScore>();
            playerKillText = GameObject.Find("Kill Text (TMP)").GetComponent<TextMeshProUGUI>();
            reroadUi = GameObject.Find("AttackSpeed").GetComponent<Image>();
            playerSp = GetComponent<PlayerSkill_Specificity>();
            testHealth = GetComponent<TestHealth>();
            playerController = GetComponent<PlayerController>();
            consumableItem = GameObject.Find("Consumable Keys").GetComponent<ConsumableItem>();
            playermove = GetComponent<PlayerMove>();
            if (PV.IsMine)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                itemslot = GameObject.Find("WindowSlot").GetComponent<ItemSlot>();
                bulletText = GameObject.Find("BulletText_Win").GetComponent<TextMeshProUGUI>();
#elif UNITY_ANDROID
            itemslot = GameObject.Find("BulletSlot").GetComponent<ItemSlot>();
            bulletText = GameObject.Find("BulletText (TMP)").GetComponent<TextMeshProUGUI>();
#endif
                gunName = GameObject.Find("GunText (TMP)").GetComponent<TextMeshProUGUI>();
                consumableText = GameObject.Find("ConsumableText (TMP)").GetComponent<TextMeshProUGUI>();
                consumableCount = GameObject.Find("ConsumableCount (TMP)").GetComponent<TextMeshProUGUI>();
                mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
                ConsumableItemCount = 0;
            }
        }
        else
        {
            killCount = 0;
            CurrentblletCount = gunMax;
            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody>();
            playerScore = GetComponent<PlayerScore>();
            //playerKillText = GameObject.Find("Kill Text (TMP)").GetComponent<TextMeshProUGUI>();
            reroadUi = GameObject.Find("AttackSpeed").GetComponent<Image>();
            playerSp = GetComponent<PlayerSkill_Specificity>();
            testHealth = GetComponent<TestHealth>();
            playerController = GetComponent<PlayerController>();
            consumableItem = GameObject.Find("Consumable Keys").GetComponent<ConsumableItem>();
            playermove = GetComponent<PlayerMove>();

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            itemslot = GameObject.Find("WindowSlot").GetComponent<ItemSlot>();
            bulletText = GameObject.Find("BulletText_Win").GetComponent<TextMeshProUGUI>();
#elif UNITY_ANDROID
            itemslot = GameObject.Find("BulletSlot").GetComponent<ItemSlot>();
            bulletText = GameObject.Find("BulletText (TMP)").GetComponent<TextMeshProUGUI>();
#endif
            gunName = GameObject.Find("GunText (TMP)").GetComponent<TextMeshProUGUI>();
            consumableText = GameObject.Find("ConsumableText (TMP)").GetComponent<TextMeshProUGUI>();
            consumableCount = GameObject.Find("ConsumableCount (TMP)").GetComponent<TextMeshProUGUI>();
            mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            ConsumableItemCount = 0;

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (testHealth.shock)
            return;

        if (!TrainingController.instance.training)
        {
            if (PV.IsMine)
            {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
                if (Input.GetButton("Fire1") && attackKing == false)
                {
                    if (playerParticle[14].gameObject.activeSelf && currentBulletCount <= 0)
                    {
                        PV.RPC("ParticleSet", RpcTarget.All, 12, false);
                    }
                    //Debug.Log("F");
                    PV.RPC("AttackRPC", RpcTarget.All);
                }
                else if (Input.GetKey(KeyCode.F) && !testHealth.isDeath)
                {
                    if (consumableItem.itemUse == true)
                        return;
                    if (ConsumableItemCount == 0)
                        return;
                    if (consumableType == 1 || consumableType == 3 || consumableType == 4)
                    {
                        consumableItem.grenadeTr.gameObject.SetActive(true);

                        consumableItem.arrVec = playermove.lookRot.normalized;

                        consumableItem.m_vecMove = new Vector3(consumableItem.arrVec.x * consumableItem.speed * Time.deltaTime, 0f, consumableItem.arrVec.z * consumableItem.speed * Time.deltaTime);
                        consumableItem.grenadeTr.eulerAngles = new Vector3(0, Mathf.Atan2(consumableItem.arrVec.x, consumableItem.arrVec.z) * Mathf.Rad2Deg, 0);

                        consumableItem.grenadeTr.position += consumableItem.m_vecMove;

                        StartCoroutine(consumableItem.GrenadeUse());
                    }
                }
                else if (Input.GetKeyUp(KeyCode.F) && !testHealth.isDeath)
                {
                    if (consumableItem.itemUse == true)
                        return;
                    if (ConsumableItemCount == 0)
                        return;

                    consumableItem.arrVec = Vector3.zero;
                    consumableItem.ItemUse();
                }
#endif
                if (!attackKing && helling)
                {
                    helling = false;
                    PV.RPC("ParticleSet", RpcTarget.All, 12, false);
                }

                AnimatorHand();
                playerKillText.text = killCount.ToString();

                if (CurrentblletCount <= 0)
                {
                    if (itemType != 0)
                        AudioManager.Instance.PlaySound("BulletEnd", transform.position, 1f + Random.Range(-0.1f, 0.1f));

                    itemType = 0;
                    itemslot.itemImage.color = new Color(1, 1, 1, 0);
                    bulletText.gameObject.SetActive(false);
                    playerController.PV.RPC("SetArsenal", RpcTarget.All, "Empty");
                    if (itemType == 0)
                        testHealth.ObserverCam();
                    gunName.text = "Fist";
                }
                else
                {
                    itemslot.itemImage.color = new Color(1, 1, 1, 1);
                    bulletText.gameObject.SetActive(true);
                    bulletText.text = (CurrentblletCount.ToString() + "");
                    ImageChange();
                }

                if (ConsumableItemCount <= 0)
                {
                    consumableCount.gameObject.SetActive(false);
                    consumableText.gameObject.SetActive(false);
                }
                else
                {
                    consumableCount.gameObject.SetActive(true);
                    consumableText.gameObject.SetActive(true);
                    consumableCount.text = (ConsumableItemCount.ToString());
                    ConsumableItem();
                }
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);
                transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
            }
        }
        else
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            if (Input.GetButton("Fire1") && attackKing == false)
            {
                if (playerParticle[14].gameObject.activeSelf && currentBulletCount <= 0)
                {
                    helling = false;
                    ParticleSet(12,false);
                }
                Debug.Log("F");
                AttackRPC();
            }
            else if (Input.GetKey(KeyCode.F) && !testHealth.isDeath)
            {
                if (consumableItem.itemUse == true)
                    return;
                if (ConsumableItemCount == 0)
                    return;
                if (consumableType == 1 || consumableType == 3 || consumableType == 4)
                {
                    consumableItem.grenadeTr.gameObject.SetActive(true);

                    consumableItem.arrVec = playermove.lookRot.normalized;

                    consumableItem.m_vecMove = new Vector3(consumableItem.arrVec.x * consumableItem.speed * Time.deltaTime, 0f, consumableItem.arrVec.z * consumableItem.speed * Time.deltaTime);
                    consumableItem.grenadeTr.eulerAngles = new Vector3(0, Mathf.Atan2(consumableItem.arrVec.x, consumableItem.arrVec.z) * Mathf.Rad2Deg, 0);

                    consumableItem.grenadeTr.position += consumableItem.m_vecMove;

                    StartCoroutine(consumableItem.GrenadeUse());
                }
            }
            else if (Input.GetKeyUp(KeyCode.F) && !testHealth.isDeath)
            {
                if (consumableItem.itemUse == true)
                    return;
                if (ConsumableItemCount == 0)
                    return;

                consumableItem.arrVec = Vector3.zero;
                consumableItem.ItemUse();
            }
#endif
            AnimatorHand();

            if (!attackKing && helling)
            {
                helling = false;
                ParticleSet(12, false);
            }

            if (CurrentblletCount <= 0)
            {
                itemslot.itemImage.color = new Color(1, 1, 1, 0);
                bulletText.gameObject.SetActive(false);
                itemType = 0;
                playerController.SetArsenal("Empty");
                gunName.text = "Fist";
            }
            else
            {
                itemslot.itemImage.color = new Color(1, 1, 1, 1);
                bulletText.gameObject.SetActive(true);
                bulletText.text = (CurrentblletCount.ToString() + "");
                ImageChange();
            }

            if (ConsumableItemCount <= 0)
            {
                consumableCount.gameObject.SetActive(false);
                consumableText.gameObject.SetActive(false);
            }
            else
            {
                consumableCount.gameObject.SetActive(true);
                consumableText.gameObject.SetActive(true);
                consumableCount.text = (ConsumableItemCount.ToString());
                ConsumableItem();
            }
        }
    }

    [PunRPC]
    public void AttackRPC()
    {
        StartCoroutine(Shoot());
    }

    [PunRPC]
    public void GunChange()
    {
        if (itemType == 1)
        {
            playerController.SetArsenal("M4");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "M4");
        }
        else if (itemType == 2)
        {
            playerController.SetArsenal("Pistol");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Pistol");
        }
        else if (itemType == 3)
        {
            playerController.SetArsenal("Uzi");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Uzi");
        }
        else if (itemType == 4)
        {
            playerController.SetArsenal("Smaw");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Smaw");
        }
        else if (itemType == 5)
        {
            playerController.SetArsenal("Shotgun");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Shotgun");
        }
        else if (itemType == 6)
        {
            playerController.SetArsenal("Sniper");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Sniper");
        }
        else if (itemType == 7)
        {
            playerController.SetArsenal("Gatling");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Gatling");
        }
        else if (itemType == 8)
        {
            playerController.SetArsenal("Ak");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Ak");
        }
        else if (itemType == 9)
        {
            playerController.SetArsenal("Laser");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "Laser");
        }
        else if (itemType == 10)
        {
            playerController.SetArsenal("GoldLightGun");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "GoldLightGun");
        }
        else if (itemType == 11)
        {
            playerController.SetArsenal("GL");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "GL");
        }
        else if (itemType == 12)
        {
            playerController.SetArsenal("HellWailer");
            if (!TrainingController.instance.training)
                playerController.PV.RPC("SetArsenal", RpcTarget.Others, "HellWailer");
        }
        else
            return;
    }

    public void ImageChange()
    {
        testHealth.ObserverCam();
        if (itemType == 1)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.m4sprite;
            gunName.text = "M4";
        }
        else if (itemType == 2)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.pistolsprite;
            gunName.text = "Pistol";
        }
        else if (itemType == 3)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.Uzisprite;
            gunName.text = "Uzi";
        }
        else if (itemType == 4)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.Smawsprite;
            gunName.text = "Smaw";

        }
        else if (itemType == 5)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.shootgunsprite;
            gunName.text = "ShootGun";
        }
        else if (itemType == 6)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.snipersprite;
            gunName.text = "Sniper";
        }
        else if (itemType == 7)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.gatlingsprite;
            gunName.text = "Gatling";
        }
        else if (itemType == 8)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.aksprite;
            gunName.text = "AK";
        }
        else if (itemType == 9)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.lasersprite;
            gunName.text = "Laser";
        }
        else if (itemType == 10)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.LightGunsprite;
            gunName.text = "LightGun";
        }
        else if (itemType == 11)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.LightGunsprite;
            gunName.text = "G.L";
        }
        else if (itemType == 12)
        {
            itemslot.itemImage.sprite = itemAssets.Instance.LightGunsprite;
            gunName.text = "HellWailer";
        }
        else
            return;
    }

    public void ConsumableItem()
    {
        if (consumableType == 1)
        {
            consumableItemUi.sprite = itemAssets.Instance.grenadeSprite;
            consumableText.text = "Grenade";
        }
        else if (consumableType == 2)
        {
            consumableItemUi.sprite = itemAssets.Instance.mineSprite;
            consumableText.text = "Mine";
        }
        else if (consumableType == 3)
        {
            consumableItemUi.sprite = itemAssets.Instance.smokeSprite;
            consumableText.text = "Smoke";
        }
        else if (consumableType == 4)
        {
            consumableItemUi.sprite = itemAssets.Instance.flashSprite;
            consumableText.text = "EMP";
        }
        else if (consumableType == 5)
        {
            consumableItemUi.sprite = itemAssets.Instance.painkillerSprite;
            consumableText.text = "PainKiller";
        }
    }



    public void ShootLine()
    {
        if (TrainingController.instance.training)
        {
            if (itemType != 9)
            {
                Ray shootRay = new Ray(gunBarrelEnd.position, gunBarrelEnd.forward);
                playerAttackLine.enabled = true;

                playerAttackLine.SetPosition(0, shootRay.origin);

                if (Physics.Raycast(shootRay, out RaycastHit hit, 10f, laserLayer))
                {
                    playerAttackLine.SetPosition(1, hit.point);
                }
                else
                {
                    playerAttackLine.SetPosition(1, shootRay.origin + shootRay.direction * 10f);
                }
            }
            StartCoroutine(ShootLineReady());
        }
        else
        {
            if (PV.IsMine)
            {
                if (itemType != 9)
                {
                    Ray shootRay = new Ray(gunBarrelEnd.position, gunBarrelEnd.forward);
                    playerAttackLine.enabled = true;

                    playerAttackLine.SetPosition(0, shootRay.origin);

                    if (Physics.Raycast(shootRay, out RaycastHit hit, 10f, laserLayer))
                    {
                        playerAttackLine.SetPosition(1, hit.point);
                    }
                    else
                    {
                        playerAttackLine.SetPosition(1, shootRay.origin + shootRay.direction * 10f);
                    }
                }
                StartCoroutine(ShootLineReady());
            }
        }
    }

    IEnumerator ShootLineReady()
    {
        if (shootLineReady == false)
            yield return new WaitForSecondsRealtime(0.25f);
        shootLineReady = true;
    }

    public void ShootLineEnd()
    {
        if (TrainingController.instance.training)
        {
            if (itemType != 9)
                playerAttackLine.enabled = false;
            shootLineReady = false;
        }
        else
        {
            if (PV.IsMine)
            {
                if (itemType != 9)
                    playerAttackLine.enabled = false;
                shootLineReady = false;
            }
        }
    }

    IEnumerator Shoot()
    {
        attackKing = true;

        if (itemType == 0)  //주먹
        {
            if (TrainingController.instance.training)
            {
                animator.SetTrigger("Attack");
                RaycastHit attack;
                Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward * 2f, Color.blue, 0.3f);
                if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward, out attack, 2f, layerMask))
                {
                    float AttackDmg = 5 + 5 * playerSp.spAttackPlus;
                    if (attack.collider != null && attack.collider.tag == "Enemy")
                    {
                        attack.collider.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        attack.collider.GetComponent<EnemyHealthTest>().TakeDamageMurderN(gameObject);
                        //if (attack.collider.GetComponent<EnemyHealthTest>().currentHealth <= 0)
                        //    KillCountPlus();
                    }
                    Debug.Log("적충돌");
                }

                float AttackSpeed = 0.5f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.5f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.5f - AttackSpeed);

            }
            else
            {
                if (PV.IsMine)
                {
                    animator.SetTrigger("Attack");

                    RaycastHit attack;
                    Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward * 2f, Color.blue, 0.3f);
                    if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z), transform.forward, out attack, 2f, layerMask))
                    {
                        print("Hit : " + attack.collider.gameObject.name);
                        float AttackDmg = 5 + 5 * playerSp.spAttackPlus;
                        if (attack.collider != null && attack.collider.tag == "Enemy")
                        {
                            EnemyHealthTest eh = attack.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(gameObject);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else if (attack.collider.tag == "Player")
                        {
                            TestHealth ph = attack.collider.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(gameObject);
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else
                            yield return null;
                    }
                    float AttackSpeed = 0.5f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.5f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.5f - AttackSpeed);
                }
            } 
        }
        else if (itemType == 1)  //M4
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            AudioManager.Instance.PlaySound("M4", transform.position, 1f + Random.Range(-0.1f, 0.1f));

            if (TrainingController.instance.training)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                for (int i = 0; i < 3; i++)
                {
                    ObjectManager.instance.ObjectShot(1, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            else
            {
                animator.SetTrigger("Shoot");
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    //animator.SetTrigger("Shoot");
                    for (int i = 0; i < 3; i++)
                    {
                        PV.RPC("Bullet", RpcTarget.All,0,false);
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
            }
            if (CurrentblletCount > 0)
                for (int i = 0; i < 3; i++)
                {
                    CurrentblletCount--;
                }

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.6f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.6f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.6f - AttackSpeed);
                ParticleSet(1, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.6f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.6f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.6f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 1, false);
                }
            }
            
        }
        else if (itemType == 2) //리볼버
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            

            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                ObjectManager.instance.ObjectShot(1, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All,0,false);

                }
            }
            AudioManager.Instance.PlaySound("Pistol", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (CurrentblletCount > 0)
                CurrentblletCount--;
           
            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.6f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.6f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.6f - AttackSpeed);
                ParticleSet(2, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.6f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.6f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.6f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 2, false);
                }
            }

            
        }
        else if (itemType == 3)  //핸드건
        {

            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            
            AudioManager.Instance.PlaySound("Uzi", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                int random = Random.Range(-2, 3);
                GameObject uziM = ObjectManager.instance.ObjectShot(1, gameObject, UziGunEnd.transform.position, UziGunEnd.transform.rotation, true);
                uziM.transform.Rotate(new Vector3(0, random, 0));
                int random2 = Random.Range(-2, 3);
                GameObject uziM2 = ObjectManager.instance.ObjectShot(1, gameObject, UziGunEnd2.transform.position, UziGunEnd2.transform.rotation, true);
                uziM2.transform.Rotate(new Vector3(0, random2, 0));
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All,1,false);
                    PV.RPC("Bullet", RpcTarget.All, 2,false);
                }
            }
            if (CurrentblletCount > 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    CurrentblletCount--;
                }

            }
            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.2f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.2f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.2f - AttackSpeed);
                ParticleSet(3, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.2f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.2f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.2f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 3, false);
                }
            }
            
        }
        else if (itemType == 4)  //로켓런처
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                ObjectManager.instance.ObjectShot(3, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All, 0,true);
                }
            }
            AudioManager.Instance.PlaySound("SMAW", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (CurrentblletCount > 0)
                CurrentblletCount--;

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 2.5f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(2.5f - AttackSpeed));
                yield return new WaitForSecondsRealtime(2.5f - AttackSpeed);
                ParticleSet(4, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 2.5f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(2.5f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(2.5f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 4, false);
                }
            }

            
        }
        else if (itemType == 5)  // 샷건
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            AudioManager.Instance.PlaySound("Shotgun", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                for (int i = -4; i < 4; i++)
                {
                    GameObject shootgunBullet = ObjectManager.instance.ObjectShot(1, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
                    shootgunBullet.transform.Rotate(new Vector3(0, i * 2, 0));
                }
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All, 3,false);
                }            
            }
            
            if (CurrentblletCount > 0)
                CurrentblletCount--;

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 2f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(2f - AttackSpeed));
                yield return new WaitForSecondsRealtime(2f - AttackSpeed);
                ParticleSet(5, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 2f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(2f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(2f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 5, false);
                }
            }
            
        }
        else if (itemType == 6)    //저격총
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            AudioManager.Instance.PlaySound("Sniper", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                ObjectManager.instance.ObjectShot(1, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All,0,false);
                }
            }

            if (CurrentblletCount > 0)
                CurrentblletCount--;

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 2.8f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(2.8f - AttackSpeed));
                yield return new WaitForSecondsRealtime(3f - AttackSpeed);
                ParticleSet(6, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 3f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(2.8f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(3f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 6, false);
                }
            }
            
        }
        else if (itemType == 7)  //게틀링건
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            
            AudioManager.Instance.PlaySound("Gatling", transform.position, 1f + Random.Range(-0.1f, 0.1f));

            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                ObjectManager.instance.ObjectShot(1, gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation, true);
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All,0,false);
                    
                }
            }
            if (CurrentblletCount > 0)
                CurrentblletCount--;

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.1f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.1f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.1f - AttackSpeed);
                ParticleSet(7, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.1f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.1f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.1f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 7, false);
                }
            }
            
        }
        else if (itemType == 8)  //AK
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            AudioManager.Instance.PlaySound("AK", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                for (int i = 0; i < 2; i++)
                {
                    ObjectManager.instance.ObjectShot(1, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation);
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    for (int i = 0; i < 2; i++)
                    {
                        PV.RPC("Bullet", RpcTarget.All,0,false);
                        yield return new WaitForSecondsRealtime(0.1f);
                    }
                }
            }
            if (CurrentblletCount > 0)
                for (int i = 0; i < 2; i++)
                {
                    CurrentblletCount--;
                }

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.5f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.5f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.5f - AttackSpeed);
                ParticleSet(8, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.5f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.5f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.5f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 8, false);
                }
            }

            
        }
        else if (itemType == 9)  //레이저
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            Ray shootRay = new Ray(gunBarrelEnd.position, gunBarrelEnd.forward);
            AudioManager.Instance.PlaySound("Laser", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            

            lineRenderer.SetPosition(0, shootRay.origin);
            if (Physics.Raycast(shootRay, out RaycastHit hit, 50f, laserLayer))
            {
                if (TrainingController.instance.training == true)
                {
                    ParticleSet(itemType, true);
                    animator.SetTrigger("Shoot");
                    if (hit.collider.tag == "Enemy")
                    {
                        float AttackDmg = 3 + 3 * playerSp.spAttackPlus;
                        hit.collider.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hit.collider.GetComponent<EnemyHealthTest>().TakeDamageMurderN(gameObject);
                    }
                    yield return new WaitForSecondsRealtime(0.01f);
                }
                else
                {
                    if (PV.IsMine)
                    {
                        PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                        animator.SetTrigger("Shoot");
                        if (hit.collider.tag == "Player")
                        {
                            float AttackDmg = 3 + 3 * playerSp.spAttackPlus;
                            TestHealth ph = hit.collider.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(gameObject);
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else if (hit.collider.tag == "Enemy")
                        {
                            float AttackDmg = 3 + 3 * playerSp.spAttackPlus;
                            EnemyHealthTest eh = hit.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(gameObject);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        yield return new WaitForSecondsRealtime(0.01f);
                    }
                }
                lineRenderer.SetPosition(1, hit.point);
                playerParticle[11].gameObject.transform.position = hit.point;
                
            }
            else
            {
                lineRenderer.SetPosition(1, shootRay.origin + shootRay.direction * 50f);
            }

            if (CurrentblletCount > 0)
                CurrentblletCount--;

            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.01f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.01f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.01f - AttackSpeed);
                ParticleSet(9, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.01f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.01f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.01f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 9, false);
                }
            }

            
        }
        else if (itemType == 10)  //LightGun
        {
            if (CurrentblletCount <= 0)
            {
                attackKing = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                yield break;
            }
            
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                ObjectManager.instance.ObjectShot(4, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All, 1,true);
                }
            }
            AudioManager.Instance.PlaySound("LightGun", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (CurrentblletCount > 0)
                    CurrentblletCount--;
            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 1.2f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(1.2f - AttackSpeed));
                yield return new WaitForSecondsRealtime(1.2f - AttackSpeed);
                ParticleSet(10, false);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 1.2f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(1.2f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(1.2f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 10, false);
                }
            }
            
        }
        else if (itemType == 11)  //GL
        {
            if (CurrentblletCount <= 0)
            {
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, false);
                else
                    ParticleSet(itemType, false);
                attackKing = false;
                yield break;
            }
           
            if (TrainingController.instance.training == true)
            {
                ParticleSet(itemType, true);
                animator.SetTrigger("Shoot");
                for (int i = -1; i < 2; i++)
                {
                    GameObject GLbullet = ObjectManager.instance.ObjectShot(5, gameObject, gunBarrelEnd.transform.position, gunBarrelEnd.transform.rotation, true);
                    GLbullet.transform.Rotate(0, i * 30, 0);
                    yield return new WaitForSecondsRealtime(0.1f);
                }
            }
            else
            {
                if (PV.IsMine)
                {
                    PV.RPC("ParticleSet", RpcTarget.All, itemType, true);
                    animator.SetTrigger("Shoot");
                    PV.RPC("Bullet", RpcTarget.All, 2, true);
                }
            }
            AudioManager.Instance.PlaySound("GL", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (CurrentblletCount > 0)
                for (int i = 0; i < 3; i++)
                {
                    CurrentblletCount--;
                }
                    
            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 1.75f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(1.75f - AttackSpeed));
                yield return new WaitForSecondsRealtime(1.75f - AttackSpeed);
                ParticleSet(11, false);

            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 1.75f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(1.75f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(1.75f - AttackSpeed);
                    PV.RPC("ParticleSet", RpcTarget.All, 11, false);
                }
            }
            
        }
        else if (itemType == 12)  //HW
        {
            if (CurrentblletCount <= 0)
            {
                helling = false;
                if (!TrainingController.instance.training)
                    PV.RPC("ParticleSet", RpcTarget.All, 12, false);
                else
                    ParticleSet(12, false);
                attackKing = false;
                yield break;
            }
            
            if (TrainingController.instance.training == true)
            {
                animator.SetTrigger("Shoot");
            }
            else
            {
                if (PV.IsMine)
                {
                    animator.SetTrigger("Shoot");
                }
            }
            
            AudioManager.Instance.PlaySound("Hell", transform.position, 1f + Random.Range(-0.1f, 0.1f));
           
            Debug.DrawRay(gunBarrelEnd.position, gunBarrelEnd.forward * maxDistance, Color.blue, 0.3f);

            helling = true;

            if (TrainingController.instance.training)
                ParticleSet(itemType, true);
            else
                PV.RPC("ParticleSet", RpcTarget.All, itemType, true);

            hits = Physics.RaycastAll(gunBarrelEnd.position, gunBarrelEnd.forward * maxDistance, layerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (TrainingController.instance.training == true)
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        float AttackDmg = 5 + 5 * playerSp.spAttackPlus;
                        hit.collider.GetComponent<EnemyHealthTest>().TakeDamage(AttackDmg);
                        hit.collider.GetComponent<EnemyHealthTest>().TakeDamageMurderN(gameObject);
                    }
                }
                else
                {
                    if (PV.IsMine)
                    {
                        float AttackDmg = 5 + 5 * playerSp.spAttackPlus;
                        if (hit.collider.tag == "Player")
                        {
                            TestHealth ph = hit.collider.GetComponent<TestHealth>();
                            ph.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            
                            if (ph.currentHealth <= 0 && !ph.isDeath)
                            {
                                ph.TakeDamageMurderN(gameObject);
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                ph.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else if (hit.collider.tag == "Enemy")
                        {
                            EnemyHealthTest eh = hit.collider.GetComponent<EnemyHealthTest>();
                            eh.pV.RPC("TakeDamage", RpcTarget.All, AttackDmg);
                            eh.TakeDamageMurderN(gameObject);
                            if (eh.currentHealth <= 0 && !eh.isDeath)
                            {
                                PV.RPC("KillCountPlus", RpcTarget.AllViaServer);
                                eh.pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                    }
                }
                yield return new WaitForSecondsRealtime(0.05f);
            }
            if (CurrentblletCount > 0)
                CurrentblletCount--;
            else
            {
                if (!TrainingController.instance.training)
                {
                    if (PV.IsMine)
                    {
                        helling = false;
                        PV.RPC("ParticleSet", RpcTarget.All, 12, false);
                    }
                }
                else
                {
                    helling = false;
                    ParticleSet(12, false);
                }
            }
            if (TrainingController.instance.training == true)
            {
                float AttackSpeed = 0.02f * playerSp.spAttackSpeed;
                StartCoroutine(ReroadAttack(0.02f - AttackSpeed));
                yield return new WaitForSecondsRealtime(0.02f - AttackSpeed);
            }
            else
            {
                if (PV.IsMine)
                {
                    float AttackSpeed = 0.02f * playerSp.spAttackSpeed;
                    StartCoroutine(ReroadAttack(0.02f - AttackSpeed));
                    yield return new WaitForSecondsRealtime(0.02f - AttackSpeed);
                }
            }
            
        }
        else
            yield break;

    }

    [PunRPC]
    void ParticleSet(int count, bool isActive)
    {
        Debug.Log("partic" + " / " + isActive);

        switch (count)
        {
            case 1:
                if (isActive)
                {
                    playerParticle[0].gameObject.SetActive(true);
                    playerParticle[0].Play();
                }
                else
                {
                    playerParticle[0].Stop();
                    playerParticle[0].gameObject.SetActive(false);
                }
                break;
            case 2:
                if (isActive)
                {
                    playerParticle[3].gameObject.SetActive(true);
                    playerParticle[3].Play();
                }
                else
                {
                    playerParticle[3].Stop();
                    playerParticle[3].gameObject.SetActive(false);
                }
                break;
            case 3:
                if (isActive)
                {
                    playerParticle[7].gameObject.SetActive(true);
                    playerParticle[7].Play();
                    playerParticle[8].gameObject.SetActive(true);
                    playerParticle[8].Play();
                }
                else
                {
                    playerParticle[7].gameObject.SetActive(false);
                    playerParticle[7].Stop();
                    playerParticle[8].gameObject.SetActive(false);
                    playerParticle[8].Stop();
                }
                break;
            case 4:
                if (isActive)
                {
                    playerParticle[4].gameObject.SetActive(true);
                    playerParticle[4].Play();
                }
                else
                {
                    playerParticle[4].gameObject.SetActive(false);
                    playerParticle[4].Stop();
                }
                break;
            case 5:
                if (isActive)
                {
                    playerParticle[5].gameObject.SetActive(true);
                    playerParticle[5].Play();
                }
                else
                {
                    playerParticle[5].gameObject.SetActive(false);
                    playerParticle[5].Stop();
                }
                break;
            case 6:
                if (isActive)
                {
                    playerParticle[6].gameObject.SetActive(true);
                    playerParticle[6].Play();
                }
                else
                {
                    playerParticle[6].gameObject.SetActive(false);
                    playerParticle[6].Stop();
                }
                break;
            case 7:
                if (isActive)
                {
                    playerParticle[1].gameObject.SetActive(true);
                    playerParticle[1].Play();
                }
                else
                {
                    playerParticle[1].gameObject.SetActive(false);
                    playerParticle[1].Stop();
                }
                break;
            case 8:
                if (isActive)
                {
                    playerParticle[2].gameObject.SetActive(true);
                    playerParticle[2].Play();
                }
                else
                {
                    playerParticle[2].gameObject.SetActive(false);
                    playerParticle[2].Stop();
                }
                break;
            case 9:
                if (isActive)
                {
                    lineRenderer.enabled = true;
                    playerParticle[9].gameObject.SetActive(true);
                    playerParticle[9].Play();
                    playerParticle[10].gameObject.SetActive(true);
                    playerParticle[10].Play();
                    playerParticle[11].gameObject.SetActive(true);
                    playerParticle[11].Play();
                }
                else
                {
                    playerParticle[9].gameObject.SetActive(false);
                    playerParticle[9].Stop();
                    playerParticle[10].gameObject.SetActive(false);
                    playerParticle[10].Stop();
                    playerParticle[11].gameObject.SetActive(false);
                    playerParticle[11].Stop();
                    lineRenderer.enabled = false;
                }
                break;
            case 10:
                if (isActive)
                {
                    playerParticle[12].gameObject.SetActive(true);
                    playerParticle[12].Play();
                }
                else
                {
                    playerParticle[12].gameObject.SetActive(false);
                    playerParticle[12].Stop();
                }
                break;
            case 11:
                if (isActive)
                {
                    playerParticle[13].gameObject.SetActive(true);
                    playerParticle[13].Play();
                }
                else
                {
                    playerParticle[13].gameObject.SetActive(false);
                    playerParticle[13].Stop();
                }
                break;
            case 12:
                if (isActive)
                {
                    playerParticle[14].gameObject.SetActive(true);
                    playerParticle[14].Play();
                }
                else
                {
                    playerParticle[14].gameObject.SetActive(false);
                    playerParticle[14].Stop();
                }
                break;
            default:
                break;
        }
    }

    [PunRPC]
    void Bullet(int posCount = 0, bool isRare = false)
    {
        if (!isRare)
        {
            GameObject bullet = ObjectManager.instance.PoolGet(1);
            if (posCount == 0)
            {
                bullet.GetComponent<bullet>().SetBullet(gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation);
            }
            else if (posCount == 1)
            {
                bullet.GetComponent<bullet>().SetBullet(gameObject, UziGunEnd.transform.position, UziGunEnd.transform.rotation);
                int random = Random.Range(-2, 3);
                bullet.transform.Rotate(new Vector3(0, random, 0));
            }
            else if (posCount == 2)
            {
                bullet.GetComponent<bullet>().SetBullet(gameObject, UziGunEnd2.transform.position, UziGunEnd2.transform.rotation);
                int random = Random.Range(-2, 3);
                bullet.transform.Rotate(new Vector3(0, random, 0));
            }
            else if (posCount == 3)
            {
                for (int i = -4; i < 4; i++)
                {
                    GameObject _bullet = ObjectManager.instance.PoolGet(1);
                    _bullet.GetComponent<bullet>().SetBullet(gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation);
                    _bullet.transform.Rotate(new Vector3(0, i * 2, 0));
                }
            }
        }
        else
        {
            if (posCount == 0)
            {
                GameObject bullet = ObjectManager.instance.PoolGet(3);
                bullet.GetComponent<bulletM>().SetBullet(gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation);
            }
            else if (posCount == 1)
            {
                GameObject bullet = ObjectManager.instance.PoolGet(4);
                bullet.GetComponent<Lightbullet>().SetBullet(gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation);
            }
            else if (posCount == 2)
            {
                for (int i = -1; i < 2; i++)
                {
                    GameObject bullet = ObjectManager.instance.PoolGet(5);
                    bullet.GetComponent<GLbullet>().SetBullet(gameObject, gatlingEnd.transform.position, gatlingEnd.transform.rotation);
                    bullet.transform.Rotate(0, i * 30, 0);
                }
            }
        }
    }

    public void ConsumableItemUse()
    {
        if (consumableType == 2)
        {
            AudioManager.Instance.PlaySound("MineThro", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                Instantiate(conItem[1], gunBarrelEnd.transform.position, Quaternion.identity)
                        .GetComponent<Mine>().player = gameObject;
                if (ConsumableItemCount > 0)
                    ConsumableItemCount--;
            }
            else
            {
                if (PV.IsMine)
                {
                    PhotonNetwork.Instantiate("MineAT (Consumable)", gunBarrelEnd.transform.position, Quaternion.identity)
                        .GetComponent<Mine>().player = gameObject;
                    if (ConsumableItemCount > 0)
                        ConsumableItemCount--;
                }
            }

        }

        else if (consumableType == 5)
        {
            AudioManager.Instance.PlaySound("PainKiller", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                StartCoroutine(testHealth.UsePainKiller(5));
                if (ConsumableItemCount > 0)
                    ConsumableItemCount--;
            }
            else
            {
                if (PV.IsMine)
                {
                    testHealth.pV.RPC("RPCUsePainKiller", RpcTarget.All,5);
                    if (ConsumableItemCount > 0)
                        ConsumableItemCount--;
                }
            }
        }
        else
            return;
    }

    IEnumerator GrenadeThrow()
    {
        if (consumableType == 1)
        {
            AudioManager.Instance.PlaySound("Throw", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                Instantiate(conItem[0], gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                             .GetComponent<Grenade>().player = gameObject;
                if (ConsumableItemCount > 0)
                    ConsumableItemCount--;
            }
            else
            {
                if (PV.IsMine)
                {
                    PhotonNetwork.Instantiate("GrenadeAT (Consumable)", gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                         .GetComponent<Grenade>().player = gameObject;
                    if (ConsumableItemCount > 0)
                        ConsumableItemCount--;
                }
            }
            yield return new WaitForSecondsRealtime(0.5f);
        }
        else if (consumableType == 3)
        {
            AudioManager.Instance.PlaySound("Throw", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                Instantiate(conItem[2], gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                            .GetComponent<Grenade>().player = gameObject;
                if (ConsumableItemCount > 0)
                    ConsumableItemCount--;
            }
            else
            {
                if (PV.IsMine)
                {
                    PhotonNetwork.Instantiate("SmokeAT (Consumable)", gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                        .GetComponent<Grenade>().player = gameObject;
                    if (ConsumableItemCount > 0)
                        ConsumableItemCount--;
                }
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
        else if (consumableType == 4)
        {
            AudioManager.Instance.PlaySound("Throw", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            if (TrainingController.instance.training == true)
            {
                Instantiate(conItem[3], gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                                 .GetComponent<Grenade>().player = gameObject;
                if (ConsumableItemCount > 0)
                    ConsumableItemCount--;
            }
            else
            {
                if (PV.IsMine)
                {

                    PhotonNetwork.Instantiate("EMPAT (Consumable)", gunBarrelEnd.transform.position, targetPoint.transform.rotation)
                              .GetComponent<Grenade>().player = gameObject;

                    if (ConsumableItemCount > 0)
                        ConsumableItemCount--;
                }
            }

            yield return new WaitForSecondsRealtime(0.5f);
        }
        else
            yield break;
    }

    public IEnumerator Boob()
    {
        boom = true;
        yield return new WaitForSecondsRealtime(3f);
        boom = false;
    }

    public void ConsumableCount(int _Count)
    {
        ConsumableItemCount = _Count;
    }

    public void Reroad(int GunMax)
    {
        CurrentblletCount = GunMax;               // 장전
    }

    [PunRPC]
    public void KillCountPlus()
    {
        killCount++;
    }

    IEnumerator ReroadAttack(float maxReroad)
    {
        if (TrainingController.instance.training == true)
        {
            while (currentReroad <= maxReroad)
            {
                currentReroad += 0.01f;
                yield return new WaitForSecondsRealtime(0.01f);
                reroadUi.fillAmount = currentReroad / maxReroad;
            }
            currentReroad = 0;
            maxReroad = 0;
            reroadUi.fillAmount = currentReroad / maxReroad;
            attackKing = false;
            yield break;
        }
        else
        {
            if (PV.IsMine)
            {
                while (currentReroad <= maxReroad)
                {
                    currentReroad += 0.01f;
                    yield return new WaitForSecondsRealtime(0.01f);
                    reroadUi.fillAmount = currentReroad / maxReroad;
                }
                currentReroad = 0;
                maxReroad = 0;
                reroadUi.fillAmount = currentReroad / maxReroad;
                attackKing = false;
                yield break;
            }
        }

    }

    public void BulletUp()
    {

        if (TrainingController.instance.training == true)
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
                    if (itemType == 10)
                        bulletUp = Random.Range(10, 20);
                    if (itemType == 11)
                        bulletUp = Random.Range(15, 31);
                    if (itemType == 12)
                        bulletUp = Random.Range(50, 101);
                    GameObject healthCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
                    healthCanvas.GetComponent<DamageText>().ShowDamageEffect(bulletUp, 3, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
                    CurrentblletCount = CurrentblletCount + bulletUp;
                }
            }

        }
        else
        {
            if (PV.IsMine)
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
                        if (itemType == 9)
                            bulletUp = Random.Range(25, 51);
                        GameObject healthCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
                        healthCanvas.GetComponent<DamageText>().ShowDamageEffect(bulletUp, 3, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
                        CurrentblletCount = CurrentblletCount + bulletUp;
                    }
                }
            }
        }

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(itemType);
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            itemType = (int)stream.ReceiveNext();
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
        }
    }

    public void AnimatorHand()
    {
        if (itemType == 0)
        {
            animator.SetBool("FreeHand", true);
            animator.SetBool("OneHand", false);
            animator.SetBool("TwoHand", false);
            animator.SetBool("SniperHand", false);
        }
        else if (itemType == 2)
        {
            animator.SetBool("FreeHand", false);
            animator.SetBool("OneHand", true);
            animator.SetBool("TwoHand", false);
            animator.SetBool("SniperHand", false);
        }
        else if (itemType == 3)
        {
            animator.SetBool("FreeHand", false);
            animator.SetBool("OneHand", false);
            animator.SetBool("TwoHand", true);
            animator.SetBool("SniperHand", false);
        }
        else
        {
            animator.SetBool("FreeHand", false);
            animator.SetBool("OneHand", false);
            animator.SetBool("TwoHand", false);
            animator.SetBool("SniperHand", true);
        }
    }

}