using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class WeaponTriggerBase : MonoBehaviour
{
    public virtual enum_PlayerWeaponTriggerType m_Type => enum_PlayerWeaponTriggerType.Invalid;
    public bool m_TriggerDown { get; protected set; }
    protected WeaponBase m_Weapon { get; private set; }
    public virtual bool m_Triggering => false;
    protected void Init(WeaponBase weapon)
    {
        m_Weapon = weapon;
        m_TriggerDown = false;
    }
    public void OnSetTrigger(bool down)=> m_TriggerDown = down;

    public virtual void Stop()=>  m_TriggerDown = false;
}
