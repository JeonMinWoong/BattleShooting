using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerWin;
    public TextMeshProUGUI playerKill;
    public TextMeshProUGUI playerIng;
    public TextMeshProUGUI playerLos;
    public TextMeshProUGUI playerExp;
    public TextMeshProUGUI playerLv;
    public TextMeshProUGUI playerNameUI;

    public GameObject player;
    public string playerNaming;
    public int playerWinCount;
    public int playerKillCount;
    public int playerIngCount;
    public int playerLosCount;
    public int playerGold;
    public int playerMedal;
    public int playerLevel;
    public float playerCurrentExp;
    public float playerMaxExp;
    public float playerExcess;
    public int playerSkill;

    private void Awake()
    {
        if (TrainingController.instance.training != true)
        {
            playerName = GameObject.Find("NameIPText (TMP)").GetComponent<TextMeshProUGUI>();
            playerWin = GameObject.Find("WinScore Text (TMP)").GetComponent<TextMeshProUGUI>();
            playerKill = GameObject.Find("KillScore Text (TMP)").GetComponent<TextMeshProUGUI>();
            playerIng = GameObject.Find("IngScore Text (TMP)").GetComponent<TextMeshProUGUI>();
            playerLos = GameObject.Find("LosScore Text (TMP)").GetComponent<TextMeshProUGUI>();
            playerExp = GameObject.Find("ExpScore Text (TMP)").GetComponent<TextMeshProUGUI>();
            playerLv = GameObject.Find("PlayerLv (TMP)").GetComponent<TextMeshProUGUI>();
            playerNameUI = GameObject.Find("PlayerName Text (TMP)").GetComponent<TextMeshProUGUI>();
        }
    }

    public void Start()
    {

        if (AuthManager.instance != null)
        {
            playerExcess = 0;
            playerNaming = player.GetComponent<TestHealth>().playerName;
            playerWinCount = AuthManager.instance.userWin;
            playerKillCount = AuthManager.instance.userKill;
            playerIngCount = AuthManager.instance.userIng;
            playerLosCount = AuthManager.instance.userLos;
            playerGold = AuthManager.instance.userGold;
            playerMedal = AuthManager.instance.userMedal;
            playerLevel = AuthManager.instance.userLevel;
            playerCurrentExp = AuthManager.instance.userCurrentExp;
            playerMaxExp = AuthManager.instance.userMaxExp;
            playerSkill = AuthManager.instance.userSkill;
            gameObject.GetComponent<PlayerSkill_Specificity>().skillType = playerSkill;
            playerLv.text = "Lv." + AuthManager.instance.userLevel.ToString();
            playerNameUI.text = AuthManager.instance.usernickName.ToString();
        }
    }

    public void ScoreUpdate()
    {
        playerName.text = player.GetComponent<TestHealth>().playerName;
        playerWin.text = AuthManager.instance.userWin.ToString();
        playerKill.text = AuthManager.instance.userKill.ToString();
        playerIng.text = AuthManager.instance.userIng.ToString();
        playerLos.text = AuthManager.instance.userLos.ToString();
        playerExp.text = AuthManager.instance.userCurrentExp.ToString();
    }

    public void StateUpdate()
    {
        AuthManager.instance.OnClickUpdateChildren();
        ScoreUpdate();
    }

    public void KillUp()
    {
        AuthManager.instance.userKill++;
        StateUpdate();
    }

    public void WinUp()
    {
        AuthManager.instance.userWin++;
        StateUpdate();
    }

    public void IngUp()
    {
        AuthManager.instance.userIng++;
        StateUpdate();
    }

    public void LosUp()
    {
        AuthManager.instance.userLos++;
        StateUpdate();
    }

    public void GoldUp()
    {
        if (player.GetComponent<TestHealth>().ranking == 0)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 100;
        else if (player.GetComponent<TestHealth>().ranking == 2)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 75;
        else if (player.GetComponent<TestHealth>().ranking == 3)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 50;
        else if (player.GetComponent<TestHealth>().ranking == 4 || player.GetComponent<TestHealth>().ranking == 5)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 40;
        else if (player.GetComponent<TestHealth>().ranking == 6 || player.GetComponent<TestHealth>().ranking == 7)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 30;
        else if (player.GetComponent<TestHealth>().ranking == 8 || player.GetComponent<TestHealth>().ranking == 9
            || player.GetComponent<TestHealth>().ranking == 10 || player.GetComponent<TestHealth>().ranking == 11)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 25;
        else if (player.GetComponent<TestHealth>().ranking == 12 || player.GetComponent<TestHealth>().ranking == 13
            || player.GetComponent<TestHealth>().ranking == 14 || player.GetComponent<TestHealth>().ranking == 15)
            AuthManager.instance.userGold = AuthManager.instance.userGold + 20;
        else
            AuthManager.instance.userGold = AuthManager.instance.userGold + 10;

        AuthManager.instance.userGold = AuthManager.instance.userGold + player.GetComponent<TestShoot>().killCount * 10 + player.GetComponent<PlayerSkill_Specificity>().spMoneyPlus;

        StateUpdate();
    }

    public void MedalUp()
    {
        if (player.GetComponent<TestHealth>().ranking == 0)
            AuthManager.instance.userMedal = AuthManager.instance.userMedal + 3;
        else if (player.GetComponent<TestHealth>().ranking == 2)
            AuthManager.instance.userMedal = AuthManager.instance.userMedal + 2;
        else if (player.GetComponent<TestHealth>().ranking == 3)
            AuthManager.instance.userMedal = AuthManager.instance.userMedal + 1;
        else
            return;
        StateUpdate();
    }

    public void LevelUp()
    {
        AuthManager.instance.userLevel++;
        AuthManager.instance.userPlayerSpPoint++;
        StateUpdate();
        MaxExpUp();
    }

    public void ExpUp()
    {
        if (player.GetComponent<TestHealth>().ranking == 0)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 30 + player.GetComponent<TestShoot>().killCount * 5;
        else if (player.GetComponent<TestHealth>().ranking == 2)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 28.5f ;
        else if (player.GetComponent<TestHealth>().ranking == 3)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 27f;
        else if (player.GetComponent<TestHealth>().ranking == 4 || player.GetComponent<TestHealth>().ranking == 5)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 25;
        else if (player.GetComponent<TestHealth>().ranking == 6 || player.GetComponent<TestHealth>().ranking == 7)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 20;
        else if (player.GetComponent<TestHealth>().ranking == 8 || player.GetComponent<TestHealth>().ranking == 9
            || player.GetComponent<TestHealth>().ranking == 10 || player.GetComponent<TestHealth>().ranking == 11)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 18;
        else if (player.GetComponent<TestHealth>().ranking == 12 || player.GetComponent<TestHealth>().ranking == 13
            || player.GetComponent<TestHealth>().ranking == 14 || player.GetComponent<TestHealth>().ranking == 15)
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 15;
        else
            AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + 10;

        AuthManager.instance.userCurrentExp = AuthManager.instance.userCurrentExp + player.GetComponent<TestShoot>().killCount * 5;

        StateUpdate();

        if (AuthManager.instance.userCurrentExp >= AuthManager.instance.userMaxExp)
        {
            LevelUp();
        }
        
    }

    public void MaxExpUp()
    {
        if (AuthManager.instance.userCurrentExp == AuthManager.instance.userMaxExp)
        {
            AuthManager.instance.userCurrentExp = 0;
            AuthManager.instance.userMaxExp = AuthManager.instance.userMaxExp + (AuthManager.instance.userLevel - 1) * 25;
        }
        else if (AuthManager.instance.userCurrentExp > AuthManager.instance.userMaxExp)
        {
            playerExcess = AuthManager.instance.userCurrentExp - AuthManager.instance.userMaxExp;
            AuthManager.instance.userCurrentExp = 0;
            AuthManager.instance.userCurrentExp += playerExcess;
            AuthManager.instance.userMaxExp = AuthManager.instance.userMaxExp + (AuthManager.instance.userLevel - 1) * 25;
        }
        else
            return;

        StateUpdate();
    }
}