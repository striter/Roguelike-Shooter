using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using UnityEngine.UI;

public class UI_ActionAdjustment : UIPageBase {

    UIT_GridControllerMonoItem<UIGI_ActionItemSelect> m_Grid;
    Button btn_remove, btn_upgrade;
    UIT_TextExtend txt_removeAmount, txt_upgradeAmount;
    UI_MessageBoxRemove m_Remove;
    UI_MessageBoxUpgrade m_Upgrade;
    int m_selectIndex = -1;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerMonoItem<UIGI_ActionItemSelect>(tf_Container.Find("ScrollView/Viewport/ActionGrid"));
        m_Grid.transform.localScale = UIManager.Instance.m_FittedScale;
        btn_remove = tf_Container.Find("BtnRemove").GetComponent<Button>();
        btn_remove.onClick.AddListener(OnRemoveClick);
        txt_removeAmount = btn_remove.transform.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
        btn_upgrade = tf_Container.Find("BtnUpgrade").GetComponent<Button>();
        btn_upgrade.onClick.AddListener(OnUpgradeClick);
        txt_upgradeAmount = btn_upgrade.transform.Find("Cost/Amount").GetComponent<UIT_TextExtend>();
        m_Remove = transform.Find("Remove").GetComponent<UI_MessageBoxRemove>();
        m_Upgrade = transform.Find("Upgrade").GetComponent<UI_MessageBoxUpgrade>();
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
        for (int i = 0; i < m_Interact.m_Interactor.m_ActionStored.Count; i++)
            m_Grid.AddItem(i).SetInfo(m_Interact.m_Interactor.m_ActionStored[i],OnItemSelected,true);
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
    void OnRemoveClick()=>m_Remove.Play(m_Interact.RemovePrice,m_Interact.m_Interactor.m_ActionStored[m_selectIndex],OnRemoveConfirmed);
    void OnRemoveConfirmed()
    {
        m_Interact.OnRemovalAction(m_selectIndex);
        OnActionStatus();
    }

    void OnUpgradeClick() => m_Upgrade.Play(m_Interact.UpgradePrice, m_Interact.m_Interactor.m_ActionStored[m_selectIndex], OnUpgradeConfirmed);
    void OnUpgradeConfirmed()
    {
        m_Interact.OnUpgradeAction(m_selectIndex);
        OnActionStatus();
    }
}
