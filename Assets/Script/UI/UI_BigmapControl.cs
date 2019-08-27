using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using UnityEngine.UI;

public class UI_BigmapControl : UIPageBase {        //This Page Won't Hide(One Page Show Bigmap/Minimap)
    UIT_GridControllerMono<UIGI_BigmapLevelInfo> gc_BigMapController;
    RectTransform rtf_MapPlayer;
    Button btn_Bigmap;
    Image img_BigmapRaycast, img_CancelRaycast;
    RectTransform rtf_MapGrid;
    public bool B_ShowBigmap = false;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        gc_BigMapController = new UIT_GridControllerMono<UIGI_BigmapLevelInfo>(tf_Container.Find("MapGrid"));
        rtf_MapGrid = gc_BigMapController.transform.GetComponent<RectTransform>();
        rtf_MapPlayer = gc_BigMapController.transform.Find("MapPlayer").GetComponent<RectTransform>();
        btn_Bigmap = transform.Find("ShowBigmap").GetComponent<Button>();
        img_BigmapRaycast = btn_Bigmap.GetComponent<Image>();
        img_CancelRaycast = btn_Cancel.GetComponent<Image>();
        btn_Bigmap.onClick.AddListener(OnBigmapBtnClick);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_UIStatus>.Add<SBigmapLevelInfo[,], TileAxis>(enum_BC_UIStatus.UI_LevelStatusChange, OnLevelStatusChanged);
        ShowMap(false, false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_UIStatus>.Remove<SBigmapLevelInfo[,], TileAxis>(enum_BC_UIStatus.UI_LevelStatusChange, OnLevelStatusChanged);
    }
    void OnBattleStart()
    {
        ShowMap(false);
        this.SetActivate(false);
    }
    void OnBattleFinish()
    {
        this.SetActivate(true);
    }

    protected void OnBigmapBtnClick()
    {
        ShowMap(true);
    }
    protected override void OnCancelBtnClick()
    {
        ShowMap(false);
    }
    void ShowMap(bool showBigmap,bool useAnim=true)     //useAnim To Be Continued
    {
        B_ShowBigmap = showBigmap;
        img_BigmapRaycast.raycastTarget = !B_ShowBigmap;
        img_CancelRaycast.raycastTarget = B_ShowBigmap;
        btn_Bigmap.SetActivate(!showBigmap);
        rtf_MapGrid.SetActivate(showBigmap);
        img_Background.color = TCommon.ColorAlpha(img_Background.color,B_ShowBigmap? f_bgAlphaStart:0f);
    }
    void OnMapTileClick(TileAxis axis)
    {
        if (!B_ShowBigmap)
        {
            ShowMap(true);
            return;
        }
        EnviormentManager.Instance.OnChangeLevel(axis);
    }

    void OnLevelStatusChanged(SBigmapLevelInfo[,] bigMap, TileAxis playerAxis)
    {
        if (gc_BigMapController.I_Count == 0)
        {
            gc_BigMapController.m_GridLayout.constraintCount = bigMap.GetLength(0);
            bigMap.Traversal((SBigmapLevelInfo levelInfo) => { gc_BigMapController.AddItem(UIBigmapTileIndex(levelInfo.m_TileAxis, bigMap.GetLength(0), bigMap.GetLength(1))).Init(OnMapTileClick); });
            gc_BigMapController.SortChildrenSibling();
            rtf_MapPlayer.transform.SetAsLastSibling();
        }

        bigMap.Traversal((SBigmapLevelInfo levelInfo) => {
            UIGI_BigmapLevelInfo infoUI = gc_BigMapController.GetItem(UIBigmapTileIndex(levelInfo.m_TileAxis, bigMap.GetLength(0), bigMap.GetLength(1)));
            Dictionary<enum_TileDirection, bool> connectionActivate = new Dictionary<enum_TileDirection, bool>();
            foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            {
                connectionActivate.Add(direction, levelInfo.m_Connections.ContainsKey(direction)
                    && levelInfo.m_Connections[direction].X != -1
                    && bigMap.Get(levelInfo.m_Connections[direction]).m_TileLocking != enum_TileLocking.Locked);
            }
            infoUI.SetBigmapLevelInfo(levelInfo, connectionActivate);
            if (levelInfo.m_TileAxis == playerAxis)
            {
                rtf_MapPlayer.SetParent(infoUI.transform);
                rtf_MapPlayer.anchoredPosition = Vector2.zero;
            }
        });
    }
    int UIBigmapTileIndex(TileAxis axis, int width, int height)
    {
        return axis.X + axis.Y * height * 1000;
    }
}
