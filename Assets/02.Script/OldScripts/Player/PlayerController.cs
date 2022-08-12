using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent (typeof (Animator))]
public class PlayerController : MonoBehaviourPunCallbacks 
{

	public Transform rightGunBone;
	public Transform leftGunBone;
	public Arsenal[] arsenal;
	public PhotonView PV;

	void Awake()
	{
		PV = GetComponent<PhotonView>();
		if (arsenal.Length > 0)
			SetArsenal(arsenal[0].name);

	}

	[PunRPC]
	public void SetArsenal(string name)
	{
		foreach (Arsenal hand in arsenal)
		{
			if (hand.name == name)
			{
				if (rightGunBone.childCount > 0)
					Destroy(rightGunBone.GetChild(0).gameObject);
				if (leftGunBone.childCount > 0)
					Destroy(leftGunBone.GetChild(0).gameObject);
				if (hand.rightGun != null)
				{
					GameObject newRightGun = (GameObject)Instantiate(hand.rightGun);
					newRightGun.transform.parent = rightGunBone;
					newRightGun.transform.localPosition = Vector3.zero;
					newRightGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				if (hand.leftGun != null)
				{
					GameObject newLeftGun = (GameObject)Instantiate(hand.leftGun);
					newLeftGun.transform.parent = leftGunBone;
					newLeftGun.transform.localPosition = Vector3.zero;
					newLeftGun.transform.localRotation = Quaternion.Euler(90, 0, 0);
				}
				return;
			}

		}
	}

    [System.Serializable]
	public struct Arsenal {
		public string name;
		public GameObject rightGun;
		public GameObject leftGun;
	}
}
