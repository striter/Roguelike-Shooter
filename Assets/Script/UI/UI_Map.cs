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
        UIT_EventTriggerListener m_EventTrigger;
        Vector2 m_MapOffsetBase,m_PreValidOffset;
        float m_MapScaleBase;
        public UIC_Map(Transform transform) : base(transform, LevelConst.I_UIMapScale)
        {
            m_EventTrigger = transform.GetComponent<UIT_EventTriggerListener>();
            m_EventTrigger.OnDragDelta = OnMapDrag;
            m_EventTrigger.OnWorldClick = OnMapClick;
            DoMapInit();
            m_MapScaleBase = GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y);
            UpdateMapRotation(m_MapScaleBase);
            UpdatePlayer();
            m_MapOffsetBase = m_Player.rectTransform.anchoredPosition;
            m_PreValidOffset = m_MapOffsetBase;
            m_Map_Origin_Base.rectTransform.anchoredPosition = Vector2.zero;
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
            Debug.Log("Map Pos Clicked+" + position);
        }

        bool isMapPosValid(int x, int y) => (x>=0&&y>=0&& x < m_Map_Origin_Base_Fog.texture.width&&y < m_Map_Origin_Base_Fog.texture.height)&& GameLevelManager.Instance.CheckTileRevealed(x,y);

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
    Transform m_StageInfo;
    UIT_TextExtend m_Stage,m_Style;
    protected override void Init()
    {
        base.Init();
        m_MapContainer = rtf_Container.Find("MapContainer");
        m_Map = new UIC_Map( m_MapContainer.Find("Map"));

        m_StageInfo = m_MapContainer.Find("StageInfo");
        m_Stage = m_StageInfo.Find("Stage").GetComponent<UIT_TextExtend>();
        m_Style = m_StageInfo.Find("Style").GetComponent<UIT_TextExtend>();

        m_Stage.localizeKey = GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey();
        m_Style.localizeKey = GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey();
    }
    private void Update()
    {
        m_Map.Tick(Time.deltaTime);
    }
    
}
