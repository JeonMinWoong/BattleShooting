using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class Skill : MonoBehaviourPun
{

    public GameObject player;
    public Image skillimage;
    public TextMeshProUGUI skillText;
    public Image coolTiem;
    public float currentCoolTime;
    public float maxCoolTime;
    public bool skillUse;
    private TestHealth playerHealth;
    private PlayerSkill_Specificity playerSkill;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerTest");
        playerHealth = player.GetComponent<TestHealth>();
        playerSkill = player.GetComponent<PlayerSkill_Specificity>();
    }

    public void ItemUse()
    {
        if (skillUse == false)
        {
            if (playerSkill.skillType == 1)
            {
                if (TrainingController.instance.training != true)
                    playerSkill.PV.RPC("SpeedUpRPC", RpcTarget.All);
                else
                    playerSkill.SpeedUpRPC();
                maxCoolTime = 200;
                currentCoolTime = maxCoolTime;
                StartCoroutine(CoolTime());
                skillUse = true;
            }
            else if (playerSkill.skillType == 2)
            {
                if (TrainingController.instance.training != true)
                    playerSkill.PV.RPC("ShieldRPC", RpcTarget.All);
                else
                    playerSkill.ShieldRPC();
                maxCoolTime = 400;
                currentCoolTime = maxCoolTime;
                StartCoroutine(CoolTime());
                skillUse = true;
            }
            else if (playerSkill.skillType == 3)
            {
                if (TrainingController.instance.training != true)
                    playerSkill.PV.RPC("TeleportationRPC", RpcTarget.All);
                else
                    playerSkill.TeleportationRPC();
                maxCoolTime = 250;
                currentCoolTime = maxCoolTime;
                StartCoroutine(CoolTime());
                skillUse = true;
            }
            else
                return;
        }
    }

    public void Click()
    {
        if (playerHealth.isDeath == true)
            return;
        if (skillUse == true)
            return;

        ItemUse();
    }


    IEnumerator CoolTime()
    {
        coolTiem.gameObject.SetActive(true);

        while (true)
        {
            currentCoolTime -= 0.1f;
            coolTiem.fillAmount = currentCoolTime / maxCoolTime;
            if (currentCoolTime < 0)
                break;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        coolTiem.gameObject.SetActive(false);
        currentCoolTime = maxCoolTime;
        coolTiem.fillAmount = currentCoolTime / maxCoolTime;
        skillUse = false;
    }
}
