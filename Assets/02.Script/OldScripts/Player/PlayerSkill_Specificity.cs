using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerSkill_Specificity : MonoBehaviourPun
{
    [Header("Skill")]
    public int skillType;
    public Image skillImage;
    public bool skillUse;
    public Transform targetPos;
    public PhotonView PV;
    public TrailRenderer leftRander;
    public TrailRenderer rightRander;
    public ParticleSystem shieldParticle;
    public ParticleSystem teleportParticle;
    public ParticleSystem speedParticle;
    public GameObject teleport;
    public LayerMask layerMask;

    [Header("Specifi")]
    public int specifiType;
    public Image spBg;
    public Image spImage;
    public float spAttackPlus;
    public float spBulletSpeed;
    public float spGrenadeRange;
    public float spAttackSpeed;
    public float spBulletSize;
    public float spSpeedPlus;
    public float spHeathRecovery;
    public int spMoneyPlus;
    public float spBulletPlus;
    public int spWarning;
    public bool spVirus;
    public int spBasicShield;
    public float spDefensePlus;
    public int spMaxHeath;
    public float spShieldRegen;
    public float spHardArmor;
    public float spConsumPlus;

    public void Start()
    {
        if (TrainingController.instance.training == true)
        {
            skillImage = GameObject.Find("SKillimage").GetComponent<Image>();
            spImage = GameObject.Find("SpecificityImage").GetComponent<Image>();
            spBg = GameObject.Find("SpecificityUI").GetComponent<Image>();
            if (AuthManager.instance != null)
            {
                skillType = AuthManager.instance.userSkill;
                specifiType = AuthManager.instance.userSp;
                SkillSprite();
                SpSprite();
                ApplySp();
            }

        }
        else
        {
            if (PV.IsMine)
            {
                skillImage = GameObject.Find("SKillimage").GetComponent<Image>();
                spImage = GameObject.Find("SpecificityImage").GetComponent<Image>();
                spBg = GameObject.Find("SpecificityUI").GetComponent<Image>();
                if (AuthManager.instance != null)
                {
                    skillType = AuthManager.instance.userSkill;
                    specifiType = AuthManager.instance.userSp;
                    SkillSprite();
                    SpSprite();
                    ApplySp();
                }
            }
        }
       
    }

    public void SkillSprite()
    {
        if (skillType == 1)
        {
            skillImage.sprite = itemAssets.Instance.speedSprite;
        }
        else if (skillType == 2)
        {
            skillImage.sprite = itemAssets.Instance.shieldSprite;
        }
        else if (skillType == 3)
        {
            skillImage.sprite = itemAssets.Instance.telepoSprite;
        }
    }

    public void SpSprite()
    {

        if (specifiType <= 6)
            spBg.sprite = itemAssets.Instance.red;
        else if (specifiType <= 12 && specifiType > 6)
            spBg.sprite = itemAssets.Instance.green;
        else if (specifiType > 12)
            spBg.sprite = itemAssets.Instance.blue;

        if (specifiType == 1)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.attackPlusSprite;
        }
        else if (specifiType == 2)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.bulletSpeedSprite;
        }
        else if (specifiType == 3)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.grenadeRangeSprite;
        }
        else if (specifiType == 4)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.attackSpeedSprite;
        }
        else if (specifiType == 5)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.bulletSizeSprite;
        }
        else if (specifiType == 6)
        {
            spImage.color = new Color(1, 0.8f, 0);
            spImage.sprite = itemAssets.Instance.gunMasterSprite;
        }
        else if (specifiType == 7)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.heathRecoverySprite;
        }

        else if (specifiType == 8)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.SpeedPlusSprite;
        }
        else if (specifiType == 9)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.moneyPlusSprite;
        }
        else if (specifiType == 10)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.bulletPlusSprite;
        }
        else if (specifiType == 11)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.counsumPlusprite;
        }
        else if (specifiType == 12)
        {
            spImage.color = new Color(1, 0.8f, 0);
            spImage.sprite = itemAssets.Instance.utiltyMasterSprite;
        }
        else if (specifiType == 13)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.basicShieldSprite;
        }
        else if (specifiType == 14)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.defensePlusSprite;
        }
        else if (specifiType == 15)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.maxHealthSprite;
        }
        else if (specifiType == 16)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.shieldRegenSprite;
        }
        else if (specifiType == 17)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.hardArmorSprite;
        }
        else if (specifiType == 18)
        {
            spImage.color = new Color(1, 0.8f, 0);
            spImage.sprite = itemAssets.Instance.defenseMasterSprite;
        }
        else
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.XSprite;

        }
    }

    public void ApplySp()
    {
        if (specifiType == 1)
            spAttackPlus = 0.05f * AuthManager.instance.userSpCount1_1;
        else if (specifiType == 2)
            spBulletSpeed = 0.1f * AuthManager.instance.userSpCount1_2;
        else if (specifiType == 3)
            spGrenadeRange = 0.5f * AuthManager.instance.userSpCount1_3;
        else if (specifiType == 4)
            spAttackSpeed = 0.10f * AuthManager.instance.userSpCount1_4;
        else if (specifiType == 5)
            spBulletSize = 0.10f * AuthManager.instance.userSpCount1_5;
        else if (specifiType == 6)
            GunMaster();
        else if (specifiType == 7)
            spSpeedPlus = 0.10f * AuthManager.instance.userSpCount2_1;
        else if (specifiType == 8)
            spHeathRecovery = 0.1f + (0.03f * AuthManager.instance.userSpCount2_2);
        else if (specifiType == 9)
            spMoneyPlus = 5 * AuthManager.instance.userSpCount2_3;
        else if (specifiType == 10)
            spBulletPlus = 0.1f * AuthManager.instance.userSpCount2_4;
        else if (specifiType == 11)
            spConsumPlus = 0.25f * AuthManager.instance.userSpCount2_5;
        else if (specifiType == 12)
            UtilityMaster();
        else if (specifiType == 13)
            spBasicShield = 3 * AuthManager.instance.userSpCount3_1;
        else if (specifiType == 14)
            spDefensePlus = 0.03f * AuthManager.instance.userSpCount3_2;
        else if (specifiType == 15)
            spMaxHeath = 5 * AuthManager.instance.userSpCount3_3;
        else if (specifiType == 16)
            spShieldRegen = 5 + (3f * AuthManager.instance.userSpCount3_4);
        else if (specifiType == 17)
            spHardArmor = 0.05f * AuthManager.instance.userSpCount3_5;
        else if (specifiType == 18)
            DefenseMaster();
        else
            return;
    }

    public void GunMaster()
    {
        spAttackPlus = 0.02f * AuthManager.instance.userSpCount1_6;
        spAttackSpeed = 0.05f * AuthManager.instance.userSpCount1_6;
        spGrenadeRange = 0.25f * AuthManager.instance.userSpCount1_6;
        spBulletSpeed = 0.05f * AuthManager.instance.userSpCount1_6;
    }

    public void UtilityMaster()
    {
        spHeathRecovery = 0.03f * AuthManager.instance.userSpCount2_6;
        spSpeedPlus = 0.05f * AuthManager.instance.userSpCount2_6;
        spMoneyPlus = 3 * AuthManager.instance.userSpCount2_6;
        spBulletPlus = 0.05f * AuthManager.instance.userSpCount2_6;
    }
    
    public void DefenseMaster()
    {
        spBasicShield = 2 * AuthManager.instance.userSpCount3_6;
        spDefensePlus = 0.02f * AuthManager.instance.userSpCount3_6;
        spMaxHeath = 3 * AuthManager.instance.userSpCount3_6;
        spShieldRegen = 3 + (2 * AuthManager.instance.userSpCount3_6);
    }

    [PunRPC]
    public void SpeedUpRPC()
    {
        StartCoroutine(SpeedUp());
    }

    [PunRPC]
    public void ShieldRPC()
    {
        StartCoroutine(Shield());
    }

    [PunRPC]
    public void TeleportationRPC()
    {
        StartCoroutine(Teleportation());
    }

    IEnumerator SpeedUp()
    {
        skillUse = true;
        leftRander.enabled = true;
        rightRander.enabled = true;
        speedParticle.gameObject.SetActive(true);
        speedParticle.Play();
        yield return new WaitForSecondsRealtime(5f);
        speedParticle.gameObject.SetActive(false);
        leftRander.enabled = false;
        rightRander.enabled = false;
        skillUse = false;
    }

    IEnumerator Shield()
    {
        skillUse = true;
        shieldParticle.gameObject.SetActive(true);
        shieldParticle.Play();
        yield return new WaitForSecondsRealtime(2f);
        shieldParticle.gameObject.SetActive(false);
        skillUse = false;
    }

    IEnumerator Teleportation()
    {
        Ray playerRay = new Ray(new Vector3(transform.position.x, transform.position.y + 1.5f, transform.position.z), transform.forward);
        if (Physics.Raycast(playerRay, out RaycastHit hit, 10f, layerMask))
        {
            if (hit.collider.tag == "Environment")
            {
                skillUse = true;
                teleportParticle.gameObject.SetActive(true);
                teleportParticle.Play();
                if(TrainingController.instance.training != true)
                    PhotonNetwork.Instantiate("Teleport", new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                else
                    Instantiate(teleport, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
                transform.position = hit.point;
                yield return new WaitForSecondsRealtime(1.1f);
                teleportParticle.gameObject.SetActive(false);
                skillUse = false;
               
            }
                
        }
        else
        {
            skillUse = true;
            teleportParticle.gameObject.SetActive(true);
            teleportParticle.Play();
            if (TrainingController.instance.training != true)
                PhotonNetwork.Instantiate("Teleport", new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            else
                Instantiate(teleport, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);
            transform.position = targetPos.transform.position;
            yield return new WaitForSecondsRealtime(1.1f);
            teleportParticle.gameObject.SetActive(false);
            skillUse = false;
        }
    }

}
