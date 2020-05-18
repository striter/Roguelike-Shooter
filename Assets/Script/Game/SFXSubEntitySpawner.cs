using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class SFXSubEntitySpawner : SFXWeaponBase {
    public bool B_SpawnAtTarget = false;
    public int I_EntitySpawnID;
    public int I_DelayIndicator = 0;
    public float F_DelayDuration = 0;
    EntityCharacterBase m_Spawner;
    Vector3 m_targetPos;
    public void Play(EntityCharacterBase character, Vector3 target)
    {
        base.PlaySFX(character.m_EntityID,1f,F_DelayDuration,true);
        m_Spawner = character;
        m_targetPos = target;
        if (I_DelayIndicator > 0)
            GameObjectManager.SpawnIndicator(I_DelayIndicator, transform.position, Vector3.up).PlayUncontrolled(character.m_EntityID,F_DelayDuration);
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        GameManager.Instance.m_GameBattle.GenerateGameCharacter(I_EntitySpawnID, m_Spawner.m_Flag, m_targetPos, false, m_Spawner.m_EntityID);
    }

}
