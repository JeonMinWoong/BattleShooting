using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class PlayerName : MonoBehaviour
{
    private TextMeshProUGUI nameText;

    // Start is called before the first frame update
    void Start()
    {
        nameText = GetComponent<TextMeshProUGUI>();

        if (AuthManager.instance.User != null)
        {
            nameText.text = $"{AuthManager.instance.usernickName}";
        }
        else
        {
            nameText.text = "null";
        }
    } 
}
