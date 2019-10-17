using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

public class InteractPickupAmount : InteractPickup {
    public float m_Amount { get; private set; }
    bool m_moveAble;
    float m_speed;
    Transform m_moveTowards;
    public virtual InteractPickupAmount Play(float amount, Transform moveTowards)
    {
        m_speed = 0;
        m_moveAble = false;
        m_moveTowards = moveTowards;
        m_Amount = amount;
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        return this;
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    void OnBattleFinish()
    {
        m_moveAble = true;
    }
    private void Update()
    {
        if (!m_moveAble||m_moveTowards==null)
            return;

        m_speed += GameConst.F_CoinsAcceleration * Time.deltaTime;
        Vector3 direction = (m_moveTowards.position - transform.position).normalized;
        transform.Translate(direction * m_speed * Time.deltaTime);
    }
}
