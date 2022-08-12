using Photon.Pun;
using UnityEngine;
using UnityEngine.UI; // UI 관련 코드
using Cinemachine;
using System.Collections;
using Photon.Realtime;

// 플레이어 캐릭터의 생명체로서의 동작을 담당
public class TestHealth : MonoBehaviourPun, IPunObservable
{

    public Image healthUi;
    public Image shieldUi;
    public Button endButton;
    private CinemachineVirtualCamera observerCam;
    private Animator playerAnimator; // 플레이어의 애니메이터
    private TestMove playerMove; // 플레이어 움직임 컴포넌트
    private TestShoot playerShoot; // 플레이어 슈터 컴포넌트
    public float maxHealth = 100; // 시작 체력
    public float currentHealth;
    public int maxShield = 100; // 실드 최대치
    public float currentShield;
    public bool isDeath;
    public bool isDeathText;
    public int ranking;
    public string playerName;
    public PhotonView pV;
    private Rigidbody rigid;

    [Header("DeathZon")]
    public bool playerOnZon;
    public Image onZonUi;
    public Animator canvasAnimator;
    public GameObject mapPlayer;
    public GameObject mapSelect;
    private CinemachineVirtualCamera cV;

    [Header("새로생성")]
    public Canvas outCanvas;
    public DamageText inGameTextCanvas;
    public bool shock;
    public ParticleSystem[] playerParticle;
    public int inGameText;
    public PlayerSkill_Specificity playerSkill;
    public Vector3 playerTr;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();

        if (TrainingController.instance.training == true)
        {
            playerName = (AuthManager.instance.usernickName);
            playerAnimator = GetComponent<Animator>();
            playerMove = GetComponent<TestMove>();
            playerShoot = GetComponent<TestShoot>();
            playerSkill = GetComponent<PlayerSkill_Specificity>();
            healthUi = GameObject.Find("HPUi").GetComponent<Image>();
            shieldUi = GameObject.Find("SPUi").GetComponent<Image>();
            endButton = GameObject.Find("TrainingEnd").GetComponent<Button>();
            canvasAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
            onZonUi = canvasAnimator.gameObject.transform.Find("OnZonUi").GetComponent<Image>();
            playerOnZon = true;
            isDeath = false;
            isDeathText = false;
            playerTr = transform.position;
            float basicshield = playerSkill.spBasicShield;
            currentShield = 0 + basicshield;
            float spMaxheath = playerSkill.spMaxHeath;
            maxHealth = maxHealth + spMaxheath;
            currentHealth = maxHealth;
            observerCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
            observerCam.Follow = gameObject.transform;
            observerCam.LookAt = gameObject.transform;
            mapPlayer.GetComponent<MeshRenderer>().material.color = Color.green;
            mapSelect.GetComponent<SpriteRenderer>().color = Color.green;
            StartCoroutine("SpHeathRecover");
            StartCoroutine("SpShieldRegen");
        }
        else
        {
            pV = GetComponent<PhotonView>();
            if (pV.IsMine)
            {
                if (TestGameManager.instance.Test != true)
                    playerName = (AuthManager.instance.usernickName);
                else
                    playerName = "PlayerTest";
                // 사용할 컴포넌트를 가져오기

                playerAnimator = GetComponent<Animator>();
                playerMove = GetComponent<TestMove>();
                playerShoot = GetComponent<TestShoot>();
                playerSkill = GetComponent<PlayerSkill_Specificity>();
                healthUi = GameObject.Find("HPUi").GetComponent<Image>();
                shieldUi = GameObject.Find("SPUi").GetComponent<Image>();
                endButton = GameObject.Find("BattleEndButton").GetComponent<Button>();
                endButton.gameObject.SetActive(false);
                canvasAnimator = GameObject.Find("Canvas").GetComponent<Animator>();
                onZonUi = canvasAnimator.gameObject.transform.Find("OnZonUi").GetComponent<Image>();
                playerOnZon = true;
                isDeath = false;
                outCanvas.gameObject.SetActive(false);
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                shieldUi.fillAmount = (float)currentShield / (float)maxShield;
                isDeathText = false;
                float basicshield = playerSkill.spBasicShield;
                currentShield = 0 + basicshield;
                float spMaxheath = playerSkill.spMaxHeath;
                maxHealth = maxHealth + spMaxheath;
                currentHealth = maxHealth;
                if (TrainingController.instance.training == false)
                    StartCoroutine("OnZon");
                observerCam = GameObject.Find("CM vcam1").GetComponent<CinemachineVirtualCamera>();
                observerCam.Follow = gameObject.transform;
                observerCam.LookAt = gameObject.transform;
                mapPlayer.GetComponent<MeshRenderer>().material.color = Color.green;
                mapSelect.GetComponent<SpriteRenderer>().color = Color.green;
                StartCoroutine("SpHeathRecover");
                StartCoroutine("SpShieldRegen");
            }
            else
            {
                healthUi = outCanvas.gameObject.transform.Find("HPUi2").GetComponent<Image>();
                shieldUi = outCanvas.gameObject.transform.Find("SPUi2").GetComponent<Image>();

                mapPlayer.GetComponent<MeshRenderer>().material.color = Color.red;
                mapSelect.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }
    }

