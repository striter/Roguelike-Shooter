﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractPickup : InteractBase {
    public override bool B_InteractOnTrigger => true;
    protected override bool B_RecycleOnInteract => true;
}