using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_Indicates : UIControlBase {

    protected UIT_GridControllerGridItem<UIGI_TipItem> m_TipsGrid;

    protected override void Init()
    {
        base.Init();
        m_TipsGrid = new UIT_GridControllerGridItem<UIGI_TipItem>(transform.transform.Find("TipsGrid"));
    }
    
    int i_tipCount = 0;
    public UIT_TextExtend NewTip(enum_UITipsType tipsType) => m_TipsGrid.AddItem(i_tipCount++).Play(tipsType, OnTipFinish);
    void OnTipFinish(int index) => m_TipsGrid.RemoveItem(index);
    
}
