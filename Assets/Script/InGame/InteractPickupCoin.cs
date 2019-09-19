using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPickupCoin : InteractPickup {
    public override enum_Interaction m_InteractType => enum_Interaction.PickupCoin;
    public int m_CoinAmount { get; private set; }
    float m_speed;
    Transform m_moveTowards;
    public InteractPickupCoin Play(int coinAmount,Transform moveTowards)
    {
        base.Play();
        m_speed = 0;
        m_moveTowards = moveTowards;
        m_CoinAmount = coinAmount;
        return this;
    }
    private void Update()
    {
        if (!m_moveTowards)
            return;

        m_speed += GameConst.F_CoinsAcceleration*Time.deltaTime;
        Vector3 direction = (m_moveTowards.position - transform.position).normalized;
        transform.Translate(direction * m_speed * Time.deltaTime);
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_PlayerInfo.OnCoinsReceive(m_CoinAmount);
    }
}
