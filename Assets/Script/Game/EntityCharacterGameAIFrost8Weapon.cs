using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterGameAIFrost8Weapon : EntityCharacterGameAI
{
    ModelBlink m_Blink;
    float timeElapsed;
    bool b_selfDetonating;
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_Blink = new ModelBlink(tf_Model.Find("BlinkModel"), .25f, .25f, Color.red);
    }
    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        m_Blink.OnReset();
        b_selfDetonating = false;
        timeElapsed = 0;
    }

    protected override void OnAttackAnimTrigger()
    {
        b_selfDetonating = true;
    }
    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        if (!b_selfDetonating)
            return;
        timeElapsed += deltaTime;
        float timeMultiply = 2f * (timeElapsed / 2f);
        m_Blink.Tick(Time.deltaTime * timeMultiply);
        if (timeElapsed > 2f)
        {
            m_Weapon.OnPlay(false, m_Target);
            b_selfDetonating = false;
            OnDead();
        }
    }
}