    // 데미지 처리
    [PunRPC]
    public void TakeDamage(float Damage)
    {
        if (isDeath == true)
            return;

        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            return;
        GameObject healthCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
        healthCanvas.GetComponent<DamageText>().ShowDamageEffect(Damage, 0, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));

        float defense = Damage * playerSkill.spDefensePlus;
        float hardDefense = Damage * playerSkill.spHardArmor;

        if (playerSkill.specifiType != 17)
        {
            if (currentShield > 0)
            {
                float ExcessDm = currentShield - (Damage - defense);
                currentShield -= (Damage - defense);
                shieldUi.fillAmount = (float)currentShield / (float)maxShield;
                if (ExcessDm < 0)
                {
                    currentHealth += ExcessDm;
                    healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                    currentShield = 0;
                    if (currentHealth <= 0)
                    {
                        if (TrainingController.instance.training != true)
                            pV.RPC("DieRPC", RpcTarget.All);
                        else
                            DieRPC();
                    }
                }
            }
            else
            {
                currentHealth -= (Damage - defense);
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                if (currentHealth <= 0)
                {
                    if (TrainingController.instance.training != true)
                        pV.RPC("DieRPC", RpcTarget.All);
                    else
                        DieRPC();
                }

            }
        }
        else if (playerSkill.specifiType == 17)
        {
            if (currentShield > 0)
            {
                float ExcessDm = currentShield - (Damage - hardDefense);
                currentShield -= (Damage - hardDefense);
                shieldUi.fillAmount = (float)currentShield / (float)maxShield;
                if (ExcessDm < 0)
                {
                    currentHealth += ExcessDm;
                    healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                    currentShield = 0;
                    if (currentHealth <= 0)
                    {
                        if (TrainingController.instance.training != true)
                            pV.RPC("DieRPC", RpcTarget.All);
                        else
                            DieRPC();
                    }
                }
            }
            else
            {
                currentHealth -= (Damage - hardDefense);
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                if (currentHealth <= 0)
                {
                    if (TrainingController.instance.training != true)
                        pV.RPC("DieRPC", RpcTarget.All);
                    else
                        DieRPC();
                }

            }
        }
    }

    public void TakeDamageMurderN(GameObject murderN)
    {
        if (currentHealth <= 0)
        {
            if (murderN.tag == "Enemy")
            {
                murderN.GetComponent<EnemyAttackTest>().targetTr = null;
                murderN.GetComponent<EnemyHealthTest>().revenged = null;
            }
            DieText(murderN);
        }
    }

    public void DieText(GameObject murderN)
    {
        if (isDeathText == false)
        {
            if (TrainingController.instance.training != true)
            {
                ScoreTest.instance.Murder(gameObject, murderN);
                isDeathText = true;
            }
        }
    }

    [PunRPC]
    public void RPCUsePainKiller(int value)
    {
        StartCoroutine(UsePainKiller(value));
    }

    public IEnumerator UsePainKiller(int newHealth)
    {
        // 체력 갱신
        for (int i = 0; i < 5; i++)
        {
            if (currentHealth <= maxHealth)
            {
                Debug.Log("회복중");
                currentHealth += newHealth;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                GameObject healthCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
                healthCanvas.GetComponent<DamageText>().ShowDamageEffect(newHealth, 1,new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
                yield return new WaitForSecondsRealtime(0.5f);
            }
            else if (currentHealth > maxHealth)
            {
                Debug.Log("회복끝");
                currentHealth = maxHealth;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
                yield return null;
            }
            else
                yield return null;
        }
        yield return null;
    }

    IEnumerator SpHeathRecover()
    {
        while (isDeath == false)
        {
            if (currentHealth < maxHealth)
            {
                float heathRe = playerSkill.spHeathRecovery;
                currentHealth += heathRe;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;

            }
            else if (currentHealth >= maxHealth)
            {
                currentHealth = maxHealth;
                healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
            }

            yield return new WaitForSecondsRealtime(1f);
        }
    }

    IEnumerator SpShieldRegen()
    {
        while (isDeath == false)
        {
            if (currentShield < maxShield)
            {
                float shieldRe = playerSkill.spShieldRegen;
                currentShield += shieldRe;
                shieldUi.fillAmount = (float)currentShield / (float)maxShield;

            }
            else if (currentShield >= maxShield)
            {
                currentShield = maxShield;
                shieldUi.fillAmount = (float)currentShield / (float)maxShield;
            }

            yield return new WaitForSecondsRealtime(30f);
        }
    }

    public void RestoreShield(int newShield)
    {
        // 실드
        GameObject shieldCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
        shieldCanvas.GetComponent<DamageText>().ShowDamageEffect(newShield, 2, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        if (currentShield < maxShield)
        {
            currentShield += newShield;
            shieldUi.fillAmount = (float)currentShield / (float)maxShield;
        }
        else if (currentShield >= maxShield)
        {
            currentShield = maxShield;
            shieldUi.fillAmount = (float)currentShield / (float)maxShield;
        }
    }


    public void RestoreHealth(int newHealth)
    {
        // 체력 갱신
        GameObject healthCanvas = ObjectManager.instance.PoolGet(2,TrainingController.instance.training);
        healthCanvas.GetComponent<DamageText>().ShowDamageEffect(newHealth, 1, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        if (currentHealth < maxHealth)
        {
            currentHealth += newHealth;
            healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
        }
        else if (currentHealth >= maxHealth)
        {
            currentHealth = maxHealth;
            healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
        }
    }


    [PunRPC]
    public void DieRPC()
    {
        if (TrainingController.instance.training == true)
        {
            playerAnimator.SetTrigger("Death");
            playerShoot.enabled = false;
            isDeath = true;
            gameObject.transform.Find("Map").gameObject.SetActive(false);
            StartCoroutine(PlayerReSet());
        }
        else
        {
            if (pV.IsMine)
            {
                GetComponent<PlayerMove>().enabled = false;
                playerAnimator.SetTrigger("Death");
                playerShoot.enabled = false;
                endButton.GetComponent<battleEnd>().PlayerDie();
                //isDeath = true;
                TestGameManager.instance.StartCoroutine("PlayerDeath");
            }
            gameObject.transform.Find("Map").gameObject.SetActive(false);
            StartCoroutine(PlayerActive());
        }
    }
    [PunRPC]
    public void DeathCheck()
    {
        isDeath = true;
    }
    IEnumerator PlayerReSet()
    {
        yield return new WaitForSeconds(1f);
        transform.position = playerTr;
        isDeath = false;
        playerShoot.enabled = true;
        gameObject.transform.Find("Map").gameObject.SetActive(true);
        currentHealth = maxHealth;
        healthUi.fillAmount = (float)currentHealth / (float)maxHealth;
        playerAnimator.Rebind();
        playerShoot.currentBulletCount = 0;
        playerShoot.ConsumableItemCount = 0;
        playerShoot.itemType = 0;
        gameObject.SetActive(true);
    }

    IEnumerator PlayerActive()
    {
        yield return new WaitForSecondsRealtime(3);
        gameObject.SetActive(false);
    }

    [PunRPC]
    public void Push(Quaternion _Pos, float _pushSize)
    {
        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            return;

        rigid.velocity = Vector3.zero;
        //Debug.Log(_Pos * Vector3.forward * _pushSize);
        rigid.AddForce(_Pos * Vector3.forward * _pushSize, ForceMode.Impulse);
    }

    [PunRPC]
    public void ExPush(Quaternion _Pos)
    {
        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            return;

        rigid.velocity = Vector3.zero;
        rigid.AddForce(_Pos * Vector3.forward * 10, ForceMode.Impulse);
    }

    [PunRPC]
    public void ExPushEnd(Vector3 _Pos, float _pushSize)
    {
        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            return;

        Vector3 Ex = transform.position - _Pos;

        Ex = Ex.normalized;

        Ex += Vector3.up;

        rigid.AddForce(Ex * _pushSize, ForceMode.Impulse);
    }

    [PunRPC]
    public void Emp()
    {
        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            return;

        currentShield = 0;
        shieldUi.fillAmount = (float)currentShield / (float)maxShield;
    }

    [PunRPC]
    public void Shock()
    {
        StartCoroutine(EmpShock());
    }

    IEnumerator EmpShock()
    {
        if (playerSkill.skillUse == true && playerSkill.skillType == 2)
            yield break;

        shock = true;
        playerParticle[0].gameObject.SetActive(true);
        playerParticle[0].Play();
        yield return new WaitForSecondsRealtime(2f);
        shock = false;
        playerParticle[0].gameObject.SetActive(false);
        playerParticle[0].Stop();
    }

    IEnumerator OnZon()
    {
        if (TrainingController.instance.training == true)
            yield break;
        else
        {
            if (pV.IsMine)
            {
                if (TestGameManager.instance.gameEnd == false)
                {
                    if (playerOnZon == true)
                    {
                        onZonUi.gameObject.SetActive(false);
                    }
                    if (playerOnZon == false)
                    {
                        if (TrainingController.instance.training != true)
                        {
                            gameObject.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.AllBuffered, 5f);
                            if (currentHealth <= 0 && !isDeath)
                            {
                                pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                            }
                        }
                        else
                        {
                            gameObject.GetComponent<TestHealth>().TakeDamage(5f);
                            if (currentHealth <= 0 && !isDeath)
                            {
                                gameObject.GetComponent<TestHealth>().TakeDamageMurderN(TestGameManager.instance.deathZon.gameObject);
                                DeathCheck();
                            }
                        }
                        
                        canvasAnimator.SetTrigger("OutZon");
                    }
                    yield return new WaitForSecondsRealtime(1f);
                    StartCoroutine("OnZon");
                }
            }
        }
    }

    public void ObserverCam()
    {

        if (gameObject.GetComponent<TestShoot>().itemType == 4)
        {
            observerCam.m_Lens.FieldOfView = 60;
        }
        else if (gameObject.GetComponent<TestShoot>().itemType == 6)
        {
            observerCam.m_Lens.FieldOfView = 75;
        }
        else if (gameObject.GetComponent<TestShoot>().itemType == 7)
        {
            observerCam.m_Lens.FieldOfView = 50;
        }
        else if (gameObject.GetComponent<TestShoot>().itemType == 8)
        {
            observerCam.m_Lens.FieldOfView = 55;
        }
        else if (gameObject.GetComponent<TestShoot>().itemType == 9)
        {
            observerCam.m_Lens.FieldOfView = 40;
        }

        else
            observerCam.m_Lens.FieldOfView = 45;
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 로컬 오브젝트라면 쓰기 부분이 실행됨
        if (stream.IsWriting)
        {
            // 네트워크를 통해 score 값을 보내기
            stream.SendNext(currentHealth);
            stream.SendNext(currentShield);
            stream.SendNext(playerName);
            stream.SendNext(ranking);
        }
        else
        {
            // 리모트 오브젝트라면 읽기 부분이 실행됨         

            // 네트워크를 통해 score 값 받기
            currentHealth = (float)stream.ReceiveNext();
            currentShield = (float)stream.ReceiveNext();
            playerName = (string)stream.ReceiveNext();
            ranking = (int)stream.ReceiveNext();
        }
    }
}
