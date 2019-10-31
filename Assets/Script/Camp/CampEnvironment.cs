using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class CampEnvironment : SimpleSingletonMono<CampEnvironment>
{
    Transform tf_Farm, tf_Plot, tf_Status, tf_FarmCameraPos;
    List<CampFarmPlot> m_Plots=new List<CampFarmPlot>();
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
    
    public void BeginFarm()
    {
        Debug.Log("Begin");
        CameraController.Instance.Attach(tf_FarmCameraPos);
        CameraController.Instance.CameraLookAt(tf_FarmCameraPos);
        CampUIManager.Instance.BeginFarm(OnDragDown,OnDrag, OnExitFarm);
    }

    void OnExitFarm()
    {
        Debug.Log("Exit");
        CameraController.Instance.Attach(CampManager.Instance.tf_PlayerHead);
        CameraController.Instance.CameraLookAt(null);
    }


    CampFarmItem m_Item;
    void OnDragDown(bool down,Vector2 inputPos)
    {
    }

    void OnDrag(Vector2 inputPos)
    {

    }
}
