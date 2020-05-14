using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractHealShrine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.HealShrine;
    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=> !_interactor.m_Health.m_HealthFull&& base.OnTryInteractCheck(_interactor);
    int m_TryCount;
    float m_BaseTradePrice;
    public InteractHealShrine Play(float tradePrice)
    {
        base.Play();
        m_TryCount = 0;
        SetTradePrice(tradePrice);
        m_BaseTradePrice = tradePrice;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_TryCount++;
        SetTradePrice(m_BaseTradePrice*(1f+ GameExpression.GetHealShrinePriceMultiply(m_TryCount)));
        _interactor.m_HitCheck.TryHit(new DamageInfo(-1, enum_DamageIdentity.Environment).SetDamage(-GameConst.F_HealShrineHealthReceive, enum_DamageType.HealthPenetrate));
        return m_TryCount < GameConst.I_HealShrineTryCountMax;
    }
}
