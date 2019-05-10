using UnityEngine;
using GameSetting;
public class SFXBulletRocket : SFXBullet {
    protected override void OnHitStatic()
    {
        DoBlast();
    }
    protected override void OnHitDynamic()
    {
        DoBlast();
    }
    protected override void OnHitEntity(HitCheckEntity entity)
    {
        if (GameExpression.B_CanHitTarget(entity, I_SourceID))
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
