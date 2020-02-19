using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractTeleport : InteractGameBase {
    protected override bool E_InteractOnEnable => false;
    Transform tf_Top;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_Top = transform.Find("Top");
    }
    public void SetPlay(bool play) => tf_Top.SetActivate(play);
    public new InteractTeleport Play()
    {
        base.Play();
        return this;
    }

}
