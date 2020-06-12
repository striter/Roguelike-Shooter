using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_TreasureChest : UIT_GridItem, IGridHighlight
{
    [SerializeField] Text m_name;
    [SerializeField] Image m_picture;

    int m_posId;
    Prize m_data;
    Vector3 m_pos;
    Action<int> OnButtonClick;
    int num = 415;
    void Update()
    {
        if (transform.position.x < -500&& transform.position.x>-2000)
        {
            //Debug.Log(UI_ShoppingMall.m_coordinate);
            UI_ShoppingMall.m_coordinate += num;
            transform.localPosition = new Vector3(UI_ShoppingMall.m_coordinate, 0);
            Prize prize = UI_ShoppingMall.Instance.RandomNumber();
            if (prize != null)
            {
                OnPlay(prize);
            }
        }
        //Debug.Log(UI_ShoppingMall.Instance.m_animator.transform.localPosition.x);
        if (UI_ShoppingMall.Instance.m_animator.transform.localPosition.x == -830)
        {
            transform.localPosition = m_pos;
        }

        if (m_posId == 2 && UI_ShoppingMall.Instance.m_animator.transform.localPosition.x <= -13270)
        {
            if (!UI_ShoppingMall.m_awardWinning)
            {
                UI_ShoppingMall.m_awardWinning = true;
                UI_ShoppingMall.Instance.AwardWinning(m_data);
                Debug.Log("获取" + TLocalization.GetKeyLocalized(m_data.m_Name));
            }
        }
    }
    public void OnPlay(Prize data)
    {
        m_data = data;
        m_name.text = TLocalization.GetKeyLocalized(data.m_Name);
        m_picture.overrideSprite = data.m_sprite;
    }
    public void SetLocation(Vector3 pos,int posId)
    {
        m_pos = pos;
        transform.localPosition = pos;
        m_posId = posId;
    }
    public void AttachSelectButton(Action<int> OnButtonClick) => this.OnButtonClick = OnButtonClick;
    public void OnHighlight(bool highlight)
    {

    }
}
