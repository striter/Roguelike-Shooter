using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXCastHammerOfDawn : SFXCast {

    int m_DawnSFXIndex;

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        m_DawnSFXIndex = GameExpression.GetWeaponSubIndex(_identity);
    }
    protected override List<EntityBase> DoCastDealtDamage()
    {
        List<EntityBase> entities= base.DoCastDealtDamage();

        entities.Traversal((EntityBase entity) => { GameObjectManager.SpawnSFXWeapon<SFXCast>(m_DawnSFXIndex, entity.transform.position, Vector3.up).Play(m_DamageInfo); });

        return entities;
    }

    public override void OnHitEntityDealtDamage(HitCheckEntity entity, Vector3 hitPoint, Vector3 hitNormal)
    {
        // Dealt No Damage
    }
}
