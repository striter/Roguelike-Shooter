using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
public class CampFarmPlot : MonoBehaviour {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItem m_Status { get; private set; }
    public CampFarmItem m_PlotItem { get; private set; }
    public float Init(int index,CampPlotInfo info)
    {
        m_Index = index;
        m_StartStamp = info.m_StartStamp;
        ResetPlotObj(info.m_Status);
        return 0;
    }

    public void Hybrid(enum_CampFarmItem status)
    {
        m_StartStamp = TTimeTools.GetTimeStampNow();
        ResetPlotObj(status);
    }

    public void Clear()
    {
        m_StartStamp = -1;
        ResetPlotObj(enum_CampFarmItem.Empty);
    }

    void ResetPlotObj(enum_CampFarmItem status)
    {
        if (m_PlotItem)
        {
            m_PlotItem.Unbind();
            ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.Recycle(m_Status, m_PlotItem);
        }
        m_Status = status;
        m_PlotItem = ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.Spawn(m_Status, null);
        m_PlotItem.Bind(this);
        EndDrag();
    }

    public void BeginDrag()
    {

    }
    public void Move(Vector3 pos)
    {
        m_PlotItem.transform.position = pos;
    }
    public void EndDrag()
    {
        m_PlotItem.transform.position = transform.position;
        m_PlotItem.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
}
