using GameSetting;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_ActionAcquire : UIPageBase {

    Button m_Confirm;
    UIT_GridControllerMonoItem<UIGI_ActionSelectItem> m_Grid;
    bool m_SingleGrid;
    Action<List<int>> OnIndexSelect;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        m_Confirm.onClick.AddListener(OnConfirmClick);
    }
    public void Play(List<ActionBase> actions,Action<List<int>> _OnIndexSelect, int selectAmount)
    {
        OnIndexSelect = _OnIndexSelect;
           m_SingleGrid = selectAmount == 1;
        if (m_SingleGrid)
            m_Grid = new UIT_GridDefaultSingle<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"), OnItemSelected, true);
        else
            m_Grid = new UIT_GridDefaultMulti<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"), selectAmount, OnItemSelected);
        m_Confirm.interactable = false;
        m_Grid.ClearGrid();
        for (int i = 0; i < actions.Count; i++)
            m_Grid.AddItem(i).SetInfo(actions[i]);
    }

    void OnItemSelected(int index)
    {
        m_Confirm.interactable = B_CanSelect;
    }
    bool B_CanSelect => m_SingleGrid ? true : (m_Grid as UIT_GridDefaultMulti<UIGI_ActionSelectItem>).m_AllSelected;
    List<int> m_SelectIndexes => m_SingleGrid ? new List<int>() { (m_Grid as UIT_GridDefaultSingle<UIGI_ActionSelectItem>).I_CurrentSelecting } : (m_Grid as UIT_GridDefaultMulti<UIGI_ActionSelectItem>).m_Selecting;

    void OnConfirmClick()
    {
        if (!B_CanSelect)
            return;
        m_Confirm.interactable = false;
        OnIndexSelect(m_SelectIndexes);
        OnCancelBtnClick();
    }
}
