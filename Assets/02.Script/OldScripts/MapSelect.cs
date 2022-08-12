using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class MapSelect : MonoBehaviourPunCallbacks
{
    public GameObject player;

    private void Awake()
    {
            player = GameObject.Find("PlayerTest");
    }

    public void OnClick_Map()
    {
            var Point = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
            //Debug.Log("월드의 스크린 좌표: " + Point);

            Vector3 worldMapPos;
            worldMapPos.x = Point.x * 120;
            worldMapPos.z = Point.y * 120;
            worldMapPos.y = 7.5f;

            player.transform.position = worldMapPos;
        
    }
}
