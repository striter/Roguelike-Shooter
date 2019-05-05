using System;
using UnityEngine;

public class UIT_AndroidTouchController : SimpleSingletonMono<UIT_AndroidTouchController>
{
    public static Action<Vector2> OnLeftDelta,OnRightDelta;
    UIT_EventTriggerListener LeftTrigger, RightTrigger;
    float f_LeftStickRadius;
    RectTransform rtf_LeftJoyStick, rtf_LeftJoyStickCenter;
    protected override void Awake()
    {
        base.Awake();
        LeftTrigger = transform.Find("Left").GetComponent<UIT_EventTriggerListener>();
        RightTrigger = transform.Find("Right").GetComponent<UIT_EventTriggerListener>();
        LeftTrigger.D_OnPress = OnLeftPress;
        LeftTrigger.D_OnDrag = OnLeftDrag;
        RightTrigger.D_OnDragDelta = OnRightDrag;
        rtf_LeftJoyStick = transform.Find("JoySticks/LeftJoyStick").GetComponent<RectTransform>();
        rtf_LeftJoyStickCenter = transform.Find("JoySticks/LeftJoyStick/Center").GetComponent<RectTransform>();
        rtf_LeftJoyStick.SetActivate(false);
        f_LeftStickRadius = rtf_LeftJoyStick.sizeDelta.x/2-rtf_LeftJoyStickCenter.sizeDelta.x/2;
    }
    public static void SetEnabled(bool enabled)
    {
        Instance.LeftTrigger.SetActivate(enabled);
        Instance.RightTrigger.SetActivate(enabled);
    }
    bool b_leftPressing;
    Vector2 v2_leftStartPos,v2_leftCurPos;
    void OnLeftPress(bool down,Vector2 deltaPos)
    {
        b_leftPressing = down;
        if (down)
        {
            v2_leftStartPos = deltaPos;
            rtf_LeftJoyStickCenter.anchoredPosition = Vector2.zero;
            rtf_LeftJoyStick.anchoredPosition = v2_leftStartPos;
        }
        rtf_LeftJoyStick.SetActivate(down);
    }
    void OnLeftDrag(Vector2 deltaPos)
    {
        v2_leftCurPos = deltaPos;
     }
    
    void OnRightDrag(Vector2 deltaPos)
    {
        if (OnRightDelta != null)
            OnRightDelta(deltaPos);
    }
    private void Update()
    {
        if (OnLeftDelta != null)
        {
            if (!b_leftPressing)
                OnLeftDelta(Vector2.zero);
            else
            {
                float distance = Vector2.Distance(v2_leftCurPos, v2_leftStartPos);
                rtf_LeftJoyStickCenter.anchoredPosition = distance > f_LeftStickRadius ? (v2_leftCurPos - v2_leftStartPos).normalized * f_LeftStickRadius : v2_leftCurPos - v2_leftStartPos;
                OnLeftDelta(new Vector2(rtf_LeftJoyStickCenter.anchoredPosition.x / f_LeftStickRadius, rtf_LeftJoyStickCenter.anchoredPosition.y / f_LeftStickRadius));
            }
        }

    }
}
