using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class GameEnermyCommander : CSimplePoolObjectMono<int> {
    public bool m_Playing { get; private set; } = false;
    public bool m_Battling { get; private set; } = false;
    public bool m_EntityShowing => m_Entity;
    public float m_EntityHealthScale => m_EntityShowing ? m_Entity.m_Health.F_HealthMaxScale : -1f;
    int m_EntityID;
    EntityCharacterAI m_Entity;
    float m_EntityHealth=0;
    Transform m_LocalPlayer;
    public void Play(int entityID,Transform localPlayer)
    {
        m_EntityID = entityID;
        m_LocalPlayer = localPlayer;
        m_Playing = true;
        m_Battling = false;
    }

    public bool OnCharacterDead(EntityCharacterBase character)
    {
        if (!m_Playing||!m_Entity||character.m_EntityID != m_Entity.m_EntityID)
            return false;

        m_Playing = false;
        return true;
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

            m_Entity = GameObjectManager.SpawnEntityCharacterAI(m_EntityID, NavigationManager.NavMeshPosition(transform.position), transform.forward, enum_EntityFlag.Enermy, GameManager.Instance.m_GameLevel.m_GameDifficulty, GameManager.Instance.m_GameLevel.m_GameStage,m_Battling, -1, m_EntityHealth) as EntityCharacterAI;
        }
    }
}
