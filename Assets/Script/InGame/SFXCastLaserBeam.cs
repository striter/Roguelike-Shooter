﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class SFXCastLaserBeam : SFXCast {
    LineRenderer m_Beam;
    Transform m_Muzzle, m_Impact;
    ParticleSystem[] m_Muzzles, m_Impacts;
    float f_castLength=0;
    protected override float F_CastLength => f_castLength <= 0 ? V3_CastSize.z : f_castLength;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Beam = GetComponent<LineRenderer>();
        m_Muzzle = transform.Find("Muzzle");
        m_Impact = transform.Find("Impact");
        m_Muzzles = m_Muzzle.GetComponentsInChildren<ParticleSystem>();
        m_Impacts = m_Impact.GetComponentsInChildren<ParticleSystem>();
        m_Muzzles.Traversal((ParticleSystem particle) => { particle.Stop(); });
        m_Impacts.Traversal((ParticleSystem particle) => { particle.Stop(); });
    }
    public override void PlayDelayed(int sourceID,DamageBuffInfo buffInfo)
    {
        base.Play(sourceID, buffInfo);
        m_Muzzles.Traversal((ParticleSystem particle) => { particle.Play(); });
        m_Impacts.Traversal((ParticleSystem particle) => { particle.Play(); });
        
    }
    protected override void Update()
    {
        base.Update();
        m_Beam.enabled = B_Casting;
        if (!B_Casting)
            return;
        f_castLength = V3_CastSize.z;
        RaycastHit hit;
        bool hitted = false ;
        if (Physics.BoxCast(transform.position, new Vector3(V3_CastSize.x / 2, V3_CastSize.y / 2, .01f), transform.forward, out hit, Quaternion.LookRotation(transform.forward, transform.up), f_castLength, GameLayer.Physics.I_StaticEntity))
        {
            f_castLength = TCommon.GetXZDistance(transform.position, hit.point)+.2f;
            hitted = true; 
        }

        Vector3 targetPoint = transform.position + transform.forward * f_castLength;
        m_Impact.SetActivate(hitted);
        m_Impact.transform.position = targetPoint;
        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, targetPoint);
    }
}
