using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampEnvironment : SimpleSingletonMono<CampEnvironment> {
    public Transform tf_Interactions { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        tf_Interactions = transform.Find("Interactions");
    }
}
