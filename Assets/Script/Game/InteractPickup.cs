using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPickup : InteractGameBase {

    protected override bool B_SelfRecycleOnInteract => true;
    float m_MoveSpeed;
    Transform m_MoveTowards;
    TimerBase m_DropTimer=new TimerBase(.5f,true);
    Vector3  m_DropPosition;

    public InteractPickup PlayDropAnim(Vector3 dropPosition)
    {
        m_DropTimer.Replay();
        m_DropPosition = dropPosition;
        return this;
    }

    public InteractPickup PlayMoveAnim(Transform towards)
    {
        m_MoveSpeed = 0;
        m_MoveTowards = towards;
        return this;
    }

    private void Update()
    {
        if (!m_InteractEnable)
            return;

        if(m_DropTimer.m_Timing)
        {
            transform.position = Vector3.Lerp(m_DropPosition,transform.position, m_DropTimer.m_TimeLeftScale);
            m_DropTimer.Tick(Time.deltaTime);
            return;
        }

        if (m_MoveTowards == null)
            return;

        if (m_MoveSpeed < GameConst.F_PickupMaxSpeed)
            m_MoveSpeed += GameConst.F_PickupAcceleration * Time.deltaTime;

        Vector3 offset = m_MoveTowards.transform.position - transform.position;
        float travelDistance = m_MoveSpeed * Time.deltaTime;
        if (offset.sqrMagnitude > travelDistance*travelDistance)
        {
            transform.position += offset.normalized * travelDistance;
            return;
        }
        transform.position = m_MoveTowards.transform.position+TCommon.RandomXZCircle()*.5f;
        m_MoveTowards = null;
    }

    private void OnDisable()
    {
        m_MoveTowards = null;
        m_DropTimer.Stop();
    }
}
