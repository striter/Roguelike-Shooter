using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIT_JoyStick : SimpleSingletonMono<UIT_JoyStick> {
    RectTransform rtf_Main;
    RectTransform rtf_Center;
    public Vector2 Init()
    {
        rtf_Main = GetComponent<RectTransform>();
        rtf_Center = transform.Find("Center").GetComponent<RectTransform>();
        rtf_Main.SetActivate(false);
        return rtf_Main.sizeDelta;
    }
    public void SetPos(Vector2 startPos,Vector2 centerPos)
    {
        rtf_Main.anchoredPosition = startPos;
        rtf_Center.anchoredPosition = centerPos;
    }
}
