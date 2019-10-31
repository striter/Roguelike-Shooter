using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
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

    Transform previousTrans;
    public void BeginFarm()
    {
        previousTrans = CameraController.Instance.tf_AttachTo;
        CameraController.Instance.Attach(tf_FarmCameraPos);
        CameraController.Instance.CameraLookAt(tf_FarmCameraPos);
    }

    public void EndFarm()
    {
        CameraController.Instance.Attach(previousTrans);
        CameraController.Instance.CameraLookAt(null);
    }
}
