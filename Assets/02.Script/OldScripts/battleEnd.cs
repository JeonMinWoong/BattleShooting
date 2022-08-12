using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class battleEnd : MonoBehaviour
{
    public GameObject battleEndYesNo;

    public void PlayerDie()
    {
        gameObject.SetActive(true);
    }


    public void OnEndButton()
    {
        battleEndYesNo.SetActive(true);
    }

}
