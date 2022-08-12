using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class Loding : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI loding;
    public int lodingCount;

    private void Awake()
    {
        if (TestGameManager.instance.Test == true)
            lodingCount = 0;
        else
            lodingCount = 5;
        
    }

    private void Start()
    {
        StartCoroutine("LodingStart");
        
    }

    IEnumerator LodingStart()
    {
        loding.text = ("Loding.." + lodingCount.ToString());
        lodingCount--;
        yield return new WaitForSecondsRealtime(1f);
        if (lodingCount > 0)
            StartCoroutine("LodingStart");
        else
        {
            loding.text = ("START!!");
            TestGameManager.instance.BattleStart();
        }
    }
}
