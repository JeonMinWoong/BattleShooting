using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.EventSystems;

public class TrainingGameManager : MonoBehaviourPunCallbacks , IPunObservable
{
    public static TrainingGameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<TrainingGameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static TrainingGameManager m_instance;



    public Image suppliseSP;
    public GameObject myPlayer;
    public GameObject floor;
    public int enemyCount;
    public Image zonTimeUi;
    public Image zonEx;
    public float maxZonTime;
    public float currentZonTime;
    public GameObject deathZon;
    public int deathZonCount;
    public int deathZonDownSize;
    private bool isBattle;
    public Animator animator;
    public bool gameEnd;
    public bool gameStart;
    //public PhotonView PV;
    public GameObject[] enemyGr;
    public bool Test;
    private int randomz;
    private int randomx;
    private int randomX;
    private int randomZ;
    public GameObject playerTouch;
    public GameObject playerAttack;
    public GameObject playerItem;
    public int supplieRandam;
    public int supplieRandam2;
    public TextMeshProUGUI playTimeText;
    public float playTimeSec;
    public int playTimeMin;

    private void Awake()
    {
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        Application.targetFrameRate = 60;
        //PV = GetComponent<PhotonView>();

    }

    private void Start()
    {
        ObjectManager.instance.ObjectInit();

        gameEnd = false;
        gameStart = false;
        maxZonTime = 100;
        currentZonTime = 0;
        deathZonCount = 0;
        deathZonDownSize = 15;
        playerTouch.SetActive(true);
        playerAttack.SetActive(true);
        playerItem.SetActive(true);
        supplieRandam = Random.Range(1, 11);
        supplieRandam2 = Random.Range(1, 11);

#if UNITY_EDITOR_WIN
        playerTouch.SetActive(false);
        playerAttack.GetComponent<PlayerAttackArrow>().enabled = false;
        playerAttack.GetComponent<Image>().sprite = null;
        playerAttack.GetComponent<Image>().color = new Color(0, 0, 0, 0.4f);
        playerAttack.transform.GetChild(0).gameObject.SetActive(false);
        playerAttack.GetComponent<RectTransform>().localScale = new Vector3(1.5f, 1.5f, 1.5f);
        playerItem.transform.GetChild(0).GetComponent<EventTrigger>().enabled = false;
        playerItem.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
#elif UNITY_ANDROID
        playerTouch.SetActive(true);
        playerAttack.GetComponent<PlayerAttackArrow>().enabled = true;
        playerAttack.transform.GetChild(1).gameObject.SetActive(false);
        playerItem.transform.GetChild(0).GetChild(5).gameObject.SetActive(false);
#endif
    }

    private void Update()
    {
        //if (PV.IsMine)
        {
            GameTime();

            if (currentZonTime >= maxZonTime)
                return;
            
            if (isBattle == true)
                zonTimeUi.fillAmount = currentZonTime / maxZonTime;
        }
        if (deathZonCount == 10)
        {
            ZonTimeFull();
            //PV.RPC("ZonTimeFull", RpcTarget.All);
        }

        
    }

    public void GameTime()
    {
        playTimeSec += Time.deltaTime;

        playTimeText.text = string.Format("{0:D2}:{1:D2}", playTimeMin, (int)playTimeSec);

        if (playTimeSec > 60)
        {
            playTimeSec = 0;
            playTimeMin++;
        }

    }

    [PunRPC]
    void ZonTimeFull()
    {
        zonEx.color = new Color(1, 0, 0, 1);
    }

