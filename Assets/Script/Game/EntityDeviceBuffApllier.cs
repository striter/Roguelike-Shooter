using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityDeviceBuffApllier : EntityDeviceBase {
    Func<DamageDeliverInfo> OnApplyPlayer, OnApplyAlly;
    float f_refreshCheck;
    float f_refreshDuration;
    public override void OnActivate(enum_EntityFlag _flag, float startHealth)
    {
        base.OnActivate(_flag,startHealth);
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
    protected override bool CanConnectTarget(EntityCharacterBase target) => base.CanConnectTarget(target) && target.m_Flag == m_Flag;

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

        m_Connections.m_ItemDic.Traversal((EntityCharacterBase entity) =>
        {
            switch (entity.m_Controller)
            {
                case enum_EntityController.AI:
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, OnApplyAlly()));
                    break;
                case enum_EntityController.Player:
                    entity.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, OnApplyPlayer()));
                    break;
            }
        });
    }
}
