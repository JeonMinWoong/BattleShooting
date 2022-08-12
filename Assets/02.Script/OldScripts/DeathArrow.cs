using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class DeathArrow : MonoBehaviourPun
{
    public GameObject deathZon;
	public float speed=5;
	public PhotonView PV;

	private void Start()
	{
		deathZon = GameObject.Find("DeathArrow");
		PV = gameObject.transform.parent.gameObject.GetComponent<PhotonView>();

		if (TrainingController.instance.training == true)
		{
			return;
		}
		else
		{
			if (!PV.IsMine)
				gameObject.SetActive(false);
		}
	}

	void Update()
	{
		Vector3 dir = deathZon.transform.position - transform.position;
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * speed);
	}

		


}
