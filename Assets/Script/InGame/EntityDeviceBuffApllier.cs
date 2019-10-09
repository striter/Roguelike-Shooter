using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityDeviceBuffApllier : EntityDeviceBase {
    Func<DamageDeliverInfo> OnApplyPlayer, OnApplyAlly;
    float f_refreshCheck;
    float f_refreshDuration;
    public override void OnActivate(enum_EntityFlag _flag)
    {
        base.OnActivate(_flag);
        OnApplyPlayer = null;
        OnApplyAlly = null;
    }
    public void SetBuffApply(Func<DamageDeliverInfo> applyPlayer,Func<DamageDeliverInfo> applyAlly,float refreshDuration=.8f)
    {
        OnApplyPlayer = applyPlayer;
        OnApplyAlly = applyAlly;
        f_refreshDuration = refreshDuration;
        f_refreshCheck = 0f;
    }

    protected override void Update()
    {
        base.Update();
        if (m_Health.b_IsDead)
            return;

        if (f_refreshCheck > 0)
        {
            f_refreshCheck -= Time.deltaTime;
            return;
        }
        f_refreshCheck = f_refreshDuration;

        m_DetectLink.Traversal((EntityCharacterBase entity) =>
        {
            if (entity.m_Flag != m_Flag||entity.I_EntityID!=I_EntityID)
                return;

            switch (entity.m_Controller)
            {
                case enum_EntityController.AI:
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common, OnApplyAlly()));
                    break;
                case enum_EntityController.Player:
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Common, OnApplyPlayer()));
                    break;
            }
        });
    }
}
