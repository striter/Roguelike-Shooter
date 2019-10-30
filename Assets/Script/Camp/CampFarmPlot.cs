using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
public class CampFarmPlot : MonoBehaviour {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItem m_Status { get; private set; }
    CampFarmItem m_PlotObj;
    public void Init(int index,CampPlotInfo info)
    {
        m_Index = index;
        m_StartStamp = info.m_StartStamp;
        m_Status = info.m_Status;
        SpawnPlotObj();
    }
    void SpawnPlotObj()
    {
        m_PlotObj = ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.Spawn(m_Status, transform);
        m_PlotObj.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    } 

    public void Hybrid()
    {
        if (m_Status < enum_CampFarmItem.Progress1)
        {
            Debug.LogError("Can't Hybrid Item:"+m_Status);
            return;
        }
        Clear();
        m_StartStamp = TTimeTools.GetTimeStampNow();
        if (m_Status <= enum_CampFarmItem.Progress5) m_Status += 1;
        SpawnPlotObj();
    }

    public void Clear()
    {
        ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.Recycle(m_Status, m_PlotObj);
        m_StartStamp = -1;
        m_Status = enum_CampFarmItem.Invalid;
    }
}
