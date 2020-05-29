using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
public class UIGI_DetailPerkSelect : UIT_GridItem,IGridHighlight {

    Image m_Image;
    Text m_Count;
    Transform m_Highlight;

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        m_Highlight = rtf_Container.Find("Highlight");
        m_Count = rtf_Container.Find("Count").GetComponent<Text>();
        GetComponent<Button>().onClick.AddListener(() => { OnButtonClick(m_Identity); });
    }

    public void Init(ExpirePlayerPerkBase expirePerk)
    {
        m_Image.sprite = BattleUIManager.Instance.m_ExpireSprites[expirePerk.m_Index.ToString()];
        bool stacked = expirePerk.m_Stack > 1;
        m_Count.SetActivate(stacked);
        if (stacked)
            m_Count.text = "x" + expirePerk.m_Stack;
    }

    Action<int> OnButtonClick;
    public void AttachSelectButton(Action<int> OnButtonClick)
    {
        this.OnButtonClick = OnButtonClick;
    }
    public void OnHighlight(bool highlight) => m_Highlight.SetActivate(highlight);
}
