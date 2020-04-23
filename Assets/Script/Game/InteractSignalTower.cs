using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractSignalTower : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.SignalTower;
    Transform tf_Canvas;
    Text m_Count;
    public Transform m_PortalPos { get; private set; }
    TimerBase m_ActivateTimer = new TimerBase(GameConst.F_SignalTowerTransmitDuration);
    EntityCharacterBase m_Activator;
    bool m_Activated=>m_Activator;
    bool m_Transmitting = false;
    Func<InteractSignalTower, bool> OnTransmitCountFinish;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_Canvas = transform.Find("Canvas");
        m_Count = tf_Canvas.Find("Count").GetComponent<Text>();
        m_PortalPos = transform.Find("PortalPos");
    }

    public InteractSignalTower Play(Func<InteractSignalTower, bool> OnTransmitCountFinish)
    {
        base.Play();
        m_Activator = null;
        tf_Canvas.SetActivate(false);
        m_Transmitting = false;
        this.OnTransmitCountFinish = OnTransmitCountFinish;
        return this;
    }
    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Activator = _interactor;
        m_ActivateTimer.Replay();
        tf_Canvas.SetActivate(true);
        m_Count.text = "0";
        m_Transmitting = true;
        return false;
    }
    
    private void Update()
    {
        if (!m_Activated)
            return;

        tf_Canvas.rotation = CameraController.CameraProjectionOnPlane(tf_Canvas.position);
        m_ActivateTimer.Tick(Time.deltaTime);

        if (m_Transmitting && !m_ActivateTimer.m_Timing)
            m_Transmitting = !OnTransmitCountFinish(this);

        m_Count.text = ((int)((1-m_ActivateTimer.m_TimeLeftScale)*80f+(m_Transmitting?0f:20f))).ToString();
    }
}
