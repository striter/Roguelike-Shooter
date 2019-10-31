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
    public float m_Profit { get; private set; } = 0;
    public int m_LastProfitStamp { get; private set; } = 0;
    Transform tf_Plot, tf_Status, tf_FarmCameraPos;
    Action OnExitFarm;
    CampFarmPlot m_HybridPlot;
    RaycastHit m_rayHit;
    float f_timeCheck;
    protected override void Awake()
    {
        base.Awake();
        tf_Plot = transform.Find("Plot");
        tf_Status = transform.Find("Status");
        tf_FarmCameraPos = transform.Find("CameraPos");
    }
    private void Start()
    {
        TCommon.TraversalEnum((enum_CampFarmItemStatus status) =>
        {
            ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.Register(status, tf_Status.Find(status.ToString()).GetComponent<CampFarmItem>(), 1, null);
        });

        if (tf_Plot.childCount != GameDataManager.m_CampFarmData.m_PlotStatus.Count)
            GameDataManager.RecreateCampFarmData();

        m_Profit = GameDataManager.m_CampFarmData.m_Profit;
        m_LastProfitStamp = GameDataManager.m_CampFarmData.m_OffsiteProfitStamp;

        for (int i = 0; i < GameDataManager.m_CampFarmData.m_PlotStatus.Count; i++)
        {
            CampFarmPlot plot = tf_Plot.Find("Plot" + i.ToString()).GetComponent<CampFarmPlot>();
            plot.Init(i,GameDataManager.m_CampFarmData.m_PlotStatus[i]);
            m_Plots.Add(plot);
        }
        CheckProfit();
        f_timeCheck = GameConst.I_CampFarmProfitTickDuration;
    }
    private void OnDisable()
    {
        CheckProfit();
        ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.OnSceneChange();
    }

    private void Update()
    {
        if (f_timeCheck > 0)
        {
            f_timeCheck -= Time.deltaTime* f_TimeScale;
            return;
        }
        f_timeCheck = GameConst.I_CampFarmProfitTickDuration;
        CheckProfit();
    }

    void CheckProfit()
    {
        int stampNow = TTimeTools.GetTimeStampNow();
        for (int i = 0; i < m_Plots.Count; i++)
            m_Profit += m_Plots[i].GenerateProfit(m_LastProfitStamp, stampNow);
        m_LastProfitStamp = stampNow;
        GameDataManager.SaveCampFarmData(this);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampFarmProfitStatus, m_Profit);
    }

    void OnFarmBuy()
    {
        if (GameDataManager.m_PlayerCampData.f_Credits < GameConst.I_CampFarmItemAcquire)
            return;

        CampFarmPlot emptyPlot = GetItemEmptySlot();
        if (emptyPlot==null)
            return;
        emptyPlot.Hybrid(TCommon.RandomPercentage(GameExpression.GetFarmGeneratePercentage));

        CampManager.Instance.OnCreditStatus(-GameConst.I_CampFarmItemAcquire);
    }
    void OnProfitClick()
    {
        CampManager.Instance.OnCreditStatus(m_Profit);
        m_Profit = 0;
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampFarmProfitStatus, m_Profit);
        GameDataManager.SaveCampFarmData(this);
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

    CampFarmPlot GetItemEmptySlot()
    {
        for (int i = 0; i < m_Plots.Count; i++)
        {
            if (m_Plots[i].m_Status == enum_CampFarmItemStatus.Empty)
                return m_Plots[i];
        }
        return null;
    }

    #region Interact
    public Transform Begin(Action _OnExitFarm)
    {
        OnExitFarm = _OnExitFarm;
        CampUIManager.Instance.BeginFarm(OnDragDown, OnDrag, OnFarmBuy, End,OnProfitClick);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_CampFarmProfitStatus, m_Profit);
        return tf_FarmCameraPos;
    }

    void End()
    {
        OnExitFarm();
        if (!m_HybridPlot)
            return;
        m_HybridPlot.EndDrag();
        m_HybridPlot = null;
    }

    void OnDragDown(bool down,Vector2 inputPos)
    {
        CampFarmPlot targetPlot = null;
        if (CameraController.Instance.InputRayCheck(inputPos, GameLayer.Mask.I_Interact, ref m_rayHit))
            targetPlot = m_rayHit.collider.GetComponent<CampFarmPlot>();

        if(targetPlot!=null)
        {
            if (down && targetPlot.m_Status == enum_CampFarmItemStatus.Decayed)
            {
                targetPlot.Clear();
                return;
            }

            if (targetPlot != m_HybridPlot&&GameExpression.CanGenerateprofit(targetPlot.m_Status))
            {
                if (down)
                {
                    m_HybridPlot = targetPlot;
                    m_HybridPlot.BeginDrag();
                    return;
                }

                if (!down&&m_HybridPlot)
                    OnHybrid(m_HybridPlot, targetPlot);
            }
        }

        if (!down && m_HybridPlot)
        {
            m_HybridPlot.EndDrag();
            m_HybridPlot = null;
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
