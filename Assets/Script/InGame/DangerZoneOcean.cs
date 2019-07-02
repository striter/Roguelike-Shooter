using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

[RequireComponent(typeof(Collider))]
public class DangerZoneOcean : DangerZoneBase {
    protected override void OnHitCheckEntity(HitCheckEntity entity,bool enter)
    {
        base.OnHitCheckEntity(entity,enter);
        if(enter)
        GameManager.Instance.OnEntityFall(entity);
    }
}
