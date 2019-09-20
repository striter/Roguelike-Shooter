using GameSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;
using UnityEngine.UI;

public class UI_MapControl : UIPageBase {        //This Page Won't Hide(One Page Show Bigmap/Minimap)
    UIT_GridControllerMonoItem<UIGI_MapControlCell> gc_BigMapController;
    public bool B_Playing = false;

    RectTransform rtf_MapPlayer;
    UIGI_MapControlCell m_PlayerCell;
    UIGI_MapControlCell m_TargetCell;
    float m_AnimateCheck;
    protected override void Init(bool useAnim)
    {
        base.Init(useAnim);
        gc_BigMapController = new UIT_GridControllerMonoItem<UIGI_MapControlCell>(tf_Container.Find("MapGrid"));
        rtf_MapPlayer = gc_BigMapController.transform.Find("MapPlayer").GetComponent<RectTransform>();
        tf_Container.Find("ChestTips").SetActivate( GameManager.Instance.B_ShowChestTips);
        OnLevelStatusChanged(LevelManager.Instance.m_MapLevelInfo, LevelManager.Instance.m_currentLevel.m_TileAxis);
        B_Playing = true;
        m_TargetCell = null;
    }

    protected override void OnCancelBtnClick()
    {
        base.OnCancelBtnClick();
        B_Playing = false;
    }

    int UIBigmapTileIndex(TileAxis axis, int width, int height) => axis.X + axis.Y * height * 1000;
    void OnLevelStatusChanged(SBigmapLevelInfo[,] bigMap, TileAxis playerAxis)
    {
        if (gc_BigMapController.I_Count == 0)
        {
            gc_BigMapController.m_GridLayout.constraintCount = bigMap.GetLength(0);
            bigMap.Traversal((SBigmapLevelInfo levelInfo) => { gc_BigMapController.AddItem(UIBigmapTileIndex(levelInfo.m_TileAxis, bigMap.GetLength(0), bigMap.GetLength(1))).Init(OnMapTileClick); });
            gc_BigMapController.SortChildrenSibling();
            rtf_MapPlayer.SetAsLastSibling();
        }

        bigMap.Traversal((SBigmapLevelInfo levelInfo) => {
            UIGI_MapControlCell infoUI = gc_BigMapController.GetItem(UIBigmapTileIndex(levelInfo.m_TileAxis, bigMap.GetLength(0), bigMap.GetLength(1)));
            Dictionary<enum_TileDirection, bool> connectionActivate = new Dictionary<enum_TileDirection, bool>();
            foreach (enum_TileDirection direction in TTiles.TTiles.m_FourDirections)
            {
                connectionActivate.Add(direction, levelInfo.m_Connections.ContainsKey(direction)
                    && levelInfo.m_Connections[direction].X != -1
                    && bigMap.Get(levelInfo.m_Connections[direction]).m_TileLocking != enum_TileLocking.Locked);
            }
            infoUI.SetBigmapLevelInfo(levelInfo, connectionActivate);

            if (levelInfo.m_TileAxis == playerAxis)
                m_PlayerCell = infoUI;
        });
    }
    void OnMapTileClick(UIGI_MapControlCell targetCell)
    {
        if (!B_Playing||m_TargetCell!=null)
            return;

        m_TargetCell = targetCell;
        m_AnimateCheck = UIConst.F_MapAnimateTime;
    }
    private void Update()
    {
        if (!B_Playing)
            return;

        if (!m_TargetCell)
        {
            rtf_MapPlayer.anchoredPosition = m_PlayerCell.rectTransform.anchoredPosition;
            return;
        }

        m_AnimateCheck -= Time.deltaTime;
        rtf_MapPlayer.anchoredPosition = Vector2.Lerp(m_TargetCell.rectTransform.anchoredPosition, m_PlayerCell.rectTransform.anchoredPosition, m_AnimateCheck / UIConst.F_MapAnimateTime);
        if (m_AnimateCheck > 0f)
            return;

        B_Playing = false;
        OnCancelBtnClick();
        LevelManager.Instance.OnChangeLevel(m_TargetCell.m_Axis);
    }

}
