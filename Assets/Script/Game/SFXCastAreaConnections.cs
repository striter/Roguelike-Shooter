using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastAreaConnections : SFXCast {
    ObjectPoolSimple<EntityBase, ConnectionsItem> m_Connections;
    class ConnectionsItem
    {
        public Transform transform { get; private set; }
        Transform targetTrans;
        LineRenderer m_Renderer;
        TSpecialClasses.ParticleControlBase m_Particles;
        bool m_Play=>targetTrans!=null;
        public ConnectionsItem(Transform _transform)
        {
            transform = _transform;
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
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        Transform connections = transform.Find("Connections");
        m_Connections = new ObjectPoolSimple<EntityBase, ConnectionsItem>(connections.Find("Item").gameObject,connections,(Transform trans, EntityBase entity)=>new ConnectionsItem(trans),(ConnectionsItem item)=>item.transform);
        m_GroundParticles = new TSpecialClasses.ParticleControlBase(transform.Find("ParticlesGround"));
    }
    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
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
    float check;
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;
        
        m_Connections.m_ActiveItemDic.Traversal((ConnectionsItem item)=>item.Tick(Time.deltaTime));

        if (check>0)
        {
            check -= Time.deltaTime;
            return;
        }
        check = .1f;

        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_Entity);
        List<EntityBase> entityEffecting = new List<EntityBase>();
        for (int i = 0; i < hits.Length; i++)
        {
            HitCheckEntity entity = hits[i].collider.DetectEntity();
            if (!GameManager.B_CanSFXHitTarget(entity, m_sourceID))
                continue;
            entityEffecting.Add(entity.m_Attacher);
        }

        m_Connections.m_ActiveItemDic.Traversal((EntityBase key) => { if (!entityEffecting.Contains(key)) m_Connections.RemoveItem(key); }, true);
        entityEffecting.Traversal((EntityBase entity) => {
            if (m_Connections.ContainsItem(entity))
                return;

            EntityCharacterBase character = entity as EntityCharacterBase;
            m_Connections.AddItem(entity).SetTarget(character? character.tf_Head:entity.transform);
        });
    }
}
