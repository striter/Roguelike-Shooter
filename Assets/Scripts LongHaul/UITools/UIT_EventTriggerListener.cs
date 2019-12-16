using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIT_EventTriggerListener : EventTrigger {
    public Action<float> OnPressDuration;
    public Action<bool, Vector2> OnPressStatus;
    public Action<bool,Vector2> D_OnDragStatus;
    public Action<Vector2> D_OnDrag,D_OnDragDelta;
    public Action D_OnRaycast;
    float m_pressDuration;
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        m_pressDuration = 0f;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        OnPressDuration?.Invoke(m_pressDuration);
        m_pressDuration = 0f;
    }
    private void Update()
    {
        m_pressDuration += Time.deltaTime;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        D_OnDrag?.Invoke(eventData.position);
        D_OnDragDelta?.Invoke(eventData.delta);
    }
    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        D_OnDragStatus?.Invoke(true, eventData.position);
    }
    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        D_OnDragStatus?.Invoke(false, eventData.position);
    }
    public void OnRaycast()
    {
        D_OnRaycast?.Invoke();
    }
}
