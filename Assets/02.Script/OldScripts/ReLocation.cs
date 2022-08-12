using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ReLocation : MonoBehaviour
{
    private NavMeshAgent navMesh;
    private bool reLocation;
    
    void Awake()
    {
        navMesh = GetComponent<NavMeshAgent>();
        reLocation = false;
        StartCoroutine(EndReLocation());
    }

    private void OnCollisionStay(Collision collision) 
    { 
        if (collision.collider.tag == "Environment" && reLocation == false)
        {
            float randomX = Random.Range(-20f, 20f);
            float randomZ = Random.Range(-20f, 20f);
            navMesh.enabled = false;
            transform.position = new Vector3(transform.position.x + randomX, 1, transform.position.z + randomZ);
            //Debug.Log("탈출");
            navMesh.enabled = true;
        }
    }

    IEnumerator EndReLocation()
    {
        yield return new WaitForSeconds(2f);
        reLocation = true;
    }
}
