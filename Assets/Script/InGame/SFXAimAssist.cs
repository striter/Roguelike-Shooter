using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TPhysics;
using GameSetting;
public class SFXAimAssist : SFXBase {
    Transform tf_Dot;
    Transform tf_check, tf_muzzle;
    LineRenderer m_lineRenderer;
    public Vector3 m_assistTarget { get; private set; } = Vector3.zero;
    float m_assistDistance;
    int m_castMask;
    Func<Collider, bool> CanHitCollider;
    protected override bool m_AutoRecycle => false;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_lineRenderer = transform.GetComponentInChildren<LineRenderer>();
        m_lineRenderer.positionCount = 2;
        tf_Dot = transform.Find("Dot");
    }
    public void Play(int sourceID, Transform muzzle, Transform check, float distance, int mask, Func<Collider, bool> _CanHitCollider)
    {
        tf_check = check;
        tf_muzzle = muzzle;
        m_assistDistance = distance;
        m_castMask = mask;
        CanHitCollider = _CanHitCollider;
        m_lineRenderer.enabled = true;
        base.PlaySFX(sourceID,-1);
    }

    protected override void Update()
    {
        base.Update();
        if (!m_lineRenderer.enabled)
            return;

        tf_Dot.SetActivate(false);
        RaycastHit hit;
        m_assistTarget = tf_check.position + tf_check.forward * m_assistDistance;
        if (Physics.Raycast(tf_check.position, tf_check.forward, out hit, m_assistDistance, m_castMask) && CanHitCollider(hit.collider))
        {
            m_assistTarget = hit.point;
            tf_Dot.position = hit.point;
            tf_Dot.SetActivate(true);
        }
        m_lineRenderer.SetPosition(1, tf_muzzle.position);
        m_lineRenderer.SetPosition(0, m_assistTarget);
    }

    public void SetEnable(bool activate)
    {
        m_lineRenderer.enabled = activate;
        tf_Dot.SetActivate(activate);
    }
}
