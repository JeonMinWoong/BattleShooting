using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInpo : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI playerName;
    [SerializeField]
    TextMeshProUGUI playerLv;
    void Start()
    {
        if (AuthManager.instance != null)
        {
            playerName.text = AuthManager.instance.usernickName.ToString();
            playerLv.text = "Lv." + AuthManager.instance.userLevel.ToString();
        }
    }

}
