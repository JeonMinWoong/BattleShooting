using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cancal : MonoBehaviour
{
    public GameObject signUpPanel;
    public GameObject signUpButten;

    private void Start()
    {
        signUpButten = GameObject.Find("SignUpButton");
    }

    public void OnClikCancel()
    {
        signUpPanel.SetActive(false);
        signUpButten.GetComponent<SignMake>().clickCount++;
    }
}
