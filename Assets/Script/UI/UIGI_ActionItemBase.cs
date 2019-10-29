﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionItemBase : UIT_GridItem {
    Image m_ActionImage;
    Image m_TypeIcon,m_TypeBottom;
    UIC_RarityLevel_BG m_Rarity;
    UIT_TextExtend m_Cost, m_Name;
    Image m_Costable;
    public override void Init()
    {
        base.Init();
        m_ActionImage = tf_Container.Find("Icon/Image").GetComponent<Image>();
        m_TypeIcon = tf_Container.Find("Type/Icon").GetComponent<Image>();
        m_TypeBottom = tf_Container.Find("Type/Bottom").GetComponent<Image>();
        m_Rarity = new UIC_RarityLevel_BG(tf_Container.Find("ActionRarity"));
        m_Costable = tf_Container.Find("Cost/Cost").GetComponent<Image>();
        m_Cost = tf_Container.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
    }
    protected void SetInfo(ActionBase actionInfo)
    {
        m_TypeIcon.sprite = UIManager.Instance.m_InGameSprites[actionInfo.m_ActionType.GetIconSprite()];
        m_TypeBottom.sprite = UIManager.Instance.m_InGameSprites[actionInfo.m_ActionType.GetNameBGSprite()];
        m_Rarity.SetLevel(actionInfo.m_rarity);
        m_Cost.text = actionInfo.I_Cost.ToString();
        m_Name.localizeText = actionInfo.GetNameLocalizeKey();
        m_ActionImage.sprite = UIManager.Instance.m_ActionSprites.Contains(actionInfo.m_Index.ToString())? UIManager.Instance.m_ActionSprites[actionInfo.m_Index.ToString()]:null;
        m_Costable.sprite = UIManager.Instance.m_InGameSprites[actionInfo.m_ActionType.GetCostBGSprite()];
    }

    protected void SetCostable(bool _costable)
    {
        m_Costable.SetActivate(_costable);
    }
}
