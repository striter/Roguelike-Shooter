using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class SFXCastDetonate : SFXCastOverlapSphere {
    ModelBlink m_Blink;
    Transform m_Model;
    public Color c_blinkColor;
    float f_blinkCheck;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Blink = new ModelBlink(transform.Find("BlinkModel"), .25f, .25f, c_blinkColor);
        m_Model = transform.Find("Model");
    }
    public override void Play(DamageDeliverInfo buffInfo)
    {
        base.Play(buffInfo);
        f_blinkCheck = F_DelayDuration;
        m_Blink.OnReset();
        m_Model.SetActivate(true);
        if (I_MuzzleIndex > 0)
            GameObjectManager.SpawnParticles<SFXMuzzle>(I_MuzzleIndex, transform.position, Vector3.up).Play(buffInfo.I_SourceID);
    }
    public override void PlayDelayed()
    {
        base.PlayDelayed();
        m_Blink.SetShow(false);
        m_Model.SetActivate(false);
    }
    protected override void Update()
    {
        base.Update();
        if (f_blinkCheck < 0f)
            return;
        f_blinkCheck -= Time.deltaTime;
        float timeMultiply = 2f * (1 - f_blinkCheck / F_DelayDuration);
        m_Blink.Tick(Time.deltaTime * timeMultiply);
        if (f_blinkCheck < 0f)
            m_Blink.SetShow(false);
    }
}
