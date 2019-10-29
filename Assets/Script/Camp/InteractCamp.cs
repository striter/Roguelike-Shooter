using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSetting;

public class InteractCamp : InteractBase {
    public override bool B_InteractOnce => false;
    private void Start()
    {
        Init();
        base.Play();
    }
}
