﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGI_CharacterSelectItem : UIT_GridItem,IGridHighlight {
    Text m_Name;
    Transform m_Highlight;
    public override void Init()
    {
        base.Init();
        m_Name = rtf_Container.Find("Name").GetComponent<Text>();
        m_Highlight = rtf_Container.Find("Highlight");
    }

    public override void OnActivate(int _index)
    {
        base.OnActivate(_index);
        m_Name.text = ((enum_PlayerCharacter)_index).ToString();
    }


    public void AttachSelectButton(Action<int> OnButtonClick)
    {
        GetComponentInChildren<Button>().onClick.AddListener(()=> { OnButtonClick(m_Index); });
    }

    public void OnHighlight(bool highlight)
    {
        m_Highlight.SetActivate(highlight);
    }

}