using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillShow : MonoBehaviour
{
    public GameObject showImage;


    public void OnEnter()
    {
        showImage.SetActive(true);
    }

    public void OnOut()
    {
        showImage.SetActive(false) ;
    }    
}
