using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using Cinemachine.Utility;
using System.Runtime.Serialization;
using Photon.Pun;
using Photon.Realtime;
using JetBrains.Annotations;

public class EnemyAttackTest : MonoBehaviourPun, IPunObservable
{
    [SerializeField]
    private PhotonView PV;

    private Animator animator;
    private NavMeshAgent nvAgent;
    [SerializeField]
    private EnemyHealthTest enemyHealth;
    private Rigidbody enemyRigid;
    private Collider enemyCollider;
    private ParticleSystem[] enemyParticle;
    private float dist;
    [SerializeField]
    private bool patrolingMove;
    [SerializeField]
    private bool isAttack;
    [SerializeField]
    private bool reSetIng;
    [SerializeField]
    private int enemyBulletCount;
    [SerializeField]
    private int enemyBulletMaxCount = 7;
    [SerializeField]
    private float attackDist = 10.0f;
    [SerializeField]
    private float meleeDist = 2.5f;
    [SerializeField]
    private float traccDist = 15.0f;

    private IEnumerator checkEnemyState;
    private IEnumerator enemyAction;

    //적 캐릭터에게 필요한 컴포넌트들
    public Transform targetTr;
    public GameObject enemyGunBarrelEnd;
    public LayerMask targetMask;
    public Vector3 pos;
    public Quaternion rot;

    //적 캐릭터에게 필요한 변수들
    public enum EnemyState { idle, trace, melee, attack, die, reLoad, patrol, revenge };
    public EnemyState enemyState = EnemyState.idle;

    // Use this for initialization
    void Awake()
    {
        if(TrainingController.instance.training != true)
            PV = GetComponent<PhotonView>();
        enemyRigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        nvAgent = this.gameObject.GetComponent<NavMeshAgent>();
        enemyBulletCount = enemyBulletMaxCount;
        enemyHealth = GetComponent<EnemyHealthTest>();
        enemyParticle = GetComponentsInChildren<ParticleSystem>();
        enemyCollider = GetComponent<CapsuleCollider>();
        checkEnemyState = CheckEnemyState();
        enemyAction = EnemyAction();
        rot = transform.localRotation;
    }

    private void Update()
    {
        if (!TrainingController.instance.training)
        {
            if (PV != null)
            {
                if (nvAgent.velocity.magnitude > 1f && nvAgent.enabled == true)
                {
                    if (PV.IsMine)
                        animator.SetBool("EnemyRun", true);
                }
                else
                {
                    if (PV.IsMine)
                        animator.SetBool("EnemyRun", false);
                }

                if (!PV.IsMine)
                {
                    transform.position = Vector3.Lerp(transform.position, pos, Time.deltaTime * 10f);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
                }
            }
        }
        else
        {
            if (nvAgent.velocity.magnitude > 1f && nvAgent.enabled == true)
                animator.SetBool("EnemyRun", true);
            else
                animator.SetBool("EnemyRun", false);
        }
    }


    public void Start()
    {
        if (!TrainingController.instance.training)
        {
            if (PV.IsMine)
            {
                enemyState = EnemyState.idle;
                reSetIng = true;
                ReSetEnd();
            } 
        }
        else
        {
            enemyState = EnemyState.idle;
            reSetIng = true;
            ReSetEnd();
        }
    }

    public void ReSet()
    {
        nvAgent.enabled = false;
        nvAgent.updatePosition = false;
        nvAgent.updateRotation = false;
        StopCoroutine(checkEnemyState);
        StopCoroutine(enemyAction);
        reSetIng = true;
    }

    public void ReSetEnd()
    {
        if (reSetIng == true)
        {
            reSetIng = false;
            nvAgent.enabled = true;
            nvAgent.updatePosition = true;
            nvAgent.updateRotation = true;
            StartCoroutine(checkEnemyState);
            StartCoroutine(enemyAction);
        }
    }

    


