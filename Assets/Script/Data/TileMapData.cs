﻿using System.Collections.Generic;
using UnityEngine;


public class TileMapData : ScriptableObject
{
    [System.Serializable]
    public class TileInfo
    {
        public int i_axisX;
        public int i_axisY;
        public int m_Status;
        public Vector3 m_Offset;     //offset from origin
        public TileInfo(int _axisX, int _axisY, Vector3 _offset, int _status)
        {
            i_axisX = _axisX;
            i_axisY = _axisY;
            m_Offset = _offset;
            m_Status = _status;
        }
    }

    [SerializeField]
    public int I_Width;
    [SerializeField]
    public int I_Height;
    [SerializeField]
    public Vector2 m_Offset;
    [SerializeField]
    public List<TileInfo> m_MapData;
    public void Bake(Transform target,int _width,int _height,Vector2 offset,float heightDetect,bool bakeUnavailable=true)
    {
        if (_width >= 100 || _height >= 100)
        {
            Debug.LogError("Shouldnt Contains That Many Nodes Thas Above" + 100);
            return;
        }

        m_Offset = offset;
        I_Width = _width;
        I_Height = _height;
        m_MapData = new List<TileInfo>();
        Vector3 origin = target.position;
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                Vector3 cellOffset = new Vector3(offset.x * (-_width / 2 + i), 0f, offset.y * (-_height / 2 + j));
                Vector3 tileCenter = origin + cellOffset;
                bool available = TopDownRayHit(tileCenter, ref cellOffset.y) && cellOffset.y < 3;
                if (available&& heightDetect > 0f)
                {
                    float offsetTopLeft = -1f, offsetTopRight = -1f, offsetBottomLeft = -1f, offsetBottomRight = -1f;
                    available = TopDownRayHit(tileCenter + new Vector3(-offset.x / 2, cellOffset.y, offset.y / 2), ref offsetTopLeft);
                    if (available)
                        available = TopDownRayHit(tileCenter + new Vector3(offset.x / 2, cellOffset.y, offset.y / 2), ref offsetTopRight);
                    if (available)
                        available = TopDownRayHit(tileCenter + new Vector3(-offset.x / 2, cellOffset.y, -offset.y / 2), ref offsetBottomLeft);
                    if (available)
                        available = TopDownRayHit(tileCenter + new Vector3(offset.x / 2, cellOffset.y, -offset.y / 2), ref offsetBottomRight);
                    if (available)
                        available = (Mathf.Abs(offsetTopLeft) + Mathf.Abs(offsetTopRight) + Mathf.Abs(offsetBottomLeft) + Mathf.Abs(offsetBottomRight)) < heightDetect;
                }

                if(!bakeUnavailable||available)
                    m_MapData.Add( new TileInfo(i,j,cellOffset, available ? 0 : -1));
            }
        }
    }
    RaycastHit hit;
    bool TopDownRayHit(Vector3 position, ref float offset)
    {
        if (Physics.Raycast(new Ray(position + Vector3.up * 50f, Vector3.down), out hit, 60f))
        {
            offset= hit.point.y - position.y;
            return true;
        }
        return false;
    }
}
