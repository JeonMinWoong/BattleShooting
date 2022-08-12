using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpNum : MonoBehaviour 
{
    public int spCount;
    public Button spPlus;
    public Button spSub;
    public SpCount SP;
    public GameObject spShow;
    public GameObject spImage;

    public void Plus()
    {
        SP.Plus(spCount);
    }

    public void Sub()
    {
        SP.Subtract(spCount);
    }

    public void Show()
    {
        spShow.SetActive(true);
    }


    public void ShowEnd()
    {
        spShow.SetActive(false);
    }
}