    /// <summary>
    /// 적 상태 확인 코루틴
    /// </summary>
    IEnumerator CheckEnemyState()
    {
        if (!TrainingController.instance.training)
        {
            if (PV.IsMine)
            {
                while (enemyState != EnemyState.die)
                {
                    yield return new WaitForSecondsRealtime(0.5f);
                    if (targetTr != null)
                        dist = Vector3.Distance(targetTr.position, transform.position);
                    else
                        dist = 30;
                    if (enemyHealth.currentHealth <= 0)
                    {
                        enemyState = EnemyState.die;
                    }
                    else if (enemyState == EnemyState.idle && enemyHealth.isPush == false)
                    {
                        enemyState = EnemyState.patrol;
                        PV.RPC("EnemyEndMoveRPC", RpcTarget.All);
                    }
                    else if (enemyBulletCount <= 0)
                    {
                        enemyState = EnemyState.reLoad;
                    }
                    else if (enemyHealth.isDamage == true && enemyHealth.revenged != null)
                    {
                        enemyState = EnemyState.revenge;
                    }
                    else if (dist <= meleeDist)
                    {
                        if (targetTr.tag == "Player" && targetTr.gameObject.GetComponent<TestHealth>().isDeath == false)
                            enemyState = EnemyState.melee;
                        else if (targetTr.tag == "Enemy" && targetTr.gameObject.GetComponent<EnemyHealthTest>().isDeath == false)
                            enemyState = EnemyState.melee;
                        else
                            enemyState = EnemyState.patrol;
                    }
                    else if (dist <= attackDist && enemyBulletCount > 0)
                    {
                        if (targetTr.tag == "Player" && targetTr.gameObject.GetComponent<TestHealth>().isDeath == false)
                            enemyState = EnemyState.attack;
                        else if (targetTr.tag == "Enemy" && targetTr.gameObject.GetComponent<EnemyHealthTest>().isDeath == false)
                            enemyState = EnemyState.attack;
                        else
                            enemyState = EnemyState.patrol;
                    }
                    else if (dist <= traccDist && enemyState != EnemyState.attack)
                    {
                        enemyState = EnemyState.trace;
                    }
                    else
                        enemyState = EnemyState.patrol;
                    yield return null;
                }
            }
        }
        else
        {
            while (enemyState != EnemyState.die)
            {
                yield return new WaitForSecondsRealtime(0.5f);
                if (targetTr != null)
                    dist = Vector3.Distance(targetTr.position, transform.position);
                else
                    dist = 30;

                if (enemyHealth.currentHealth <= 0)
                {
                    enemyState = EnemyState.die;
                }
                else if (enemyState == EnemyState.idle)
                {
                    enemyState = EnemyState.patrol;
                    EnemyEndMoveRPC();
                }
                else if (enemyBulletCount <= 0)
                {
                    enemyState = EnemyState.reLoad;
                }
                else if (enemyHealth.isDamage == true && enemyHealth.revenged != null)
                {
                    enemyState = EnemyState.revenge;
                }
                else if (dist <= meleeDist)
                {
                    if (targetTr.tag == "Player" && targetTr.gameObject.GetComponent<TestHealth>().isDeath == false)
                        enemyState = EnemyState.melee;
                    else
                        enemyState = EnemyState.patrol;
                }
                else if (dist <= attackDist && enemyBulletCount > 0)
                {
                    if (targetTr.tag == "Player" && targetTr.gameObject.GetComponent<TestHealth>().isDeath == false)
                        enemyState = EnemyState.attack;
                    else
                        enemyState = EnemyState.patrol;
                }
                else if (dist <= traccDist && enemyState != EnemyState.attack)
                {
                    enemyState = EnemyState.trace;
                }
                else
                    enemyState = EnemyState.patrol;
                yield return null;
            }
        }
    }

    Vector3 targetPos;

