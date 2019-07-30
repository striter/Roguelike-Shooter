using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;

public class LevelItemBase : MonoBehaviour {
    public LevelBase m_LevelParent { get; private set; }
    public int m_sizeXAxis = 1;
    public int m_sizeYAxis = 1;
    public enum_LevelItemType m_ItemType = enum_LevelItemType.Invalid;
    Transform tf_Model;
    public void Init(LevelBase levelParent,enum_TileDirection direction)
    {
        m_LevelParent = levelParent;
        tf_Model = transform.Find("Model");
        ItemRecenter(direction == enum_TileDirection.Right || direction == enum_TileDirection.Left);
        tf_Model.localRotation = Quaternion.Euler(0, (int)direction * 90, 0);
        transform.SetActivate(true);
    }

    public void ItemRecenter(bool inverse=false)
    {
        transform.GetChild(0).localPosition = new Vector3(((inverse?m_sizeYAxis:m_sizeXAxis) - 1) * GameConst.F_LevelTileSize, 0f, ((inverse ? m_sizeXAxis:m_sizeYAxis) - 1) * GameConst.F_LevelTileSize) / 2;
    }
#if UNITY_EDITOR
    public bool b_showGizmos = true;
    public bool b_AutoCenter = true;
    public bool b_AutoRotation = true;
    public void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying||!b_showGizmos)
            return;
        
        transform.localPosition = Vector3.zero;

        if (UnityEditor.EditorApplication.isPlaying)
            return;

        Gizmos.color = TCommon.ColorAlpha(Color.blue,.3f);

        for (int i = 0; i < m_sizeXAxis; i++)
            for (int j = 0; j < m_sizeYAxis; j++)
                Gizmos.DrawCube(transform.position + new Vector3(i * GameConst.F_LevelTileSize, .5f, j * GameConst.F_LevelTileSize), new Vector3(GameConst.F_LevelTileSize, 1f, GameConst.F_LevelTileSize));

        if (b_AutoCenter)
            ItemRecenter();
    }
#endif
}
