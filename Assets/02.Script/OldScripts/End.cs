using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class End : MonoBehaviour
{
    public void OnEnd()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
