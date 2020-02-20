using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using LevelSetting;
using System;

public class UI_Map : UIPage {
    Transform m_MapContainer;
    UIC_Map m_Map;
    class UIC_Map : UIC_MapBase
    {
        UIT_GridControllerGridItem<UIGI_MapLocations> m_LocationsGrid;
        UIT_EventTriggerListener m_EventTrigger;
        Action<int,bool> OnLocationClick;
        Vector2 m_MapOffset;
        RectTransform m_LocationSelect;
        public int m_LocationSelecting { get; private set; }
        public UIC_Map(Transform transform,Action<int,bool> OnLocationClick) : base(transform, LevelConst.I_UIMapSize)
        {
            m_LocationsGrid = new UIT_GridControllerGridItem<UIGI_MapLocations>(m_Map_Origin_Base.transform.Find("LocationsGrid"));
            m_LocationSelect = m_Map_Origin_Base.transform.Find("LocationSelect") as RectTransform;
            m_LocationSelecting = -1;
            m_LocationSelect.SetActivate(false);
            m_EventTrigger = transform.GetComponent<UIT_EventTriggerListener>();
            m_EventTrigger.OnDragDelta = (Vector2 delta)=> { m_MapOffset += delta; };
            m_EventTrigger.OnWorldClick = OnMapClick;
            DoMapInit();
            this.OnLocationClick = OnLocationClick;
            UpdateMap(GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y));
            UpdateIconStatus();
            m_MapOffset = Vector2.zero;
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

                UIGI_MapLocations locations = m_LocationsGrid.AddItem(chunkIndex);
                locations.Play( iconSprite);
                locations.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(iconPosition);
                locations.transform.rotation = Quaternion.identity;
            });
        }
        void OnMapClick(Vector2 position)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Map_Origin_Base_Fog.rectTransform, position, null, out position))
                return;
            int castIndex = -1;
            float alpha = (m_Map_Origin_Base_Fog.texture as Texture2D).GetPixel((int)position.x, (int)position.y).a;
            if (alpha == 0)
            {
                m_LocationsGrid.m_Pool.m_ActiveItemDic.TraversalBreak((UIGI_MapLocations item) => {
                    castIndex = item.MapCastCheck(position);
                    return castIndex != -1;
                });
            }
            OnLocationSelect(castIndex);
        }

        void OnLocationSelect(int locationIndex)
        {
            bool doubleClick = m_LocationSelecting == locationIndex;
            m_LocationSelecting = locationIndex;
            OnLocationClick(m_LocationSelecting,doubleClick);
            m_LocationSelect.SetActivate(m_LocationSelecting != -1);
            if (m_LocationSelecting != -1)
                m_LocationSelect.anchoredPosition = m_LocationsGrid.GetItem(m_LocationSelecting).rectTransform.anchoredPosition;
        }

        public void Tick(float deltaTime)
        {
            m_Map_Origin.anchoredPosition = Vector2.Lerp(m_Map_Origin.anchoredPosition,m_MapOffset,deltaTime*20);
            if (m_LocationSelecting!=-1)
                m_LocationSelect.anchoredPosition = Vector2.Lerp(m_LocationSelect.anchoredPosition,m_LocationsGrid.GetItem(m_LocationSelecting).rectTransform.anchoredPosition, deltaTime * 20);
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
        m_Stage.localizeKey = GameManager.Instance.m_GameLevel.m_GameStyle.GetLocalizeKey();
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
