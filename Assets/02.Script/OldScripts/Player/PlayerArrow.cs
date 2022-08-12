using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayerArrow : MonoBehaviour
{

    // 공개
    public Transform arrow;         // 조이스틱.
    public GameObject player;
    public Animator playerAni;
    public Transform playerTr;

    // 비공개
    private Vector3 arrowFirstPos;  // 조이스틱의 처음 위치.
    private Vector3 arrVec;         // 조이스틱의 벡터(방향)
    private float Radius;           // 조이스틱 배경의 반 지름.
    private bool moveFlag;          // 움직임 유무
    public float speed = 6;
    public TestHealth playerHealth;
    public GameObject rotationArrow;
    public GameObject conSumArrow;
    Vector3 m_vecMove;

    void Start()
    {
        player = GameObject.Find("PlayerTest");
        playerAni = player.GetComponent<Animator>();
        playerTr = player.GetComponent<Transform>();
        playerHealth = player.GetComponent<TestHealth>();


        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        arrowFirstPos = arrow.transform.position;

        // 캔버스 크기에대한 반지름 조절.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;

        moveFlag = false;
    }

    private void Update()
    {
        if (player != null)
        {
            if (player.GetComponent<TestHealth>().shock == true)
            {
                gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
                DragEnd();
            }
            else
                gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0.35f);

            if (player.GetComponent<PlayerSkill_Specificity>().skillType == 1 && player.GetComponent<PlayerSkill_Specificity>().skillUse == true)
            {
                float Speed = 6 * player.GetComponent<PlayerSkill_Specificity>().spSpeedPlus;
                speed = 12 + Speed * 2;
            }
                
            else
            {
                float Speed = 6 * player.GetComponent<PlayerSkill_Specificity>().spSpeedPlus;
                speed = 6 + Speed;
            }

            if (moveFlag)
            {
                playerTr.position += m_vecMove;
                //player.transform.Translate(Vector3.forward * Time.deltaTime * 6f);
                playerAni.SetBool("IsWalking", true);
            }
            else
                playerAni.SetBool("IsWalking", false);
        }
    }

    public void Drag(BaseEventData _Data)
    {
        if (playerHealth.shock == true)
            return;

        if (playerHealth.isDeath == true)
            return;

        moveFlag = true;
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

        m_vecMove = new Vector3(arrVec.x * speed * Time.deltaTime, 0f, arrVec.y * speed * Time.deltaTime);
        if (rotationArrow.GetComponent<PlayerAttackArrow>().rotation == false && conSumArrow.GetComponent<ConsumableItem>().rotation == false)
            playerTr.eulerAngles = new Vector3(0, Mathf.Atan2(arrVec.x, arrVec.y) * Mathf.Rad2Deg, 0);
    }

    // 드래그 끝.
    public void DragEnd()
    {
        arrow.position = arrowFirstPos; // 스틱을 원래의 위치로.
        arrVec = Vector3.zero;          // 방향을 0으로.
        moveFlag = false;
    }
}