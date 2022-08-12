using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class loginDateManager : MonoBehaviour
{
    public InputField emailInfo;

    public void Start()
    {
        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);
        Load();
    }

    public void Save()
    {
        PlayerPrefs.SetString("email", emailInfo.text);
    }

    public void Load()
    {
        if(PlayerPrefs.HasKey("email"))
            emailInfo.text = PlayerPrefs.GetString("email");
    }
}
