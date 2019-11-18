using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TTiles;
using System;

public class UIGI_MapControlCell : UIT_GridDefaultItem {
    Transform tf_TileLocked;
    Image img_Level,img_Background;
    Action<UIGI_MapControlCell> OnChangeLevelClick;
    Dictionary<enum_TileDirection, Image> dic_TileConnections=new Dictionary<enum_TileDirection, Image>();
    public SBigmapLevelInfo m_TileInfo { get; private set; }
    public override void Init()
    {
        base.Init();
        img_Level = tf_Container.Find("TileImage").GetComponent<Image>();
        img_Background = tf_Container.Find("Background").GetComponent<Image>();
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
        img_Background.color =TCommon.GetHexColor( GetBackgroundColor(levelInfo.m_TileLocking,playerAt));
        img_Level.sprite = GameUIManager.Instance.m_InGameSprites[levelInfo.m_LevelType.GetSpriteName()];
        foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            dic_TileConnections[direction].SetActivate(connectionActivate[direction]);
    }
    string GetBackgroundColor(enum_TileLocking type,bool playerAt)
    {
        if (playerAt)
            return "B9FF01FF";

        switch (type)
        {
            default:
                return "000000FF";
            case enum_TileLocking.Unlockable:
                return "A7A7A764";
            case enum_TileLocking.Locked:
            case enum_TileLocking.Unlocked:
                return "A7A7A7FF";
        }
    }
}
