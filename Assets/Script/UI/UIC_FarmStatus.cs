using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_FarmStatus : UIControlBase {
    
    Text m_ProfitAmount;
    UIT_GridControllerGridItem<UIGI_FarmPlotDetail> m_PlotGrid;
    protected override void Init()
    {
        base.Init();
        m_PlotGrid = new UIT_GridControllerGridItem<UIGI_FarmPlotDetail>(transform.Find("DetailGrid"));
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public void Play(List<CampFarmPlot> plots, Action<int> _OnBuyClick,Action<int> _OnClearClick)
    {
        m_PlotGrid.ClearGrid();
        for (int i = 0; i < plots.Count; i++)
        {
            m_PlotGrid.AddItem(i).SetPlotInfo(plots[i], _OnBuyClick,_OnClearClick);
        }
    }

    public void UpdatePlot(int index) => m_PlotGrid.GetItem(index).UpdateInfo();

    public void OnProfitChange(int plotIndex,float profitOffset)
    {
        m_PlotGrid.GetItem(plotIndex).PlayProfit(profitOffset);
    }
}
