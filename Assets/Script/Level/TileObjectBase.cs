using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;
using GameSetting;
public class TileObjectBase : TileItemBase,IObjectPoolStaticBase<enum_TileObjectType>,ICoroutineHelperClass {
    public enum_TileObjectType m_ObjectType= enum_TileObjectType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Object;
    Action<enum_TileObjectType, MonoBehaviour> OnRecycle;
    public override TileAxis GetDirectionedSize(enum_TileDirection direction) => m_ObjectType.GetSizeAxis(direction);

    public void OnPoolItemInit(enum_TileObjectType identity, Action<enum_TileObjectType, MonoBehaviour> OnRecycle)
    {
        Init();
        m_ObjectType = identity;
        this.OnRecycle = OnRecycle;
    }
    public void OnPoolItemRecycle()
    {
    }


    public override void DoRecycle()=> OnRecycle(m_ObjectType, this);

}
