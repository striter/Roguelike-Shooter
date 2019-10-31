using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class CampEnvironment : SimpleSingletonMono<CampEnvironment>
{
    Transform tf_Farm, tf_Plot, tf_Status, tf_FarmCameraPos;
    List<CampFarmPlot> m_Plots=new List<CampFarmPlot>();
    Action OnExitFarm;
    CampFarmPlot m_HybridPlot;
    RaycastHit m_rayHit;
    protected override void Awake()
    {
        base.Awake();
        tf_Farm = transform.Find("Farm");
        tf_Plot = tf_Farm.Find("Plot");
        tf_Status = tf_Farm.Find("Status");
        tf_FarmCameraPos = tf_Farm.Find("CameraPos");
    }
    private void Start()
    {
        TCommon.TraversalEnum((enum_CampFarmItem status) =>
        {
            ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.Register(status, tf_Status.Find(status.ToString()).GetComponent<CampFarmItem>(), 1, null);
        });

        for (int i = 0; i < tf_Plot.childCount; i++)
        {
            CampFarmPlot plot = tf_Plot.Find("Plot" + i.ToString()).GetComponent<CampFarmPlot>();
            plot.Init(i,GameDataManager.m_CampFarmData.m_PlotStatus[i]);
            m_Plots.Add(plot);
        }
    }
    private void OnDisable()
    {
        ObjectPoolManager<enum_CampFarmItem, CampFarmItem>.OnSceneChange();
    }

    public Transform BeginFarm(Action _OnExitFarm)
    {
        OnExitFarm = _OnExitFarm;
        CampUIManager.Instance.BeginFarm(OnDragDown,OnDrag, OnFarmBuy,EndFarm);
        return tf_FarmCameraPos;
    }

    void EndFarm()
    {
        OnExitFarm();
        GameDataManager.SaveCampFarmData(m_Plots);
        if (!m_HybridPlot)
            return;
        m_HybridPlot.EndDrag();
        m_HybridPlot = null;
    }

    void OnFarmBuy()
    {
        CampFarmPlot emptyPlot = GetItemEmptySlot();
        if (emptyPlot==null)
            return;

        emptyPlot.Hybrid(TCommon.RandomPercentage(GameExpression.GetFarmGeneratePercentage));
        GameDataManager.SaveCampFarmData(m_Plots);
    }

    void OnHybrid(CampFarmPlot _plotDrag,CampFarmPlot _plotTarget)
    {
        if (_plotDrag.m_Status != _plotTarget.m_Status)
            return;

        enum_CampFarmItem hybridStatus = _plotDrag.m_Status;
        if (hybridStatus != enum_CampFarmItem.Progress5) hybridStatus++;
        _plotTarget.Hybrid(hybridStatus);
        _plotDrag.Clear();
        GameDataManager.SaveCampFarmData(m_Plots);
    }

    CampFarmPlot GetItemEmptySlot()
    {
        if (GameDataManager.m_PlayerCampData.f_Credits < GameConst.I_CampFarmItemAcquire)
            return null;

        for (int i = 0; i < m_Plots.Count; i++)
        {
            if (m_Plots[i].m_Status == enum_CampFarmItem.Empty)
                return m_Plots[i];
        }
        return null;
    }

    #region DragNDrop
    void OnDragDown(bool down,Vector2 inputPos)
    {
        CampFarmPlot targetPlot = null;
        if (CameraController.Instance.InputRayCheck(inputPos, GameLayer.Mask.I_Interact, ref m_rayHit))
            targetPlot = m_rayHit.collider.GetComponent<CampFarmPlot>();

        if(targetPlot!=null)
        {
            if (down && targetPlot.m_Status == enum_CampFarmItem.Decayed)
            {
                targetPlot.Clear();
                return;
            }

            if (CanHybridPlot(targetPlot))
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

    bool CanHybridPlot(CampFarmPlot plot)
    {
        if (plot == m_HybridPlot)
            return false;

        switch (plot.m_Status)
        {
            default: return false;
            case enum_CampFarmItem.Progress1:
            case enum_CampFarmItem.Progress2:
            case enum_CampFarmItem.Progress3:
            case enum_CampFarmItem.Progress4:
            case enum_CampFarmItem.Progress5:
                return true;
        }
    }
    #endregion
}
