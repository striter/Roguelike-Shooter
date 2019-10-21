using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastDetonate : SFXCastOverlapSphere {
    ModelBlink m_Blink;
    Transform m_Model;
    public Color c_blinkColor;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Blink = new ModelBlink(transform.Find("BlinkModel"), .25f, .25f, c_blinkColor);
        m_Model = transform.Find("Model");
    }
    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        m_Blink.OnReset();
        m_Model.SetActivate(true);
        if (I_MuzzleIndex > 0)
            GameObjectManager.SpawnParticles<SFXMuzzle>(I_MuzzleIndex, transform.position, Vector3.up).Play(buffInfo.I_SourceID);
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Blink.SetShow(false);
        m_Model.SetActivate(false);
    }
    protected override void Update()
    {
        base.Update();
        if (!B_Delaying)
            return;
        float timeMultiply = 2f * (1-f_delayTimeLeft / F_DelayDuration);
        m_Blink.Tick(Time.deltaTime * timeMultiply);
    }
}
