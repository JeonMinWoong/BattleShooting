using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerAttackArrow : MonoBehaviourPunCallbacks
{

    // 공개
    public Transform arrow;         // 조이스틱.
    public GameObject player;
    public Transform playerTr;

    // 비공개
    private Vector3 arrowFirstPos;  // 조이스틱의 처음 위치.
    private Vector3 arrVec;         // 조이스틱의 벡터(방향)
    private float Radius;           // 조이스틱 배경의 반 지름.
    public bool rotation;          // 움직임 유무
    public TestHealth playerHealth;
    public bool attackTouch;
    Animator playerAni;
    void Start()
    {
        player = GameObject.Find("PlayerTest");
        playerTr = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<TestHealth>();
        playerAni = player.GetComponent<Animator>();
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        arrowFirstPos = arrow.transform.position;

        // 캔버스 크기에대한 반지름 조절.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;

        rotation = false;
    }

    public void FixedUpdate()
    {
        if (player.GetComponent<TestShoot>().attackKing == false && attackTouch == true)
        {
            if (player.GetComponent<TestHealth>().shock == true)
            {
                gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
                DragEnd();
            }
            else
                gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.35f);

            if (TrainingController.instance.training != true)
            {
                if (player.GetComponent<TestShoot>().PV.IsMine)
                {
                    player.GetComponent<TestShoot>().PV.RPC("AttackRPC", RpcTarget.All);
                    Debug.Log("Ani");
                }
            }
            else
            {
                player.GetComponent<TestShoot>().AttackRPC();
            }
        }
    }






    public void Drag(BaseEventData _Data)
    {
        if (playerHealth.isDeath == true)
            return;

        if (playerHealth.shock == true)
            return;
        if (!player.GetComponent<TestShoot>().PV.IsMine)
            return;
            
        rotation = true;
        PointerEventData Data = _Data as PointerEventData;
        Vector3 Pos = Data.position;

        // 조이스틱을 이동시킬 방향을 구함.(오른쪽,왼쪽,위,아래)
        arrVec = (Pos - arrowFirstPos).normalized;

        // 조이스틱의 처음 위치와 현재 내가 터치하고있는 위치의 거리를 구한다.
        float Dis = Vector3.Distance(Pos, arrowFirstPos);

        // 거리가 반지름보다 작으면 조이스틱을 현재 터치하고 있는곳으로 이동. 
        if (Dis < Radius)
            arrow.position = arrowFirstPos + arrVec * Dis;
        // 거리가 반지름보다 커지면 조이스틱을 반지름의 크기만큼만 이동.
        else
            arrow.position = arrowFirstPos + arrVec * Radius;

        attackTouch = true;
        player.GetComponent<TestShoot>().ShootLine();
        playerTr.eulerAngles = new Vector3(0, Mathf.Atan2(arrVec.x, arrVec.y) * Mathf.Rad2Deg, 0);
    }

    // 드래그 끝.
    public void DragEnd()
    {
        if (!player.GetComponent<TestShoot>().PV.IsMine)
            return;

        player.GetComponent<TestShoot>().ShootLineEnd();
        arrow.position = arrowFirstPos; // 스틱을 원래의 위치로.
        arrVec = Vector3.zero;          // 방향을 0으로.
        rotation = false;
        attackTouch = false;
    }
}
