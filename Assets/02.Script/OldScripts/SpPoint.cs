using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpPoint : MonoBehaviour
{
    public int playerSpPoint;
    public TextMeshProUGUI pointText;

    private void Update()
    {
        playerSpPoint = AuthManager.instance.userPlayerSpPoint;
        pointText.text = playerSpPoint.ToString();
    }

    public void SpPointPlus()
    {
        AuthManager.instance.userPlayerSpPoint++;
        Debug.Log("sub");
        AuthManager.instance.OnClickUpdateChildren();
    }

    public void SpPointSub()
    {
        AuthManager.instance.userPlayerSpPoint--;
        AuthManager.instance.OnClickUpdateChildren();
    }
}
