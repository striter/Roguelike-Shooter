using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TTiles;
using System;

public class UIGI_MapControlCell : UIT_GridDefaultItem {
    Transform tf_PlayerAt,tf_TileLocked;
    Image img_Level;
    Action<UIGI_MapControlCell> OnChangeLevelClick;
    Dictionary<enum_TileDirection, Image> dic_TileConnections=new Dictionary<enum_TileDirection, Image>();
    public SBigmapLevelInfo m_TileInfo { get; private set; }
    public override void Init(UIT_GridController parent)
    {
        base.Init(parent);
        img_Level = tf_Container.Find("TileImage").GetComponent<Image>();
        tf_PlayerAt = tf_Container.Find("PlayerAt");
        tf_TileLocked = tf_Container.Find("TileLocked");
        for (int i = 0; i < TTiles.TTiles.m_FourDirections.Count; i++)
            dic_TileConnections.Add(TTiles.TTiles.m_FourDirections[i], tf_Container.Find(TTiles.TTiles.m_FourDirections[i].ToString()).GetComponent<Image>());
    }
    
    public void SetBigmapLevelInfo(SBigmapLevelInfo levelInfo,bool playerAt, Dictionary<enum_TileDirection, bool> connectionActivate)
    {
        m_TileInfo = levelInfo;
        if (levelInfo.m_TileLocking == enum_TileLocking.Invalid || levelInfo.m_TileLocking == enum_TileLocking.Unseen)
        {
            tf_Container.SetActivate(false);
            return;
        }
        tf_TileLocked.SetActivate(levelInfo.m_TileLocking== enum_TileLocking.Locked);
        tf_PlayerAt.SetActivate(playerAt);
        img_Level.sprite =UIManager.Instance.m_commonSprites[levelInfo.m_TileType.GetSpriteName()];
        foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            dic_TileConnections[direction].SetActivate(connectionActivate[direction]);
    }
}
