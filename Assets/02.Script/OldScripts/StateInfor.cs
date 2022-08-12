using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StateInfor : MonoBehaviour
{
    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerWin;
    public TextMeshProUGUI playerKill;
    public TextMeshProUGUI playerIng;
    public TextMeshProUGUI playerLos;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI medalText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI currentExpSize;
    public TextMeshProUGUI currentExpText;
    public Image levelExp;
    public GameObject trainingMg;

    public void Awake()
    {

        if (AuthManager.instance.User != null)
        {
            playerName.text = AuthManager.instance.usernickName.ToString(); 
            playerWin.text = AuthManager.instance.userWin.ToString();       
            playerKill.text = AuthManager.instance.userKill.ToString();     
            playerIng.text = AuthManager.instance.userIng.ToString();
            playerLos.text = AuthManager.instance.userLos.ToString();
            goldText.text = AuthManager.instance.userGold.ToString();
            medalText.text = AuthManager.instance.userMedal.ToString();
            levelText.text = AuthManager.instance.userLevel.ToString();
            currentExpSize.text = ((AuthManager.instance.userCurrentExp / AuthManager.instance.userMaxExp)*100).ToString() + "%";
            levelExp.fillAmount = AuthManager.instance.userCurrentExp / AuthManager.instance.userMaxExp;
            currentExpText.text = AuthManager.instance.userCurrentExp.ToString() + "/" + AuthManager.instance.userMaxExp.ToString();
            trainingMg.GetComponent<TrainingManager>().TrainingStart();
        }                                                    
        else
        {
            playerName.text = "";
            playerWin.text = "";
            playerKill.text = "";
            playerIng.text = "";
            playerLos.text = "";
            goldText.text = "";
            medalText.text = "";
            levelText.text = "";
            currentExpText.text = "";
        }
    }
}
