﻿using UnityEngine;
using GameSetting;
public class SFXProjectileBlastTrigger : SFXProjectile {
    protected override void OnHitTarget(RaycastHit hitInfo)
    {
        if (i_impactSFXIndex != -1)
            ObjectManager.SpawnSFX<SFXParticles>(i_impactSFXIndex, hitInfo.point, hitInfo.normal, hitInfo.transform).Play(I_SourceID);
        if (i_blastSFXIndex == -1)
            Debug.LogError("Error!Should Set Blast Index While Using This SFXProjectile:" + I_SFXIndex);

        SFXBlast blast = ObjectManager.SpawnSFX<SFXBlast>(i_blastSFXIndex, transform.position, Vector3.up);
        blast.transform.rotation = Quaternion.LookRotation(Vector3.up);
        blast.Play(I_SourceID, m_Damage);
        OnPlayFinished();
    }
}
