using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTime;
using System;

public class CampFarmPlot : MonoBehaviour {
    public int m_Index { get; private set; }
    public int m_StartStamp { get; private set; }
    public enum_CampFarmItemStatus m_Status { get; private set; } = enum_CampFarmItemStatus.Invalid;
    public CampFarmItem m_PlotItem { get; private set; }
    public bool m_CanGenerateProfit { get; private set; }
    public int m_StampLeft { get; private set; }
    public string m_Timeleft => TTimeTools.GetHMS(m_StampLeft);
    public float m_StampLeftScale => (float)m_StampLeft / GameConst.I_CampFarmDecayDuration
;    Action<int> OnPlotStatusChanged;
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
        if (!m_CanGenerateProfit)
            return;

        m_profitStamp = stampNow;
        m_StampCheck = stampNow +  Mathf.RoundToInt(GameConst.F_CampFarmItemTickAmount / GameConst.GetFarmCreditPerSecond[m_Status]);
    }

    public float TickProfit(int stampNow)
    {
        if (!m_CanGenerateProfit)
            return 0;

        m_StampLeft = m_StartStamp+ GameConst.I_CampFarmDecayDuration - stampNow;
        if (stampNow < m_StampCheck&&m_StampLeft>0)
            return 0;

        float profit = CheckGenerateProfit(m_profitStamp, stampNow,true);
        m_profitStamp = stampNow;
        ResetCheck(stampNow);
        return profit;
    }

    public float EndProfit(int stampNow)=> CheckGenerateProfit(m_profitStamp, stampNow,false);

    public void Hybrid(enum_CampFarmItemStatus status)
    {
        m_StartStamp = TTimeTools.GetTimeStampNow();
        m_StampLeft = GameConst.I_CampFarmDecayDuration;
        ResetPlotStatus(status,true);
        ResetCheck(m_StartStamp);
    }

    public void Clear()
    {
        m_StampLeft = 0;
        m_StartStamp = -1;
        ResetPlotStatus(enum_CampFarmItemStatus.Empty,true);
    }
    
    void ResetPlotStatus(enum_CampFarmItemStatus status,bool showAnim)
    {
        if (m_Status == status)
            return;

        if (showAnim)
            GameObjectManager.SpawnSFX<SFXMuzzle>(10021, transform.position, Vector3.up).PlayOnce(-1);

        if (m_PlotItem)
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Recycle(m_Status, m_PlotItem);

        m_Status = status;
        m_PlotItem = ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Spawn(m_Status, null,Vector3.zero,Quaternion.identity);
        m_CanGenerateProfit = GameExpression.CanGenerateprofit(m_Status);

        OnPlotStatusChanged(m_Index);
        EndDrag();
    }

    float CheckGenerateProfit(int previousStamp, int stampNow,bool showDecayAnim)
    {
        if (!m_CanGenerateProfit)
            return 0;

        m_StampLeft = m_StartStamp+ GameConst.I_CampFarmDecayDuration - stampNow;
        bool decayed = m_StampLeft <= 0f;
        int profitBegin = previousStamp < m_StartStamp ? m_StartStamp : previousStamp;
        int profitEnd = decayed ? m_StartStamp + GameConst.I_CampFarmDecayDuration : stampNow;
        int profitStampOffset = profitEnd - profitBegin;
        float profit = profitStampOffset * GameConst.GetFarmCreditPerSecond[m_Status];
        if (decayed)
        {
            ResetPlotStatus(enum_CampFarmItemStatus.Decayed, showDecayAnim);
            m_StampLeft = 0;
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
