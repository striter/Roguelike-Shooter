using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractBloodShrine : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.BloodShrine;
    public int I_MuzzleSuccess;
    int m_TryCount;
    float m_damageHealthScale;
    public new void Play()
    {
        base.Play();
        m_TryCount = 0;
        m_damageHealthScale = GameExpression.GetBloodShrineHealthCostMultiple(m_TryCount);
    }
    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor)=>_interactor.m_Health.F_HealthMaxScale >m_damageHealthScale&&base.OnTryInteractCheck(_interactor);

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        _interactor.m_HitCheck.TryHit(new DamageInfo(-1, _interactor.m_Health.m_MaxHealth*m_damageHealthScale, enum_DamageType.HealthPenetrate));
        m_TryCount++;
        m_damageHealthScale = GameExpression.GetBloodShrineHealthCostMultiple(m_TryCount);
        if (TCommon.RandomPercentage() > GameConst.I_BloodShrineCoinsRate)
        {
            GameObjectManager.SpawnInteract<InteractPickupCoin>(NavigationManager.NavMeshPosition(_interactor.transform.position + TCommon.RandomXZCircle() * 4f), Quaternion.identity).Play(GameConst.I_BloodShrineCoinsAmount, false);
            GameObjectManager.PlayMuzzle(-1, _interactor.transform.position, Vector3.up, I_MuzzleSuccess);
        }
        return m_TryCount < GameConst.I_BloodShrineTryCountMax;
    }

}
