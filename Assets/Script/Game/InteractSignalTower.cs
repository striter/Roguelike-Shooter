using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;
using UnityEngine.UI;

public class InteractSignalTower : InteractBattleBase {
    public override enum_Interaction m_InteractType => enum_Interaction.SignalTower;
    public Transform m_Canvas { get; private set; }
    public Transform m_PortalPos { get; private set; }

    Text m_Count;
    Action OnSignalTowerTrigger;
    PE_DepthCircleArea m_TransmitArea;
    ValueLerpSeconds m_TransmitColorLerp;

    Color colorFillValid = TCommon.GetHexColor("2DFF6B2F");
    Color colorFillInvalid = TCommon.ColorAlpha(Color.red, .5f);
    Color colorEdgeValid = TCommon.GetHexColor("00FF0CFF");
    Color colorEdgeInvalid = Color.red;


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
        this.OnSignalTowerTrigger = OnSignalTowerTrigger;
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Canvas.SetActivate(true);
        m_Count.text = "0";
        OnSignalTowerTrigger();

        GameDataManager.m_portal++;
        Debug.Log("开启信号塔");
        return false;
    }


    public void OnTransmitSet(bool begin)
    {
        m_TransmitArea = CameraController.Instance.m_Effect.SetDepthAreaCircle(begin, transform.position, 10f, .3f, 1f).SetTexture(TResources.m_HolographTex, .5f, new Vector2(.5f, .5f));
        if (begin)
        {
            m_TransmitColorLerp = new ValueLerpSeconds(0f, .5f, .2f, (float value)=> { m_TransmitArea.SetColor(Color.Lerp(colorFillInvalid, colorFillValid, value), Color.Lerp( colorEdgeInvalid, colorEdgeValid, value)); });
            m_TransmitColorLerp.SetFinalValue(1f);
        }
        else
        {
            m_TransmitArea = null;
            m_TransmitColorLerp = null;
        }
    }

    public void Tick(float deltaTime,float progress,bool available)
    {
        m_Canvas.rotation = CameraController.CameraProjectionOnPlane(m_Canvas.position);
        m_Count.text = ((int)progress).ToString();

        m_TransmitColorLerp.SetLerpValue(available?1f:0f);
        m_TransmitColorLerp.TickDelta(deltaTime);
    }
}
