using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class GameEnermyCommander : MonoBehaviour {
    EntityCharacterAI m_Entity;
    Transform m_LocalPlayer;
    int m_EntityID;
    float m_EntityHealth=0;

    public void Play(int entityID,Transform localPlayer)
    {
        m_EntityID = entityID;
        m_LocalPlayer = localPlayer;
    }

    TimeCounter m_Timer=new TimeCounter(GameConst.F_AIMovementCheckParam);
    private void Update()
    {
        if (m_Timer.m_Timing)
        {
            m_Timer.Tick(Time.deltaTime);
            return;
        }

        if (m_Entity&&m_Entity.m_IsDead)
            return;

        if (m_Entity)
        {
            if (!m_Entity.m_Idling)
                return;

            if (TCommon.GetXZDistance(transform.position, m_LocalPlayer.position) < GameConst.F_AIShowDistance)
                return;

            m_Entity.DoItemRecycle();
            m_Entity = null;
        }
        else
        {
            if (TCommon.GetXZDistance(transform.position, m_LocalPlayer.position) > GameConst.F_AIShowDistance)
                return;

            m_Entity = GameObjectManager.SpawnEntityCharacter(m_EntityID, NavigationManager.NavMeshPosition(transform.position), transform.forward, enum_EntityFlag.Enermy,-1, m_EntityHealth) as EntityCharacterAI;
            if (m_EntityHealth == 0)
            {
                m_Entity.SetExtraDifficulty(GameExpression.GetAIBaseHealthMultiplier(GameManager.Instance.m_GameLevel.m_GameDifficulty), GameExpression.GetAIMaxHealthMultiplier(GameManager.Instance.m_GameLevel.m_GameStage), GameExpression.GetEnermyGameDifficultyBuffIndex(GameManager.Instance.m_GameLevel.m_GameDifficulty));
                m_EntityHealth = m_Entity.m_Health.F_TotalEHP;
            }
        }
    }



}
