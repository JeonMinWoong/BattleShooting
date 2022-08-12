using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Realtime;

public class GameManager :  MonoBehaviourPunCallbacks , IPunObservable
{
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수

    public Image zonTimeUi;
    public Image zonEx;
    public float maxZonTime;
    public float currentZonTime;
    public GameObject deathZon;
    public int deathZonCount;
    public int deathZonDownSize;
    public GameObject floor;
    private bool isBattle;
    public GameObject mapSelectUi;
    public GameObject SeletCam;
    public GameObject mainCam;
    public GameObject myPlayer;
    public GameObject mapCam;
    public Animator animator;
    public bool gameEnd;
    public bool gameStart;
    public GameObject playerPrefab;

    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
            PhotonNetwork.CurrentRoom.IsOpen = false;
            int randomx = Random.Range(-100, 101);
            int randomz = Random.Range(-100, 101);
        myPlayer = PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(floor.transform.position.x + randomx, 5.0f, floor.transform.position.x + randomz), Quaternion.identity);
        myPlayer.name = playerPrefab.name;

    }

    private void Start()
    {
        Debug.Log(PhotonNetwork.PlayerList.Length);

        gameEnd = false;
        gameStart = false;
        maxZonTime = 100;
        currentZonTime = 0;
        deathZonCount = 0;
        deathZonDownSize = 70;
        mapSelectUi.SetActive(true);
        mainCam.SetActive(false);
        mapCam.SetActive(true);
        

    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (currentZonTime >= maxZonTime)
                return;
            if (isBattle == true)
                zonTimeUi.fillAmount = currentZonTime / maxZonTime;
        }
        if(Input.GetKeyDown(KeyCode.Escape))
        {

            PhotonNetwork.LeaveRoom();
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Ready");
    }


    IEnumerator ZonTime()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (deathZonCount < 6)
            {
                if (currentZonTime < 0)
                    currentZonTime = 0;
                if (currentZonTime < maxZonTime)
                {
                    currentZonTime = currentZonTime + 0.1f;
                    zonEx.color = new Color(1, 1, 1, currentZonTime * 0.01f);
                    zonTimeUi.fillAmount = currentZonTime / maxZonTime;
                    yield return new WaitForSeconds(0.01f);
                    StartCoroutine("ZonTime");
                }
                if (currentZonTime >= maxZonTime)
                {
                    currentZonTime = 0;
                    StartCoroutine("ZonTimeEx");
                    deathZonCount++;

                }
            }
        }
    }

    IEnumerator ZonTimeEx()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            float randomX = Random.Range(-120, 121);
            float randomZ = Random.Range(-120, 121);
            if (deathZonCount == 0)
                deathZon.transform.position = new Vector3(transform.position.x + randomX, 0, transform.position.z + randomZ);
            Vector3 deathZonSize = deathZon.transform.localScale;
            deathZon.transform.localScale = new Vector3(350 - deathZonCount * deathZonDownSize, 50, 350 - deathZonCount * deathZonDownSize);
            yield return null;
        }
    }
    IEnumerator GameOver()
    {
        if (myPlayer != null)
        {
            myPlayer.GetComponent<PlayerShoot>().LosUp();
            yield return new WaitForSeconds(3f);
        }
    }

    IEnumerator GameEnd()
    {
        gameEnd = true;
        animator.SetTrigger("End");
        if (myPlayer != null)
        {
            myPlayer.GetComponent<PlayerShoot>().WinUp();
            myPlayer.GetComponent<PlayerShoot>().GoldUp();
            myPlayer.GetComponent<PlayerShoot>().MedalUp();
            myPlayer.GetComponent<PlayerShoot>().Stats();
        }
        yield return new WaitForSeconds(10f);
        PhotonNetwork.LeaveRoom();
    }

    public void BattleStart()
    {
        mapCam.SetActive(false);
        floor.SetActive(true);
        deathZon.SetActive(true);
        isBattle = true;
        mapSelectUi.SetActive(false);
        SeletCam.SetActive(false);
        mainCam.SetActive(true);
            myPlayer.GetComponent<PlayerMove>().enabled = true;
            myPlayer.GetComponent<PlayerShoot>().enabled = true;
            myPlayer.GetComponent<Rigidbody>().useGravity = true;
            myPlayer.transform.Find("MapSelect").gameObject.SetActive(false);
            myPlayer.GetComponent<PlayerShoot>().IngUp();
        StartCoroutine("ZonTime");
        gameStart = true;

    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(currentZonTime);
            stream.SendNext(maxZonTime);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            currentZonTime = (float)stream.ReceiveNext();
            maxZonTime = (float)stream.ReceiveNext();
            zonEx.color = new Color(1, 1, 1, currentZonTime * 0.01f);
            zonTimeUi.fillAmount = currentZonTime / maxZonTime;
        }
    }

}
