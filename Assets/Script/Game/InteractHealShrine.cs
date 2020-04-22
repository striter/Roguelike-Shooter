using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractHealShrine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.HealShrine;
    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=> !_interactor.m_Health.m_HealthFull&& base.OnTryInteractCheck(_interactor);
    int m_TryCount;
    public new InteractHealShrine Play()
    {
        base.Play();
        m_TryCount = 0;
        m_TradePrice = GameExpression.GetHealShrinePrice(m_TryCount);
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_TryCount++;
        m_TradePrice = GameExpression.GetHealShrinePrice(m_TryCount);
        _interactor.m_HitCheck.TryHit(new DamageInfo(-1, -GameConst.F_HealShrineHealthReceive, enum_DamageType.HealthPenetrate));
        return m_TryCount < GameConst.I_HealShrineTryCountMax;
    }
}
