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
    TimerBase m_ActivateTimer = new TimerBase(GameConst.F_SignalTowerTransmitDuration);
    EntityCharacterBase m_Activator;
    bool m_Activated=>m_Activator;
    bool m_Transmitting = false;
    Func<InteractSignalTower, bool> OnTransmitCountFinish;
    Action<InteractSignalTower> OnTransmitStart;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        m_Canvas = transform.Find("Canvas");
        m_Count = m_Canvas.Find("Count").GetComponent<Text>();
        m_PortalPos = transform.Find("PortalPos");
    }

    public InteractSignalTower Play(Action<InteractSignalTower> OnTransmitStart, Func<InteractSignalTower, bool> OnTransmitCountFinish)
    {
        base.Play();
        m_Activator = null;
        m_Canvas.SetActivate(false);
        m_Transmitting = false;
        this.OnTransmitStart = OnTransmitStart;
        this.OnTransmitCountFinish = OnTransmitCountFinish;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Activator = _interactor;
        m_ActivateTimer.Replay();
        m_Canvas.SetActivate(true);
        m_Count.text = "0";
        m_Transmitting = true;
        OnTransmitStart(this);
        return false;
    }
    
    private void Update()
    {
        if (!m_Activated)
            return;

        m_Canvas.rotation = CameraController.CameraProjectionOnPlane(m_Canvas.position);
        m_ActivateTimer.Tick(Time.deltaTime);

        if (m_Transmitting && !m_ActivateTimer.m_Timing)
            m_Transmitting = !OnTransmitCountFinish(this);

        m_Count.text = ((int)((1-m_ActivateTimer.m_TimeLeftScale)*99f+(m_Transmitting?0f:1f))).ToString();
    }
}
