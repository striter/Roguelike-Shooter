using UnityEngine;
using GameSetting;
public class SFXBulletRocket : SFXBullet {
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameLayer.I_Entity)
        {
            HitCheckEntity hitCheck = other.GetComponent<HitCheckEntity>();
            if (hitCheck.I_AttacherID != I_SourceID)
                DoBlast();
        }
        else if (other.gameObject.layer == GameLayer.I_Static)
        {
            DoBlast();
        }
    }
    public void DoBlast()
    {
        SFXBlast sfx= ObjectManager.SpawnSFX(enum_SFX.Blast_Rocket, transform) as SFXBlast;
        sfx.transform.rotation = Quaternion.LookRotation(Vector3.up);
        sfx.Play(I_SourceID,m_bulletDamage,GameConst.I_RocketBlastRadius);
        OnPlayFinished();
    }
}
