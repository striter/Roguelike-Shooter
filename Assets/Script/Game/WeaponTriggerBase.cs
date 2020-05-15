using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponTriggerBase : MonoBehaviour
{
    public float F_FireRate = .1f;
    public virtual enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Invalid;
    public bool m_TriggerDown { get; protected set; }
    protected WeaponBase m_Weapon { get; private set; }
    protected TimerBase m_TriggerTimer { get; private set; } 
    protected void Init(WeaponBase weapon)
    {
        m_Weapon = weapon;
        m_TriggerDown = false;
        m_TriggerTimer = new TimerBase(F_FireRate,true);
    }
    public virtual void OnSetTrigger(bool down)
    {
        m_TriggerDown = down;
    }

    public virtual void OnTriggerStop()=>  m_TriggerDown = false;

    public virtual void Tick(bool paused, float deltaTime)=>m_TriggerTimer.Tick(deltaTime);
}
