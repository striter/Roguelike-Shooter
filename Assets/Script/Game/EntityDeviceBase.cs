using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityDeviceBase : EntityCharacterBase {
    public override enum_EntityType m_ControllType => enum_EntityType.GameDevice; 
    EntityDetector m_Detect;
    ParticleSystem[] m_Particles;
    public ObjectPoolListComponent<EntityCharacterBase, LineRenderer> m_Connections { get; private set; }
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        m_Detect = transform.Find("EntityDetector").GetComponent<EntityDetector>();
        m_Detect.Init(OnEntityDetect);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        Transform connectionsParent = transform.Find("Connections");
        m_Connections = new ObjectPoolListComponent<EntityCharacterBase, LineRenderer>(connectionsParent,"Item");
    }
    public override void OnPoolSpawn()
    {
        base.OnPoolSpawn();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    protected override void OnEntityActivate(enum_EntityFlag flag)
    {
        base.OnEntityActivate(flag);
        m_Particles.Traversal((ParticleSystem particle) => { particle.Play(); });
    }
    void OnCharacterDead(EntityCharacterBase character)
    {
        if (m_Connections.ContainsItem(character))
            m_Connections.RemoveItem(character);
    }

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        m_Connections.m_ActiveItemDic.Traversal((EntityCharacterBase target,LineRenderer item)=>
        {
            item.SetPosition(0,tf_Head.position);
            item.SetPosition(1, target.tf_Head.position);
        });
    }
    protected override void OnDead()
    {
        base.OnDead();
        m_Connections.Clear();
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }
    void OnEntityDetect(HitCheckEntity entity, bool enter)
    {
        if (m_IsDead)
            return;

        switch (entity.m_Attacher.m_ControllType)
        {
            default:break;
            case enum_EntityType.Player:
            case enum_EntityType.GameEntity:
                {
                    EntityCharacterBase target = entity.m_Attacher as EntityCharacterBase;
                    if (!CanConnectTarget(target))
                        return;

                    if (enter)
                        m_Connections.AddItem(target);
                    else
                        m_Connections.RemoveItem(target);
                } 
                break;
        }
    }
    protected virtual bool CanConnectTarget(EntityCharacterBase target) => target.m_EntityID != m_EntityID;
}
