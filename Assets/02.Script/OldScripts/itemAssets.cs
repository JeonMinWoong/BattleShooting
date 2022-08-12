using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemAssets : MonoBehaviour
{
    //읽기 전용 싱글톤 인스턴스
    public static itemAssets Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    //총 종류 및 스프라이트
    public Sprite m4sprite;
    public Transform pfitemM4;
    public Sprite pistolsprite;
    public Transform pfitemPistol;
    public Sprite Uzisprite;
    public Transform pfitemUzi;
    public Sprite Smawsprite;
    public Transform pfitemSmaw;
    public Sprite shootgunsprite;
    public Transform pfitemshogun;
    public Sprite snipersprite;
    public Transform pfitemsniper;
    public Sprite gatlingsprite;
    public Transform pfitemGatling;
    public Sprite aksprite;
    public Transform pfitemAk;
    public Sprite lasersprite;
    public Transform pfitemLaser;
    public Sprite LightGunsprite;
    public Transform pfitemLightGun;
    public Sprite gLsprite;
    public Transform pfitemGL;
    public Sprite hellwailersprite;
    public Transform pfitemHellwailer;

    //소비 아이템 스프라이트
    public Sprite grenadeSprite;
    public Sprite mineSprite;
    public Sprite smokeSprite;
    public Sprite flashSprite;
    public Sprite painkillerSprite;

    //스킬 스프라이트
    public Sprite speedSprite;
    public Sprite shieldSprite;
    public Sprite telepoSprite;

    //특성 스프라이트
    public Sprite red;
    public Sprite green;
    public Sprite blue;
    public Sprite attackPlusSprite;
    public Sprite bulletSpeedSprite;
    public Sprite grenadeRangeSprite;
    public Sprite attackSpeedSprite;
    public Sprite bulletSizeSprite;
    public Sprite gunMasterSprite;
    public Sprite SpeedPlusSprite;
    public Sprite heathRecoverySprite;
    public Sprite moneyPlusSprite;
    public Sprite bulletPlusSprite;
    public Sprite counsumPlusprite;
    public Sprite utiltyMasterSprite;
    public Sprite basicShieldSprite;
    public Sprite defensePlusSprite;
    public Sprite maxHealthSprite;
    public Sprite shieldRegenSprite;
    public Sprite hardArmorSprite;
    public Sprite defenseMasterSprite;
    public Sprite XSprite;

}
