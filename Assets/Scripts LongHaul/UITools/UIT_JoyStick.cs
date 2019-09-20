using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIT_JoyStick : MonoBehaviour {
    RectTransform rtf_Main;
    RectTransform rtf_Center;
    public float m_JoyStickRaidus { get; private set; }
    public void Awake()
    {
        rtf_Main = GetComponent<RectTransform>();
        rtf_Center = transform.Find("Center").GetComponent<RectTransform>();
        rtf_Main.SetActivate(false);
        m_JoyStickRaidus= rtf_Main.sizeDelta.y/2-rtf_Center.sizeDelta.y/2;
    }
    public void SetPos(Vector2 startPos,Vector2 centerPos)
    {
        rtf_Main.anchoredPosition = startPos;
        rtf_Center.anchoredPosition = centerPos;
    }
}
