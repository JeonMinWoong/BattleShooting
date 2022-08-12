using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

public class EnemyReSpawn : Singleton<EnemyReSpawn>
{
    public GameObject enemy;
    public float enemyReSpawnTime;
    public int enemyCount = 0;
    public Transform[] enemyReSpawnPos;
    
    void Start()
    {
        InvokeRepeating("EnemySpawn", enemyReSpawnTime, enemyReSpawnTime);
    }

    void EnemySpawn()
    {
        if (enemyCount >= 1)
            return;
        for (int i = 0; i < 4; i++)
        {
            Instantiate(enemy, enemyReSpawnPos[i].position, Quaternion.identity);
            enemyCount++;
        }
        
    }
}
