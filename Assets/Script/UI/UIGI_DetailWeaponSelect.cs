using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_DetailWeaponSelect : UIT_GridItem,IGridHighlight {

    Image m_Image;
    Transform m_Highlight;

    public override void OnInitItem()
    {
        base.OnInitItem();
        m_Image = rtf_Container.Find("Image").GetComponent<Image>();
        m_Highlight = rtf_Container.Find("Highlight");
        GetComponent<Button>().onClick.AddListener(()=> { OnButtonClick(m_Identity); });
    }

    public void Init(enum_PlayerWeapon weapon)
    {
        m_Image.sprite = UIManager.Instance.m_WeaponSprites[weapon.GetIconSprite()];
    }

    Action<int> OnButtonClick;
    public void AttachSelectButton(Action<int> OnButtonClick)
    {
        this.OnButtonClick = OnButtonClick;
    }
    public void OnHighlight(bool highlight) => m_Highlight.SetActivate(highlight);
}
