using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_ActionAdjustment : UIPageBase {

    UIT_GridControllerGridItem<UIGI_ActionItemSelect> m_Grid;
    Button btn_remove, btn_upgrade;
    UIT_TextExtend txt_removeAmount, txt_upgradeAmount;
    int m_selectIndex = -1;
    protected override void Init()
    {
        base.Init();
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemSelect>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
        btn_remove = tf_Container.Find("BtnRemove").GetComponent<Button>();
        btn_remove.onClick.AddListener(OnRemoveClick);
        txt_removeAmount = btn_remove.transform.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
        btn_upgrade = tf_Container.Find("BtnUpgrade").GetComponent<Button>();
        btn_upgrade.onClick.AddListener(OnUpgradeClick);
        txt_upgradeAmount = btn_upgrade.transform.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
    }
    InteractActionAdjustment m_Interact;
    public void Play(InteractActionAdjustment _adjust)
    {
        m_Interact = _adjust;
        OnActionStatus();
    }
    void OnActionStatus()
    {
        m_Grid.ClearGrid();
        for (int i = 0; i < m_Interact.m_Interactor.m_ActionPlaying.Count; i++)
            m_Grid.AddItem(i).SetInfo(m_Interact.m_Interactor.m_ActionPlaying[i],OnItemSelected,true);
        m_selectIndex = -1;
        OnAdjustmentBtnStatus();
    }

    void OnItemSelected(int index)
    {
        if (m_selectIndex != -1)
            m_Grid.GetItem(m_selectIndex).SetHighlight(false);
        m_selectIndex = index;
        m_Grid.GetItem(m_selectIndex).SetHighlight(true);

       OnAdjustmentBtnStatus();
    }
    void OnAdjustmentBtnStatus()
    {
        txt_upgradeAmount.text = m_Interact.UpgradePrice.ToString();
        txt_removeAmount.text = m_Interact.RemovePrice.ToString();
        btn_upgrade.interactable = m_selectIndex >= 0 && m_Interact.E_UpgradeType(m_selectIndex) == enum_UI_ActionUpgradeType.Upgradeable;
        btn_remove.interactable = m_selectIndex >= 0 && m_Interact.B_Removeable(m_selectIndex);
    }

    void OnRemoveClick()=>UIManager.Instance.ShowMessageBox<UIM_ActionRemove>().Play(m_Interact.RemovePrice,m_Interact.m_Interactor.m_ActionPlaying[m_selectIndex],OnRemoveConfirmed);
    void OnRemoveConfirmed()
    {
        m_Interact.OnRemovalAction(m_selectIndex);
        OnActionStatus();
    }

    void OnUpgradeClick() => UIManager.Instance.ShowMessageBox<UIM_ActionUpgrade>().Play(m_Interact.UpgradePrice, m_Interact.m_Interactor.m_ActionPlaying[m_selectIndex], OnUpgradeConfirmed);
    void OnUpgradeConfirmed()
    {
        m_Interact.OnUpgradeAction(m_selectIndex);
        OnActionStatus();
    }
}
