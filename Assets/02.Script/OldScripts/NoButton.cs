using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoButton : MonoBehaviour
{

    public void OnNoButton()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
