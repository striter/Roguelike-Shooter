using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LevelSetting;
using TTiles;

public class TileItemBase : MonoBehaviour {
    public virtual enum_TileSubType m_Type => enum_TileSubType.Invalid;
    protected Transform m_Model { get; private set; }
    public virtual TileAxis GetDirectionedSize(enum_TileDirection direction) => TileAxis.One;
    protected virtual int GetTexSelection(System.Random random) => -1;

    public virtual void Init(ChunkTileData _data,System.Random random)
    {
        m_Model = transform.Find("Model");
        m_Model.localPosition = TileTools.GetLocalPosBySizeAxis(GetDirectionedSize(_data.m_Direction));
        m_Model.localRotation = TileTools.GetDirectionRotation(_data.m_Direction);

        int texSelection = GetTexSelection(random);
        if (texSelection != -1)
        {
            MaterialPropertyBlock _block = new MaterialPropertyBlock();
            _block.SetFloat("_TexSelection", texSelection);
            m_Model.GetComponentInChildren<MeshRenderer>().SetPropertyBlock(_block);
        }
    }


    public virtual void DoRecycle()
    {
        Debug.LogError("Override Thie Please");
    }
}
