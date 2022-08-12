using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingBar : MonoBehaviour
{
    public void OnClick()
    {
        Setting.Instance.OnSetting();
    }
}
