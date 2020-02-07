using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractPickup {
    public int m_Amount { get; private set; }
    protected bool m_OutOfBattle { get; private set; }
    float m_speed;
    Transform m_moveTowards;
    public virtual InteractPickupAmount Play(int amount)
    {
        base.Play();
        m_speed = 0;
        m_OutOfBattle = false;
        m_Amount = amount;
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
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        _interactor.OnInteractPickup(this,m_Amount);
        return false;
    }

    void OnBattleFinish()
    {
        if (!m_InteractEnable)
            return;
        m_moveTowards = GameManager.Instance.m_LocalPlayer.transform;
        m_OutOfBattle = true;
    }
    private void Update()
    {
        if (!m_OutOfBattle || m_moveTowards == null)
            return;

        if (m_speed<12f)
            m_speed += GameConst.F_PickupAcceleration * Time.deltaTime;
        Vector3 direction =  (m_moveTowards.position - transform.position).normalized;
        transform.position +=  direction * m_speed * Time.deltaTime;
    }
}
