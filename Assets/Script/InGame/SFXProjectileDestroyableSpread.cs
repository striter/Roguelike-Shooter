using GameSetting;
using UnityEngine;

public class SFXProjectileDestroyableSpread : SFXProjectileDestroyable {
    
    [Range(0, 90)]
    public int I_SpreadAngleEach = 30;
    public float F_SpreadDuration = .5f;
    public int I_SpreadCount = 10;
    int i_spreadCountCheck = 0;
    float f_spreadCheck = 0;
    protected override bool B_RecycleOnHit => false;
    protected override bool B_DisablePhysicsOnHit => false;
    protected override bool B_DealDamage => false;
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (F_SpreadDuration <= 0)
            Debug.LogError("Spread Duration Less Or Equals 0!");
        if (I_SpreadCount <= 0)
            Debug.LogError("Spread Count Less Or Equals 0!");
        f_spreadCheck = 0;
        i_spreadCountCheck = 0;
    }
    protected override void Update()
    {
        base.Update();
        if (!B_SimulatePhysics)
            return;

        f_spreadCheck += Time.deltaTime;
        if (f_spreadCheck < F_SpreadDuration)
            return;

        f_spreadCheck -= F_SpreadDuration;

        Vector3 splitDirection = transform.forward.RotateDirection(Vector3.up, i_spreadCountCheck * I_SpreadAngleEach);
        ObjectManager.SpawnDamageSource<SFXProjectile>(GameExpression.GetEnermyWeaponSubIndex(I_SFXIndex), m_CenterPos, Vector3.up).Play(I_SourceID, splitDirection, m_CenterPos + splitDirection * 10, m_DamageInfo.m_BuffApply);

        i_spreadCountCheck++;
        if (i_spreadCountCheck >= I_SpreadCount)
        {
            OnPlayFinished();
            return;
        }
    }
}
