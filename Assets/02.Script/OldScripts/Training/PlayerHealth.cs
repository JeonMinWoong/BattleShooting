using Photon.Pun;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드
using Cinemachine;
using System.Collections;
using Photon.Realtime;

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class PlayerHealth : MonoBehaviourPunCallbacks
{   

    public Image healthUi;
    public Image shieldUi;
    public Button endButton;
    private CinemachineVirtualCamera observerCam;
    public Vector3 playerTr;
    private Animator playerAnimator; // 플레이어의 애니메이터
    private PlayerMove playerMove; // 플레이어 움직임 컴포넌트
    private PlayerShoot playerShoot; // 플레이어 슈터 컴포넌트
    public int maxHealth = 100; // 시작 체력
    public int currentHealth { get; protected set; } // 현재 체력
    public int maxShield = 100; // 실드 최대치
    public int currentShield { get; protected set; } // 현재 실드
    public bool isDeath { get; protected set; } // 사망 상태
    public PhotonView PV;


    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        if (PV.IsMine)
        {
            // 사용할 컴포넌트를 가져오기
            playerAnimator = GetComponent<Animator>();
            playerMove = GetComponent<PlayerMove>();
            playerShoot = GetComponent<PlayerShoot>();
            healthUi = GameObject.Find("HPUi").GetComponent<Image>();
            shieldUi = GameObject.Find("SPUi").GetComponent<Image>();
            endButton = GameObject.Find("BattleEndButton").GetComponent<Button>();
        }
    }

    private void Start()
    {
        
        if (PV.IsMine)
        {
            currentShield = 0;
            currentHealth = maxHealth;
            observerCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            playerTr = new Vector3(transform.position.x, transform.position.y, transform.position.z);
            observerCam.Follow = gameObject.transform;
            observerCam.LookAt = gameObject.transform;
        }
    }

    private void Update()
    {
        if (PV.IsMine)
        {
            if (isDeath == true)
                return;

            if (currentHealth > maxHealth)
                currentHealth = maxHealth;
            if (currentShield > maxShield)
                currentShield = maxShield;

            healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
            shieldUi.fillAmount = (float)currentShield / (float)maxShield;
        }
    }

    // 체력 회복
    public void RestoreHealth(int newHealth)
    {
        // 체력 갱신
        currentHealth += newHealth;
        healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
    }

    [PunRPC]
    public void RestoreShield(int newShield)
    {
        // 체력 갱신
        currentShield += newShield;
        shieldUi.fillAmount = (float)currentShield / (float)maxShield;
    }


    // 데미지 처리
    [PunRPC]
    public void TakeDamage(int Damage, GameObject murder)
    {
        if (photonView.IsMine)
        {
            if (isDeath == true)
                return;

            if (currentShield > 0)
            {
                currentShield -= Damage;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
            }
            else
            {
                currentHealth -= Damage;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                if (currentHealth <= 0)
                {
                    if (TrainingController.instance.training == false)
                    {
                        photonView.RPC("DieRPC", RpcTarget.AllBuffered);
                    }
                    else
                        TrainingDie();
                }

            }
        }
    }

    
    [PunRPC]
    public void DieRPC()
    {
            //Score.instance.Murder(gameObject, murder);
            // 애니메이터의 Die 트리거를 발동시켜 사망 애니메이션 재생
            playerAnimator.SetTrigger("Death");
            //Score.instance.Murder(gameObject, murder);
            // 플레이어 조작을 받는 컴포넌트들 비활성화
            playerMove.enabled = false;
            playerShoot.enabled = false;
            endButton.GetComponent<battleEnd>().PlayerDie();
            if (TrainingController.instance.training == true)
                Invoke("PlayerReSet", 1f);
        //observerCam.Follow = murder.transform;
        //observerCam.LookAt = murder.transform;
        GameManager.instance.StartCoroutine("GameOver");
          //gameObject.SetActive(false);
    }

   
    

    // 부활 처리
    void TrainingDie()
    {
        playerAnimator.SetTrigger("Death");
        GetComponent<PlayerMove>().enabled = false;
        StartCoroutine("PlayerReSet");
        gameObject.SetActive(false);    
    }

    IEnumerator PlayerReSet()
    {
        yield return new WaitForSeconds(2f);
        transform.position = playerTr;
        isDeath = false;
        GetComponent<PlayerMove>().enabled = true;
        currentHealth = maxHealth;
        playerAnimator.Rebind();
        playerShoot.currentBulletCount = 0;
        gameObject.SetActive(true);
    }
}
