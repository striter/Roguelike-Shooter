using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIGI_ActionItemBase : UIT_GridItem {
    Image m_ActionImage;
    Image m_TypeIcon,m_TypeBottom;
    UIC_RarityLevel m_Rarity;
    protected UIT_TextExtend m_Cost { get; private set; }
    UIT_TextExtend m_Name;
    Image m_Costable;
    Transform tf_Cost;
    public override void Init()
    {
        base.Init();
        m_ActionImage = tf_Container.Find("Icon/Image").GetComponent<Image>();
        m_TypeIcon = tf_Container.Find("Type/Icon").GetComponent<Image>();
        m_TypeBottom = tf_Container.Find("Type/Bottom").GetComponent<Image>();
        m_Rarity = new UIC_RarityLevel(tf_Container.Find("ActionRarity"));
        tf_Cost = tf_Container.Find("Cost");
        m_Costable = tf_Cost.Find("Cost").GetComponent<Image>();
        m_Cost = tf_Cost.Find("Amount").GetComponent<UIT_TextExtend>();
        m_Name = tf_Container.Find("Name").GetComponent<UIT_TextExtend>();
    }
    public virtual void SetInfo(ActionBase actionInfo)
    {
        m_Name.localizeKey = actionInfo.GetNameLocalizeKey();
        m_TypeIcon.sprite = UIManager.Instance.m_ActionSprites[actionInfo.m_ActionType.GetIconSprite()];
        m_TypeBottom.sprite = UIManager.Instance.m_ActionSprites[actionInfo.m_ActionType.GetNameBGSprite()];
        m_Rarity.SetRarity(actionInfo.m_rarity);
        m_ActionImage.sprite = UIManager.Instance.m_ActionSprites.Contains(actionInfo.m_Index.ToString())? UIManager.Instance.m_ActionSprites[actionInfo.m_Index.ToString()]:null;
        m_Costable.sprite = UIManager.Instance.m_ActionSprites[actionInfo.m_ActionType.GetCostBGSprite()];
        tf_Cost.SetActivate(actionInfo.m_ActionType == enum_ActionType.Ability);
    }
}
