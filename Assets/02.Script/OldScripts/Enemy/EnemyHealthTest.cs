using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class EnemyHealthTest : MonoBehaviourPun , IPunObservable
{
    [SerializeField]
    EnemyAttackTest enemyAttack;

    public float maxHealth = 100;     //최대 체력
    public float currentHealth;       //현재 체력
    private Animator animator;
    public bool isDeath = false;     //죽었는가? 체크용 변수
    public bool isDeadText;
    public ParticleSystem[] enemyParticle;
    public bool enemyOnZon;
    public bool isDamage;
    public int ranking;
    public string enemyName;
    public PhotonView pV;
    public GameObject revenged;
    public Image enemyHpUi;
    public Rigidbody rigid;
    public bool isPush;
    public bool shock;

    void Awake()
    {
        if (TrainingController.instance.training != true)
            pV = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        enemyAttack = GetComponent<EnemyAttackTest>();
        currentHealth = maxHealth;
        enemyParticle = GetComponentsInChildren<ParticleSystem>();
        rigid = GetComponent<Rigidbody>();
        isDamage = false;
        enemyOnZon = true;
        isDeadText = false;
        //if (TrainingController.Instance.training == false)
        //    enemyName = ("Enemy" + GameStart.instance.enemyCount.ToString());
    }



    private void Start()
    {
        if (!TrainingController.instance.training)
            StartCoroutine(OnZon());
    }

    IEnumerator OnZon()
    {
        if (TestGameManager.instance.gameEnd == false)
        {
            if (enemyOnZon == false)
            {
                TakeDamage(5);
                TakeDamageMurderN(TestGameManager.instance.deathZon.gameObject);
                if (currentHealth <= 0 && !isDeath)
                {
                    if (!TrainingController.instance.training)
                        pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                    else
                        DeathCheck();
                }
            }
            yield return new WaitForSecondsRealtime(1f);
            StartCoroutine(OnZon());
        }
    }


    //데미지 받는 함수
    [PunRPC]
    public void TakeDamage(float damage)
    {
        if (isDeath)
            return;

        GameObject damageCanvas = ObjectManager.instance.PoolGet(2, TrainingController.instance.training);
        damageCanvas.GetComponent<DamageText>().ShowDamageEffect(damage, 0, new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));

        currentHealth -= damage;

        enemyHpUi.fillAmount = currentHealth / maxHealth;

        enemyParticle[0].Play();

        StartCoroutine(IsDamage());

        if (currentHealth <= 0)
        {
            if (!TrainingController.instance.training)
                pV.RPC("Die", RpcTarget.All);
            else
                Die();
        }
    }

    public void TakeDamageMurderN(GameObject murderN)
    {
        if (murderN.tag == "DeathZon")
            revenged = null;
        else
            revenged = murderN;

        if (currentHealth <= 0)
        {
            if(!TrainingController.instance.training)
                DieText(murderN);

            if (murderN.tag == "Enemy")
            {
                murderN.GetComponent<EnemyAttackTest>().targetTr = null;
                murderN.GetComponent<EnemyHealthTest>().revenged = null;
            }
        }
    }
    [PunRPC]
    public void EnemyShock()
    {
        StartCoroutine(CoEnemyShock());
    }

    IEnumerator CoEnemyShock()
    {
        shock = true;
        enemyParticle[24].Play();
        if(enemyAttack != null)
            enemyAttack.ReSet();
        yield return new WaitForSecondsRealtime(2f);
        enemyParticle[24].Stop();
        if (enemyAttack != null)
            enemyAttack.ReSetEnd();
        shock = false;
    }

    public IEnumerator ExTakePush(float _time)
    {
        isPush = true;
        if (enemyAttack != null)
            enemyAttack.ReSet();
        yield return new WaitForSecondsRealtime(_time);
        isPush = false;
        if (enemyAttack != null)
            enemyAttack.ReSetEnd();
    }

    public IEnumerator bulletPush()
    {
        if (enemyAttack == null)
            yield break;

        isPush = true;
        enemyAttack.ReSet();
        yield return new WaitForSecondsRealtime(0.2f);
        isPush = false;
        enemyAttack.ReSetEnd();
    }

    [PunRPC]
    public void Push(Quaternion _Pos, float _pushSize)
    {
        rigid.velocity = Vector3.zero;
        //Debug.Log(_Pos * Vector3.forward * _pushSize);

        rigid.AddForce(_Pos * Vector3.forward * _pushSize, ForceMode.Impulse);
    }

    [PunRPC]
    public void ExPush(Quaternion _Pos)
    {
        rigid.velocity = Vector3.zero;
        rigid.AddForce(_Pos * Vector3.forward * 20, ForceMode.Impulse);
    }

    [PunRPC]
    public void ExPushEnd(Vector3 _Pos, float _pushSize)
    {
        Vector3 Ex = transform.position - _Pos;

        Ex = Ex.normalized;

        Ex += Vector3.up;

        rigid.AddForce(Ex * _pushSize, ForceMode.Impulse);
    }

    IEnumerator IsDamage()
    {
        isDamage = true;
        yield return new WaitForSeconds(2f);
        isDamage = false;
    }

    public void DieText(GameObject murderN)
    {
        if (isDeadText == false)
        {
            if (murderN.tag == "Player")
            {
                if (murderN.GetComponent<PhotonView>().IsMine)
                {
                    if (!TrainingController.instance.training && !TestGameManager.instance.Test)
                        murderN.gameObject.GetComponent<PlayerScore>().KillUp();
                }
            }
            ScoreTest.instance.Murder(gameObject, murderN);

            isDeadText = true;
        }
    
    }

    [PunRPC]
    public void Die()
    {
        AudioManager.Instance.PlaySound("EnemyDie", transform.position, 1f + Random.Range(-0.1f, 0.1f));
        //Destroy(enemyParticle[0]);
        enemyParticle[9].Play();
        if (TrainingController.instance.training)
        {
            EnemyReSpawn.Instance.enemyCount--;
            isDeath = true;
        }
        animator.SetTrigger("EnemyDie");
        StartCoroutine(EnemyActive());
        transform.Find("Map").gameObject.SetActive(false);


        enabled = false;
    }

    [PunRPC]
    public void DeathCheck()
    {
        isDeath = true;
    }

    IEnumerator EnemyActive()
    {
        yield return new WaitForSecondsRealtime(4);
        gameObject.SetActive(false);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (TrainingController.instance.training)
            return;

        if (stream.IsWriting)
        {
            stream.SendNext(enemyName);
            stream.SendNext(currentHealth);
            stream.SendNext(maxHealth);
            stream.SendNext(enemyHpUi.fillAmount);
            stream.SendNext(ranking);
        }
        else
        {
            enemyName = (string)stream.ReceiveNext();
            currentHealth = (float)stream.ReceiveNext();
            maxHealth = (float)stream.ReceiveNext();
            enemyHpUi.fillAmount = (float)stream.ReceiveNext();
            ranking = (int)stream.ReceiveNext();
        }
    }
}
