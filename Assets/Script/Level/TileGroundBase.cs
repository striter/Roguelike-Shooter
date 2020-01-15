using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;

public class TileGroundBase : TileItemBase,ObjectPoolItem<enum_TileGroundType> {
    public enum_TileGroundType m_GroundType = enum_TileGroundType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Ground;
    Action<enum_TileGroundType, MonoBehaviour> OnRecycle;
    public void OnPoolItemInit(enum_TileGroundType identity, Action<enum_TileGroundType, MonoBehaviour> OnRecycle)
    {
        m_GroundType = identity;
        this.OnRecycle = OnRecycle;
    }
    public override void Init(ChunkTileData _data, System.Random random)
    {
        base.Init(_data, random);
        int texSelection = -1;
        switch(m_GroundType)
        {
            case enum_TileGroundType.Main:
                texSelection = random.Next(0, 4);
                break;
            case enum_TileGroundType.Sub1:
                texSelection = random.Next(0, 2);
                break;
            case enum_TileGroundType.Road1:
                texSelection = 0;
                break;
            case enum_TileGroundType.Road2:
                texSelection = 1;
                break;
            case enum_TileGroundType.Road3:
                texSelection = 2;
                break;
            case enum_TileGroundType.Road4:
                texSelection = 3;
                break;
        }

        if (texSelection != -1)
        {
            MaterialPropertyBlock _block = new MaterialPropertyBlock();
            _block.SetFloat("_TexSelection", texSelection);
            m_Model.GetComponent<MeshRenderer>().SetPropertyBlock(_block);
        }
    }
    public override void DoRecycle()
    {
        OnRecycle(m_GroundType,this);
    }
}
