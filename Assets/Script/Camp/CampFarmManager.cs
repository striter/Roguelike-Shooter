using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;
using TTime;
public class CampFarmManager : SimpleSingletonMono<CampFarmManager>
{
    public float f_TimeScale=1f;
    public List<CampFarmPlot> m_Plots { get; private set; } = new List<CampFarmPlot>();
    public int m_LastProfitStamp { get; private set; } = 0;
    Transform tf_Plot, tf_Status, tf_FarmCameraPos;
    Action OnExitFarm;
    CampFarmPlot m_HybridPlot;
    RaycastHit m_rayHit;
    UIC_FarmStatus m_FarmStatus;
    protected override void Awake()
    {
        base.Awake();
        tf_Plot = transform.Find("Plot");
        tf_Status = transform.Find("Status");
        tf_FarmCameraPos = transform.Find("CameraPos");
    }
    public void OnCampEnter()
    {
        TCommon.TraversalEnum((enum_CampFarmItemStatus status) =>
        {
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Register(status, tf_Status.Find(status.ToString()).GetComponent<CampFarmItem>(), 1, null);
        });

        if (tf_Plot.childCount != GameDataManager.m_CampFarmData.m_PlotStatus.Count)
            GameDataManager.RecreateCampFarmData();

        m_LastProfitStamp = GameDataManager.m_CampFarmData.m_OffsiteProfitStamp;
        int stampNow = TTimeTools.GetTimeStampNow();

        float offcampProfit = 0;
        for (int i = 0; i < GameDataManager.m_CampFarmData.m_PlotStatus.Count; i++)
        {
            CampFarmPlot plot = tf_Plot.Find("Plot" + i.ToString()).GetComponent<CampFarmPlot>();
            offcampProfit += plot.Init(i,GameDataManager.m_CampFarmData.m_PlotStatus[i], m_LastProfitStamp, stampNow,OnPlotStatusChanged);
            m_Plots.Add(plot);
        }

        m_LastProfitStamp = stampNow;
        CampManager.Instance.OnCreditStatus(offcampProfit);
        GameDataManager.SaveCampFarmData(this);
    }
    private void OnDisable()
    {
        m_LastProfitStamp = TTimeTools.GetTimeStampNow();
        GameDataManager.SaveCampFarmData(this);
        ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.OnSceneChange();
    }
    public Transform Begin(Action _OnExitFarm)
    {
        OnExitFarm = _OnExitFarm;
        m_FarmStatus = CampUIManager.Instance.BeginFarm(OnDragDown, OnDrag);
        m_FarmStatus.Play(m_Plots, OnFarmBuy,OnFarmClear);
        return tf_FarmCameraPos;
    }

    void OnExit()
    {
        CampUIManager.Instance.ExitFarm();
        m_FarmStatus = null;
        OnExitFarm();

        if (!m_HybridPlot)
            return;
        m_HybridPlot.EndDrag();
        m_HybridPlot = null;
    }

    int curStamp = 0;
    private void Update()
    {
        int stampNow= TTimeTools.GetTimeStampNow();
        if (curStamp == stampNow)
            return;
        curStamp = stampNow;

        for (int i = 0; i < m_Plots.Count; i++)
        {
            float profit = m_Plots[i].TickProfit(stampNow);
            if (profit <= 0)
                continue;

            CampManager.Instance.OnCreditStatus(profit);
            if( m_FarmStatus) m_FarmStatus.OnProfitChange(i, profit);
        }

    }

    void OnHybrid(CampFarmPlot _plotDrag,CampFarmPlot _plotTarget)
    {
        if (_plotDrag.m_Status != _plotTarget.m_Status)
            return;

        enum_CampFarmItemStatus hybridStatus = _plotDrag.m_Status;
        if (hybridStatus != enum_CampFarmItemStatus.Progress5) hybridStatus++;
        _plotTarget.Hybrid(hybridStatus);
        _plotDrag.Clear();
    }

    void OnFarmBuy(int plotIndex)
    {
        if (m_Plots[plotIndex].m_Status != enum_CampFarmItemStatus.Empty || GameDataManager.m_GameData.f_Credits < GameConst.I_CampFarmItemAcquire)
            return;

        m_Plots[plotIndex].Hybrid(TCommon.RandomPercentage(GameExpression.GetFarmGeneratePercentage));
        CampManager.Instance.OnCreditStatus(-GameConst.I_CampFarmItemAcquire);
    }

    void OnFarmClear(int plotIndex)
    {
        if (m_Plots[plotIndex].m_Status != enum_CampFarmItemStatus.Decayed)
            return;

        m_Plots[plotIndex].Clear();
    }

    void OnPlotStatusChanged(int index)
    {
        if (m_FarmStatus)
            m_FarmStatus.UpdatePlot(index);
    }
    #region Interact
    void OnDragDown(bool down,Vector2 inputPos)
    {
        CampFarmPlot targetInteract = null;
        if (CameraController.Instance.InputRayCheck(inputPos, GameLayer.Mask.I_Interact, ref m_rayHit))
            targetInteract = m_rayHit.collider.GetComponent<CampFarmPlot>();


        if (targetInteract)
            OnDragInteract(targetInteract,down);
     

        if (!down && m_HybridPlot)
        {
            m_HybridPlot.EndDrag();
            m_HybridPlot = null;
        }
    }
    void OnDragInteract(CampFarmPlot interact, bool down)
    {

        CampFarmPlot targetPlot = interact as CampFarmPlot;

        if (down && targetPlot.m_Status == enum_CampFarmItemStatus.Decayed)
        {
            targetPlot.Clear();
            return;
        }

        if (targetPlot != m_HybridPlot && GameExpression.CanGenerateprofit(targetPlot.m_Status))
        {
            if (down)
            {
                m_HybridPlot = targetPlot;
                m_HybridPlot.BeginDrag();
                return;
            }

            if (!down && m_HybridPlot)
                OnHybrid(m_HybridPlot, targetPlot);
        }
    }

    void OnDrag(Vector2 inputPos)
    {
        if (!m_HybridPlot)
            return;

        if (CameraController.Instance.InputRayCheck(inputPos, GameLayer.Mask.I_Static, ref m_rayHit))
            m_HybridPlot.Move(m_rayHit.point);
    }
    #endregion
}
