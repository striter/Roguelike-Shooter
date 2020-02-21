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
        Action<int,bool> OnLocationClick;
        Vector2 m_MapOffsetBase,m_PreValidOffset;
        float m_MapScaleBase;
        RectTransform m_LocationSelect;
        public int m_LocationSelecting { get; private set; }

        public UIC_Map(Transform transform,Action<int,bool> OnLocationClick) : base(transform, LevelConst.I_UIMapScale)
        {
            m_LocationsGrid = new UIT_GridControllerGridItem<UIGI_MapLocations>(m_Map_Origin_Base.transform.Find("LocationsGrid"));
            m_LocationSelect = m_Map_Origin_Base.transform.Find("LocationSelect") as RectTransform;
            m_LocationSelecting = -1;
            m_LocationSelect.SetActivate(false);
            m_EventTrigger = transform.GetComponent<UIT_EventTriggerListener>();
            m_EventTrigger.OnDragDelta = OnMapDrag;
            m_EventTrigger.OnWorldClick = OnMapClick;
            DoMapInit();
            this.OnLocationClick = OnLocationClick;
            m_MapScaleBase = GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y);
            UpdateMap(m_MapScaleBase);
            UpdateIconStatus();
            m_MapOffsetBase = m_Player.rectTransform.anchoredPosition;
            m_PreValidOffset = m_MapOffsetBase;
            m_Map_Origin_Base.rectTransform.anchoredPosition = Vector2.zero;
        }


        Vector3 GetIconScale() => Vector3.one * LevelConst.F_UIMapIconBaseScale / m_MapScale;
        void OnMapScaleChange(float scale)
        {
            base.ChangeMapScale(scale);
            Vector3 iconScale = GetIconScale();
            m_LocationsGrid.m_Pool.m_ActiveItemDic.Traversal((UIGI_MapLocations locations) => { locations.transform.localScale = iconScale; });
            m_LocationSelect.transform.localScale = iconScale;
            m_Map_Origin_Base.rectTransform.anchoredPosition = m_MapOffsetBase * -m_MapScale;
        }

        void UpdateIconStatus()
        {
            Vector3 iconScale = GetIconScale();
            m_LocationsGrid.ClearGrid();
            GameManager.Instance.m_GameChunkData.Traversal((int chunkIndex, GameChunk chunkData) =>
            {
                if (!chunkData.GetChunkMapIconShow)
                    return;

                UIGI_MapLocations locations = m_LocationsGrid.AddItem(chunkIndex);
                locations.Play( chunkData);
                locations.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(chunkData.GetChunkMapLocationPosition );
                locations.transform.rotation = Quaternion.identity;
                locations.transform.localScale = iconScale;
            });
            m_LocationSelect.transform.rotation = Quaternion.identity;
            m_LocationSelect.transform.localScale = iconScale;
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
            OnLocationSelect(castIndex);
        }

        bool isMapPosValid(int x, int y) => (x>=0&&y>=0&& x < m_Map_Origin_Base_Fog.texture.width&&y < m_Map_Origin_Base_Fog.texture.height)&& (m_Map_Origin_Base_Fog.texture as Texture2D).GetPixel(x, y).a == 0;


        void OnLocationSelect(int locationIndex)
        {
            bool doubleClick = m_LocationSelecting == locationIndex;
            m_LocationSelecting = locationIndex;
            OnLocationClick(m_LocationSelecting,doubleClick);
            m_LocationSelect.SetActivate(m_LocationSelecting != -1);
            if (m_LocationSelecting != -1)
                m_LocationSelect.anchoredPosition = m_LocationsGrid.GetItem(m_LocationSelecting).rectTransform.anchoredPosition;
        }

        void OnMapDrag(Vector2 delta)
        {
            delta=m_Map_Origin_Base.rectTransform.InverseTransformDirection(delta);
            m_MapOffsetBase -= delta / m_MapScale;
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
    UIT_TextExtend m_LocationName, m_LocationIntro,m_Stage,m_Style;
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

        m_Stage.localizeKey = GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey();
        m_Style.localizeKey = GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey();
        m_LocationInfo.SetActivate(false);
    }
    private void Update()
    {
        m_Map.Tick(Time.deltaTime);
    }

    void OnChunkSelect(int chunkIndex,bool doubleClick)
    {
        m_LocationInfo.SetActivate(chunkIndex!=-1);
        if (chunkIndex == -1)
            return;

        GameChunk chunk = GameManager.Instance.m_GameChunkData[chunkIndex];
        m_LocationName.localizeKey = chunk.GetChunkMapNameKey;
        m_LocationIntro.localizeKey = chunk.GetChunkMapIntroKey;
        m_LocationInfo.SetActivate(true);

        if(doubleClick && chunk.OnMapDoubleClick())
            OnCancelBtnClick();
    }
}
