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
    RectTransform rtf_MinimapTrans, rtf_BigmapTrans,rtf_MapGrid;
    public bool B_IsBigmapMode = false;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        gc_BigMapController = new UIT_GridControllerMono<UIGI_BigmapLevelInfo>(tf_Container.Find("MapBtn"));
        rtf_MapGrid = gc_BigMapController.transform.GetComponent<RectTransform>();
        rtf_MapPlayer = gc_BigMapController.transform.Find("MapPlayer").GetComponent<RectTransform>();
        btn_Bigmap = gc_BigMapController.transform.GetComponent<Button>();
        img_BigmapRaycast = btn_Bigmap.GetComponent<Image>();
        img_CancelRaycast = btn_Cancel.GetComponent<Image>();
        btn_Bigmap.onClick.AddListener(OnBigmapBtnClick);
        rtf_MinimapTrans = tf_Container.Find("MinimapTrans").GetComponent<RectTransform>();
        rtf_BigmapTrans = tf_Container.Find("BigmapTrans").GetComponent<RectTransform>();
        SwitchMapmode(false,false);
        TBroadCaster<enum_BC_UIStatusChanged>.Add<SBigmapLevelInfo[,], TileAxis>(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, OnLevelStatusChanged);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatusChanged>.Remove<SBigmapLevelInfo[,], TileAxis>(enum_BC_UIStatusChanged.PlayerLevelStatusChanged, OnLevelStatusChanged);
    }
    protected void OnBigmapBtnClick()
    {
        SwitchMapmode(true);
    }
    protected override void OnCancelBtnClick()
    {
        SwitchMapmode(false);
    }
    void SwitchMapmode(bool isBigmapMode,bool useAnim=true)
    {
        B_IsBigmapMode = isBigmapMode;
        img_BigmapRaycast.raycastTarget = !B_IsBigmapMode;
        img_CancelRaycast.raycastTarget = B_IsBigmapMode;
        
        rtf_MapGrid.pivot = B_IsBigmapMode? rtf_BigmapTrans.pivot:rtf_MinimapTrans.pivot;
        rtf_MapGrid.position = B_IsBigmapMode ? rtf_BigmapTrans.position : rtf_MinimapTrans.position;
        rtf_MapGrid.localScale = B_IsBigmapMode ? rtf_BigmapTrans.localScale : rtf_MinimapTrans.localScale;
        img_Background.color = TCommon.ColorAlpha(img_Background.color,B_IsBigmapMode? f_bgAlphaStart:0f);
    }
    void OnMapTileClick(TileAxis axis)
    {
        if (!B_IsBigmapMode)
        {
            SwitchMapmode(true);
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
            foreach (enum_TileDirection direction in TTiles.TTiles.m_AllDirections)
            {
                connectionActivate.Add(direction, levelInfo.m_Connections.ContainsKey(direction)
                    && levelInfo.m_Connections[direction].m_AxisX != -1
                    && bigMap.Get(levelInfo.m_Connections[direction]).m_TileLocking != enum_LevelLocking.Locked);
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
        return axis.m_AxisX + axis.m_AxisY * height * 1000;
    }
}
