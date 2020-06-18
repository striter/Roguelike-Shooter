using System;
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
    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        tf_Model = transform.Find("Model");
        m_trail = tf_Model.GetComponentInChildren<TrailRenderer>();
    }

    protected override void Play()
    {
        base.Play();
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
