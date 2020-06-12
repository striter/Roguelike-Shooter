﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_Commodity : UIT_GridItem, IGridHighlight
{
    Action<int> OnButtonClick;

    [SerializeField] UIT_TextExtend m_name;
    [SerializeField] Text m_timeRemaining;
    [SerializeField] Text m_introduce;
    [SerializeField] Image m_picture;
    [SerializeField] GameObject m_newObj;
    [SerializeField] GameObject m_goldCoinNew;
    [SerializeField] GameObject m_diamondsCoinNew;
    [SerializeField] Text m_num;
    [SerializeField] GameObject m_diamonds;
    [SerializeField] Text m_price;
    [SerializeField] Button m_button;
    [SerializeField] Commodity m_commodity;
    [SerializeField] GameObject m_purchased;

    public void Awake()
    {
        m_button.onClick.AddListener(OnClick);
    }

    public void OnPlay(Commodity data)
    {
        m_commodity = data;
        m_name.localizeKey= data.m_name;
        m_introduce.text = TLocalization.GetKeyLocalized(data.m_introduction);
        m_picture.overrideSprite = data.m_sprite;
        //设置剩余时间
        if (data.m_type == enum_CommodityType.LuckDraw)
        {
            m_timeRemaining.SetActivate(true);
        }
        else
        {
            m_timeRemaining.SetActivate(false);
        }
        //设置购买按钮样式
        if (data.m_type == enum_CommodityType.LuckDraw|| data.m_type == enum_CommodityType.GoldCoin)
        {
            m_diamonds.SetActivate(true);
            m_price.transform.localPosition = new Vector3(37, -4);
            m_price.text = data.m_price.ToString(); 
        }
        else
        {
            m_diamonds.SetActivate(false);
            m_price.transform.localPosition = new Vector3(0, -4);
            m_price.text = string.Format("￥{0}",data.m_price);
        }

        //设置简介文字位置
        if (data.m_type == enum_CommodityType.VIP || data.m_type == enum_CommodityType.Novice || data.m_type == enum_CommodityType.LuckDraw)
        {
            m_introduce.transform.localPosition = new Vector3(0, -135);
            m_newObj.SetActive(false);
        }
        else
        {
            m_introduce.transform.localPosition = new Vector3(0, -100);
            m_newObj.SetActive(true);
            m_num.text = data.m_num.ToString();
            if (data.m_type == enum_CommodityType.GoldCoin)
            {
                m_diamondsCoinNew.SetActive(false);
                m_goldCoinNew.SetActive(true);
            }
            else if (data.m_type == enum_CommodityType.Diamonds)
            {             
                m_goldCoinNew.SetActive(false);
                m_diamondsCoinNew.SetActive(true);
            }
        }

        if (data.m_type == enum_CommodityType.VIP && GameDataManager.m_CGameShopData.m_Vip == 1)
        {
            m_button.SetActivate(false);
            m_purchased.SetActive(true);
        }
        else if (data.m_type == enum_CommodityType.Novice && GameDataManager.m_CGameShopData.m_NoviceGiftBag == 1)
        {
            m_button.SetActivate(false);
            m_purchased.SetActive(true);
        }
        else
        {
            m_button.SetActivate(true);
            m_purchased.SetActive(false);
        }

    }
    void OnClick()
    {
        UI_ShoppingMall.Instance.Purchase(m_commodity);
    }
    public void AttachSelectButton(Action<int> OnButtonClick) => this.OnButtonClick = OnButtonClick;
    public void OnHighlight(bool highlight)
    {
        
    }
}
