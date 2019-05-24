using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;

[RequireComponent(typeof(Collider))]
public class DangerZoneOcean : DangerZoneBase {
    protected override void OnHitCheckEntity(HitCheckEntity entity)
    {
        base.OnHitCheckEntity(entity);
        if(b_IsTriggerEnter)
        GameManager.Instance.OnEntityFall(entity);
    }
}
