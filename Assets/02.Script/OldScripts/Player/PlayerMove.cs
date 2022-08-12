using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMove : MonoBehaviourPun
{
    TestHealth testHealth;
    private Rigidbody rigid;
    private Animator animator;
    Animator playerAni;
    public Vector3 lookRot;
    public float speed;     //이동 속도
    public LayerMask targetLayer;       //마우스위치 감지용 레이어 마스크
    public PhotonView pV;
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        if (!TrainingController.instance.training)
            pV = GetComponent<PhotonView>();
        testHealth = GetComponent<TestHealth>();
#if UNITY_EDITOR_WIN
        enabled = true;
#elif UNITY_ANDROID
        enabled = false;
#endif
    }

    void FixedUpdate()
    {
        if (testHealth.shock)
            return;

        if(pV == null)
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Move(h, v);     //이동 처리
            Turn();         //회전 처리 (마우스 방향으로)
            Animate(h, v);  //애니메이션 처리
        }
        else if (photonView.IsMine)
        {
            //Horizontal, Vertical 축 인풋을 받아서 저장
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            Move(h, v);     //이동 처리
            Turn();         //회전 처리 (마우스 방향으로)
            Animate(h, v);  //애니메이션 처리
        }
    }

    void Move(float h, float v)
    {
        Vector3 moveVector = new Vector3(h, 0f, v);
        
        moveVector = moveVector.normalized  *Time.deltaTime* speed;

        if(h == 0 && v == 0)
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

    void Animate(float h, float v)
    {
        animator.SetBool("IsWalking", (h != 0 || v != 0) ? true : false);
    }

}
