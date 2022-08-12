using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonText : MonoBehaviour
{

    public void SetText(int textValue,Vector3 pos)
    {
        transform.GetComponentInChildren<Text>().text = textValue.ToString();
        transform.SetParent(transform.parent.parent);
        transform.position = pos;
        StartCoroutine(ReCanvas(gameObject));
    }

    IEnumerator ReCanvas(GameObject damageCanvas)
    {
        while (true)
        {
            damageCanvas.transform.Translate(0, 0.02f, 0);
            if (damageCanvas.transform.position.y >= 2)
            {
                damageCanvas.transform.SetParent(OJM.instance.poolingGroup[0]);
                damageCanvas.SetActive(false);
                break;
            }
            yield return null;
        }
    }

}
