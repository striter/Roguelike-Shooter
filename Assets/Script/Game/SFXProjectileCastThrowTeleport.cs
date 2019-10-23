using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileCastThrowTeleport : SFXProjectileCastThrowable {

    protected override void OnCastTrigger(Vector3 point)
    {
        if (b_trigger)
            return;
        SpawnImpact(point, Vector3.up);
        GameManager.Instance.GetEntity(I_SourceID).transform.position=LevelManager.NavMeshPosition(point, true);
    }
}
