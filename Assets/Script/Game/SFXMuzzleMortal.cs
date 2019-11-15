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
    public override void OnPoolItemInit(int identity)
    {
        base.OnPoolItemInit(identity);
        tf_Model = transform.Find("Model");
        m_trail = tf_Model.GetComponentInChildren<TrailRenderer>();
    }
    public override void Play(int sourceID, float duration = 0,float delayDuration=0)
    {
        base.Play(sourceID, F_PlayTime,delayDuration);
        tf_Model.position = transform.position;
        tf_Model.rotation = Quaternion.LookRotation(Vector3.up);
        m_trail.Clear();
    }
    protected override void Update()
    {
        base.Update();
        tf_Model.transform.position +=  Time.deltaTime * Vector3.up*F_Speed;
    }
}
