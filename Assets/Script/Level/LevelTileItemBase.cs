using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;

public class LevelTileItemBase : MonoBehaviour {
    public virtual enum_TileSubType m_Type => enum_TileSubType.Invalid;
    protected Transform m_Model { get; private set; }
    public virtual TileAxis GetDirectionedSize(enum_TileDirection direction) => TileAxis.One;

    public virtual void Init(ChunkTileData _data)
    {
        m_Model = transform.Find("Model");
        m_Model.localPosition = TileTools.GetLocalPosBySizeAxis(GetDirectionedSize(_data.m_Direction));
        m_Model.localRotation = TileTools.GetDirectionRotation(_data.m_Direction);
    }

    public virtual void DoRecycle()
    {
        Debug.LogError("Override Thie Please");
    }
}
