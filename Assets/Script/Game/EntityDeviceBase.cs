using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityDeviceBase : EntityCharacterBase {
    public override enum_EntityController m_Controller => enum_EntityController.Device; 
    EntityDetector m_Detect;
    ParticleSystem[] m_Particles;
    public ObjectPoolSimple<EntityCharacterBase, LineRenderer> m_Connections { get; private set; }
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Detect = transform.Find("EntityDetector").GetComponent<EntityDetector>();
        m_Detect.Init(OnEntityDetect);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        Transform connectionsParent = transform.Find("Connections");
        m_Connections = new ObjectPoolSimple<EntityCharacterBase, LineRenderer>(connectionsParent.Find("Item").gameObject, connectionsParent,(Transform transform,EntityCharacterBase identity)=>transform.GetComponent<LineRenderer>(),(LineRenderer target)=>target.transform);
    }
    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    public override void OnActivate(enum_EntityFlag _flag, int _spawnerID = -1, float startHealth = 0)
    {
        base.OnActivate(_flag,_spawnerID, startHealth);
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
        m_Connections.ClearPool();
        m_Particles.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }
    void OnEntityDetect(HitCheckEntity entity, bool enter)
    {
        if (m_IsDead)
            return;

        switch (entity.m_Attacher.m_Controller)
        {
            default:break;
            case enum_EntityController.Player:
            case enum_EntityController.AI:
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
