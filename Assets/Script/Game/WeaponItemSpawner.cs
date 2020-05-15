using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItemSpawner : WeaponItemBase {
    public int m_SpawnIndex;
    protected override void OnKeyAnim()
    {
        base.OnKeyAnim();
        GameObjectManager.SpawnGameCharcter(m_SpawnIndex,NavigationManager.NavMeshPosition( transform.position+TCommon.RandomXZCircle()*2f),Quaternion.identity).OnSubActivate(m_Attacher.m_Flag,m_Attacher.m_EntityID);
    }
}
