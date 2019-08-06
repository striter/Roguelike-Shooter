using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TPhysics;
using UnityEngine;

public class SFXProjectileTargetRangeDrop : SFXProjectile {
    public float F_DropDuration = .5f;
    public int I_DropCount = 10;
    public float F_DropStartHeight = 10f;
    public float F_DropRange = 3f;

    int i_dropCountCheck = 0;
    float f_dropCheck = 0;
    protected override bool B_DealDamage => true;
    protected override float F_Duration(Vector3 startPos, Vector3 endPos) => Vector3.Distance(startPos,endPos)/F_Speed+(I_DropCount+2)*F_DropDuration;
    protected override PhysicsSimulator<HitCheckBase> GetSimulator(Vector3 direction, Vector3 targetPosition) => new ProjectilePhysicsLerpSimulator(transform, transform.position, targetPosition+Vector3.up*F_DropStartHeight, OnAnimFinished, Vector3.Distance(transform.position, targetPosition) / F_Speed, F_Height, F_Radius, GameLayer.Mask.I_All, OnHitTargetBreak,CanHitTarget);
    protected override void OnPlayPreset()
    {
        base.OnPlayPreset();
        if (F_DropDuration<=0)
            Debug.LogError("Spread Duration Less Or Equals 0!");
        if (I_DropCount <= 0)
            Debug.LogError("Spread Count Less Or Equals 0!");
        f_dropCheck = 0;
        i_dropCountCheck = 0;
    }
    protected override void Update()
    {
        base.Update();
        if (B_SimulatePhysics)
            return;
        
        f_dropCheck += Time.deltaTime;
        if (f_dropCheck < F_DropDuration)
            return;
        f_dropCheck -= F_DropDuration;

        Vector3 startPos = transform.position + Vector3.forward * Random.Range(F_DropRange, -F_DropRange) + Vector3.right * Random.Range(F_DropRange, -F_DropRange);
        ObjectManager.SpawnDamageSource<SFXProjectile>(GameExpression.GetEnermyWeaponSubIndex(I_SFXIndex), startPos, Vector3.down).Play(I_SourceID,Vector3.down,startPos+Vector3.down*F_DropStartHeight,m_DamageInfo.m_BuffApply);

        i_dropCountCheck++;
        if (i_dropCountCheck >= I_DropCount)
        {
            OnRecycle();
            return;
        }
    }
    protected void OnAnimFinished()
    {
        B_SimulatePhysics = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_PhysicsDebugGizmos)
            return;
        if (B_SimulatePhysics)
            return;

        Gizmos.color = Color.red;
        Gizmos_Extend.DrawCylinder(transform.position,Quaternion.LookRotation(Vector3.down),F_DropRange,F_DropStartHeight);
    }
#endif
}
