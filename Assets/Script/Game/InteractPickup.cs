using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPickup : InteractGameBase {

    public override bool B_InteractOnTrigger => true;
    protected override bool B_SelfRecycleOnInteract => true;
    float m_speed;
    EntityCharacterPlayer m_moveTowards;
    protected virtual void Play(bool moveTowardsPlayer)
    {
        base.Play();
        m_speed = 0;
        if (moveTowardsPlayer)
            OnBattleFinish();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    private void OnDisable()
    {
        if (!m_InteractEnable)
            return;
        m_moveTowards = null;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    void OnBattleFinish() => m_moveTowards = GameManager.Instance.m_LocalPlayer;
    private void Update()
    {
        if (!m_InteractEnable || m_moveTowards == null)
            return;

        if (m_speed < GameConst.F_PickupMaxSpeed)
            m_speed += GameConst.F_PickupAcceleration * Time.deltaTime;

        Vector3 offset = m_moveTowards.transform.position - transform.position;
        float travelDistance = m_speed * Time.deltaTime;
        if (offset.magnitude > travelDistance)
        {
            transform.position += offset.normalized * travelDistance;
            return;
        }
        transform.position = m_moveTowards.transform.position;
        TryInteract(m_moveTowards);
    }
}
