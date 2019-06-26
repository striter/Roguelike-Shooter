using UnityEngine;
using GameSetting;
public class SFXProjectileBlastTrigger : SFXProjectile {
    protected override void OnHitTarget(RaycastHit hitInfo)
    {
        if (i_impactSFXIndex == -1)
            Debug.LogError("Error!Should Set Impact Index While Using This SFXProjectile:"+I_SFXIndex);

        SFXBlast sfx = ObjectManager.SpawnSFX<SFXBlast>(i_impactSFXIndex, transform.position, Vector3.up);
        sfx.transform.rotation = Quaternion.LookRotation(Vector3.up);
        sfx.Play(I_SourceID, m_Damage, GameConst.I_RocketBlastRadius);
        OnPlayFinished();
    }
}
