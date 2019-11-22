﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXCastLaserBeam : SFXCast {
    LineRenderer m_Beam;
    TSpecialClasses.ParticleControlBase m_Impact;
    float f_castLength;
    protected override float F_CastLength => f_castLength;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Beam = transform.Find("Connections").GetComponent<LineRenderer>();
        m_Impact =new TSpecialClasses.ParticleControlBase( transform.Find("Impact"));
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        f_castLength = 0f;
        m_Impact.Play();
    }
    protected override void OnStop()
    {
        base.OnStop();
        m_Impact.Stop();
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
            if (!GameManager.B_CanSFXHitTarget(hits[i].collider.Detect(), m_sourceID))
                continue;

            float targetLength = TCommon.GetXZDistance(CastTransform.position, hits[i].point)+.2f;
            if (targetLength >= f_castLength)
                continue;

            f_castLength = targetLength;
            hitPoint = hits[i].point == Vector3.zero ? transform.position : hits[i].point;
        }
        bool hitted = hitPoint!=Vector3.zero;
        m_Impact.SetActive(hitted);
        m_Impact.transform.position = hitPoint;
        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, hitted?hitPoint:CastTransform.position+CastTransform.forward* f_castLength);
    }
}
