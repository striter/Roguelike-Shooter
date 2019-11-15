using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityDeviceBase : EntityCharacterBase {
    public override enum_EntityController m_Controller => enum_EntityController.Device; 
    EntityDetector m_Detect;
    ParticleSystem[] m_Particles;
    public ObjectPoolMono<EntityCharacterBase, LineRenderer> m_Connections { get; private set; }
    public override void Init(int _poolIndex)
    {
        base.Init(_poolIndex);
        m_Detect = transform.Find("EntityDetector").GetComponent<EntityDetector>();
        m_Detect.Init(OnEntityDetect);
        m_Particles = GetComponentsInChildren<ParticleSystem>();
        Transform connectionsParent = transform.Find("Connections");
        m_Connections = new ObjectPoolMono<EntityCharacterBase, LineRenderer>(connectionsParent.Find("Item").gameObject, connectionsParent);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnCharacterDead, OnCharacterDead);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
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

    protected override void OnCharacterUpdate(float deltaTime)
    {
        base.OnCharacterUpdate(deltaTime);
        m_Connections.m_ItemDic.Traversal((EntityCharacterBase target,LineRenderer item)=>
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
        if (m_Health.b_IsDead)
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
