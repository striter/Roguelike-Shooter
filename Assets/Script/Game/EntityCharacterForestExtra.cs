﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterForestExtra : EntityCharacterBase {

    [Range(0, 90)]
    public int I_SpreadAngleEach = 30;
    public float F_SpreadDuration = .5f;
    public int I_SpreadCount = 10;
    int i_spreadCountCheck = 0;
    float f_spreadCheck = 0;
    public override Transform tf_Weapon => tf_Head;
    WeaponHelperBase equipment;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        equipment = WeaponHelperBase.AcquireWeaponHelper(GameExpression.GetAIWeaponIndex(_identity),this,m_CharacterInfo.GetDamageBuffInfo);
    }
    protected override void OnActivate(enum_EntityFlag flag, float startHealth = 0)
    {
        base.OnActivate(flag, startHealth);
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

        Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i_spreadCountCheck * I_SpreadAngleEach);
        equipment.OnPlay(null, transform.position + splitDirection * 20);
        i_spreadCountCheck++;
        if (i_spreadCountCheck > I_SpreadCount)
            OnDead();
    }

}
