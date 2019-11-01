using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
public class CampFarmPlot : MonoBehaviour {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItemStatus m_Status { get; private set; }
    public CampFarmItem m_PlotItem { get; private set; }
    public float m_DecayProgress(int stampNow) => (stampNow - m_StartStamp) /(float)GameConst.I_CampFarmItemDecayStampDuration;
    int m_profitStamp,m_StampCheck;
    public float Init(int index,CampPlotInfo info,int offlineStamp,int stampNow)
    {
        m_Index = index;
        m_StartStamp = info.m_StartStamp;
        ResetPlotObj(info.m_Status);
        m_profitStamp = stampNow;
        m_StampCheck = stampNow + Random.Range(25, 35);
        return CheckGenerateProfit(offlineStamp,stampNow);
    }

    public float Tick(int stampNow)
    {
        if (stampNow < m_StampCheck)
            return 0;
        m_StampCheck = stampNow + GameExpression.I_CampFarmPlotProfitDuration.Random();
        
        float profit = CheckGenerateProfit(m_profitStamp, stampNow);
        m_profitStamp = stampNow;
        return profit;
    }

    public float DisableProfitCheck(int stampNow)=> CheckGenerateProfit(m_profitStamp, stampNow);

    public void Hybrid(enum_CampFarmItemStatus status)
    {
        m_StartStamp = TTimeTools.GetTimeStampNow();
        ResetPlotObj(status);
    }

    public void Clear()
    {
        m_StartStamp = -1;
        ResetPlotObj(enum_CampFarmItemStatus.Empty);
    }

    void ResetPlotObj(enum_CampFarmItemStatus status)
    {
        if (m_PlotItem)
        {
            m_PlotItem.Unbind();
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Recycle(m_Status, m_PlotItem);
        }
        m_Status = status;
        m_PlotItem = ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Spawn(m_Status, null);
        m_PlotItem.Bind(this);
        m_PlotItem.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        EndDrag();
    }

    float CheckGenerateProfit(int previousStamp, int stampNow)
    {
        if (!GameExpression.CanGenerateprofit(m_Status))
            return 0;

        bool decayed = m_DecayProgress(stampNow) >= 1f;
        int profitBegin = previousStamp < m_StartStamp ? m_StartStamp : previousStamp;
        int profitEnd = decayed ? m_StartStamp + GameConst.I_CampFarmItemDecayStampDuration : stampNow;
        int profitStampOffset = profitEnd - profitBegin;
        float profit = profitStampOffset * GameExpression.GetFarmCreditGeneratePerSecond[m_Status];
        if (decayed)
        {
            m_StartStamp = -1;
            ResetPlotObj(enum_CampFarmItemStatus.Decayed);
        }
        return profit;
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

    }
}
