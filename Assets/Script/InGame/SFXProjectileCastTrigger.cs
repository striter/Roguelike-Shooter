using UnityEngine;
public class SFXProjectileCastTrigger : SFXProjectile {
    protected override void Play(int sourceID, int impactSFXIndex, int blastSFXIndex, Vector3 direction, Vector3 destination, float damage, float horiSpeed, float horiDistance, float vertiSpeed, float vertiAcceleration, float duration)
    {
        base.Play(sourceID, impactSFXIndex, blastSFXIndex, direction, destination, damage, horiSpeed, horiDistance, vertiSpeed, vertiAcceleration, Vector3.Distance(transform.position, destination) / horiSpeed);
    }

    protected override void OnPlayFinished()
    {
        base.OnPlayFinished();

        if (i_castSFXIndex == -1)
            Debug.LogError("Error!Should Set Blast Index While Using This SFXProjectile:" + I_SFXIndex);

        ObjectManager.SpawnSFX<SFXCast>(i_castSFXIndex, transform.position, Vector3.up).Play(I_SourceID, m_Damage);
    }
}
