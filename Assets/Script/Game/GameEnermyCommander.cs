using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class GameEnermyCommander : CSimplePoolObjectMono<int> {
    public bool m_Playing { get; private set; } = false;
    public bool m_Battling { get; private set; } = false;
    EntityCharacterAI m_Entity;
    Transform m_LocalPlayer;
    int m_EntityID;
    float m_EntityHealth=0;
    int m_ChunkID;
    public void Play(int chunkID, int entityID,Transform localPlayer)
    {
        m_ChunkID = chunkID;
        m_EntityID = entityID;
        m_LocalPlayer = localPlayer;
        m_Playing = true;
        m_Battling = false;
    }

    public void OnCharacterDead(EntityCharacterBase character)
    {
        if (character.m_EntityID != m_Entity.m_EntityID)
            return;

        m_Playing = false;
    }

    public void DoBattle()
    {
        m_Battling = true;
        if (m_Entity)
            m_Entity.AIActivate(m_Battling);
    }
    
    TimeCounter m_Timer=new TimeCounter(GameConst.AI.F_AIMovementCheckParam);
    private void Update()
    {
        if (!m_Playing)
            return;

        if (m_Timer.m_Timing)
        {
            m_Timer.Tick(Time.deltaTime);
            return;
        }
        m_Timer.SetTimer(GameConst.AI.F_AIMovementCheckParam);

        bool entityVisible = TCommon.GetXZDistance(transform.position, m_LocalPlayer.position) < GameConst.AI.F_AIShowDistance;

        if (m_Entity)
        {
            if (!m_Entity.m_IdlePatrol|| entityVisible)
                return;

            m_EntityHealth = m_Entity.m_Health.F_TotalEHP;
            m_Entity.DoRecycle();
            m_Entity = null;
        }
        else
        {
            if (!entityVisible)
                return;

            m_Entity = GameObjectManager.SpawnEntityCharactterAI(m_EntityID, NavigationManager.NavMeshPosition(transform.position), transform.forward, enum_EntityFlag.Enermy, GameManager.Instance.m_GameLevel.m_GameDifficulty, GameManager.Instance.m_GameLevel.m_GameStage,m_Battling, -1, m_EntityHealth) as EntityCharacterAI;
        }
    }
}
