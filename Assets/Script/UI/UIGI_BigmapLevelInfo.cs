﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TTiles;
using System;

public class UIGI_BigmapLevelInfo : UIT_GridItem {
    Image img_Level;
    Button btn_ChangeLevel; Action<TileAxis> OnChangeLevelClick;
    Dictionary<enum_TileDirection, Image> dic_TileConnections=new Dictionary<enum_TileDirection, Image>();
    TileAxis m_CurrentAxis;
    protected override void Init()
    {
        base.Init();
        img_Level = tf_Container.Find("Level").GetComponent<Image>();
        btn_ChangeLevel = tf_Container.Find("ChangeLevel").GetComponent<Button>();
        btn_ChangeLevel.onClick.AddListener(OnChangeLevelBtnClick);
        for (int i = 0; i < TTiles.TTiles.m_AllDirections.Count; i++)
            dic_TileConnections.Add(TTiles.TTiles.m_AllDirections[i],tf_Container.Find(TTiles.TTiles.m_AllDirections[i].ToString()).GetComponent<Image>());
    }
    public void Init(Action<TileAxis> _OnChangeLevelClick)
    {
        OnChangeLevelClick = _OnChangeLevelClick;
    }
    public void SetBigmapLevelInfo(SBigmapLevelInfo levelInfo,Dictionary<enum_TileDirection,bool> connectionActivate)
    {
        m_CurrentAxis = levelInfo.m_TileAxis;
        tf_Container.localScale = levelInfo.m_TileLocking == enum_LevelLocking.Locked ? Vector3.zero : Vector3.one;
        img_Level.color = UIExpression.BigmapTileColor(levelInfo.m_TileLocking,levelInfo.m_TileType);
        foreach (enum_TileDirection direction in TTiles.TTiles.m_AllDirections)
            dic_TileConnections[direction].SetActivate(connectionActivate[direction]);
    }

    void OnChangeLevelBtnClick()
    {
        OnChangeLevelClick(m_CurrentAxis);
    }
}
