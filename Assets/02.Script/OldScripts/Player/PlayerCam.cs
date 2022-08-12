using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PlayerCam : MonoBehaviourPunCallbacks
{
    public CinemachineVirtualCamera dieCam;
    public GameObject player;

    private void Start()
    {
        player = GameObject.Find("PlayerTest");
    }

    public void Update()
    {
        if (SceneManager.GetActiveScene().name.Contains("BattleTest"))
            return;

        int P = Random.Range(0,2);
        if (ScoreTest.instance.player.Length == 0)
            P = 1;
        else if (ScoreTest.instance.enemy.Length == 0)
            P = 0;
        int PC = Random.Range(0, ScoreTest.instance.player.Length);
        int EC = Random.Range(0, ScoreTest.instance.enemy.Length);
        if (player != null)
        {
            if (player.gameObject.activeSelf == false)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (P == 0)
                    {
                        if (ScoreTest.instance.player[PC] != null)
                        {
                            dieCam.Follow = ScoreTest.instance.player[PC].gameObject.transform;
                            dieCam.LookAt = ScoreTest.instance.player[PC].gameObject.transform;
                        }
                    }
                    else if (P == 1)
                    {
                        if (ScoreTest.instance.enemy[EC] != null)
                        {
                            dieCam.Follow = ScoreTest.instance.enemy[EC].gameObject.transform;
                            dieCam.LookAt = ScoreTest.instance.enemy[EC].gameObject.transform;
                        }
                    }
                    else
                        return;
                }
            }
        }
    }
}
