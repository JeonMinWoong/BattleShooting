using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpToggle : MonoBehaviour
{
    public GameObject specifiPanel;
    public GameObject[] panelFade;

    public void OnToggle()
    {
        specifiPanel.SetActive(true);
        for (int i = 0; i < 2; i++)
        {
            panelFade[i].gameObject.SetActive(false);
        }
        
    }


}
