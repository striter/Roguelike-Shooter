using GameSetting;
using System;
using System.Collections.Generic;
public class UI_ActionAcquire : UIPageBase {
    EntityCharacterPlayer m_player;
    List<ActionBase> m_actions;
    UIC_Button m_Confirm;
    UIT_GridControllerGridItem<UIGI_ActionItemSelect> m_Grid;
    int m_selectIndex;
    protected override void Init()
    {
        base.Init();
        m_Confirm =new UIC_Button( tf_Container.Find("Confirm"),OnConfirmClick);
        m_Grid = new UIT_GridControllerGridItem<UIGI_ActionItemSelect>(tf_Container.Find("ActionGrid"));
    }
    public void Play(List<ActionBase> actions,EntityCharacterPlayer _player, int selectAmount)
    {
        m_Grid.m_GridLayout.spacing = new UnityEngine.Vector2(m_Grid.m_GridLayout.spacing.x*2/actions.Count,m_Grid.m_GridLayout.spacing.y);
        m_Confirm.SetInteractable(false);
        m_selectIndex = -1;
        m_actions = actions;
        m_player = _player;
        m_Grid.ClearGrid();
        for (int i = 0; i < actions.Count; i++)
            m_Grid.AddItem(i).SetInfo(actions[i],OnItemSelected,true);
    }

    void OnItemSelected(int index)
    {
        if (m_selectIndex != -1)
            m_Grid.GetItem(m_selectIndex).SetHighlight(false);
        m_selectIndex = index;
        m_Grid.GetItem(m_selectIndex).SetHighlight(true);

        m_Confirm.SetInteractable(true);
    }

    void OnConfirmClick()
    {
        m_Confirm.SetInteractable(false);
        m_player.m_PlayerInfo.AddStoredAction(m_actions[m_selectIndex]);
        OnCancel();
    }

    protected override void OnCancelBtnClick()=>GameUIManager.Instance.ShowMessageBox<UIM_Intro>().Play("UI_Title_ExitActionAcquire", "UI_Intro_ExitActionAcquire", "UI_Option_ExitActionAcquireConfirm", OnCancel);
    void OnCancel()
    {
        GameUIManager.Instance.m_GameControl.ShowMapBtn(true);
        base.OnCancelBtnClick();
    }
}
