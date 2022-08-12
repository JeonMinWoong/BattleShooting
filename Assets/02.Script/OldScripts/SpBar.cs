using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpBar : MonoBehaviour
{
    public int buttonCount;
    public GameObject spBar;
    public Image spImage;
    public int sP;
    public GameObject[] selectSp;
    public SpCount spATCount_1;
    public SpCount spATCount_2;
    public SpCount spATCount_3;
    public GameObject skillBar;
    public GameObject skillButton;

    public void Start()
    {
        if (AuthManager.instance.User != null)
        {
            sP = AuthManager.instance.userSp;
            SpType();
        }
        else
        {
            sP = 0;
            SpType();
        }
    }

    public void OnClick()
    {
        buttonCount++;
        if (buttonCount % 2 == 1)
        {
            spBar.SetActive(true);
            skillBar.SetActive(false);
            skillButton.GetComponent<SkillBar>().buttonCount = 0;
        }
        else
            spBar.SetActive(false);
    }

    public void OnOkButton()
    {
        buttonCount = 0;
        spBar.SetActive(false);
    }

    public void SpUpdate()
    {
        AuthManager.instance.OnClickUpdateChildren();
    }

    public void SpType()
    {
        StartSp(AuthManager.instance.userSp);

        if (sP == 1)
        {
            OnSp1_1();
        }
        else if (sP == 2)
        {
            OnSp1_2();
        }
        else if (sP == 3)
        {
            OnSp1_3();
        }
        else if (sP == 4)
        {
            OnSp1_4();
        }
        else if (sP == 5)
        {
            OnSp1_5();
        }
        else if (sP == 6)
        {
            OnSp1_6();
        }
        else if (sP == 7)
        {
            OnSp2_1();
        }
        else if (sP == 8)
        {
            OnSp2_2();
        }
        else if (sP == 9)
        {
            OnSp2_3();
        }
        else if (sP == 10)
        {
            OnSp2_4();
        }
        else if (sP == 11)
        {
            OnSp2_5();
        }
        else if (sP == 12)
        {
            OnSp2_6();
        }
        else if (sP == 13)
        {
            OnSp3_1();
        }
        else if (sP == 14)
        {
            OnSp3_2();
        }
        else if (sP == 15)
        {
            OnSp3_3();
        }
        else if (sP == 16)
        {
            OnSp3_4();
        }
        else if (sP == 17)
        {
            OnSp3_5();
        }
        else if (sP == 18)
        {
            OnSp3_6();
        }
        else
            NotSp();
    }

    public void StartSp(int _sp)
    {
        for (int i = 0; i < 18; i++)
        {
            if (i == _sp-1)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        if (_sp == 1)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.attackPlusSprite;
        }
        else if (_sp == 2)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.bulletSpeedSprite ;
        }
        else if (_sp == 3)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.grenadeRangeSprite;
        }
        else if (_sp == 4)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.attackSpeedSprite;
        }
        else if (_sp == 5)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.bulletSizeSprite;
        }
        else if (_sp == 6)
        {
            spImage.color = new Color(1, 0.8f, 0);
            spImage.sprite = itemAssets.Instance.gunMasterSprite;
        }
        else if (_sp == 7)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.SpeedPlusSprite;
        }

        else if (_sp == 8)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.heathRecoverySprite;
        }
        else if (_sp == 9)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.moneyPlusSprite;
        }
        else if (_sp == 10)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.bulletPlusSprite;
        }
        else if (_sp == 11)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.counsumPlusprite;
        }
        else if (_sp == 12)
        {
            spImage.color = new Color(1, 0.8f, 0);
            spImage.sprite = itemAssets.Instance.utiltyMasterSprite;
        }
        else if (_sp == 13)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.basicShieldSprite;
        }
        else if (_sp == 14)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.defensePlusSprite;
        }
        else if (_sp == 15)
        {
            spImage.color = new Color(1, 1, 1);
            spImage.sprite = itemAssets.Instance.maxHealthSprite;
        }
        else if (_sp == 16)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.shieldRegenSprite;
        }
        else if (_sp == 17)
        {
            spImage.color = new Color(1, 0.5f, 0.5f);
            spImage.sprite = itemAssets.Instance.hardArmorSprite;
        }
        else if (_sp == 18)
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

    public void OffSp(int _count)
    {
        selectSp[_count].SetActive(false);
        spImage.color = new Color(1, 1, 1);
        AuthManager.instance.userSp = 0;
        spImage.sprite = itemAssets.Instance.XSprite;
        SpUpdate();
    }

    public void OnSp1_1()
    {
        if (spATCount_1.spCount[0] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 0 )
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.attackPlusSprite;
        AuthManager.instance.userSp = 1;
        SpUpdate();
    }

    public void OnSp1_2()
    {
        if (spATCount_1.spCount[1] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 1)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.bulletSpeedSprite;
        AuthManager.instance.userSp = 2;
        SpUpdate();
    }

    public void OnSp1_3()
    {
        if (spATCount_1.spCount[2] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 2)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.grenadeRangeSprite;
        AuthManager.instance.userSp = 3;
        SpUpdate();
    }

    public void OnSp1_4()
    {
        if (spATCount_1.spCount[3] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 3)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.attackSpeedSprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 4;
        SpUpdate();
    }

    public void OnSp1_5()
    {
        if (spATCount_1.spCount[4] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 4)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.bulletSizeSprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 5;
        SpUpdate();
    }

    public void OnSp1_6()
    {
        if (spATCount_1.spCount[5] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 5)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.gunMasterSprite; 
        spImage.color = new Color(1, 0.8f, 0);
        AuthManager.instance.userSp = 6;
        SpUpdate();
    }

    public void OnSp2_1()
    {
        if (spATCount_2.spCount[0] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 6)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.SpeedPlusSprite;
        AuthManager.instance.userSp = 7;
        SpUpdate();
    }
    public void OnSp2_2()
    {
        if (spATCount_2.spCount[1] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 7)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.heathRecoverySprite;
        AuthManager.instance.userSp = 8;
        SpUpdate();
    }
    public void OnSp2_3()
    {
        if (spATCount_2.spCount[2] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 8)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.moneyPlusSprite;
        AuthManager.instance.userSp = 9;
        SpUpdate();
    }
    public void OnSp2_4()
    {
        if (spATCount_2.spCount[3] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 9)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.bulletPlusSprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 10;
        SpUpdate();
    }
    public void OnSp2_5()
    {
        if (spATCount_2.spCount[4] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 10)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.counsumPlusprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 11;
        SpUpdate();
    }

    public void OnSp2_6()
    {
        if (spATCount_2.spCount[5] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 11)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.utiltyMasterSprite; 
        spImage.color = new Color(1, 0.8f, 0);
        AuthManager.instance.userSp = 12;
        SpUpdate();
    }

    public void OnSp3_1()
    {
        if (spATCount_3.spCount[0] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 12)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.basicShieldSprite;
        AuthManager.instance.userSp = 13;
        SpUpdate();
    }

    public void OnSp3_2()
    {
        if (spATCount_3.spCount[1] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 13)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.defensePlusSprite;
        AuthManager.instance.userSp = 14;
        SpUpdate();
    }

    public void OnSp3_3()
    {
        if (spATCount_3.spCount[2] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 14)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.maxHealthSprite;
        AuthManager.instance.userSp = 15;
        SpUpdate();
    }

    public void OnSp3_4()
    {
        if (spATCount_3.spCount[3] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 15)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.shieldRegenSprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 16;
        SpUpdate();
    }

    public void OnSp3_5()
    {
        if (spATCount_3.spCount[4] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 16)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.hardArmorSprite; 
        spImage.color = new Color(1, 0.5f, 0.5f);
        AuthManager.instance.userSp = 17;
        SpUpdate();
    }

    public void OnSp3_6()
    {
        if (spATCount_3.spCount[5] == 0)
            return;

        for (int i = 0; i < 18; i++)
        {
            if (i == 17)
                selectSp[i].SetActive(true);
            else
                selectSp[i].SetActive(false);
        }
        spImage.sprite = itemAssets.Instance.defenseMasterSprite;
        spImage.color = new Color(1, 0.8f, 0);
        AuthManager.instance.userSp = 18;
        SpUpdate();
    }

    public void NotSp()
    {
        for (int i = 0; i < 18; i++)
        {
                selectSp[i].SetActive(false);
        }
        spImage.color = new Color(1, 1, 1);
        spImage.sprite = itemAssets.Instance.XSprite;
        AuthManager.instance.userSp = 0;
        SpUpdate();
    }

}
