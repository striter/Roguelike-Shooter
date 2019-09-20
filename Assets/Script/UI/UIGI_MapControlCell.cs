using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TTiles;
using System;

public class UIGI_MapControlCell : UIT_GridItem {
    Image img_Level;
    Button btn_ChangeLevel; Action<UIGI_MapControlCell> OnChangeLevelClick;
    Dictionary<enum_TileDirection, Image> dic_TileConnections=new Dictionary<enum_TileDirection, Image>();
    public TileAxis m_Axis { get; private set; }
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        img_Level = tf_Container.Find("Level").GetComponent<Image>();
        btn_ChangeLevel = tf_Container.Find("ChangeLevel").GetComponent<Button>();
        btn_ChangeLevel.onClick.AddListener(OnChangeLevelBtnClick);
        for (int i = 0; i < TTiles.TTiles.m_FourDirections.Count; i++)
            dic_TileConnections.Add(TTiles.TTiles.m_FourDirections[i], tf_Container.Find(TTiles.TTiles.m_FourDirections[i].ToString()).GetComponent<Image>());
    }

    public void Init(Action<UIGI_MapControlCell> _OnChangeLevelClick)
    {
        OnChangeLevelClick = _OnChangeLevelClick;
    }
    public void SetBigmapLevelInfo(SBigmapLevelInfo levelInfo,Dictionary<enum_TileDirection,bool> connectionActivate)
    {
        m_Axis = levelInfo.m_TileAxis;
        tf_Container.localScale = levelInfo.m_TileLocking == enum_TileLocking.Locked ? Vector3.zero : Vector3.one;
        img_Level.color = UIExpression.BigmapTileColor(levelInfo.m_TileLocking,levelInfo.m_TileType);
        foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            dic_TileConnections[direction].SetActivate(connectionActivate[direction]);
    }

    void OnChangeLevelBtnClick()
    {
        OnChangeLevelClick(this);
    }
}
