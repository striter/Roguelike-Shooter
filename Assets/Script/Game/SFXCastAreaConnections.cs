﻿using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastAreaConnections : SFXCast {
    ObjectPoolSimpleClass<EntityBase, ConnectionsItem> m_Connections;
    class ConnectionsItem:CSimplePool<EntityBase>
    {
        Transform targetTrans;
        LineRenderer m_Renderer;
        TSpecialClasses.ParticleControlBase m_Particles;
        bool m_Play=>targetTrans!=null;
        public override void OnPoolInit(Transform _transform, EntityBase _identity)
        {
            base.OnPoolInit(_transform, _identity);
            m_Particles = new TSpecialClasses.ParticleControlBase(transform.Find("Particles"));
            m_Renderer = transform.GetComponent<LineRenderer>();
            SetTarget(null);
        }

        public void SetTarget(Transform target)
        {
            targetTrans = target;
            if (m_Play)
            {
                m_Renderer.enabled = true;
                m_Particles.Play();
                Tick(0f);
            }
            else
            {
                m_Renderer.enabled = false;
                m_Particles.Stop();
            }
        }

        public void Tick(float deltaTime)
        {
            m_Particles.transform.position = targetTrans.position;
            m_Renderer.SetPosition(0, transform.position);
            m_Renderer.SetPosition(1, targetTrans.position);
        }
    }
    TSpecialClasses.ParticleControlBase m_GroundParticles;
    Transform tf_GroundAttach;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        Transform connections = transform.Find("Connections");
        m_Connections = new ObjectPoolSimpleClass<EntityBase, ConnectionsItem>(connections,"Item");
        m_GroundParticles = new TSpecialClasses.ParticleControlBase(transform.Find("ParticlesGround"));
    }
    public override void PlayControlled(int sourceID, EntityCharacterBase entity, Transform directionTrans,DamageDeliverInfo idInfo)
    {
        base.PlayControlled(sourceID, entity, directionTrans,idInfo);
        tf_GroundAttach = entity.transform;
        m_Particle.Clear();
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        m_GroundParticles.Play();
        m_Connections.ClearPool();
    }

    protected override void OnStop()
    {
        base.OnStop();
        m_GroundParticles.Stop();
        m_Connections.ClearPool();
    }
    protected override List<EntityBase> DoCastDealtDamage()
    {
        List<EntityBase> entityEffecting = base.DoCastDealtDamage();
        m_Connections.m_ActiveItemDic.Traversal((EntityBase key) => { if (!entityEffecting.Contains(key)) m_Connections.RemoveItem(key); }, true);
        entityEffecting.Traversal((EntityBase entity) => {
            if (m_Connections.ContainsItem(entity))
                return;

            EntityCharacterBase character = entity as EntityCharacterBase;
            m_Connections.AddItem(entity).SetTarget(character ? character.tf_Head : entity.transform);
        });
        return entityEffecting;
    }
    float check;
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;

        m_GroundParticles.transform.position = tf_GroundAttach.position;
        m_GroundParticles.transform.rotation = tf_GroundAttach.rotation;
        m_Connections.m_ActiveItemDic.Traversal((ConnectionsItem item)=>item.Tick(Time.deltaTime));
    }
}
