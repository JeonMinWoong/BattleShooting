using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameOk : MonoBehaviour
{

    public void OnNameOk()
    {
        gameObject.transform.parent.gameObject.SetActive(false);
    }
}
