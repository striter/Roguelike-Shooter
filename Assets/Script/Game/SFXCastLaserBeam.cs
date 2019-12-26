using GameSetting;
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
        m_Beam.SetPosition(0, Vector3.zero);
        m_Beam.SetPosition(1, Vector3.zero);
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        BeamCheck();
        m_Beam.enabled = true;
        m_Impact.Play();
    }
    protected override void OnStop()
    {
        base.OnStop();
        m_Beam.enabled = false;
        m_Impact.Stop();
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Playing)
            return;
        BeamCheck();
    }

    void BeamCheck()
    {
        f_castLength = V4_CastInfo.z;
        Vector3 hitPoint = Vector3.zero;
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_StaticEntity);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!GameManager.B_CanSFXHitTarget(hits[i].collider.Detect(), I_SourceID))
                continue;

            Vector3 offsetPoint = hits[i].point;
            if (offsetPoint == Vector3.zero) offsetPoint = CastTransform.position;      //Cast Item At The Start Of The Sweep;

            float lengthOffset = TCommon.GetXZDistance(CastTransform.position, offsetPoint) + (E_AreaType == enum_CastAreaType.ForwardCapsule ? V4_CastInfo.x : 0);
            if (lengthOffset >= f_castLength)
                continue;

            f_castLength = lengthOffset;
            hitPoint = CastTransform.position + CastTransform.forward * f_castLength;
        }
        bool hitted = hitPoint != Vector3.zero;
        m_Impact.SetActive(hitted);
        m_Impact.transform.position = hitPoint;
        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, hitted ? hitPoint : CastTransform.position + CastTransform.forward * f_castLength);
    }
}
