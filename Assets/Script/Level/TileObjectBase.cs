using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileObjectBase : TileItemBase,ObjectPoolItem<enum_TileObjectType> {
    public enum_TileObjectType m_ObjectType= enum_TileObjectType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Object;
    Action<enum_TileObjectType, MonoBehaviour> OnRecycle;
    public override TileAxis GetDirectionedSize(enum_TileDirection direction) => m_ObjectType.GetSizeAxis(direction);
    public void OnPoolItemInit(enum_TileObjectType identity, Action<enum_TileObjectType, MonoBehaviour> OnRecycle)
    {
        m_ObjectType = identity;
        this.OnRecycle = OnRecycle;
    }

    public override void Init(ChunkTileData _data,System.Random random)
    {
        base.Init(_data,random);
        int texSelection = 1;
        if (m_ObjectType == enum_TileObjectType.Main)
            texSelection = random.Next(0, 4);
        else if (m_ObjectType == enum_TileObjectType.Sub1)
            texSelection = random.Next(0, 2);
        if(texSelection!=1)
        {
            MaterialPropertyBlock _block = new MaterialPropertyBlock();
            _block.SetFloat("_TexSelection",texSelection);
            m_Model.GetComponent<MeshRenderer>().SetPropertyBlock(_block);
        }
    }

    public override void DoRecycle()
    {
        OnRecycle(m_ObjectType, this);
    }
    
}
