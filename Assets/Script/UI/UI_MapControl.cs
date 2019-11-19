using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapControl : UIPageBase {
    Transform tf_MapTile, tf_MapInfo, tf_TileDetail;
    UIT_GridDefaultSingle<UIGI_MapControlCell> m_AllTilesGrid;
    UIT_TextExtend txt_Stage, txt_Style;
    Animation m_TileTypeAnim;
    Image img_TileTypeCurrent,img_TileTypeSelect, img_TileBattleStatus1,img_TileBattleStatus2;
    UIT_TextExtend txt_Cordinates, txt_TileType, txt_BattleStatus;
    RectTransform m_Line;
    UIGI_MapControlCell m_targetTile;
    UIC_Button btn_Confirm;
    protected override void Init()
    {
        base.Init();
        tf_TileDetail = tf_Container.Find("TileDetail");
        m_TileTypeAnim = tf_TileDetail.Find("Image").GetComponent<Animation>();
        img_TileTypeCurrent = tf_TileDetail.Find("Image/TileImage/TileCurrent").GetComponent<Image>();
        img_TileTypeSelect = tf_TileDetail.Find("Image/TileImage/TileSelect").GetComponent<Image>();
        img_TileBattleStatus1 = tf_TileDetail.Find("Image/StatusImage/StatusImage1").GetComponent<Image>();
        img_TileBattleStatus2 = tf_TileDetail.Find("Image/StatusImage/StatusImage2").GetComponent<Image>();
        m_Line = tf_TileDetail.Find("Image/DetailTarget/Line").GetComponent<RectTransform>();
        txt_Cordinates = tf_TileDetail.Find("Info/Cordinates/Detail").GetComponent<UIT_TextExtend>();
        txt_BattleStatus = tf_TileDetail.Find("Info/BattleStatus/Detail").GetComponent<UIT_TextExtend>();
        txt_TileType = tf_TileDetail.Find("Info/TileType/Detail").GetComponent<UIT_TextExtend>();
        btn_Confirm = new UIC_Button(tf_TileDetail.Find("ConfirmBtn"), OnConfirmBtnClick);

        tf_MapTile = tf_Container.Find("MapTile");
        m_AllTilesGrid = new UIT_GridDefaultSingle<UIGI_MapControlCell>(tf_MapTile.Find("TileGrid"),OnMapTileClick);
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
                    && map.Get(levelInfo.m_Connections[direction]).m_TileLocking != enum_TileLocking.Unseen);
            }
            bool isPlayer = levelInfo.m_TileAxis == LevelManager.Instance.m_currentLevel.m_TileAxis;
            infoUI.SetBigmapLevelInfo(levelInfo,isPlayer, connectionActivate);
            if(isPlayer) m_AllTilesGrid.OnItemClick(infoUI.I_Index);
        });

        tf_MapInfo = tf_Container.Find("MapInfo");
        txt_Stage = tf_MapInfo.Find("Stage").GetComponent<UIT_TextExtend>();
        txt_Style = tf_MapInfo.Find("Style").GetComponent<UIT_TextExtend>();
        txt_Stage.formatKey("UI_Map_Stage",  GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey());
        txt_Style.formatKey("UI_Map_Style", GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey());
    }

    void OnMapTileClick(int index)
    {
        SetTileInfo(m_AllTilesGrid.GetItem(index));    
    }

    void SetTileInfo(UIGI_MapControlCell tile)
    {
        m_targetTile = tile;
        enum_UI_TileBattleStatus battleStatus = tile.m_TileInfo.m_LevelType.GetBattleStatus();
        txt_TileType.localizeKey=tile.m_TileInfo.m_LevelType.GetLocalizeKey();
        txt_Cordinates.text=tile.m_TileInfo.m_TileAxis.GetCordinates();
        txt_BattleStatus.text=battleStatus.GetBattlePercentage();
        this.StartSingleCoroutine(10, TIEnumerators.UI.StartTypeWriter(txt_TileType,.5f));
        this.StartSingleCoroutine(11, TIEnumerators.UI.StartTypeWriter(txt_Cordinates, .5f));
        this.StartSingleCoroutine(12, TIEnumerators.UI.StartTypeWriter(txt_BattleStatus, .5f));
        m_TileTypeAnim.Stop();
        m_TileTypeAnim.Play(PlayMode.StopAll);
        img_TileTypeCurrent.sprite = img_TileTypeSelect.sprite;
        img_TileTypeSelect.sprite = GameUIManager.Instance.m_InGameSprites[tile.m_TileInfo.m_LevelType.GetSpriteName()];
        img_TileBattleStatus1.sprite = GameUIManager.Instance.m_InGameSprites[battleStatus.GetSpriteName()];
        img_TileBattleStatus2.sprite = img_TileBattleStatus1.sprite;

        bool locked = tile.m_TileInfo.m_TileLocking == enum_TileLocking.Locked || tile.m_TileInfo.m_TileAxis == LevelManager.Instance.m_currentLevel.m_TileAxis;
        btn_Confirm.SetInteractable(!locked);
    }

    void OnConfirmBtnClick()
    {
        OnCancelBtnClick();
        GameManager.Instance.ChangeLevel(m_targetTile.m_TileInfo.m_TileAxis);
    }
    float width = 0;
    float angle = 0;
    void Update()
    {
        if (m_targetTile == null) return;

        Vector2 direction = tf_Container.InverseTransformPoint(m_targetTile.transform.position) - tf_Container.InverseTransformPoint(m_Line.transform.position);
        width = Mathf.Lerp(width, direction.magnitude, Time.deltaTime * 20);
        angle = Mathf.Lerp(angle, Vector2.SignedAngle(Vector2.up, direction), Time.deltaTime * 20);
        m_Line.sizeDelta = new Vector2(m_Line.sizeDelta.x, width);
        m_Line.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
