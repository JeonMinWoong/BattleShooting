using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class RoomText : MonoBehaviourPunCallbacks
{
    public static RoomText instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<RoomText>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static RoomText m_instance;

    public TextMeshProUGUI lobbyText;
    public string[] count = { "", ".", "..", "...", "...." };
    public bool waiting;
    public int currentPlayer;
    public int waitingCount;
    public string[] gameReadyCount;
    public string[] gameCount;
    public GameObject[] player;
    public TextMeshProUGUI playerCountText;
    public TextMeshProUGUI[] nameText;
    public int playerMaxCount;
    public Button cancelButton;

    Coroutine coCheck;

    public PhotonView PV;
    public List<GameObject> idGameObject;
    GameObject myPlayer;
    bool isCheck;

    private void Awake()
    {
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        PhotonNetwork.Instantiate("ID", transform.position, Quaternion.identity).name = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
    }

    public void Start()
    {
        PhotonNetwork.automaticallySyncScene = true;
        
        PhotonNetwork.IsMessageQueueRunning = true;
        waiting = true;
        
        PV.RPC("PlayerListAdd", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void PlayerListAdd()
    {
        //PhotonNetwork.Instantiate("ID", transform.position, Quaternion.identity).name = PhotonNetwork.LocalPlayer.ActorNumber.ToString();

        GameObject[] findPlayer = GameObject.FindGameObjectsWithTag("Player");
        idGameObject.Clear();
        for (int i = 0; i < findPlayer.Length; i++)
        {
            Debug.Log(findPlayer[i].name);
            findPlayer[i].transform.SetParent(transform);
            idGameObject.Add(findPlayer[i]);
        }

        currentPlayer = PhotonNetwork.CurrentRoom.PlayerCount;
        playerMaxCount = PhotonNetwork.CurrentRoom.MaxPlayers;
        playerCountText.text = string.Format("{0}/{1}", currentPlayer, playerMaxCount);

        for (int i = 0; i < currentPlayer; i++)
        {
            if (nameText[i].text == "" && (i + 1).ToString() == idGameObject[0].name)
            {
                nameText[i].text = idGameObject[0].GetComponent<PhotonView>().Owner.NickName;
            }
            player[i].GetComponent<Image>().color = new Color(0, 1, 0);
           
        }

        if (currentPlayer <= 1)
        {
            lobbyText.text = "Waiting for other players.";
        }
        else
        {
            PV.RPC("Set", RpcTarget.AllViaServer);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)         // 다른 유저가 접속했을 때 
    {
        Debug.Log(newPlayer + "방접속");
    }

    public void Update()
    {
        if (currentPlayer == 8)
        {
            waiting = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        if (waitingCount >= 4)
        {
            waiting = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }

        if (currentPlayer > 1)
            cancelButton.gameObject.SetActive(false);
    }

    [PunRPC]
    void Set()
    {
        if (coCheck != null)
        {
            StopAllCoroutines();

            coCheck = null;

            coCheck = StartCoroutine(PlayerWaiting());
        }
        else
        {
            coCheck = StartCoroutine(PlayerWaiting());
            waitingCount = 0;
        }
    }

    //IEnumerator CheckPlayerCount()
    //{
    //    //PhotonNetwork.PlayerList[currentPlayer - 1].NickName = idGameObject[currentPlayer - 1].GetComponent<PhotonView>().Owner.NickName;

    //    //yield return new WaitForSeconds(1);

    //    //idGameObject = idGameObject.OrderBy(go => go.name).ToList();

    //    //for (int i = 0; i < currentPlayer; i++)
    //    //{
    //    //    if (nameText[i].text == "")
    //    //    {
    //    //        Debug.Log(idGameObject[i].name);
    //    //        nameText[i].text = idGameObject[i].GetComponent<PhotonView>().Owner.NickName;
    //    //    }
    //    //    player[i].GetComponent<Image>().color = new Color(0, 1, 0);
    //    //}
    //}

    IEnumerator PlayerWaiting()
    {
        idGameObject = idGameObject.OrderBy(go => go.name).ToList();

        while (waiting)
        {
            for (int i = 0; i < 5; i++)
            {
                lobbyText.text = ("Player Waiting" + count[i]);
                yield return new WaitForSecondsRealtime(1f);
                waitingCount++;
            }
        }
        if (waiting == false)
        {
            for (int i = 0; i < 13; i++)
            {
                lobbyText.text = gameReadyCount[i];
                yield return new WaitForSecondsRealtime(0.01f);
            }
            for (int i = 0; i < 9; i++)
            {
                lobbyText.text = gameCount[i];
                yield return new WaitForSecondsRealtime(0.01f);
            }
            for (int i = 0; i < currentPlayer; i++)
            {
                if (nameText[i].text == "")
                {
                    nameText[i].text = idGameObject[i].GetComponent<PhotonView>().Owner.NickName;
                }
            }
            if (currentPlayer < playerMaxCount)
            {
                for (int i = 7; i >= currentPlayer; i--)
                {
                    player[i].GetComponent<Image>().color = new Color(1, 0, 0);
                    nameText[i].text = "Enemy";
                    //Debug.Log("색변환");
                     
                }
                playerCountText.text = "8/8";
            }
            if (PhotonNetwork.IsMasterClient)
            {
                yield return new WaitForSecondsRealtime(1f);
                PhotonNetwork.LoadLevel("Battle");
            }
        }
    }

    public void CancelButton()
    {
        PhotonNetwork.Disconnect();
        Destroy(gameObject);
    }
}
       
