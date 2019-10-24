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
        GameObjectManager.PlayMuzzle(buffInfo.I_SourceID,transform.position,Vector3.up,I_MuzzleIndex);
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
        if (!B_Delay)
            return;

        float timeMultiply = 2f * (1-f_delayTimeLeft / F_DelayDuration);
        if (timeMultiply < 0)
            return;
        m_Blink.Tick(Time.deltaTime * timeMultiply);
    }
}