    IEnumerator ZonTime()
    {
        //if (PV.IsMine)
        {
            if (deathZonCount < 10)
            {
                if (currentZonTime < 0)
                    currentZonTime = 0;
                if (currentZonTime < maxZonTime)
                {
                    currentZonTime = currentZonTime + 0.028f;
                    ZonExRPC();
                    //PV.RPC("ZonExRPC", RpcTarget.All);
                    zonTimeUi.fillAmount = currentZonTime / maxZonTime;
                    yield return new WaitForSecondsRealtime(0.01f);
                    StartCoroutine(ZonTime());
                }
                if (currentZonTime >= maxZonTime)
                {
                    currentZonTime = 0;
                    StartCoroutine(ZonTimeEx());
                    deathZonCount++;
                    if (deathZonCount == supplieRandam)
                    {
                        SuppliseEnd();
                    }
                    if (deathZonCount == supplieRandam2)
                    {
                        SuppliseEnd();
                    }


                }
            }
        }
    }

    public void SuppliseEnd()
    {
        SuppliseAni();
        //PV.RPC("SuppliseAni", RpcTarget.All);
        AudioManager.Instance.PlaySound("SuppliesDown", transform.position, 10f + Random.Range(-0.1f, 0.1f));
        //PhotonNetwork.InstantiateRoomObject("SuppliesBox", new Vector3(floor.transform.position.x, 3f, floor.transform.position.z), Quaternion.identity);

        Instantiate(Resources.Load<GameObject>("SuppliesBoxOff"), new Vector3(floor.transform.position.x, 3f, floor.transform.position.z), Quaternion.identity);
    }
    
    [PunRPC]
    public void SuppliseAni()
    {
        animator.SetTrigger("Supplies");
    }

    [PunRPC]
    public void ZonExRPC()
    {
        zonEx.color = new Color(1, 1, 1, currentZonTime * 0.01f);
    }

    IEnumerator ZonTimeEx()
    {
        //if (PV.IsMine)
        {
            randomX = Random.Range(-15, 15);
            randomZ = Random.Range(-15, 15);

            if (deathZonCount == 0)
            {
                deathZon.transform.position = new Vector3(transform.position.x + randomX, 0, transform.position.z + randomZ);
            }
            Vector3 deathZonSize = deathZon.transform.localScale;
            deathZon.transform.localScale = new Vector3(150 - deathZonCount * deathZonDownSize, 50, 150 - deathZonCount * deathZonDownSize);
            yield return null;
        }
    }

    public void TrainingStart()
    {       
        floor.SetActive(true);
        deathZon.SetActive(true);
        isBattle = true;
        myPlayer.GetComponent<PlayerScore>().enabled = true;
        myPlayer.GetComponent<TestShoot>().enabled = true;
        myPlayer.GetComponent<Rigidbody>().useGravity = true;
        StartCoroutine(ZonTime());
        gameStart = true;
    }

    IEnumerator PlayerDeath()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        playerItem.GetComponent<ConsumableItem>().currentCoolTime = 0;
        playerItem.GetComponent<ConsumableItem>().coolTiem.fillAmount = playerItem.GetComponent<ConsumableItem>().currentCoolTime / playerItem.GetComponent<ConsumableItem>().maxCoolTime;
        playerItem.GetComponent<ConsumableItem>().itemUse = false;
        playerTouch.SetActive(false);
        playerAttack.SetActive(false);
        playerItem.SetActive(false);
    }

    IEnumerator PlayerReSet()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        playerTouch.SetActive(true);
        playerAttack.SetActive(true);
        playerItem.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(currentZonTime);
            stream.SendNext(maxZonTime);
            stream.SendNext(zonTimeUi.fillAmount);
            stream.SendNext(enemyCount);
            stream.SendNext(deathZonCount);
            stream.SendNext(supplieRandam);
            stream.SendNext(supplieRandam2);
            stream.SendNext(playTimeText.text);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            currentZonTime = (float)stream.ReceiveNext();
            maxZonTime = (float)stream.ReceiveNext();
            zonTimeUi.fillAmount = (float)stream.ReceiveNext();
            enemyCount = (int)stream.ReceiveNext();
            deathZonCount = (int)stream.ReceiveNext();
            supplieRandam = (int)stream.ReceiveNext();
            supplieRandam2 = (int)stream.ReceiveNext();
            playTimeText.text = (string)stream.ReceiveNext();
        }
    }
}
