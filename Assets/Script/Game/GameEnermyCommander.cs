using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class GameEnermyCommander : MonoBehaviour {
    EntityCharacterAI m_Entity;
    Transform m_LocalPlayer;
    int m_PrefabID;
    float m_EntityHealth=0;

    int m_PoolID;
    Action<int> DoPoolRecycle;
    public void Play(int poolID,int prefabID,Transform localPlayer,Action<int> DoRecycle)
    {
        m_PoolID = poolID;
        DoPoolRecycle = DoRecycle;
        m_PrefabID = prefabID;
        m_LocalPlayer = localPlayer;
    }

    TimeCounter m_Timer=new TimeCounter(GameConst.AI.F_AIMovementCheckParam);
    private void Update()
    {
        if (m_Timer.m_Timing)
        {
            m_Timer.Tick(Time.deltaTime);
            return;
        }
        m_Timer.SetTimer(GameConst.AI.F_AIMovementCheckParam);

        if (m_Entity && m_Entity.m_IsDead)
        {
            DoPoolRecycle(m_PoolID);
            return;
        }

        bool entityVisible = TCommon.GetXZDistance(transform.position, m_LocalPlayer.position) < GameConst.AI.F_AIShowDistance;

        if (m_Entity)
        {
            if (!m_Entity.m_IdlePatrol|| entityVisible)
                return;

            m_Entity.DoRecycle();
            m_Entity = null;
        }
        else
        {
            if (!entityVisible)
                return;

            m_Entity = GameObjectManager.SpawnEntityCharacter(m_PrefabID, NavigationManager.NavMeshPosition(transform.position), transform.forward, enum_EntityFlag.Enermy,-1, m_EntityHealth) as EntityCharacterAI;
            if (m_EntityHealth == 0)
            {
                m_Entity.SetExtraDifficulty(GameExpression.GetAIBaseHealthMultiplier(GameManager.Instance.m_GameLevel.m_GameDifficulty), GameExpression.GetAIMaxHealthMultiplier(GameManager.Instance.m_GameLevel.m_GameStage), GameExpression.GetEnermyGameDifficultyBuffIndex(GameManager.Instance.m_GameLevel.m_GameDifficulty));
                m_EntityHealth = m_Entity.m_Health.F_TotalEHP;
            }
        }
    }
}
