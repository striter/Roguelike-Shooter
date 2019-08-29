using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractHealth : InteractBase {
    public override bool B_InteractOnTrigger => true;
    protected override bool B_RecycleOnInteract => true;
    public override enum_Interaction m_InteractType => enum_Interaction.Health;
    protected override bool B_CanInteract(EntityPlayerBase _interactor) => _interactor.m_HealthManager.F_HealthScale < 1;
    public float m_healAmount;
    public void Play(float _healthAmount)
    {
        m_healAmount = _healthAmount;
    }
    protected override void OnInteractSuccessful(EntityPlayerBase _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.m_HitCheck.TryHit(new DamageInfo(-m_healAmount, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(_interactTarget.I_EntityID)));
    }
}
