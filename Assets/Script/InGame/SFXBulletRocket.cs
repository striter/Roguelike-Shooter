using UnityEngine;
using GameSetting;
public class SFXBulletRocket : SFXBullet {
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
        if (GameExpression.B_CanHitTarget(hitEntity, I_SourceID))
            DoBlast();
    }
    protected override void OnHitError()
    {
        DoBlast();
    }
    public void DoBlast()
    {
        SFXBlast sfx= ObjectManager.SpawnSFX(enum_SFX.Blast_Rocket, transform) as SFXBlast;
        sfx.transform.rotation = Quaternion.LookRotation(Vector3.up);
        sfx.Play(I_SourceID,m_bulletDamage,GameConst.I_RocketBlastRadius);
        OnPlayFinished();
    }
}
