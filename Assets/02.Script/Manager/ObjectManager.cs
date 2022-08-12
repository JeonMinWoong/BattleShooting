using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class PoolType
{
    public string poolName;
    public GameObject poolOj;
    public int poolCount;
}
public class ObjectManager : MonoBehaviourPun
{
    public Transform[] poolingGroup;

    public PoolType[] poolGroup;

    public PhotonView pv;
    public static ObjectManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<ObjectManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static ObjectManager m_instance;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        TrainingCheck();
        ObjectInit();
    }

    void TrainingCheck()
    {
        poolGroup[0].poolOj = Resources.Load<GameObject>("EnemyBullet");
        poolGroup[1].poolOj = Resources.Load<GameObject>("bullet");
        poolGroup[2].poolOj = Resources.Load<GameObject>("InGameText");
        poolGroup[3].poolOj = Resources.Load<GameObject>("SmawM");
        poolGroup[4].poolOj = Resources.Load<GameObject>("LightBullet");
        poolGroup[5].poolOj = Resources.Load<GameObject>("GLBullet");
    }

    public void ObjectInit()
    {
        for (int i = 0; i < poolingGroup.Length; i++)
        {
            for (int j = 0; j < poolGroup[i].poolCount; j++)
            {
                GameObject _pool = Instantiate(poolGroup[i].poolOj, transform.position, transform.rotation);
                _pool.transform.SetParent(poolingGroup[i].transform);
                _pool.name = j.ToString();
                _pool.SetActive(false);
            }
        }
    }

    void PoolCreate(int number)
    {
        for (int j = 0; j < poolGroup[number].poolCount; j++)
        {
            GameObject _pool = Instantiate(poolGroup[number].poolOj, transform.position, transform.rotation);
            _pool.transform.SetParent(poolingGroup[number].transform);
            _pool.name = j.ToString();
            _pool.SetActive(false);
        }
    }

    public GameObject ObjectShot(int groupType, GameObject host, Vector3 pos, Quaternion quater,bool isOffLine = false)
    {
        GameObject bullet = null;

        Debug.Log("Stop");

        if (poolingGroup[groupType].transform.childCount <= 0)
            PoolCreate(groupType);

        bullet = poolingGroup[groupType].GetChild(0).gameObject;

        if (groupType == 0)
        {
            bullet.GetComponent<EnemyBullet>().host = host;
        }
        else if(groupType == 1)
        {
            bullet.GetComponent<bullet>().player = host;
        }
        else if(groupType == 3)
        {
            bullet.GetComponent<bulletM>().player = host;
        }
        else if(groupType == 4)
        {
            bullet.GetComponent<Lightbullet>().player = host;
        }
        else
        {
            bullet.GetComponent<GLbullet>().player = host;
        }

        bullet.GetComponent<Rigidbody>().velocity = Vector3.zero;
        bullet.transform.position = Vector3.zero;
        bullet.transform.SetParent(transform);
        bullet.transform.position = pos;
        bullet.transform.rotation = quater;
        bullet.SetActive(true);

        return bullet;
    }

    public GameObject PoolGet(int number, bool isOffLine = false)
    {
        if (poolingGroup[number].transform.childCount <= 8)
            PoolCreate(number);

        GameObject poolOj = poolingGroup[number].GetChild(0).gameObject;
        poolOj.SetActive(true);

        return poolOj;
    }
}
