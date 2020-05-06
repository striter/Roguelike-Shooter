using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBloodShrine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.BloodShrine;
    public int I_MuzzleSuccess;
    int m_TryCount;
    float m_damageHealthScale;
    public new InteractBloodShrine Play()
    {
        base.Play();
        m_TryCount = 0;
        m_damageHealthScale = GameExpression.GetBloodShrineHealthCostMultiple(m_TryCount);
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.m_HitCheck.TryHit(new DamageInfo(-1, _interactor.m_Health.m_CurrentHealth*m_damageHealthScale, enum_DamageType.HealthPenetrate));
        m_TryCount++;
        m_damageHealthScale = GameExpression.GetBloodShrineHealthCostMultiple(m_TryCount);
        int amount = GameConst.RI_BloodShrintCoinsAmount.Random();
        if(amount>0)
        {
            for (int i = 0; i < amount; i++)
                GameObjectManager.SpawnInteract<InteractPickupCoin>(transform.position, Quaternion.identity).Play(1).PlayDropAnim(NavigationManager.NavMeshPosition(_interactor.transform.position + TCommon.RandomXZCircle() * 4f)).PlayMoveAnim(_interactor.transform);
            GameObjectManager.PlayMuzzle(-1, _interactor.transform.position, Vector3.up, I_MuzzleSuccess);
        }
        return m_TryCount < GameConst.I_BloodShrineTryCountMax;
    }
}
