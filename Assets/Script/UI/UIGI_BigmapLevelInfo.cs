using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using TTiles;
public class UIGI_BigmapLevelInfo : UIT_GridItem {
    Image img_Level;
    Text txt_Player;
    Dictionary<enum_TileDirection, Image> dic_TileConnections=new Dictionary<enum_TileDirection, Image>();
    protected override void Init()
    {
        base.Init();
        img_Level = tf_Container.Find("Level").GetComponent<Image>();
        txt_Player = tf_Container.Find("Player").GetComponent<Text>();
        for (int i = 0; i < TTiles.TTiles.m_AllDirections.Count; i++)
            dic_TileConnections.Add(TTiles.TTiles.m_AllDirections[i],tf_Container.Find(TTiles.TTiles.m_AllDirections[i].ToString()).GetComponent<Image>());
    }
    public void SetBigmapLevelInfo(SBigmapLevelInfo levelInfo,bool playerHere)
    {
        txt_Player.SetActivate(playerHere);
        img_Level.color = GetBigmapColor(levelInfo.m_TileType);
        foreach (enum_TileDirection direction in TTiles.TTiles.m_AllDirections)
            dic_TileConnections[direction].SetActivate(levelInfo.m_Connections.ContainsKey(direction));
    }
    Color GetBigmapColor(enum_BigmapTileType type)
    {
        switch (type)
        {
            default:
                return TCommon.ColorAlpha(Color.blue,.5f);
            case enum_BigmapTileType.Battle:
                return TCommon.ColorAlpha(Color.yellow, .5f);
            case enum_BigmapTileType.Reward:
                return TCommon.ColorAlpha(Color.green, .5f);
            case enum_BigmapTileType.Start:
                return TCommon.ColorAlpha(Color.grey, .5f);
            case enum_BigmapTileType.BattleEnd:
                return TCommon.ColorAlpha(Color.black, .5f);
        }
    }
}
