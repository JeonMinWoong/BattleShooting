using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TrainingController : MonoBehaviourPunCallbacks
{
    public static TrainingController instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<TrainingController>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }

    private static TrainingController m_instance;
    public bool training;

    void Awake()
    {
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }

        if (GameObject.Find("GameManager") == null)
        {
            training = true;
        }
        else
        {   
            training = false;
        }
    }

    

}
