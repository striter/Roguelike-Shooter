using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileCastThrowTeleport : SFXProjectileCastThrowable {

    protected override void OnCastTrigger(Vector3 point)
    {
        SpawnImpact(point, Vector3.up);
        GameManager.Instance.GetEntity(m_SourceID).transform.position=NavigationManager.NavMeshPosition(point);
    }
}
