using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapControl : UIPageBase,ISingleCoroutine {        //This Page Won't Hide(One Page Show Bigmap/Minimap)
    Transform tf_MapTile, tf_MapInfo, tf_TileDetail;
    UIT_GridDefaultSingle<UIGI_MapControlCell> m_AllTilesGrid;
    UIT_TextLocalization txt_Stage, txt_Style;
    Image img_TileType, img_TileBattleStaus;
    UIT_TextLocalization txt_Cordinates, txt_TileType, txt_BattleStatus;
    UIGI_MapControlCell m_targetTile;
    UIC_Button btn_Confirm;

    public bool B_Playing = false;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        tf_Container.Find("ChestTips").SetActivate(GameManager.Instance.B_ShowChestTips);   //Test

        tf_TileDetail = tf_Container.Find("TileDetail");
        img_TileType = tf_TileDetail.Find("Image/TileType").GetComponent<Image>();
        img_TileBattleStaus = tf_TileDetail.Find("Image/BattleStatus/StatusImage").GetComponent<Image>();
        txt_Cordinates = tf_TileDetail.Find("Info/Cordinates/Detail").GetComponent<UIT_TextLocalization>();
        txt_BattleStatus = tf_TileDetail.Find("Info/BattleStatus/Detail").GetComponent<UIT_TextLocalization>();
        txt_TileType = tf_TileDetail.Find("Info/TileType/Detail").GetComponent<UIT_TextLocalization>();
        btn_Confirm = new UIC_Button(tf_TileDetail.Find("ConfirmBtn"), OnConfirmBtnClick);

        tf_MapTile = tf_Container.Find("MapTile");
        m_AllTilesGrid = new UIT_GridDefaultSingle<UIGI_MapControlCell>(tf_MapTile.Find("TileGrid"),OnMapTileClick);
        m_AllTilesGrid.transform.localScale = Vector3.one * UIManager.Instance.m_Scaler.scaleFactor;
        SBigmapLevelInfo[,] map = new SBigmapLevelInfo[6, 6];
        for (int i = 0; i < 6; i++)
            for (int j = 0; j < 6; j++)
                map[i,j] =i< LevelManager.Instance.m_MapLevelInfo.GetLength(0)&&j<LevelManager.Instance.m_MapLevelInfo.GetLength(1)? LevelManager.Instance.m_MapLevelInfo[i,j]:new SBigmapLevelInfo(new SBigmapTileInfo(new TileAxis(i,j), enum_TileType.Invalid, enum_TileLocking.Invalid));

        m_AllTilesGrid.m_GridLayout.constraintCount = map.GetLength(0);
        map.Traversal((SBigmapLevelInfo levelInfo) => { m_AllTilesGrid.AddItem(levelInfo.m_TileAxis.X, levelInfo.m_TileAxis.Y); });
        m_AllTilesGrid.SortChildrenSibling();
        map.Traversal((SBigmapLevelInfo levelInfo) => {
            UIGI_MapControlCell infoUI = m_AllTilesGrid.GetItem(levelInfo.m_TileAxis.X,levelInfo.m_TileAxis.Y);
            Dictionary<enum_TileDirection, bool> connectionActivate = new Dictionary<enum_TileDirection, bool>();
            foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            {
                connectionActivate.Add(direction, levelInfo.m_Connections.ContainsKey(direction)
                    && levelInfo.m_Connections[direction].X != -1
                    && map.Get(levelInfo.m_Connections[direction]).m_TileLocking == enum_TileLocking.Unlockable);
            }
            bool isPlayer = levelInfo.m_TileAxis == LevelManager.Instance.m_currentLevel.m_TileAxis;
            infoUI.SetBigmapLevelInfo(levelInfo,isPlayer, connectionActivate);
            if (isPlayer) m_AllTilesGrid.OnItemClick(infoUI.I_Index);
        });

        tf_MapInfo = tf_Container.Find("MapInfo");
        txt_Stage = tf_MapInfo.Find("Stage").GetComponent<UIT_TextLocalization>();
        txt_Style = tf_MapInfo.Find("Style").GetComponent<UIT_TextLocalization>();
        txt_Stage.formatKeys("UI_Map_Stage",  GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey());
        txt_Style.formatKeys("UI_Map_Stage", GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey());
    }

    void OnMapTileClick(int index)
    {
        SetTileInfo(m_AllTilesGrid.GetItem(index));    
    }

    void SetTileInfo(UIGI_MapControlCell tile)
    {
        m_targetTile = tile;
        enum_UI_TileBattleStatus battleStatus = tile.m_TileInfo.m_TileType.GetBattleStatus();
        txt_TileType.localizeText=tile.m_TileInfo.m_TileType.GetLocalizeKey();
        txt_Cordinates.text=tile.m_TileInfo.m_TileAxis.GetCordinates();
        txt_BattleStatus.text=battleStatus.GetBattlePercentage();
        this.StartSingleCoroutine(10, TIEnumerators.UI.StartTypeWriter(txt_TileType,.5f));
        this.StartSingleCoroutine(11, TIEnumerators.UI.StartTypeWriter(txt_Cordinates, .5f));
        this.StartSingleCoroutine(12, TIEnumerators.UI.StartTypeWriter(txt_BattleStatus, .5f));
        img_TileType.sprite = UIManager.Instance.m_commonSprites[tile.m_TileInfo.m_TileType.GetSpriteName()];
        img_TileBattleStaus.sprite = UIManager.Instance.m_commonSprites[battleStatus.GetSpriteName()];
        bool locked = tile.m_TileInfo.m_TileLocking == enum_TileLocking.Locked || tile.m_TileInfo.m_TileAxis == LevelManager.Instance.m_currentLevel.m_TileAxis;
        btn_Confirm.SetInteractable(!locked);
    }

    void OnConfirmBtnClick()
    {
        LevelManager.Instance.OnChangeLevel(m_targetTile.m_TileInfo.m_TileAxis);
        OnCancelBtnClick();
    }
}
