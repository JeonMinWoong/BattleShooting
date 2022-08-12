using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScoreTest : MonoBehaviourPun, IPunObservable
{
    public static ScoreTest instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<ScoreTest>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static ScoreTest m_instance;

    public TextMeshProUGUI enemyScore;
    public GameObject gameManager;
    public GameObject[] player;
    public GameObject[] enemy;
    public int players;
    public TextMeshProUGUI murderText;
    public TextMeshProUGUI murderText2;
    public TextMeshProUGUI murderText3;
    public GameObject KillBox;
    public int gameScore;
    public bool scoreStart;
    public int rankingCount;
    public GameObject winner;
    public int textCount;
    public Vector3 textRPos;
    public Vector3 textRPos2;
    public Vector3 textRPos3;
    public int playerInCount;
    public int enemyInCount;
    public PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine("ScoreStart");
        rankingCount = 9;
        textCount = 0;
        if (TrainingController.instance.training == false)
        {
            textRPos = murderText.rectTransform.position;
            textRPos2 = murderText2.rectTransform.position;
            textRPos3 = murderText3.rectTransform.position;
        }
    }

    void Update()
    {
        ScoreText();
    }

    public void ScoreText()
    {
        if (scoreStart == true && TrainingController.instance.training == false)
        {
            player = GameObject.FindGameObjectsWithTag("MapPlayer");
            enemy = GameObject.FindGameObjectsWithTag("MapEnemy");
            playerInCount = player.Length;
            enemyInCount = enemy.Length;
            enemyScore.text = (enemyInCount + playerInCount + "/8");
            gameScore = enemyInCount + playerInCount;



            if (scoreStart == true && gameScore <= 1 && TestGameManager.instance.gameEnd == false && TestGameManager.instance.gameStart == true && TestGameManager.instance.Test == false)
            {
                if (playerInCount == 0 && enemyInCount == 1)
                {
                    winner = GameObject.FindWithTag("Enemy");
                    RanKing.instance.Win(winner.GetComponent<EnemyHealthTest>().enemyName, rankingCount = 1);
                }
                else if (enemyInCount == 0 && playerInCount == 1)
                {
                    winner = GameObject.FindWithTag("Player");
                    RanKing.instance.Win(winner.GetComponent<TestHealth>().playerName, rankingCount = 1);
                }
                TestGameManager.instance.StartCoroutine("GameEnd");
                RanKing.instance.StartCoroutine("GameSet");

            }

            if (murderText.text == "" && murderText2.text != "")
            {
                murderText2.rectTransform.position = textRPos;
                murderText3.rectTransform.position = textRPos2;
                murderText.rectTransform.position = textRPos3;
                //Debug.Log("2번");

            }
            if (murderText.text == "" && murderText2.text == "" && murderText3.text != "")
            {
                murderText2.rectTransform.position = textRPos3;
                murderText3.rectTransform.position = textRPos;
                murderText.rectTransform.position = textRPos2;
                // Debug.Log("3번 >> 2번");

            }

            if (murderText.text == "" && murderText2.text == "" && murderText3.text == "")
            {
                murderText.rectTransform.position = textRPos;
                murderText2.rectTransform.position = textRPos2;
                murderText3.rectTransform.position = textRPos3;
                //Debug.Log("1번");
            }
        }


    }

    IEnumerator ScoreStart()
    {
        yield return new WaitForSecondsRealtime(6f);
        scoreStart = true;

    }

    public void Murder(GameObject _object, GameObject murder)
    {
        string dieName = "";
        if (_object.tag == "Enemy")
            dieName = _object.GetComponent<EnemyHealthTest>().enemyName.ToString();
        else
            dieName = _object.GetComponent<TestHealth>().playerName.ToString();

        PV.RPC("Ranking", RpcTarget.AllViaServer,dieName);

        Debug.Log("현재 랭킹 : " + rankingCount);

        if (rankingCount < 1)
            return;

        if (textCount == 3)
            textCount = 0;

        if (textCount == 0)
        {
            if (_object.tag == "Enemy")
                _object.gameObject.GetComponent<EnemyHealthTest>().ranking = rankingCount;
            else if (_object.tag == "Player")
                _object.gameObject.GetComponent<TestHealth>().ranking = rankingCount;

            if (_object.tag == "Enemy" && murder.tag == "Enemy")
                murderText.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "Player")
                murderText.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Enemy")
                murderText.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Player")
                murderText.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "DeathZon")
                murderText.text = (murder.name.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "DeathZon")
                murderText.text = (murder.name.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else
                murderText.text = (" 수정 필요 \n");
            PV.RPC("MurderText", RpcTarget.All, murderText.text);
        }
        else if (textCount == 1)
        {
            if (_object.tag == "Enemy")
                _object.gameObject.GetComponent<EnemyHealthTest>().ranking = rankingCount;
            else if (_object.tag == "Player")
                _object.gameObject.GetComponent<TestHealth>().ranking = rankingCount;

            if (_object.tag == "Enemy" && murder.tag == "Enemy")
                murderText2.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "Player")
                murderText2.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Enemy")
                murderText2.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Player")
                murderText2.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "DeathZon")
                murderText2.text = (murder.name.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "DeathZon")
                murderText2.text = (murder.name.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else
                murderText2.text = (" 수정 필요 \n");
            PV.RPC("MurderText2", RpcTarget.All, murderText2.text);
        }
        else if (textCount == 2)
        {
            if (_object.tag == "Enemy")
                _object.gameObject.GetComponent<EnemyHealthTest>().ranking = rankingCount;
            else if (_object.tag == "Player")
                _object.gameObject.GetComponent<TestHealth>().ranking = rankingCount;

            if (_object.tag == "Enemy" && murder.tag == "Enemy")
                murderText3.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "Player")
                murderText3.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Enemy")
                murderText3.text = (murder.GetComponent<EnemyHealthTest>().enemyName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "Player")
                murderText3.text = (murder.GetComponent<TestHealth>().playerName.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Player" && murder.tag == "DeathZon")
                murderText3.text = (murder.name.ToString() + ">>" + _object.GetComponent<TestHealth>().playerName.ToString() + "\n");
            else if (_object.tag == "Enemy" && murder.tag == "DeathZon")
                murderText3.text = (murder.name.ToString() + ">>" + _object.GetComponent<EnemyHealthTest>().enemyName.ToString() + "\n");
            else
                murderText3.text = (" 수정 필요 \n");
            PV.RPC("MurderText3", RpcTarget.All, murderText3.text);
        }
        textCount++;
    }

    [PunRPC]
    public void Ranking(string _dieName)
    {
        rankingCount--;
        RanKing.instance.Win(_dieName, rankingCount);
    }

    [PunRPC]
    public void MurderText(string KillLog)
    {
        murderText.text = KillLog;
        StartCoroutine(MurderTextGo());
    }

    [PunRPC]
    public void MurderText2(string KillLog)
    {
        murderText2.text = KillLog;
        StartCoroutine(MurderTextGo2());
    }

    [PunRPC]
    public void MurderText3(string KillLog)
    {
        murderText3.text = KillLog;
        StartCoroutine(MurderTextGo3());
    }


    IEnumerator MurderTextGo()
    {
        KillBox.SetActive(true);
        murderText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        murderText.text = "";
        textCount = 0;
    }

    IEnumerator MurderTextGo2()
    {
        KillBox.SetActive(true);
        murderText2.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2.5f);
        murderText2.text = "";
        textCount = 0;
    }

    IEnumerator MurderTextGo3()
    {
        KillBox.SetActive(true);
        murderText3.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        murderText3.text = "";
        textCount = 0;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(playerInCount);
            stream.SendNext(enemyScore.text);
            stream.SendNext(textCount);
            stream.SendNext(rankingCount);
 
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            playerInCount = (int)stream.ReceiveNext();
            enemyScore.text = (string)stream.ReceiveNext();
            textCount = (int)stream.ReceiveNext();
            rankingCount = (int)stream.ReceiveNext();
        }
    }
}