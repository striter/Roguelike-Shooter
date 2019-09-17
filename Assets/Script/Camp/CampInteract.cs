using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSetting;

public class CampInteract : InteractBase {
    public override bool B_InteractOnce => true;
    protected override bool B_RecycleOnInteract => false;
    private void Awake()
    {
        Play();
    }
}
