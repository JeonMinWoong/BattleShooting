using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouesPoint : MonoBehaviour
{
    public Texture2D cursorTexture;
    public bool hotSpotIsCenter = false;
    public Vector2 adjustHotSpot = Vector2.zero;
    private Vector2 hotSpot;

    void Start()
    {
        StartCoroutine("PcCursor");
    }


    IEnumerator PcCursor()
    {
        
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            hotSpot.x = cursorTexture.width / 2;
            hotSpot.y = cursorTexture.height / 2;
        }
        else
        {
            hotSpot = adjustHotSpot;
        }
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }


}