    IEnumerator EnemyAction()
    {
        if (!TrainingController.instance.training)
        {
            if (PV.IsMine)
            {
                while (true)
                {
                    switch (enemyState)
                    {
                        case EnemyState.revenge:
                            StartCoroutine(EnemyRevenge());
                            //Debug.Log("복수");
                            yield return new WaitForSecondsRealtime(1f);
                            break;
                        case EnemyState.melee:
                            //Debug.Log("근접");
                            StartCoroutine(EnemyMelee());
                            yield return new WaitForSecondsRealtime(0.5f);
                            break;
                        case EnemyState.trace:
                            //Debug.Log("추격");
                            StartCoroutine(Enemytrace());
                            yield return new WaitForSecondsRealtime(1f);
                            break;
                        case EnemyState.attack:
                            //Debug.Log("공격");
                            StartCoroutine(EnemyAttack());
                            yield return new WaitForSecondsRealtime(0.5f);
                            break;
                        case EnemyState.reLoad:
                            //Debug.Log("장전");
                            StartCoroutine(EnemyReload());
                            yield return new WaitForSecondsRealtime(2.3f);
                            break;
                        case EnemyState.patrol:
                            //Debug.Log("순찰");
                            if (!TrainingController.instance.training && enemyHealth.enemyOnZon == false)
                                StartCoroutine(OnZonMove());
                            else
                                StartCoroutine(EnemyMove());
                            yield return new WaitForSecondsRealtime(1f);
                            break;
                        case EnemyState.die:
                            PV.RPC("EnemyDieRPC", RpcTarget.All);
                            break;
                    }
                    yield return null;
                }
            }
        }
        else
        {
            while (true)
            {
                switch (enemyState)
                {
                    case EnemyState.revenge:
                        StartCoroutine(EnemyRevenge());
                        Debug.Log("복수");
                        yield return new WaitForSecondsRealtime(1f);
                        break;
                    case EnemyState.melee:
                        Debug.Log("근접");
                        StartCoroutine(EnemyMelee());
                        yield return new WaitForSecondsRealtime(0.5f);
                        break;
                    case EnemyState.trace:
                        Debug.Log("추격");
                        StartCoroutine(Enemytrace());
                        yield return new WaitForSecondsRealtime(1f);
                        break;
                    case EnemyState.attack:
                        Debug.Log("공격");
                        StartCoroutine(EnemyAttack());
                        yield return new WaitForSecondsRealtime(0.5f);
                        break;
                    case EnemyState.reLoad:
                        Debug.Log("장전");
                        StartCoroutine(EnemyReload());
                        yield return new WaitForSecondsRealtime(2.3f);
                        break;
                    case EnemyState.patrol:
                        Debug.Log("순찰");
                        StartCoroutine(EnemyMove());
                        yield return new WaitForSecondsRealtime(1f);
                        break;
                    case EnemyState.die:
                        EnemyDieRPC();
                        break;
                }
                yield return null;
            }
        }
    }

