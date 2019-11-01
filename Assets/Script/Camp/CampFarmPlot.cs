using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
public class CampFarmPlot : CampFarmInteract {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItemStatus m_Status { get; private set; }
    public CampFarmItem m_PlotItem { get; private set; }
    public float m_DecayProgress { get; private set; }

    float CalculateDecayProgress(int stampNow) => Mathf.Clamp(1- (stampNow - m_StartStamp) /(float)GameConst.I_CampFarmItemDecayStampDuration,0,1);
    int m_profitStamp,m_StampCheck;
    public float Init(int index,CampFarmPlotData info,int offlineStamp,int stampNow)
    {
        m_Index = index;
        m_StartStamp = info.m_StartStamp;
        ResetPlotObj(info.m_Status);
        m_profitStamp = stampNow;
        m_StampCheck = stampNow + Random.Range(25, 35);
        return CheckGenerateProfit(offlineStamp,stampNow);
    }

    public float TickProfit(int stampNow)
    {
        m_DecayProgress = CalculateDecayProgress(stampNow);

        if (stampNow < m_StampCheck)
            return 0;
        m_StampCheck = stampNow + GameExpression.I_CampFarmPlotProfitDuration.Random();
        
        float profit = CheckGenerateProfit(m_profitStamp, stampNow);
        m_profitStamp = stampNow;
        return profit;
    }

    public float EndProfit(int stampNow)=> CheckGenerateProfit(m_profitStamp, stampNow);

    public void Hybrid(enum_CampFarmItemStatus status)
    {
        m_DecayProgress = 1f;
        m_StartStamp = TTimeTools.GetTimeStampNow();
        ResetPlotObj(status);
    }

    public void Clear()
    {
        m_DecayProgress = 0;
        m_StartStamp = -1;
        ResetPlotObj(enum_CampFarmItemStatus.Empty);
    }

    void ResetPlotObj(enum_CampFarmItemStatus status)
    {
        if (m_PlotItem)
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Recycle(m_Status, m_PlotItem);

        m_Status = status;
        m_PlotItem = ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Spawn(m_Status, null);
        m_PlotItem.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
        EndDrag();
    }

    float CheckGenerateProfit(int previousStamp, int stampNow)
    {
        if (!GameExpression.CanGenerateprofit(m_Status))
            return 0;

        bool decayed = CalculateDecayProgress(stampNow) == 0f;
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
