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
    UIT_FarmStatus m_FarmStatus;
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
        int stampNow = TTimeTools.GetTimeStampNow();

        for (int i = 0; i < GameDataManager.m_CampFarmData.m_PlotStatus.Count; i++)
        {
            CampFarmPlot plot = tf_Plot.Find("Plot" + i.ToString()).GetComponent<CampFarmPlot>();
            m_Profit += plot.Init(i,GameDataManager.m_CampFarmData.m_PlotStatus[i], m_LastProfitStamp, stampNow);
            m_Plots.Add(plot);
        }

        m_LastProfitStamp = stampNow;
        f_timeCheck = GameConst.I_CampFarmStampCheck;
        GameDataManager.SaveCampFarmData(this);
    }
    private void OnDisable()
    {
        int stampNow = TTimeTools.GetTimeStampNow();
        for (int i = 0; i < m_Plots.Count; i++)
            m_Profit += m_Plots[i].EndProfit(stampNow);
        m_LastProfitStamp = stampNow;
        GameDataManager.SaveCampFarmData(this);
        ObjectPoolManager<enum_CampFarmItemStatus, CampFarmItem>.OnSceneChange();
    }

    public Transform Begin(Action _OnExitFarm)
    {
        OnExitFarm = _OnExitFarm;
        m_FarmStatus = CampUIManager.Instance.BeginFarm(OnDragDown, OnDrag);
        m_FarmStatus.Play(m_Plots, OnExit, OnFarmBuy, OnCollectProfit);
        m_FarmStatus.OnProfitChange(m_Profit);
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

    private void Update()
    {
        if(f_timeCheck>0)
        {
            f_timeCheck -= Time.unscaledDeltaTime;
            return;
        }
        f_timeCheck = GameConst.I_CampFarmStampCheck;

        int stampNow= TTimeTools.GetTimeStampNow();
        for (int i = 0; i < m_Plots.Count; i++)
        {
            float profit = m_Plots[i].TickProfit(stampNow);
            m_Profit += profit;

            if (profit>0&& m_FarmStatus) m_FarmStatus.OnProfitChange(i,m_Profit, profit);
        }

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

    void OnCollectProfit()
    {
        CampManager.Instance.OnCreditStatus(m_Profit);
        m_Profit = 0;
        if (m_FarmStatus) m_FarmStatus.OnProfitChange(m_Profit);
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
    void OnDragDown(bool down,Vector2 inputPos)
    {
        CampFarmInteract targetInteract = null;
        if (CameraController.Instance.InputRayCheck(inputPos, GameLayer.Mask.I_Interact, ref m_rayHit))
            targetInteract = m_rayHit.collider.GetComponent<CampFarmInteract>();


        if (targetInteract)
            OnDragInteract(targetInteract,down);
     

        if (!down && m_HybridPlot)
        {
            m_HybridPlot.EndDrag();
            m_HybridPlot = null;
        }
    }
    void OnDragInteract(CampFarmInteract interact, bool down)
    {
        if (interact.B_IsRecycle && !down && m_HybridPlot)
        {
            m_HybridPlot.Clear();
            m_HybridPlot = null;
            return;
        }

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
