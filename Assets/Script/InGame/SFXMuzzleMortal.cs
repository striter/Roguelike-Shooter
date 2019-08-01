using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXMuzzleMortal : SFXMuzzle
{
    public float F_Speed = 10;
    public float F_PlayTime = 5f;
    Transform tf_Model;
    TrailRenderer m_trail;
    Vector3 v3_endPos;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        tf_Model = transform.Find("Model");
        m_trail = tf_Model.GetComponentInChildren<TrailRenderer>();
    }
    public override void Play(int sourceID, float duration = -1)
    {
        F_PlayTime = F_PlayTime > m_ParticleDuration ? F_PlayTime : m_ParticleDuration;
        base.Play(sourceID, F_PlayTime);
        tf_Model.position = transform.position;
        v3_endPos = transform.position + Vector3.up * F_Speed * F_PlayTime;
        m_trail.Clear();
    }
    protected override void Update()
    {
        base.Update();
        tf_Model.transform.position = Vector3.Lerp(transform.position,v3_endPos,(1-(f_TimeCheck-Time.time)/F_PlayTime));
        tf_Model.rotation = Quaternion.LookRotation(Vector3.up);
    }
}
