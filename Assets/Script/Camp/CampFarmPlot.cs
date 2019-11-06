using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
using System;

public class CampFarmPlot : CampFarmInteract {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItemStatus m_Status { get; private set; } = enum_CampFarmItemStatus.Invalid;
    public CampFarmItem m_PlotItem { get; private set; }
    public int m_TimeLeft { get; private set; }
    Action<int> OnPlotStatusChanged;
    int m_profitStamp,m_StampCheck;
    public float Init(int index,CampFarmPlotData info,int offlineStamp,int stampNow,Action<int> _OnPlotStatusChanged)
    {
        m_Index = index;
        m_StartStamp = info.m_StartStamp;
        OnPlotStatusChanged = _OnPlotStatusChanged;
        ResetPlotStatus(info.m_Status,false);
        m_profitStamp = stampNow;
        ResetCheck(stampNow);
        return CheckGenerateProfit(offlineStamp,stampNow,false);
    }

    void ResetCheck(int stampNow)
    {
        if (!GameExpression.CanGenerateprofit(m_Status))
            return;

        m_profitStamp = stampNow;
        m_StampCheck = stampNow +  Mathf.RoundToInt(0.01f / GameExpression.GetFarmItemInfo[m_Status].m_CreditPerSecond);
    }

    public float TickProfit(int stampNow)
    {
        if (!GameExpression.CanGenerateprofit(m_Status))
            return 0;

        m_TimeLeft = m_StartStamp+ GameExpression.GetFarmItemInfo[m_Status].m_ItemDuration - stampNow;
        if (stampNow < m_StampCheck&&m_TimeLeft>0)
            return 0;

        float profit = CheckGenerateProfit(m_profitStamp, stampNow,true);
        m_profitStamp = stampNow;
        ResetCheck(stampNow);
        return profit;
    }

    public float EndProfit(int stampNow)=> CheckGenerateProfit(m_profitStamp, stampNow,false);

    public void Hybrid(enum_CampFarmItemStatus status)
    {
        GameObjectManager.SpawnParticles<SFXMuzzle>(10021, transform.position, Vector3.up).Play(-1);
        ResetPlotStatus(status,true);
        m_StartStamp = TTimeTools.GetTimeStampNow();
        m_TimeLeft = GameExpression.GetFarmItemInfo[m_Status].m_ItemDuration;
        ResetCheck(m_StartStamp);
    }

    public void Clear()
    {
        ResetPlotStatus(enum_CampFarmItemStatus.Empty,false);
        m_TimeLeft = 0;
        m_StartStamp = -1;
    }
    
    void ResetPlotStatus(enum_CampFarmItemStatus status,bool showAnim)
    {
        if (m_Status == status)
            return;

        if (showAnim)
            GameObjectManager.SpawnParticles<SFXMuzzle>(10021, transform.position, Vector3.up).Play(-1);

        if (m_PlotItem)
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Recycle(m_Status, m_PlotItem);

        m_Status = status;
        m_PlotItem = ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Spawn(m_Status, null);


        OnPlotStatusChanged(m_Index);
        EndDrag();
    }

    float CheckGenerateProfit(int previousStamp, int stampNow,bool showDecayAnim)
    {
        if (!GameExpression.CanGenerateprofit(m_Status))
            return 0;

        m_TimeLeft = m_StartStamp+GameExpression.GetFarmItemInfo[m_Status].m_ItemDuration - stampNow;
        bool decayed = m_TimeLeft <= 0f;
        int profitBegin = previousStamp < m_StartStamp ? m_StartStamp : previousStamp;
        int profitEnd = decayed ? m_StartStamp + GameExpression.GetFarmItemInfo[m_Status].m_ItemDuration : stampNow;
        int profitStampOffset = profitEnd - profitBegin;
        float profit = profitStampOffset * GameExpression.GetFarmItemInfo[m_Status].m_CreditPerSecond;
        if (decayed)
        {
            ResetPlotStatus(enum_CampFarmItemStatus.Decayed, showDecayAnim);
            m_TimeLeft = 0;
            m_StartStamp = -1;
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
