using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_Indicates : UIControlBase {

    protected UIT_GridControllerGridItem<UIGI_TipItem> m_TipsGrid;
    Transform tf_GameWarning;
    UIT_TextExtend m_WarningTitle, m_WarningText, m_WarningTips;

    protected override void Init()
    {
        base.Init();
        m_TipsGrid = new UIT_GridControllerGridItem<UIGI_TipItem>(transform.transform.Find("TipsGrid"));
        tf_GameWarning = transform.Find("GameWarning");
        m_WarningTitle = tf_GameWarning.Find("WarningTitle").GetComponent<UIT_TextExtend>();
        m_WarningText = tf_GameWarning.Find("WarningText").GetComponent<UIT_TextExtend>();
        m_WarningTips = tf_GameWarning.Find("WarningTips").GetComponent<UIT_TextExtend>();
        tf_GameWarning.SetActivate(false);
    }

    private void Update()
    {
        TickWarning(Time.deltaTime);
    }

    int i_tipCount = 0;
    public void ShowTip(string key, enum_UITipsType tipsType) => m_TipsGrid.AddItem(i_tipCount++).ShowTips(key, tipsType, OnTipFinish);
    void OnTipFinish(int index) => m_TipsGrid.RemoveItem(index);

    float f_warningCheck;
    public void ShowWarning(string warningKey,string warningTitle,string titleObj="", string warningTip="", float duration=2f)
    {
        m_WarningText.localizeKey = warningKey;
        m_WarningTitle.formatText(warningTitle,titleObj);

        bool showTips = warningTip != "";
        m_WarningTips.SetActivate(showTips);
        if (showTips) m_WarningTips.localizeKey = warningTip;
        f_warningCheck = duration;

        tf_GameWarning.SetActivate(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tf_GameWarning as RectTransform);
    }
    void TickWarning(float deltaTime)
    {
        if (f_warningCheck <= 0)
            return;
        f_warningCheck -= deltaTime;

        if (f_warningCheck <= 0)
            tf_GameWarning.SetActivate(false);
    }
}
