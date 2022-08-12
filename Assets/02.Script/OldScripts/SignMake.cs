using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignMake : MonoBehaviour
{
    public GameObject SignBox;
    public int clickCount;

    private void Awake()
    {
        clickCount = 0;
    }

    public void OnClick()
    {
        clickCount++;
        if (clickCount % 2 == 1)
            SignBox.SetActive(true);
        else
            SignBox.SetActive(false);
    }
}