    IEnumerator EnemyMelee()
    {
        if (!TrainingController.instance.training)
        {
            if (PV != null)
            {
                isAttack = true;

                if (targetTr != null)
                {
                    nvAgent.ResetPath();
                    nvAgent.isStopped = true;
                    targetPos = targetTr.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(targetPos).normalized;
                    Ray attackEnemy = new Ray(transform.position, transform.forward);
                    Debug.DrawLine(attackEnemy.origin, attackEnemy.origin + attackEnemy.direction * 2f, Color.red);
                    if (Physics.Raycast(attackEnemy, out RaycastHit hit, 2f, targetMask))
                    {
                        if ((hit.collider.tag == "Player" || hit.collider.tag == "Enemy") && hit.collider.tag != "Environment")
                        {
                            PV.RPC("EnemyMeleeAni", RpcTarget.All);
                            AudioManager.Instance.PlaySound("EnemyMelee", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                            Vector3 target = hit.transform.position - transform.position;
                            transform.rotation = Quaternion.LookRotation(target).normalized;
                            hit.collider.GetComponent<PhotonView>().RPC("TakeDamage", RpcTarget.All, 5f);
                            if (hit.collider.tag == "Player")
                            {
                                if (hit.collider.GetComponent<TestHealth>().currentHealth <= 0 && !hit.collider.GetComponent<TestHealth>().isDeath)
                                {
                                    hit.collider.GetComponent<TestHealth>().TakeDamageMurderN(gameObject);
                                    hit.collider.GetComponent<TestHealth>().pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                                }
                            }
                            else
                            {
                                hit.collider.GetComponent<EnemyHealthTest>().TakeDamageMurderN(gameObject);
                                if (hit.collider.GetComponent<EnemyHealthTest>().currentHealth <= 0 && !hit.collider.GetComponent<EnemyHealthTest>().isDeath)
                                {
                                    hit.collider.GetComponent<EnemyHealthTest>().pV.RPC("DeathCheck", RpcTarget.AllViaServer);
                                }
                            }
                            hit.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                            hit.collider.GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward * 3, ForceMode.Impulse);
                            yield return new WaitForSecondsRealtime(0.5f);
                            isAttack = false;
                        }
                    }
                }
            }
        }
        else
        {
            isAttack = true;

            if (targetTr != null)
            {
                nvAgent.ResetPath();
                nvAgent.isStopped = true;
                targetPos = targetTr.position - transform.position;
                transform.rotation = Quaternion.LookRotation(targetPos).normalized;
                Ray attackEnemy = new Ray(transform.position, transform.forward);
                Debug.DrawLine(attackEnemy.origin, attackEnemy.origin + attackEnemy.direction * 2f, Color.red);
                if (Physics.Raycast(attackEnemy, out RaycastHit hit, 2f, targetMask))
                {
                    if (hit.collider.tag == "Player" && hit.collider.tag != "Environment")
                    {
                        EnemyMeleeAni();
                        AudioManager.Instance.PlaySound("EnemyMelee", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                        Vector3 target = hit.transform.position - transform.position;
                        transform.rotation = Quaternion.LookRotation(target).normalized;
                        hit.collider.GetComponent<TestHealth>().TakeDamage(5f);
                        hit.collider.GetComponent<Rigidbody>().velocity = Vector3.zero;
                        hit.collider.GetComponent<Rigidbody>().AddForce(transform.rotation * Vector3.forward * 3, ForceMode.Impulse);
                        yield return new WaitForSecondsRealtime(0.5f);
                        isAttack = false;
                    }
                }
            }
        }
    }

    [PunRPC]
    public void EnemyMeleeAni()
    {
        animator.SetTrigger("EnemyMelee");
        enemyParticle[19].Play();
    }

    IEnumerator Enemytrace()
    {
        if (!TrainingController.instance.training)
        {
            if (PV != null)
            {
                nvAgent.ResetPath();
                nvAgent.isStopped = false;
                patrolingMove = true;
                if (targetTr != null)
                    nvAgent.SetDestination(targetTr.position);
                yield return new WaitForSecondsRealtime(1f);
            }
        }
        else
        {
            nvAgent.ResetPath();
            nvAgent.isStopped = false;
            patrolingMove = true;
            if (targetTr != null)
                nvAgent.SetDestination(targetTr.position);
            yield return new WaitForSecondsRealtime(1f);
        }
    }

    IEnumerator EnemyAttack()
    {
        if (!TrainingController.instance.training)
        {
            if (PV != null)
            {
                isAttack = true;

                if (targetTr != null)
                {
                    nvAgent.ResetPath();
                    nvAgent.isStopped = true;
                    targetPos = targetTr.position - transform.position;
                    transform.rotation = Quaternion.LookRotation(targetPos).normalized;
                    Ray attackEnemy = new Ray(enemyGunBarrelEnd.transform.position, enemyGunBarrelEnd.transform.forward);
                    Debug.DrawLine(attackEnemy.origin, attackEnemy.origin + attackEnemy.direction * 15f, Color.yellow);
                    if (Physics.Raycast(attackEnemy, out RaycastHit hit, 15f, targetMask))
                    {
                        if (hit.collider.tag == "Environment")
                        {
                            nvAgent.isStopped = false;
                            Vector3 target = hit.transform.position - transform.position;
                            transform.rotation = Quaternion.LookRotation(target).normalized;
                            nvAgent.SetDestination(targetTr.transform.position);
                            yield return new WaitForSecondsRealtime(0.5f);
                            isAttack = false;
                        }
                        else if ((hit.collider.tag.Contains("Player") || hit.collider.tag.Contains("Enemy")) && hit.collider.tag != "Environment")
                        {

                            if (!TrainingController.instance.training)
                            {
                                PV.RPC("EnemyAttackEffect", RpcTarget.All);
                            }
                            else
                                EnemyAttackEffect();
                            AudioManager.Instance.PlaySound("EnemyGun", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                            Vector3 target = hit.transform.position - transform.position;
                            transform.rotation = Quaternion.LookRotation(target).normalized;
                            if (PV.IsMine)
                                PV.RPC("Bullet", RpcTarget.All, 0);

                            enemyBulletCount--;
                            yield return new WaitForSecondsRealtime(0.5f);
                            isAttack = false;
                        }
                    }
                }
            }
        }
        else
        {
            isAttack = true;

            if (targetTr != null)
            {
                nvAgent.ResetPath();
                nvAgent.isStopped = true;
                targetPos = targetTr.position - transform.position;
                transform.rotation = Quaternion.LookRotation(targetPos).normalized;
                Ray attackEnemy = new Ray(enemyGunBarrelEnd.transform.position, enemyGunBarrelEnd.transform.forward);
                Debug.DrawLine(attackEnemy.origin, attackEnemy.origin + attackEnemy.direction * 15f, Color.yellow);
                if (Physics.Raycast(attackEnemy, out RaycastHit hit, 15f, targetMask))
                {
                    if (hit.collider.tag == "Environment")
                    {
                        nvAgent.isStopped = false;
                        Vector3 target = hit.transform.position - transform.position;
                        transform.rotation = Quaternion.LookRotation(target).normalized;
                        nvAgent.SetDestination(targetTr.transform.position);
                        yield return new WaitForSecondsRealtime(0.5f);
                        isAttack = false;
                    }
                    else if (hit.collider.tag.Contains("Player") && hit.collider.tag != "Environment")
                    {
                        EnemyAttackEffect();
                        AudioManager.Instance.PlaySound("EnemyGun", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                        Vector3 target = hit.transform.position - transform.position;
                        transform.rotation = Quaternion.LookRotation(target).normalized;
                        Bullet(0);
                        enemyBulletCount--;
                        yield return new WaitForSecondsRealtime(0.5f);
                        isAttack = false;
                    }
                }
            }
        }
    }

    [PunRPC]
    void Bullet(int posCount = 0)
    {
        GameObject bullet = ObjectManager.instance.PoolGet(0);
        if (posCount == 0)
        {
            bullet.GetComponent<EnemyBullet>().SetBullet(gameObject, enemyGunBarrelEnd.transform.position, enemyGunBarrelEnd.transform.rotation);
        }
    }

    [PunRPC]
    public void EnemyAttackEffect()
    {
        enemyParticle[15].Play();
        animator.SetTrigger("EnemyAttack");
    }

    IEnumerator EnemyReload()
    {
        if (!TrainingController.instance.training)
        {
            if (PV != null)
            {
                PV.RPC("EnemyReloadAni", RpcTarget.All);
                nvAgent.ResetPath();
                AudioManager.Instance.PlaySound("EnemyRe", transform.position, 1f + Random.Range(-0.1f, 0.1f));
                enemyBulletCount = enemyBulletMaxCount;
                yield return new WaitForSecondsRealtime(2.3f);
            }
        }
        else
        {
            EnemyReloadAni();
            nvAgent.ResetPath();
            AudioManager.Instance.PlaySound("EnemyRe", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            enemyBulletCount = enemyBulletMaxCount;
            yield return new WaitForSecondsRealtime(2.3f);
        }
    }

    [PunRPC]
    public void EnemyReloadAni()
    {
        animator.SetTrigger("EnemyReload");
    }

    IEnumerator EnemyRevenge()
    {
        if (enemyHealth.revenged == null)
            yield break;

        targetTr = enemyHealth.revenged.gameObject.transform;

        Debug.Log("복수");

        if (dist <= attackDist)
            enemyState = EnemyState.attack;
        else if (dist <= meleeDist)
            enemyState = EnemyState.melee;
        else
        {
            nvAgent.ResetPath();
            nvAgent.isStopped = false;
            patrolingMove = true;
            nvAgent.SetDestination(enemyHealth.revenged.gameObject.transform.position);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }

    IEnumerator EnemyMove()
    {
        if (enemyState != EnemyState.die && enemyHealth.enemyOnZon == true)
        {
            if (patrolingMove == false)
            {
                nvAgent.isStopped = false;
                nvAgent.ResetPath();
                float randomX = Random.Range(-100f, 100f);
                float randomZ = Random.Range(-100f, 100f);
                Vector3 patroling = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
                nvAgent.SetDestination(patroling);
                patrolingMove = true;
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }

    [PunRPC]
    public void EnemyDieRPC()
    {
        Debug.Log("죽음");
        StopAllCoroutines();
        nvAgent.isStopped = true;
        nvAgent.updatePosition = false;
        nvAgent.updateRotation = false;
        nvAgent.velocity = Vector3.zero;
        enemyRigid.velocity = Vector3.zero;
        enemyCollider.enabled = false;
    }

    [PunRPC]
    public void EnemyEndMoveRPC()
    {
        StartCoroutine(EndMove());
    }

    IEnumerator EndMove()
    {
        float randomSec = Random.Range(1, 6f);
        yield return new WaitForSecondsRealtime(randomSec);
        patrolingMove = false;
        StartCoroutine(EndMove());
    }

    IEnumerator OnZonMove()
    {
        Debug.Log("오존 안으로");
        if (enemyState != EnemyState.die)
        {
            if (patrolingMove == false)
            {
                nvAgent.isStopped = false;
                nvAgent.ResetPath();
                GameObject deathZon = GameObject.FindGameObjectWithTag("DeathZon");
                float randomX = Random.Range(-10f, 10f);
                float randomZ = Random.Range(-10f, 10f);
                Vector3 patrolingOnZon = new Vector3(deathZon.transform.position.x + randomX, transform.position.y, deathZon.transform.position.z + randomZ);
                nvAgent.SetDestination(patrolingOnZon);
                patrolingMove = true;
                yield return new WaitForSecondsRealtime(1f);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            pos = (Vector3)stream.ReceiveNext();
            rot = (Quaternion)stream.ReceiveNext();
        }
    }
}
