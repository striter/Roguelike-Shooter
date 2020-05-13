using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXCastLaserBeam : SFXCast {
    #region Preset
    public int I_IndicatorIndex;
    #endregion
    SFXIndicator m_HitIndicator;
    LineRenderer m_Beam;
    float f_castLength;
    protected override float F_CastLength => f_castLength;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Beam = transform.Find("Connections").GetComponent<LineRenderer>();
        m_Beam.SetPosition(0, Vector3.zero);
        m_Beam.SetPosition(1, Vector3.zero);
    }
    public override void PlayControlled(int sourceID, EntityCharacterBase entity, Transform directionTrans)
    {
        base.PlayControlled(sourceID, entity, directionTrans);
        BeamCheck();
    }

    protected override void OnPlay()
    {
        base.OnPlay();
        m_Beam.enabled = true;
    }
    protected override void OnStop()
    {
        base.OnStop();
        m_Beam.enabled = false;
        SetImpact(false, Vector3.zero, Vector3.zero);
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
        Vector3 hitNormal = Vector3.zero;
        RaycastHit[] hits = OnCastCheck(GameLayer.Mask.I_ProjectileMask);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!GameManager.B_CanSFXHitTarget(hits[i].collider.Detect(), m_SourceID))
                continue;

            Vector3 offsetPoint = hits[i].point;
            hitNormal = hits[i].normal;
            if (offsetPoint == Vector3.zero) offsetPoint = CastTransform.position;      //Cast Item At The Start Of The Sweep;

            float lengthOffset = TCommon.GetXZDistance(CastTransform.position, offsetPoint);
            if (E_AreaType == enum_CastAreaType.ForwardCapsule)
                lengthOffset += V4_CastInfo.x/2;

            if (lengthOffset >= f_castLength)
                continue;

            f_castLength = lengthOffset;
            hitPoint = CastTransform.position + CastTransform.forward * f_castLength;
        }

        bool hitted = hitPoint != Vector3.zero;
        SetImpact(hitted,hitPoint,hitNormal);
        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, hitted ? hitPoint : CastTransform.position + CastTransform.forward * f_castLength);
    }

    void SetImpact(bool play,Vector3 position,Vector3 normal)
    {
        if(play)
        {
            if (!m_HitIndicator)
            {
                m_HitIndicator = GameObjectManager.SpawnIndicator(I_IndicatorIndex, transform.position, transform.forward);
                m_HitIndicator.PlayControlled(m_SourceID);
            }
            m_HitIndicator.transform.position = position;
            m_HitIndicator.transform.rotation = Quaternion.LookRotation(normal);
        }
        else
        {
            if(m_HitIndicator)
            {
                m_HitIndicator.Stop();
                m_HitIndicator = null;
            }
        }

    }
}
