using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingExit : MonoBehaviour
{
    public GameObject SettingsButton;

    public void OnClick()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
