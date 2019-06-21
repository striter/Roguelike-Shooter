using UnityEngine;
using GameSetting;
public class SFXProjectileBlastTrigger : SFXProjectile {
    protected override void OnHitStatic(HitCheckStatic hitStatic)
    {
        DoBlast();
    }
    protected override void OnHitDynamic(HitCheckDynamic hitDynamic)
    {
        DoBlast();
    }
    protected override void OnHitEntity(HitCheckEntity hitEntity)
    {
        if (GameManager.B_CanHitTarget(hitEntity, I_SourceID))
            DoBlast();
    }
    protected override void OnHitError()
    {
        DoBlast();
    }
    public void DoBlast()
    {
        SFXBlast sfx= ObjectManager.SpawnSFX(enum_SFX.Blast_Rocket, m_subSFXDic[enum_SubSFXType.Projectile].transform) as SFXBlast;
        sfx.transform.rotation = Quaternion.LookRotation(Vector3.up);
        sfx.Play(I_SourceID,m_Damage,GameConst.I_RocketBlastRadius);
        OnPlayFinished();
    }
}
