using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_FarmStatus : UIControlBase {
    
    Text m_ProfitAmount;
    UIT_GridControllerGridItem<UIGI_FarmPlotDetail> m_PlotGrid;
    UIT_GridControllerGridItem<UIGI_FarmProfit> m_ProfitAnim;
    protected override void Init()
    {
        base.Init();
        m_PlotGrid = new UIT_GridControllerGridItem<UIGI_FarmPlotDetail>(transform.Find("DetailGrid"));
        m_ProfitAnim = new UIT_GridControllerGridItem<UIGI_FarmProfit>(transform.Find("ProfitGrid"));
    }
    public void Play(List<CampFarmPlot> plots, Action<int> _OnBuyClick,Action<int> _OnClearClick)
    {
        m_PlotGrid.ClearGrid();
        for (int i = 0; i < plots.Count; i++)
            m_PlotGrid.AddItem(i).SetPlotInfo(plots[i], _OnBuyClick,_OnClearClick);
    }
    public void UpdatePlot(int index) => m_PlotGrid.GetItem(index).UpdateInfo();

    int profitIndex = 0;
    public void OnProfitChange(Vector3 position,float profitOffset)=>  m_ProfitAnim.AddItem(profitIndex++).Play(position,profitOffset,OnProfitAnimFinished);
    void OnProfitAnimFinished(int index) => m_ProfitAnim.RemoveItem(index);
    public void StampTick()
    {
        m_PlotGrid.TraversalItem((int index, UIGI_FarmPlotDetail item)=>item.StampTick());
    }
}
