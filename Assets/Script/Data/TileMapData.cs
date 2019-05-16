using GameSetting;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class TileMapData : ScriptableObject
{
    public const int MaxNodeEachAxle = 10000;
    [System.Serializable]
    public class TileInfo
    {
        public int i_axisX;
        public int i_axisY;
        public int m_Status;
        public Vector3 m_Offset;     //offset from origin
        public List<int> m_NearbyTile;
        public TileInfo(int _axisX, int _axisY, Vector3 _offset, int _status)
        {
            i_axisX = _axisX;
            i_axisY = _axisY;
            m_Offset = _offset;
            m_Status = _status;
        }
        public void SetNearbyTile(List<int> _nearbyTile)
        {
            m_NearbyTile = _nearbyTile;
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
    public void Bake(Transform target,int _width,int _height,Vector2 offset,bool bakeUnavailable=true)
    {
        if (_width >= MaxNodeEachAxle || _height >= MaxNodeEachAxle)
        {
            Debug.LogError("Shouldnt Contains That Many Nodes Thas Above" + MaxNodeEachAxle);
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
                Vector3 cellOffset = new Vector3(offset.x*(-_width/2+ i),0f,offset.y*(-_height/2+j));
                bool available = false ;
                RaycastHit hit;
                if (Physics.Raycast(new Ray(origin + cellOffset+Vector3.up*10f,Vector3.down), out hit,20f))
                {
                    available = true;
                    cellOffset.y = hit.point.y-target.position.y;
                }

                if(!bakeUnavailable||available)
                    m_MapData.Add( new TileInfo(i,j,cellOffset, available ? 0 : -1));
            }
        }

        for (int i = 0; i < m_MapData.Count; i++)
        {
            List<int> nearbyTile = new List<int>();
            int tileIndex;

            tileIndex = m_MapData.FindIndex(p=>p.i_axisX==m_MapData[i].i_axisX-1 && p.i_axisX == m_MapData[i].i_axisY);
            if (tileIndex != -1)
                nearbyTile.Add(tileIndex);

            tileIndex = m_MapData.FindIndex(p => p.i_axisX == m_MapData[i].i_axisX + 1 && p.i_axisX == m_MapData[i].i_axisY);
            if (tileIndex != -1)
                nearbyTile.Add(tileIndex);

            tileIndex = m_MapData.FindIndex(p => p.i_axisX == m_MapData[i].i_axisX + 1 && p.i_axisX == m_MapData[i].i_axisY);
            if (tileIndex != -1)
                nearbyTile.Add(tileIndex);

            tileIndex = m_MapData.FindIndex(p => p.i_axisX == m_MapData[i].i_axisX + 1 && p.i_axisX == m_MapData[i].i_axisY);
            if (tileIndex != -1)
                nearbyTile.Add(tileIndex);

            m_MapData[i].SetNearbyTile(nearbyTile);
        }
    }
}
