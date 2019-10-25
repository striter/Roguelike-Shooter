using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastDetonate : SFXCastOverlapSphere {
    Transform m_Model;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Model = transform.Find("Model");
    }
    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        m_Model.SetActivate(true);
        GameObjectManager.PlayMuzzle(buffInfo.I_SourceID,transform.position,Vector3.up,I_MuzzleIndex);
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Model.SetActivate(false);
    }
}
