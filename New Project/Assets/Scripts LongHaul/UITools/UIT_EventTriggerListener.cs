using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIT_EventTriggerListener : UnityEngine.EventSystems.EventTrigger {
    public Action<bool,Vector2> D_OnPress;
    public Action<Vector2> D_OnDrag,D_OnDragDelta;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (D_OnPress != null)
            D_OnPress(true,eventData.position);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (D_OnPress != null)
            D_OnPress(false, eventData.position);
    }
    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        if (D_OnDrag != null)
            D_OnDrag(eventData.position);
        if (D_OnDragDelta != null)
            D_OnDragDelta(eventData.delta);
    }
}
