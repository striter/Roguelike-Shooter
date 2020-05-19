using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractSignalTower : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.SignalTower;
    public Transform m_Canvas { get; private set; }
    Text m_Count;
    public Transform m_PortalPos { get; private set; }
    Action OnSignalTowerTrigger;

    bool m_Available = false;
    PE_DepthCircleArea m_TransmitArea;

    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        m_Canvas = transform.Find("Canvas");
        m_Count = m_Canvas.Find("Count").GetComponent<Text>();
        m_PortalPos = transform.Find("PortalPos");
    }

    public InteractSignalTower Play(Action OnSignalTowerTrigger)
    {
        base.Play();
        m_Canvas.SetActivate(false);
        m_Available = false;
        this.OnSignalTowerTrigger = OnSignalTowerTrigger;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Canvas.SetActivate(true);
        m_Count.text = "0";
        OnSignalTowerTrigger();
        return false;
    }


    public void OnTransmitSet(bool begin)
    {
        m_TransmitArea=CameraController.Instance.m_Effect.SetDepthAreaCircle(begin, transform.position, 10f, .3f, 1f).SetTexture(TResources.m_HolographTex, .5f, new Vector2(.5f, .5f));
        if (!begin)
            m_TransmitArea = null;
        SetAvailable(true);
    }

    public void Tick(float deltaTime,float progress,bool available)
    {
        m_Canvas.rotation = CameraController.CameraProjectionOnPlane(m_Canvas.position);
        m_Count.text = ((int)progress).ToString();
        SetAvailable(available);
    }

    void SetAvailable(bool available)
    {
        if (m_Available == available)
            return;

        m_TransmitArea.SetColor(available ? TCommon.GetHexColor("2DFFF62F") : TCommon.ColorAlpha(Color.red, .7f), TCommon.GetHexColor("02025EFF"));
        m_Available = available;
    }
}
