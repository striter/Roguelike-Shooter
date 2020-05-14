using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using LevelSetting;
using UnityEngine;

public class TilePlantsBase : TileItemBase,IObjectPoolStaticBase<enum_TilePlantsType> {
    public override enum_TileSubType m_Type => enum_TileSubType.Plants;
    public enum_TilePlantsType m_PlantsType = enum_TilePlantsType.Invalid;
    Action<enum_TilePlantsType, MonoBehaviour> OnRecycle;
    public override void DoRecycle() => OnRecycle(m_PlantsType, this);

    public void OnPoolItemInit(enum_TilePlantsType identity, Action<enum_TilePlantsType, MonoBehaviour> OnRecycle)
    {
        Init();
        GetComponentsInChildren<HitCheckDynamic>().Traversal((HitCheckDynamic hitCheck) => { hitCheck.Attach(OnObjectHit); });
        this.OnRecycle = OnRecycle;
    }
    public void OnPoolItemRecycle()
    {
    }

    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_Model.transform.localRotation = Quaternion.Euler(0, random.Next(360), 0);
    }

    bool OnObjectHit(DamageInfo damage, Vector3 direction)
    {
        DoRecycle();
        return true;
    }

}
