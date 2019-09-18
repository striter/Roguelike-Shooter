using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using UnityEngine.UI;

public class UI_BigmapControl : UIPageBase {        //This Page Won't Hide(One Page Show Bigmap/Minimap)
    UIT_GridControllerMonoItem<UIGI_BigmapLevelInfo> gc_BigMapController;
    RectTransform rtf_MapPlayer;
    public bool B_ShowBigmap = false;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        gc_BigMapController = new UIT_GridControllerMonoItem<UIGI_BigmapLevelInfo>(tf_Container.Find("MapGrid"));
        rtf_MapPlayer = gc_BigMapController.transform.Find("MapPlayer").GetComponent<RectTransform>();
        tf_Container.Find("ChestTips").SetActivate( GameManager.Instance.B_ShowChestTips);
        OnLevelStatusChanged(LevelManager.Instance.m_MapLevelInfo, LevelManager.Instance.m_currentLevel.m_TileAxis);
        B_ShowBigmap = true;
    }
    
    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        B_ShowBigmap = false;
    }
    
    void OnMapTileClick(TileAxis axis)
    {
        if (!B_ShowBigmap)
            return;

        LevelManager.Instance.OnChangeLevel(axis);
        OnCancelBtnClick();
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
                rtf_MapPlayer.transform.SetParent(infoUI.transform);
                rtf_MapPlayer.anchoredPosition=Vector2.zero;
            }
        });
    }
    int UIBigmapTileIndex(TileAxis axis, int width, int height)
    {
        return axis.X + axis.Y * height * 1000;
    }
}
