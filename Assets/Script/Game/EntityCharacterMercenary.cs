using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityCharacterMercenary : EntityCharacterBase {
    public enum_MercenaryCharacter m_Character;
    public WeaponBase m_Weapon { get; private set; }
    

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
    }


}
