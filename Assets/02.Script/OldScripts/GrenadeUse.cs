using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;



public class GrenadeUse : MonoBehaviourPun
{
    public LineRenderer lineRender;
    public float rotationSpeed = 4.04f;
    public GameObject circle;
    public GameObject grenade;

    Vector3 center = Vector3.zero;
    Vector3 theArc = Vector3.zero;
    

    [System.Obsolete]
    void Start()
    {
        lineRender.SetWidth(0.5f,1.5f);
        lineRender.SetVertexCount(23);
        lineRender.enabled = false;
        circle = gameObject.transform.Find("GrenadeEx").gameObject;
    }

    void Update()
    { 
        if (circle.activeSelf == true)
        {
            lineRender.enabled = true;
            Plane playerPlane = new Plane(Vector3.up, transform.position);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float hitdist = 0.0f;
            Vector3 targetPoint = Vector3.zero;

            if (playerPlane.Raycast(ray, out hitdist))
            {
                targetPoint = circle.transform.position;

                center = (transform.position + targetPoint) * 0.5f;
                center.y -= 2.0f;

                Quaternion targetRotation = Quaternion.LookRotation(center - transform.position);

                RaycastHit hitInfo;
                if (Physics.Linecast(transform.position, targetPoint, out hitInfo))
                {
                    targetPoint = hitInfo.point;
                }
            }
            else
            {
                targetPoint = circle.transform.position;
            }
            Vector3 relCenter = transform.position - center;
            Vector3 aimRelCenter = targetPoint - center;

            for (float index = 0.0f, interval = 0.0417f; interval < 1.0f;)
            {
                theArc = Vector3.Slerp(relCenter, aimRelCenter, interval += 0.0417f);
                lineRender.SetPosition((int)index++, theArc + center);
            }
        }
        else
            lineRender.enabled = false;
    }
}
