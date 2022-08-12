using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RanKing : MonoBehaviourPunCallbacks
{
    public static RanKing instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<RanKing>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static RanKing m_instance;

    public TextMeshProUGUI[] ranking;
    public Canvas rankingTable;
    public ScoreTable scoreTable;
    public PhotonView PV;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
        PV = GetComponent<PhotonView>();
    }

    private void Start()
    {
        //ranking = GetComponentsInChildren<TextMeshProUGUI>();
        rankingTable = GameObject.Find("RanKingCanvas").GetComponent<Canvas>();
        if(TrainingController.instance.training != true)
            scoreTable = GameObject.Find("ScoreBg").GetComponent<ScoreTable>();
    }

    public void Win(string _dieName, int _ranking)
    {
        PV.RPC("WinRPC", RpcTarget.AllBufferedViaServer, _dieName, _ranking);
    }

    IEnumerator GameSet()
    {
        yield return new WaitForSecondsRealtime(3f);
        rankingTable.enabled = true;
        yield return new WaitForSecondsRealtime(3f);
        rankingTable.enabled = false;
        scoreTable.StartCoroutine("GameSetScore");
    }

    [PunRPC]
    public void WinRPC(string text,int rankingCount)
    {
        ranking[rankingCount-1].text = text;
    }

}
