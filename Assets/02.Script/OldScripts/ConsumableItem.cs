using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Photon.Pun;
using Photon.Realtime;

public class ConsumableItem : MonoBehaviourPunCallbacks
{
    public Image conItem;
    public Image coolTiem;
    public Image grenTiem;
    public float currentCoolTime;
    public float maxCoolTime;
    public float currentGrenTime;
    public float maxGrenTime;
    public bool itemUse;
    public GameObject player;
    private bool useItem;
   

    // 공개
    public Transform arrow;         // 조이스틱.
    public Transform grenadeTr;

    // 비공개
    private Vector3 arrowFirstPos;  // 조이스틱의 처음 위치.
    public Vector3 arrVec;         // 조이스틱의 벡터(방향)
    private float Radius;           // 조이스틱 배경의 반 지름.
    public bool rotation;          // 회전 유무
    public TestHealth playerHealth;
    public Vector3 m_vecMove;
    private bool moveFlag;          // 움직임 유무
    public float speed = 6;
    private IEnumerator grenadeUse;
    public bool grenThrow;

    public void Start()
    {
        player = GameObject.Find("PlayerTest");
        grenadeTr = player.transform.Find("GrenadeEx").gameObject.transform;
        playerHealth = player.GetComponent<TestHealth>();
        grenadeUse = GrenadeUse();
        player.GetComponent<TestShoot>().consumableItemUi = conItem.GetComponent<Image>();

        conItem.gameObject.SetActive(false);
        currentCoolTime = 0;
        maxCoolTime = 60;
        currentGrenTime = 0;
        maxGrenTime = 100;
        gameObject.GetComponent<Image>().enabled = false;
        Radius = GetComponent<RectTransform>().sizeDelta.y * 0.5f;
        arrowFirstPos = arrow.transform.position;

        // 캔버스 크기에대한 반지름 조절.
        float Can = transform.parent.GetComponent<RectTransform>().localScale.x;
        Radius *= Can;

        itemUse = false;
        rotation = false;
        moveFlag = false;
    }

    public void Update()
    {
        if (player != null)
        {
            if (player.GetComponent<TestHealth>().shock == true)
            {
                gameObject.GetComponent<Image>().color = new Color(0, 0, 0);
                DragEnd();
            }
            else
                gameObject.GetComponent<Image>().color = new Color(0, 0, 0, 0.5f);

            if (player.GetComponent<TestShoot>().ConsumableItemCount <= 0)
                conItem.gameObject.SetActive(false);

            if (player.GetComponent<TestShoot>().consumableType != 0 && player.GetComponent<TestShoot>().ConsumableItemCount > 0)
            {
                conItem.gameObject.SetActive(true);
            }
            if (player.GetComponent<TestShoot>().consumableType == 2 || player.GetComponent<TestShoot>().consumableType == 5 || player.GetComponent<TestShoot>().consumableType == 0)
            {
                useItem = false;
                grenadeTr.gameObject.SetActive(false);
                gameObject.GetComponent<Image>().enabled = false;
            }
            else
                useItem = true;

            if (moveFlag)
            {
                grenadeTr.position += m_vecMove;
            }
        }
    }

    public void ItemUse()
    {
        if (itemUse == false)
        {
            if (player.GetComponent<TestShoot>().consumableType == 1)
            {
                player.GetComponent<TestShoot>().StartCoroutine("GrenadeThrow");
                StartCoroutine("CoolTime");
                itemUse = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 2)
            {
                player.GetComponent<TestShoot>().ConsumableItemUse();
                StartCoroutine("CoolTime");
                itemUse = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 3)
            {
                player.GetComponent<TestShoot>().StartCoroutine("GrenadeThrow");
                StartCoroutine("CoolTime");
                itemUse = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 4)
            {
                player.GetComponent<TestShoot>().StartCoroutine("GrenadeThrow");
                StartCoroutine("CoolTime");
                itemUse = true;
            }
            else if (player.GetComponent<TestShoot>().consumableType == 5)
            {
                player.GetComponent<TestShoot>().ConsumableItemUse();
                StartCoroutine("CoolTime");
                itemUse = true;
            }
            else
                return;
        }
    }
    
    public void Click()
    {
        if (playerHealth.isDeath == true)
            return;
        if (useItem == true)
            return;
        if (itemUse == true)
            return;
        if (player.GetComponent<TestShoot>().ConsumableItemCount == 0)
            return;

        ItemUse();
    }

    public void Drag(BaseEventData _Data)
    {
        
        if (playerHealth.isDeath == true)
            return;
        if (useItem == false)
            return;
        if (itemUse == true)
            return;
        if (playerHealth.shock == true)
            return;
        if (player.GetComponent<TestShoot>().ConsumableItemCount == 0)
            return;

        gameObject.GetComponent<Image>().enabled = true;
        grenadeTr.gameObject.SetActive(true);
        moveFlag = true;
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

        m_vecMove = new Vector3(arrVec.x * speed * Time.deltaTime, 0f, arrVec.y * speed * Time.deltaTime);
        grenadeTr.eulerAngles = new Vector3(0, Mathf.Atan2(arrVec.x, arrVec.y) * Mathf.Rad2Deg, 0);

        StartCoroutine("GrenadeUse");
    }

    public IEnumerator GrenadeUse()
    {
        if (grenThrow == false)
        {
            AudioManager.Instance.PlaySound("ConsumItemUse", transform.position, 1f + Random.Range(-0.1f, 0.1f));
            grenThrow = true;
            StartCoroutine("GrenadeTime");
            yield return new WaitForSecondsRealtime(3f);
        }
    }

    public void DragEnd()
    {
        if (player.GetComponent<TestShoot>().ConsumableItemCount == 0)
            return;

        gameObject.GetComponent<Image>().enabled = false;
        arrow.position = arrowFirstPos; // 스틱을 원래의 위치로.
        arrVec = Vector3.zero;          // 방향을 0으로.
        rotation = false;
        moveFlag = false;
        if (player.GetComponent<TestHealth>().shock == false)
            ItemUse();
    }

    IEnumerator GrenadeTime()
    {
        if (currentGrenTime <= maxGrenTime)
        {
            currentGrenTime += 0.5f;
            grenTiem.fillAmount = currentGrenTime / maxGrenTime;
            yield return new WaitForSecondsRealtime(0.01f);
            StartCoroutine("GrenadeTime");
        }
        else if (currentGrenTime > maxGrenTime)
        {
            currentGrenTime = 0;
            grenTiem.fillAmount = currentGrenTime / maxGrenTime;
            grenThrow = false;
            DragEnd();
            player.GetComponent<TestShoot>().StartCoroutine(player.GetComponent<TestShoot>().Boob());
        }

    }


   IEnumerator CoolTime()
    {
        coolTiem.gameObject.SetActive(true);

        while (true)
        {
            currentCoolTime += 0.1f;
            coolTiem.fillAmount = currentCoolTime / maxCoolTime;
            if (currentCoolTime >= maxCoolTime)
                break;
            yield return new WaitForSecondsRealtime(0.01f);
        }

        coolTiem.gameObject.SetActive(false);
        currentCoolTime = 0;
        coolTiem.fillAmount = currentCoolTime / maxCoolTime;
        itemUse = false;
    }
}
