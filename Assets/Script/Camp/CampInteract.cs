using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameSetting;

public class CampInteract : InteractBase {
    protected override bool B_RecycleOnInteract => false;
    private void Start()
    {
        Init();
        base.Play();
    }
}
