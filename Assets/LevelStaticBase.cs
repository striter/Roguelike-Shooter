using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class LevelStaticBase : MonoBehaviour {
    public int m_sizeXAxis = 1;
    public int m_sizeYAxis = 1;
    public enum_LevelItemCollide m_GenerateOrder = enum_LevelItemCollide.Invalid;
    public void Init()
    {

    }

#if UNITY_EDITOR
    LevelBase baseTarget;
    public bool b_showGizmos = true;
    public void OnDrawGizmos()
    {
        if (!b_showGizmos)
            return;
        baseTarget = GetComponentInParent<LevelBase>();
        if (baseTarget!=null)
        {
            TileMapData data = EnviormentManager.GetLevelData(baseTarget.m_LevelType, baseTarget.name);
            if (data == null)
                return;
            transform.localPosition = data.m_MapData[0].m_Offset;
            Gizmos.color = Color.blue;

            for (int i = 0; i < m_sizeXAxis; i++)
            {
                for (int j = 0; j < m_sizeYAxis; j++)
                {
                    Gizmos.DrawCube(transform.position+new Vector3(i*data.m_Offset.x,1f,j*data.m_Offset.y),new Vector3(data.m_Offset.x/2,2f,data.m_Offset.y/2));
                }
            }
        }
    }
#endif
}
