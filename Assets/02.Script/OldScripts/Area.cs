using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    public GameObject player; 

    private void Awake()
    {
        player = gameObject.transform.parent.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        float randomX = Random.Range(-5, 6);
        float randomZ = Random.Range(-5, 6);

        if (other.tag == "Environment")
        {
            player.transform.position = new Vector3(player.transform.position.x + randomX , player.transform.position.y, player.transform.position.z + randomZ );
        }    
    }
}
