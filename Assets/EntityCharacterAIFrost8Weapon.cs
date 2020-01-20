using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacterAIFrost8Weapon : EntityCharacterAI
{
    ModelBlink m_Blink;
    float timeElapsed;
    bool b_selfDetonating;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Blink = new ModelBlink(tf_Model.Find("BlinkModel"), .25f, .25f, Color.red);
    }
    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID, float startHealth)
    {
        base.OnActivate(_flag, _spawnerID, startHealth);
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
            OnRecycle();
        }
    }
}
