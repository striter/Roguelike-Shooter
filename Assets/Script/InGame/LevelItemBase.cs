using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class LevelItemBase : MonoBehaviour {
    public LevelBase m_LevelParent { get; private set; }
    public int m_sizeXAxis = 1;
    public int m_sizeYAxis = 1;
    public enum_LevelItemType m_ItemType = enum_LevelItemType.Invalid;
    Transform tf_Model;
    public void Init(LevelBase levelParent)
    {
        m_LevelParent = levelParent;
         tf_Model = transform.Find("Model");
        if(m_sizeXAxis==m_sizeYAxis)
         tf_Model.localRotation = Quaternion.Euler(0, levelParent.m_seed.Next(360),0);
        transform.SetActivate(true);
    }

#if UNITY_EDITOR
    LevelBase baseTarget;
    public bool b_showGizmos = true;
    public bool b_AutoCenter = true;
    public void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying||!b_showGizmos)
            return;

        baseTarget = GetComponentInParent<LevelBase>();
        if (baseTarget!=null)
        {
            TileMapData data = TResources.GetLevelData(baseTarget.name);
            if (data == null)
                return;
            transform.localPosition = data.m_MapData[0].m_Offset;
        }

        if (UnityEditor.EditorApplication.isPlaying)
            return;

        Gizmos.color = TCommon.ColorAlpha(Color.blue,.3f);

        for (int i = 0; i < m_sizeXAxis; i++)
            for (int j = 0; j < m_sizeYAxis; j++)
                Gizmos.DrawCube(transform.position + new Vector3(i * GameConst.F_LevelTileSize, .5f, j * GameConst.F_LevelTileSize), new Vector3(GameConst.F_LevelTileSize, 1f, GameConst.F_LevelTileSize));

        if (b_AutoCenter)
            EditorRecenter();
    }
    public void EditorRecenter()
    {
        transform.GetChild(0).localPosition = new Vector3((m_sizeXAxis - 1) * GameConst.F_LevelTileSize, 0f, (m_sizeYAxis - 1) * GameConst.F_LevelTileSize) / 2;
    }
#endif
}
