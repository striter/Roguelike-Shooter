using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using LevelSetting;
using System;

public class UI_Map : UIPage {
    Transform m_MapContainer;
    UIC_Map m_MapBase;
    class UIC_Map : UIC_MapBase
    {
        UIT_GridControllerGridItem<UIGI_MapLocations> m_LocationsGrid;
        UIT_EventTriggerListener m_EventTrigger;
        Action<int> OnLocationClick;
        public UIC_Map(Transform transform,Action<int> OnLocationClick) : base(transform, LevelConst.I_UIMapSize)
        {
            m_LocationsGrid = new UIT_GridControllerGridItem<UIGI_MapLocations>(m_Map_Origin_Base.transform.Find("LocationsGrid"));
            m_EventTrigger = transform.GetComponent<UIT_EventTriggerListener>();
            m_EventTrigger.OnDragDelta = OnDragDelta;
            m_EventTrigger.OnWorldClick = OnMapClick;
            DoMapInit();
            this.OnLocationClick = OnLocationClick;
            UpdateMap(GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y));
            UpdateIconStatus();
            m_Map_Origin_Base.rectTransform.anchoredPosition = m_Player.rectTransform.anchoredPosition * -m_MapScale;
        }

        void UpdateIconStatus()
        {
            m_LocationsGrid.ClearGrid();
            GameManager.Instance.m_GameChunkData.Traversal((int chunkIndex, GameChunk chunkData) =>
            {
                Vector3 iconPosition = Vector3.zero;
                string iconSprite = "";
                if (!chunkData.CalculateMapIconLocation(ref iconPosition, ref iconSprite))
                    return;

                UIGI_MapLocations locations = m_LocationsGrid.AddItem(m_LocationsGrid.I_Count);
                locations.Play(chunkIndex, iconSprite, OnLocationClick);
                locations.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(iconPosition);
                locations.transform.rotation = Quaternion.identity;
            });
        }

        void OnDragDelta(Vector2 delta)
        {
            m_Map_Origin.anchoredPosition += delta;
        }

        void OnMapClick(Vector2 position)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Map_Origin_Base_Fog.rectTransform, position, null, out position))
                return;
            float alpha = (m_Map_Origin_Base_Fog.texture as Texture2D).GetPixel((int)position.x,(int)position.y).a;
            if (alpha != 0)
                return;
            Debug.Log("Cast");
            m_LocationsGrid.TraversalItem((int identity, UIGI_MapLocations item) => { item.MapCastCheck(position); });
        }
    }
    Transform m_StageInfo, m_LocationInfo;
    UIT_TextExtend m_LocationName, m_LocationIntro,m_Stage,m_Style;
    int m_currentSelecting;
    protected override void Init()
    {
        base.Init();
        m_MapContainer = rtf_Container.Find("MapContainer");
        m_MapBase = new UIC_Map( m_MapContainer.Find("Map"),OnChunkSelect);

        m_StageInfo = m_MapContainer.Find("StageInfo");
        m_Stage = m_StageInfo.Find("Stage").GetComponent<UIT_TextExtend>();
        m_Style = m_StageInfo.Find("Style").GetComponent<UIT_TextExtend>();
        m_LocationInfo = m_MapContainer.Find("LocationInfo");
        m_LocationName = m_LocationInfo.Find("Name").GetComponent<UIT_TextExtend>();
        m_LocationIntro = m_LocationInfo.Find("Intro").GetComponent<UIT_TextExtend>();

        m_Stage.localizeKey = GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey();
        m_Stage.localizeKey = GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey();
        m_currentSelecting = -1;
        m_LocationInfo.SetActivate(false);
    }

    void OnChunkSelect(int chunkIndex)
    {
        GameChunk chunk = GameManager.Instance.m_GameChunkData[chunkIndex];
        m_LocationName.localizeKey = chunk.GetChunkMapNameKey;
        m_LocationIntro.localizeKey = chunk.GetChunkMapIntroKey;
        m_LocationInfo.SetActivate(true);

        if(m_currentSelecting==chunkIndex&& chunk.OnMapDoubleClick())
        {
            OnCancelBtnClick();
            return;
        }

        m_currentSelecting = chunkIndex;
    }
}
