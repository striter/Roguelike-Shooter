﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityDeviceBuffApllier : EntityDeviceBase {
    SBuff m_PlayerBuff, m_AllyBuff;
    float f_refreshCheck;
    float f_refreshDuration;
    public void SetBuffApply(SBuff applyPlayer,SBuff applyAlly,float refreshDuration=.8f)
    {
        m_PlayerBuff = applyPlayer;
        m_AllyBuff = applyAlly;
        f_refreshDuration = refreshDuration;
        f_refreshCheck = 0f;
    }
    protected override bool CanConnectTarget(EntityCharacterBase target) => base.CanConnectTarget(target) && target.m_Flag == m_Flag;

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (f_refreshCheck > 0)
        {
            f_refreshCheck -= Time.deltaTime;
            return;
        }
        f_refreshCheck = f_refreshDuration;

        m_Connections.m_ActiveItemDic.Traversal((EntityCharacterBase entity) =>
        {
            switch (entity.m_ControllType)
            {
                case enum_EntityType.BattleEntity:
                    entity.m_HitCheck.TryHit(new DamageInfo(m_EntityID).AddBuffInfo(m_AllyBuff));
                    break;
                case enum_EntityType.Player:
                    entity.m_HitCheck.TryHit(new DamageInfo(m_EntityID).AddBuffInfo(m_PlayerBuff));
                    break;
            }
        });
    }
}
