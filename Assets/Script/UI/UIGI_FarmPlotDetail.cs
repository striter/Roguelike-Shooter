using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
using TTime;
public class UIGI_FarmPlotDetail : UIT_GridItem {
    Button m_Acquire;
    UIT_TextExtend m_AcquireAmount;
    Button m_Clear;
    Transform tf_Locked;
    UIT_TextExtend m_LockProj, m_LockText;
    Transform tf_Profiting;
    Slider m_Duration;
    UIT_TextExtend m_DurationLeft,m_Status;

    CampFarmPlot m_Plot;
    Action<int> OnPlotAcquireClick, OnPlotClearClick;
    public override void Init()
    {
        base.Init();
        m_Acquire = tf_Container.Find("Acquire").GetComponent<Button>();
        m_Acquire.onClick.AddListener(OnAcquireClick);
        m_AcquireAmount = m_Acquire.transform.Find("Amount").GetComponent<UIT_TextExtend>();
        m_Clear = tf_Container.Find("Clear").GetComponent<Button>();
        m_Clear.onClick.AddListener(OnClearClick);
        tf_Profiting = tf_Container.Find("Profiting");
        m_Duration = tf_Profiting.Find("Duration").GetComponent<Slider>();
        m_DurationLeft = tf_Profiting.Find("DurationLeft").GetComponent<UIT_TextExtend>();
        m_Status = tf_Profiting.Find("Status").GetComponent<UIT_TextExtend>();
        tf_Locked = tf_Container.Find("Locked");
        m_LockProj = tf_Locked.Find("LockProj").GetComponent<UIT_TextExtend>();
        m_LockText = m_LockProj.transform.Find("LockText").GetComponent<UIT_TextExtend>();
    }

    private void OnDisable()=> m_Plot = null;
    void OnAcquireClick() => OnPlotAcquireClick?.Invoke(m_Index);
    void OnClearClick() => OnPlotClearClick?.Invoke(m_Index);

    public void SetPlotInfo(CampFarmPlot _plot,Action<int> _OnPlotAcquireClick,Action<int> _OnPlotClearClick)
    {
        m_Plot = _plot;
        OnPlotAcquireClick = _OnPlotAcquireClick;
        OnPlotClearClick = _OnPlotClearClick;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        bool decayed = false;
        bool empty = false;
        bool locked = false;
        bool showProfiting = false;
        if (GameExpression.CanGenerateprofit(m_Plot.m_Status))
        {
            showProfiting = true;
            m_Status.localizeKey = m_Plot.m_Status.GetLocalizeKey();
            m_Duration.value = m_Plot.m_StampLeftScale;
            m_DurationLeft.text = m_Plot.m_Timeleft;
        }
        switch (m_Plot.m_Status)
        {
            case enum_CampFarmItemStatus.Locked:
                locked= true;
                string key="", value="" ;
                if (m_Index == 3) { key = "UI_FarmStatus_Unlock4";value = GameConst.I_CampFarmPlot4UnlockDifficulty.ToString(); }
                else if (m_Index == 4){ key = "UI_FarmStatus_Unlock5"; value = GameConst.I_CampFarmPlot5UnlockDifficulty.ToString(); }
                else if (m_Index == 5) { key = "UI_FarmStatus_Unlock6"; value = GameConst.I_CampFarmPlot6UnlockTechPoints.ToString(); }
                m_LockText.formatText(key,value);
                m_LockText.formatText(key,value);
                break;
            case enum_CampFarmItemStatus.Decayed:
                decayed = true;
                break;
            case enum_CampFarmItemStatus.Empty:
                empty = true;
                m_AcquireAmount.text = GameConst.I_CampFarmItemAcquire.ToString();
                break;
        }
        m_Clear.SetActivate(decayed);
        m_Acquire.SetActivate(empty);
        tf_Locked.SetActivate(locked);
        tf_Profiting.SetActivate(showProfiting);
    }

    private void Update()
    {
        if (!m_Plot)
            return;
        rtf_RectTransform.SetWorldViewPortAnchor(m_Plot.m_PlotItem.transform.position, CameraController.MainCamera, .2f);
        rtf_RectTransform.localScale = Vector3.one * Mathf.Lerp(1.5f, 1, rtf_RectTransform.anchorMin.y / 1f);
    }
    public void StampTick()
    {
        if (!m_Plot || !m_Plot.m_CanGenerateProfit)
            return;
        m_Duration.value = m_Plot.m_StampLeftScale;
        m_DurationLeft.text = m_Plot.m_Timeleft;
    }
}
