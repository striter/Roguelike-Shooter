using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractPickup {
    public int m_Amount { get; private set; }
    protected bool m_OutOfBattle { get; private set; }
    float m_speed;
    Transform m_moveTowards;
    public virtual InteractPickupAmount Play(int amount, Transform moveTowards)
    {
        base.Play();
        m_speed = 0;
        m_OutOfBattle = false;
        m_moveTowards = moveTowards;
        m_Amount = amount;
        return this;
    }
    private void OnEnable()
    {
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    private void OnDisable()
    {
        m_moveTowards = null;
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractOnceCanKeepInteract(_interactTarget);
        _interactTarget.OnInteractPickup(this,m_Amount);
        return false;
    }

    void OnBattleFinish()
    {
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
