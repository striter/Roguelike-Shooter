using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBullet : SFXBase
{
    LineRenderer m_line;
    float f_startTime,f_bulletDuration;
   
    Vector3 v3_origin, v3_destionation;
    protected override void Awake()
    {
        base.Awake();
        m_line = GetComponent<LineRenderer>();
    }
    public void Play(Vector3 origin, Vector3 destination,float speed,float duration=GameSettings.CI_BulletTrailLifeTime)
    {
        f_duration = duration;
        f_bulletDuration = Vector3.Distance(origin, destination) / speed;
        base.Play();
        transform.position = origin;
        v3_origin = origin;
        v3_destionation = destination;
        f_startTime = Time.time;
        m_line.SetPosition(0,origin);
        m_line.enabled = false;
        m_line.material.SetFloat("_Process", 0f);
    }
    protected override void OnTickDelta(float delta)
    {
        base.OnTickDelta(delta);
        m_line.enabled = true;
        float timeParamBulletHead = (Time.time - f_startTime)/f_bulletDuration;
        transform.position = Vector3.Lerp(v3_origin, v3_destionation, timeParamBulletHead);
        m_line.SetPosition(1, transform.position);
        float timeParamSmokeTrail = (Time.time - f_startTime)  / f_duration;
        m_line.material.SetFloat("_Process", timeParamSmokeTrail);
    }
}
