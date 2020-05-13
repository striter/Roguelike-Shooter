using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastDetonate : SFXCastOverlapSphere
{
    public int I_MuzzleIndex = 0;
    Transform m_Model;
    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_Model = transform.Find("Model");
    }
    public override void Play(DamageInfo damageInfo)
    {
        base.Play(damageInfo);
        m_Model.SetActivate(true);
        GameObjectManager.PlayMuzzle(damageInfo.m_EntityID,transform.position,Vector3.up,I_MuzzleIndex);
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Model.SetActivate(false);
    }
}
