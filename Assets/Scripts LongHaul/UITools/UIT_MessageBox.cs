using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class UIT_MessageBox : SimpleSingletonMono<UIT_MessageBox> {

    public class MessageBoxSelection
    {
        public string m_Key { get; private set; }
        public Action OnClick { get; private set; }
        public MessageBoxSelection(string _selectionKey,Action _OnClick)
        {
            m_Key = _selectionKey;
            OnClick = _OnClick;
        }
    }

    List< MessageBoxSelection> m_AllSelections=new List<MessageBoxSelection>();
    UIT_TextLocalization m_title;
    UIT_GridDefaultSingle<UIT_GridDefaultItem> m_SelectionGrid;

    protected override void Awake()
    {
        base.Awake();
        this.SetActivate(false);
        m_title = transform.Find("Title").GetComponent<UIT_TextLocalization>();
        m_SelectionGrid = new UIT_GridDefaultSingle<UIT_GridDefaultItem>(transform.Find("SelectionGrid"), OnSelectionsClick);
    }
    public void Begin(string titleKey, params MessageBoxSelection[] selections)
    {
        this.SetActivate(true);
        m_title.localizeText = titleKey;

        m_AllSelections.Clear();
        m_SelectionGrid.ClearGrid();
        for (int i = 0; i < selections.Length; i++)
        {
            m_SelectionGrid.AddItem(i).SetItemInfo(selections[i].m_Key);
            m_AllSelections.Add(selections[i]);
        }
        MessageBoxSelection cancelSelection = new MessageBoxSelection("UI_Common_Cancel", OnCancel);
        m_AllSelections.Add(cancelSelection);
        m_SelectionGrid.AddItem(m_AllSelections.Count-1).SetItemInfo(TLocalization.GetKeyLocalized( cancelSelection.m_Key));
    }
    void OnSelectionsClick(int index)
    {
        this.SetActivate(false);
        m_AllSelections[index].OnClick();
    }
    void OnCancel()
    {
        this.SetActivate(false);
    }
}
