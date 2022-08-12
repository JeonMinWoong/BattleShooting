using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBar : MonoBehaviour
{
    public GameObject skillBar;
    public int buttonCount;
    public GameObject speedImage;
    public GameObject shieldImage;
    public GameObject teleportImage;
    public Image skillUseImage;
    public int skillType;
    public GameObject spBar;
    public GameObject spButton;

    private void Awake()
    {
        buttonCount = 0;
    }

    public void Start()
    {
        if (AuthManager.instance.User != null)
        {
            skillType = AuthManager.instance.userSkill;
            SkillType();
        }
        else
        {
            skillType = 1;
            SkillType();
        }
    }

    public void SkillType()
    {
        if (skillType == 1)
        {
            OnSpeedClick();
        }
        else if (skillType == 2)
        {
            OnShieldClick();
        }
        else if (skillType == 3)
        {
            OnTeleportClick();
        }
        else
            return;
    }

    public void SkillUpdate()
    {
        AuthManager.instance.OnClickUpdateChildren();
    }

    public void OnClick()
    {
        buttonCount++;
        if (buttonCount % 2 == 1)
        {
            skillBar.SetActive(true);
            spBar.SetActive(false);
            spButton.GetComponent<SpBar>().buttonCount = 0;


        }
        else
            skillBar.SetActive(false);
    }

    public void OnOkButton()
    {
        buttonCount = 0;
        skillBar.SetActive(false);
    }

    public void OnSpeedClick()
    {
        speedImage.SetActive(true);
        shieldImage.SetActive(false);
        teleportImage.SetActive(false);
        skillUseImage.sprite = itemAssets.Instance.speedSprite;
        skillUseImage.color = new Color(0.3f, 1, 0.3f);
        AuthManager.instance.userSkill = 1;
        SkillUpdate();
    }

    public void OnShieldClick()
    {
        speedImage.SetActive(false);
        shieldImage.SetActive(true);
        teleportImage.SetActive(false);
        skillUseImage.sprite = itemAssets.Instance.shieldSprite;
        skillUseImage.color = new Color(1, 0.8f, 0);
        AuthManager.instance.userSkill = 2;
        SkillUpdate();
    }

    public void OnTeleportClick()
    {
        speedImage.SetActive(false);
        shieldImage.SetActive(false);
        teleportImage.SetActive(true);
        skillUseImage.sprite = itemAssets.Instance.telepoSprite;
        skillUseImage.color = new Color(0.65f, 0.67f, 0.95f);
        AuthManager.instance.userSkill = 3;
        SkillUpdate();
    }

}
