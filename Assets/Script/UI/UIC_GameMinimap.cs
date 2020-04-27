using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameMinimap : UIControlBase {
    Text m_Data;
    Transform m_Minimap;
    Text m_Stage, m_Style;
    UIC_Minimap m_Map;

    class UIC_Minimap : UIC_MapBase
    {
        UIT_GridControllerMono<UIGI_MapEntityLocation> m_Enermys;
        UIT_GridControllerMono<Image> m_Locations;
        public UIC_Minimap(Transform transform) : base(transform, LevelConst.I_UIMinimapSize)
        {
            m_Enermys = new UIT_GridControllerGridItem<UIGI_MapEntityLocation>(m_Map_Origin_Base.transform.Find("EnermyGrid"));
            m_Locations = new UIT_GridControllerMono<Image>(m_Map_Origin_Base.transform.Find("LocationGrid"));
        }
        public override void DoMapInit()
        {
            base.DoMapInit();
            UpdateMapRotation(GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y));

            m_Locations.ClearGrid();
            GameManager.Instance.m_StageInteracts.Traversal((InteractGameBase interactData) =>
            {
                Image image = m_Locations.AddItem(m_Locations.m_Count);
                image.sprite = GameUIManager.Instance.m_InGameSprites[interactData.GetInteractMapIcon()];
                image.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(interactData.transform.position);
                image.transform.rotation = Quaternion.identity;
            });
        }

        public void OnEntityActivate(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy)
                return;

            m_Enermys.AddItem(entity.m_EntityID).Play(entity);
        }

        public void OnEntityRecycle(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy)
                return;

            m_Enermys.RemoveItem(entity.m_EntityID);
        }

        public void MinimapUpdate(EntityCharacterPlayer player)
        {
            m_Map_Origin_Base.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(player.transform.position) * -m_MapScale;
            m_Enermys.m_Pool.m_ActiveItemDic.Traversal((int identity, UIGI_MapEntityLocation item) => { item.Tick(); });
            m_Player.Tick();
        }
    }

    protected override void Init()
    {
        base.Init();
        m_Data = transform.Find("Data").GetComponent<Text>();
        m_Minimap = transform.Find("Minimap");
        m_Minimap.GetComponent<Button>().onClick.AddListener(() => { GameUIManager.Instance.ShowPage<UI_Map>(true, true, .1f); });
        m_Stage = m_Minimap.Find("Stage").GetComponent<UIT_TextExtend>();
        m_Style = m_Minimap.Find("Style").GetComponent<UIT_TextExtend>();
        m_Map = new UIC_Minimap(m_Minimap.Find("Map"));
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
    }

    void OnStageStart()
    {
        m_Map.DoMapInit();
        m_Stage.text = string.Format("<color=#ffffff88>{0}</color>{0}:<color=#fe9e00>{1}</color>", TLocalization.GetKeyLocalized("UI_MAP_STAGE"),TLocalization.GetKeyLocalized(GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey()));
        m_Style.text = string.Format("<color=#ffffff88>{0}</color>{0}:<color=#fe9e00>{1}</color>", TLocalization.GetKeyLocalized("UI_MAP_STYLE"), TLocalization.GetKeyLocalized(GameManager.Instance.m_GameLevel.m_GameStage.GetLocalizeKey()));
    }

    private void Update()
    {
        if (GameManager.Instance.m_GameLoading)
            return;
        m_Data.text = string.Format("{0},{1},{2}",(int)GameManager.Instance.m_GameLevel.m_TimeElapsed,GameManager.Instance.m_GameLevel.m_MinutesElapsed,GameManager.Instance.m_GameLevel.m_BattleTransmiting?"Battle":"Relax");
        m_Map.MinimapUpdate(GameManager.Instance.m_LocalPlayer);
    }
}
