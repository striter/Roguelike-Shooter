﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterBattleForestExtra : EntityCharacterBattle {

    [Range(0, 90)]
    public int I_SpreadAngleEach = 30;
    public float F_SpreadDuration = .5f;
    public int I_SpreadCount = 10;
    int i_spreadCountCheck = 0;
    float f_spreadCheck = 0;
    public override Transform tf_Weapon => tf_Head;
    CharacterWeaponHelperBase m_Weapon;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_Weapon = CharacterWeaponHelperBase.AcquireCharacterWeaponHelper(GameExpression.GetAIWeaponIndex(_identity), this,F_Spread);
    }

    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        f_spreadCheck = F_SpreadDuration;
        i_spreadCountCheck = 0;
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        transform.Translate(transform.forward * F_MovementSpeed*deltaTime,Space.World);

        f_spreadCheck -= Time.deltaTime;
        if (f_spreadCheck > 0)
            return;
        f_spreadCheck = F_SpreadDuration;

        Vector3 splitDirection = transform.forward.RotateDirectionClockwise(Vector3.up, i_spreadCountCheck * I_SpreadAngleEach);
        m_Weapon.OnPlay(null, transform.position + splitDirection * 20, m_CharacterInfo.GetDamageInfo(F_BaseDamage));
        i_spreadCountCheck++;
        if (i_spreadCountCheck > I_SpreadCount)
            OnDead();
    }

}
