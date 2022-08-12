using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonPlayer : MonoBehaviourPun,IPunObservable
{
    public PhotonView pV;
    [SerializeField]
    string playerName;
    [SerializeField]
    Rigidbody rigid;
    Animator anim;
    [SerializeField]
    CinemachineVirtualCamera cam;
    [SerializeField]
    Image hpBar;
    [SerializeField]
    LayerMask targetLayer;
    Vector3 lookRot;
    public GameObject target;
    public float speed;
    public int heath;
    Coroutine coAttack;
    public int killCount;
    void Start()
    {
        pV = GetComponent<PhotonView>();
        playerName = gameObject.name;
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        cam = FindObjectOfType<CinemachineVirtualCamera>();
        hpBar = GetComponentInChildren<Image>();
        if (pV.IsMine)
        {
            cam.Follow = transform;
            cam.LookAt = transform;
        }
        speed = 5;
        heath = 100;
    }

    void Update()
    {
        hpBar.fillAmount = heath * 0.01f;

        if (pV.IsMine)
        {
            //Horizontal, Vertical 축 인풋을 받아서 저장
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Move(h, v);     //이동 처리
            Turn();
            if (Input.GetKeyDown(KeyCode.F))
                pV.RPC("Anim", RpcTarget.All);
            if (Input.GetKeyDown(KeyCode.R))
                pV.RPC("Attack", RpcTarget.All);
            if(Input.GetButtonDown("Fire1"))
            {
                pV.RPC("Bullet",RpcTarget.All);
            }

        }
    }

    void Move(float h, float v)
    {
        Vector3 moveVector = new Vector3(h, 0f, v);

        moveVector = moveVector.normalized * Time.deltaTime * speed;

        if (h == 0 && v == 0)
            moveVector = Vector3.zero;


        rigid.MovePosition(rigid.position + moveVector);
        moveVector = Vector3.zero;
    }

    void Turn()
    {
        //화면의 마우스 위치에서 수직으로 발사되는 Ray를 저장
        Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if(Physics.Raycast(camRay, out RaycastHit hit, 1000f, 1 << 8 | 1 << 10))  //8번째랑 10번째 레이어랑 충돌 체크 하겠다
        if (Physics.Raycast(camRay, out RaycastHit hit, 1000f, targetLayer))
        {
            //플레이어 => 마우스 히트 지점을 향하는 백터
            lookRot = hit.point - rigid.position;

            //캐릭터가 마우스 히트 지점을 바라보도록
            rigid.MoveRotation(Quaternion.LookRotation(lookRot));
        }

    }

    [PunRPC]
    void Anim()
    {
        anim.Play("PhotonPlayerAni");
    }

    [PunRPC]
    void Attack()
    {
        if (coAttack != null)
            return;

        coAttack = StartCoroutine(Shoot());
    }

    IEnumerator Shoot()
    {
        if (target != null)
        {
            if (pV.IsMine)
                target.GetComponent<PhotonPlayer>().pV.RPC("TakeDamage", RpcTarget.All , 5, target.transform.position);
        }
        yield return new WaitForSecondsRealtime(1f);
        coAttack = null;
    }

    [PunRPC]
    void Bullet()
    {
        Debug.Log("발사");
        GameObject bullet = OJM.instance.PoolGet(1);
        bullet.GetComponent<PhotonBullet>().SetBullet(gameObject, transform.position,transform.rotation);

    }

    [PunRPC]
    public void TakeDamage(int value ,Vector3 _targetPos)
    {
        GetComponent<PhotonPlayer>().heath -= value;
        GameObject damageCanvas = OJM.instance.PoolGet(0);
        damageCanvas.GetComponent<PhotonText>().SetText(value,_targetPos);
        //damageCanvas.GetComponentInChildren<Text>().text = "5";
        //damageCanvas.transform.SetParent(transform.parent);
        //damageCanvas.transform.position = _targetPos;
        //StartCoroutine(ReCanvas(damageCanvas));
    }

    [PunRPC]
    public void KillCount()
    {
        killCount++;
    }

    IEnumerator ReCanvas(GameObject damageCanvas)
    {
        while (true)
        {
            damageCanvas.transform.Translate(0, 0.02f, 0);
            if (damageCanvas.transform.position.y >= 2)
            {
                damageCanvas.transform.SetParent(OJM.instance.transform);
                damageCanvas.SetActive(false);
                break;
            }
            yield return null;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(playerName);
            stream.SendNext(heath);
        }
        else
        {
            playerName = (string)stream.ReceiveNext();
            gameObject.name = playerName;
            heath = (int)stream.ReceiveNext();
        }
    }
}
