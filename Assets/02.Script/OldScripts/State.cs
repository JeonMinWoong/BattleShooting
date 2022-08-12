using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    public GameObject stateBar;
    public int clickCount;

    private void Awake()
    {
        clickCount = 0;
    }

    public void OnClick()
    {
        clickCount++;
        if(clickCount % 2 == 1 )
            stateBar.SetActive(true);
        else
            stateBar.SetActive(false);
    }

}
