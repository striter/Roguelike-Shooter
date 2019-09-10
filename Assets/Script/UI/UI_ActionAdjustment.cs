using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_ActionAdjustment : UIPageBase {

    UIT_GridControllerMono<UIGI_ActionSelectItem> m_Grid;
    Button btn_remove, btn_upgrade;
    UIT_TextLocalization txt_remove, txt_upgrade,txt_coins;
    int m_selectIndex = -1;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerSingleSelecting<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"), OnItemSelected, true);
        txt_coins = tf_Container.Find("Coins").GetComponent<UIT_TextLocalization>();
        btn_remove = tf_Container.Find("BtnRemove").GetComponent<Button>();
        btn_remove.onClick.AddListener(OnRemoveClick);
        txt_remove = btn_remove.transform.Find("Text").GetComponent<UIT_TextLocalization>();
        btn_upgrade = tf_Container.Find("BtnUpgrade").GetComponent<Button>();
        btn_upgrade.onClick.AddListener(OnUpgradeClick);
        txt_upgrade = btn_upgrade.transform.Find("Text").GetComponent<UIT_TextLocalization>();
    }
    InteractActionAdjustment m_Interact;
    public void Play(InteractActionAdjustment _adjust)
    {
        m_Interact = _adjust;
        OnActionStatus();
    }
    void OnActionStatus()
    {
        txt_coins.text = m_Interact.m_Interactor.m_Coins.ToString();
        m_Grid.ClearGrid();
        for (int i = 0; i < m_Interact.m_Interactor.m_ActionStored.Count; i++)
            m_Grid.AddItem(i).SetInfo(m_Interact.m_Interactor.m_ActionStored[i]);
        m_selectIndex = -1;
        OnAdjustmentBtnStatus();
    }

    void OnItemSelected(int index)
    {
        if (m_selectIndex != -1)
            m_Grid.GetItem(m_selectIndex).SetHighLight(false);
        m_selectIndex = index;
        m_Grid.GetItem(m_selectIndex).SetHighLight(true);
       OnAdjustmentBtnStatus();
    }
    void OnAdjustmentBtnStatus()
    {
        if (m_selectIndex >= 0)
        {
            btn_upgrade.interactable = m_Interact.B_Upgradable(m_selectIndex);
            txt_upgrade.text = "Upgrade:" + (btn_upgrade.interactable ? m_Interact.UpgradePrice.ToString():"Action Max Level");
            btn_remove.interactable = m_Interact.B_Removeable(m_selectIndex);
            txt_remove.text = "Remove:" + m_Interact.RemovePrice.ToString();
        }
        else
        {
            btn_upgrade.interactable = false;
            btn_remove.interactable = false;
            txt_upgrade.text = "Upgrade";
            txt_remove.text = "Remove";
        }
    }
    void OnRemoveClick()
    {
        m_Interact.OnRemovalAction(m_selectIndex);
        OnActionStatus();
    }
    void OnUpgradeClick()
    {
        m_Interact.OnUpgradeAction(m_selectIndex);
        OnActionStatus();
    } 
}
