using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIT_FarmStatus : UIToolsBase {

    Button m_Buy, m_Exit,m_Profit;
    Text m_ProfitAmount;
    Action OnExitClick, OnBuyClick, OnProfitClick;
    UIT_GridControllerGridItem<UIGI_FarmPlotDetail> m_PlotGrid;
    protected override void Init()
    {
        base.Init();
        m_Buy = transform.Find("Buy").GetComponent<Button>();
        m_Buy.onClick.AddListener(()=> { OnBuyClick(); });
        m_Exit = transform.Find("Exit").GetComponent<Button>();
        m_Exit.onClick.AddListener(()=> {Destroy(this.gameObject); OnExitClick(); });
        m_Profit = transform.Find("Profit").GetComponent<Button>();
        m_Profit.onClick.AddListener(() => { OnProfitClick(); });
        m_ProfitAmount = m_Profit.transform.Find("Text").GetComponent<Text>();
        m_PlotGrid = new UIT_GridControllerGridItem<UIGI_FarmPlotDetail>(transform.Find("DetailGrid"));
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public void Play(List<CampFarmPlot> plots, Action _OnExitClick, Action _OnBuyClick,Action _OnProfitClick)
    {
        m_PlotGrid.ClearGrid();
        for (int i = 0; i < plots.Count; i++)
            m_PlotGrid.AddItem(i).SetPlotInfo(plots[i]);
        OnExitClick = _OnExitClick;
        OnBuyClick = _OnBuyClick;
        OnProfitClick = _OnProfitClick;
    }
    public void OnProfitChange(float profit) => m_ProfitAmount.text = profit.ToString();
    public void OnProfitChange(int plotIndex,float profit,float profitOffset)
    {
        OnProfitChange(profit);
        m_PlotGrid.GetItem(plotIndex).OnGenerateProfit(profitOffset);
    }
    
}
