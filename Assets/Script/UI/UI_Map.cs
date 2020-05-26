using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using LevelSetting;
using System;
using TTiles;

public class UI_Map : UIPage {
    Transform m_MapContainer;
    UIC_Map m_Map;
    class UIC_Map : UIC_MapBase
    {
        UIT_GridControllerGridItem<UIGI_MapLocations> m_LocationsGrid;
        UIT_EventTriggerListener m_EventTrigger;
        Vector2 m_MapOffsetBase,m_PreValidOffset;
        float m_MapRotationBase;
        Action<int> OnLocationClick;
        public UIC_Map(Transform transform, Action<int> OnLocationClick) : base(transform, LevelConst.I_UIMapScale)
        {
            m_LocationsGrid = new UIT_GridControllerGridItem<UIGI_MapLocations>(m_Map_Origin_Base.transform.Find("LocationsGrid"));
            this.OnLocationClick = OnLocationClick;
            m_EventTrigger = transform.GetComponent<UIT_EventTriggerListener>();
            m_EventTrigger.D_OnDragDelta = OnMapDrag;
            m_EventTrigger.OnWorldClick = OnMapClick;
        }
        public override void OnPlay()
        {
            base.OnPlay();

            m_MapRotationBase = GameLevelManager.Instance.GetMapAngle(CameraController.Instance.m_Yaw);
            UpdateMapRotation(m_MapRotationBase);

            m_LocationsGrid.ClearGrid();
            BattleManager.Instance.m_StageInteracts.Traversal((int interactIndex, InteractBattleBase interactData) =>
            {
                UIGI_MapLocations locations = m_LocationsGrid.AddItem(interactIndex);
                locations.Play(interactData);
                locations.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(interactData.transform.position);
                locations.transform.rotation = Quaternion.identity;
            });

            m_MapOffsetBase = m_Player.rectTransform.anchoredPosition;
            m_PreValidOffset = m_MapOffsetBase;
            m_Map_Origin_Base.rectTransform.anchoredPosition = m_MapOffsetBase * -m_MapScale;
        }
        
        void OnMapScaleChange(float scale)
        {
            base.ChangeMapScale(scale);
            m_Map_Origin_Base.rectTransform.anchoredPosition = m_MapOffsetBase * -m_MapScale;
        }

        void OnMapClick(Vector2 position)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Map_Origin_Base_Fog.rectTransform, position, null, out position))
                return;

            int castIndex = -1;
            if (isMapPosValid((int)position.x, (int)position.y))
            {
                m_LocationsGrid.m_Pool.m_ActiveItemDic.TraversalBreak((UIGI_MapLocations item) => {
                    castIndex = item.MapCastCheck(position);
                    return castIndex != -1;
                });
            }
            OnLocationClick(castIndex);
        }

        bool isMapPosValid(int x, int y) => (x>=0&&y>=0&& x < m_Map_Origin_Base_Fog.texture.width&&y < m_Map_Origin_Base_Fog.texture.height)&& GameLevelManager.Instance.CheckTileRevealed(x,y);

        void OnMapDrag(Vector2 delta)
        {
            delta=m_Map_Origin_Base.rectTransform.InverseTransformDirection(delta);
            m_MapOffsetBase -= delta / m_MapScale;
            m_Map_Origin_Base.rectTransform.anchoredPosition =Vector2.Lerp(m_Map_Origin_Base.rectTransform.anchoredPosition,  m_MapOffsetBase * -m_MapScale,.5f);
        }

        public void Tick(float deltaTime)
        {
            if (isMapAreaValidPos((int)m_MapOffsetBase.x, (int)m_MapOffsetBase.y,LevelConst.I_UIMapPullbackCheckRange))
                m_PreValidOffset = m_MapOffsetBase;
            else if(!m_EventTrigger.m_Dragging)
                m_MapOffsetBase = Vector2.Lerp(m_MapOffsetBase, m_PreValidOffset, deltaTime* LevelConst.I_UIMapPullbackSpeedMultiply);

            m_Map_Origin_Base.rectTransform.anchoredPosition = Vector2.Lerp(m_Map_Origin_Base.rectTransform.anchoredPosition, m_MapOffsetBase * -m_MapScale, deltaTime * 20);
        }

        bool isMapAreaValidPos(int x,int y,int radius)
        {
            bool valid = false;
            List<TileAxis> m_Axies = TileTools.GetAxisRange(m_Map_Origin_Base_Fog.texture.width, m_Map_Origin_Base_Fog.texture.height ,new TileAxis(x, y), 30);
            m_Axies.TraversalBreak((TileAxis axis) => { valid = isMapPosValid(axis.X,axis.Y); return valid; });
            return valid;
        }
        
    }
    Transform m_StageInfo, m_LocationInfo;
    UIT_TextExtend m_LocationName, m_LocationIntro, m_Stage, m_Style;
    protected override void Init()
    {
        base.Init();
        m_MapContainer = rtf_Container.Find("MapContainer");
        m_Map = new UIC_Map( m_MapContainer.Find("Map"),OnChunkSelect);

        m_StageInfo = m_MapContainer.Find("StageInfo");
        m_Stage = m_StageInfo.Find("Stage").GetComponent<UIT_TextExtend>();
        m_Style = m_StageInfo.Find("Style").GetComponent<UIT_TextExtend>();

        m_LocationInfo = m_MapContainer.Find("LocationInfo");
        m_LocationName = m_LocationInfo.Find("Name").GetComponent<UIT_TextExtend>();
        m_LocationIntro = m_LocationInfo.Find("Intro").GetComponent<UIT_TextExtend>();
    }

    public override void OnPlay(bool doAnim, Action<UIPageBase> OnPageExit)
    {
        base.OnPlay(doAnim, OnPageExit);
        m_Map.OnPlay();
        m_LocationInfo.SetActivate(false);
        m_Stage.localizeKey = BattleManager.Instance.m_BattleProgress.m_Stage.GetLocalizeKey();
        m_Style.localizeKey = BattleManager.Instance.m_BattleProgress.m_GameStyle.GetLocalizeKey();
    }

    private void Update()
    {
        m_Map.Tick(Time.deltaTime);
    }

    void OnChunkSelect(int chunkIndex)
    {
        m_LocationInfo.SetActivate(chunkIndex != -1);
        if (chunkIndex == -1)
            return;

        InteractBattleBase chunk = BattleManager.Instance.m_StageInteracts[chunkIndex];
        m_LocationName.localizeKey = chunk.GetUITitleKey();
        m_LocationIntro.localizeKey = chunk.GetUIIntroKey();
        m_LocationInfo.SetActivate(true);
    }
}
