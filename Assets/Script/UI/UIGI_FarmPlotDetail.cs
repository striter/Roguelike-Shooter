using GameSetting;
using System;
using UnityEngine;
using UnityEngine.UI;
public class UIGI_FarmPlotDetail : UIT_GridItem,ISingleCoroutine {
    CampFarmPlot m_Plot;
    Text m_status, m_progress, m_coin;
    Button m_Buy,m_Clear;
    Action<int> OnPlotBuyClick, OnPlotClearClick;
    public override void Init()
    {
        base.Init();
        m_status = tf_Container.Find("Status").GetComponent<Text>();
        m_progress = tf_Container.Find("Progress").GetComponent<Text>();
        m_coin = tf_Container.Find("Coin").GetComponent<Text>();
        m_Buy = tf_Container.Find("Buy").GetComponent<Button>();
        m_Buy.onClick.AddListener(OnBuyClick);
        m_Clear = tf_Container.Find("Clear").GetComponent<Button>();
        m_Clear.onClick.AddListener(OnClearClick);
        m_coin.SetActivate(false);
    }

    private void OnDisable()
    {
        m_Plot = null;
        this.StopAllSingleCoroutines();
    }

    public void SetPlotInfo(CampFarmPlot _plot,Action<int> _OnPlotBuyClick,Action<int> _OnPlotClearClick)
    {
        m_Plot = _plot;
        OnPlotBuyClick = _OnPlotBuyClick;
        OnPlotClearClick = _OnPlotClearClick;
        UpdateInfo();
    } 

    public void UpdateInfo()
    {
        string lockText = "";
        if (I_Index == 3 && m_Plot.m_Status == enum_CampFarmItemStatus.Locked)
            lockText = "Unlock At Difficulty 3";
        else if (I_Index == 4 && m_Plot.m_Status == enum_CampFarmItemStatus.Locked)
            lockText = "Unlock At Difficulty 4";
        else if (I_Index == 5 && m_Plot.m_Status == enum_CampFarmItemStatus.Locked)
            lockText = "Unlock With Extra Credits";
        m_progress.text = lockText;
        m_status.text = m_Plot.m_Status.ToString();
        m_Buy.SetActivate(m_Plot.m_Status == enum_CampFarmItemStatus.Empty);
    }

    public void PlayProfit(float profit)
    {
        m_coin.text = profit.ToString();
        m_coin.SetActivate(true);
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> { m_coin.rectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, new Vector2(0, 200), value); },0,1,2f,()=> { m_coin.SetActivate(false); }));
    }

    void OnBuyClick()
    {
        OnPlotBuyClick?.Invoke(I_Index);
    }
    void OnClearClick()
    {
        OnPlotClearClick?.Invoke(I_Index);
    }

    private void Update()
    {
        if (!m_Plot)
            return;
        rtf_RectTransform.SetWorldViewPortAnchor(m_Plot.transform.position, CameraController.MainCamera, .1f);
        if (m_Plot.m_Status == enum_CampFarmItemStatus.Locked)
            return;
        m_progress.text = m_Plot.m_TimeLeft.ToString();
    }
}
