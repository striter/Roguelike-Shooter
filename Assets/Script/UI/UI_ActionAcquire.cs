using GameSetting;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_ActionAcquire : UIPageBase {

    Button m_Confirm;
    UIT_GridControllerDefaultMono<UIGI_ActionSelectItem> m_Grid;
    Action<int> OnIndexSelect;
    int selectIndex = -1;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        m_Grid = new UIT_GridControllerDefaultMono<UIGI_ActionSelectItem>(tf_Container.Find("ActionGrid"), OnItemSelected, true);
        m_Confirm = tf_Container.Find("Confirm").GetComponent<Button>();
        m_Confirm.onClick.AddListener(OnConfirmClick);
    }
    public void Play(List<ActionBase> actions,Action<int> _OnIndexSelect)
    {
        m_Confirm.interactable = false;
        OnIndexSelect = _OnIndexSelect;
        m_Grid.ClearGrid();
        for (int i = 0; i < actions.Count; i++)
            m_Grid.AddItem(i).SetInfo(actions[i]);
    }

    void OnItemSelected(int index)
    {
        m_Confirm.interactable = true;
        selectIndex = index;
    }
    void OnConfirmClick()
    {
        if (selectIndex == -1)
            return;
        m_Confirm.interactable = false;
        OnIndexSelect(selectIndex);
        OnCancelBtnClick();
    }
}
