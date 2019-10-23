using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]
public class SFXCastLaserBeam : SFXCast {
    LineRenderer m_Beam;
    Transform m_Muzzle, m_Impact;
    ParticleSystem[] m_Muzzles, m_Impacts;
    float f_castLength;
    protected override float F_CastLength => f_castLength;
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
    protected override void OnPlay()
    {
        base.OnPlay();
        f_castLength = 0f;
        m_Muzzles.Traversal((ParticleSystem particle) => { particle.Play(); });
        m_Impacts.Traversal((ParticleSystem particle) => { particle.Play(); });
    }

    protected override void Update()
    {
        base.Update();
        m_Beam.enabled = B_Playing;
        if (!B_Playing)
            return;

        f_castLength = V4_CastInfo.z;
        Vector3 hitPoint = Vector3.zero;
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_StaticEntity);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!GameManager.B_CanHitTarget(hits[i].collider.Detect(), m_sourceID))
                continue;

            float targetLength = TCommon.GetXZDistance(CastTransform.position, hits[i].point)+.2f;
            if (targetLength >= f_castLength)
                continue;

            f_castLength = targetLength;
            hitPoint = hits[i].point == Vector3.zero ? transform.position : hits[i].point;
        }
        bool hitted = hitPoint!=Vector3.zero;
        m_Impact.SetActivate(hitted);
        m_Impact.transform.position = hitPoint;
        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, hitted?hitPoint:CastTransform.position+CastTransform.forward* f_castLength);
    }
}
