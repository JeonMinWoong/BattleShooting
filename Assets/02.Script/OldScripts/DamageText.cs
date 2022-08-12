using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Photon.Pun;

public class DamageText : MonoBehaviour
{
    public TextMeshProUGUI damageText;
    public float inGameText;

    public void ShowDamageEffect(float damage,int _inGameText,Vector3 pos)
    {
        damageText.text = damage.ToString();                 // 데미지 값 표시
        transform.position = pos;
        transform.SetParent(transform.parent.parent);
        if (_inGameText == 0)
            damageText.color = new Color(1, 0, 0);
        else if(_inGameText == 1)
            damageText.color = new Color(0, 1, 0);
        else if(_inGameText == 2)
            damageText.color = new Color(0, 0, 1);
        else
            damageText.color = new Color(1, 1, 0);

        damageText.DOFade(0f, 1f);
        transform.DOMoveY(3f, 3f).SetRelative();

        StartCoroutine(CoActive(2));
    }

    IEnumerator CoActive(float time)
    {
        //Debug.Log("텍스트");
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
        transform.SetParent(ObjectManager.instance.poolingGroup[2]);
    }

}
