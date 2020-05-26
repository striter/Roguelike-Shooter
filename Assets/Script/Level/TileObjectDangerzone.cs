using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using LevelSetting;
using TPhysics;

public class TileObjectDangerzone : TileObjectBase {
    TSpecialClasses.AnimationClipControl m_Animation;
    TimerBase m_TimerReset=new TimerBase(GameConst.F_DangerzoneResetDuration);

    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_Animation = new TSpecialClasses.AnimationClipControl(GetComponent<Animation>());
    }

    private void Update()
    {
        m_TimerReset.Tick(Time.deltaTime);
        if (m_TimerReset.m_Timing)
            return;
        m_Animation.Play(true);
        m_TimerReset.Replay();
        Physics_Extend.BoxCastAll(m_Model.position,Vector3.up,transform.forward,Vector3.one,GameLayer.Mask.I_Entity).Traversal((RaycastHit hit)=> hit.collider.DetectEntity()?.TryHit(new DamageInfo(-1, enum_DamageIdentity.Environment).SetDamage(GameConst.I_DangerzoneDamage, enum_DamageType.Basic)));
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (BattleManager.m_HaveInstance&&BattleManager.Instance.B_PhysicsDebugGizmos)
        {
            Gizmos.color = Color.red;
            Gizmos_Extend.DrawWireCube(m_Model.position, Quaternion.LookRotation(Vector3.up, transform.forward), Vector3.one);
        }
    }
    #endif
}
