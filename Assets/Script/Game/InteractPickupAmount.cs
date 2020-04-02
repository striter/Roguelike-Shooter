using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractGameBase {
    public int m_Amount { get; private set; }
    public override bool B_InteractOnTrigger => true;
    protected override bool B_SelfRecycleOnInteract => true;
    float m_speed;
    EntityCharacterPlayer m_moveTowards;
    public virtual InteractPickupAmount Play(int amount,bool moveTowardsPlayer)
    {
        base.Play();
        m_speed = 0;
        m_Amount = amount;
        if (moveTowardsPlayer)
            OnBattleFinish();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        return this;
    }

    private void OnDisable()
    {
        if (!m_InteractEnable)
            return;
        m_moveTowards = null;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    protected override bool OnInteractedCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedCheck(_interactor);
        _interactor.OnInteractPickup(this,m_Amount);
        return false;
    }

    void OnBattleFinish()=> m_moveTowards = GameManager.Instance.m_LocalPlayer;
    private void Update()
    {
        if (!m_InteractEnable|| m_moveTowards == null)
            return;

        if (m_speed<GameConst.F_PickupMaxSpeed)
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
