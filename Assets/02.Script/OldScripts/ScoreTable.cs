using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTable : MonoBehaviour
{
    public Canvas scoreTable;

    private void Awake()
    {
        scoreTable = GameObject.Find("ScoreCanvas").GetComponent<Canvas>();
    }

    IEnumerator GameSetScore()
    {
        scoreTable.enabled = true;
        yield return new WaitForSecondsRealtime(3f);
        scoreTable.enabled = false;
    }
}
