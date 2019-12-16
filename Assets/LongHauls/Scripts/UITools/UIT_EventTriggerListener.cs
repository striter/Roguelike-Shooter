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
    bool m_pressing=false;
    #region Press
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (m_pressing)
            return;

        OnPressStatus?.Invoke(true,eventData.position);
        m_pressDuration = 0f;
        m_pressing = true;
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!m_pressing)
            return;

        OnPressStatus?.Invoke(false, eventData.position);
        OnPressDuration?.Invoke(m_pressDuration);
        m_pressDuration = 0f;
        m_pressing = false;
    }
    private void Update()
    {
        if (m_pressing)
            m_pressDuration += Time.deltaTime;
    }
    private void OnDisable()
    {
        if (!m_pressing)
            return;
        m_pressing = false;
        OnPressStatus(false, Vector2.zero);
        OnPressDuration(m_pressDuration);
    }
    #endregion

    #region Drag
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
    #endregion
    public void OnRaycast()
    {
        D_OnRaycast?.Invoke();
    }
}
