using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_CharacterSelectItem : UIT_GridItem,IGridHighlight {
    Transform m_Selected,m_Normal;
    Image m_Image;
    Action<int> OnButtonClick;
    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Normal = rtf_Container.Find("Normal");
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        m_Selected = rtf_Container.Find("Selected");
        GetComponent<Button>().onClick.AddListener(() => { OnButtonClick(m_Identity); });
    }

    public override void OnAddItem(int identity)
    {
        base.OnAddItem(identity);
        m_Image.sprite = UIManager.Instance.m_CharacterSprites[((enum_PlayerCharacter)identity).GetIconSprite()]; 
    }

    public void AttachSelectButton(Action<int> OnButtonClick)=>this.OnButtonClick = OnButtonClick;

    public void OnHighlight(bool highlight)
    {
        m_Selected.SetActivate(highlight);
        m_Normal.SetActivate(!highlight);
        m_Image.color = TCommon.ColorAlpha(Color.white, highlight ? 1f : .6f);
    }

}
