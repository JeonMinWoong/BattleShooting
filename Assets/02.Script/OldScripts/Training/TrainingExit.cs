using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement ;

public class TrainingExit : MonoBehaviour
{
    public void OnClick()
    {
        SceneManager.LoadScene("Lobby");
    }
}
