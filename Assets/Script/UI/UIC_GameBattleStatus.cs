using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameBattleStatus : UIControlBase {
    Text m_Data;
    UIC_Minimap m_GameMinimap;

    class UIC_Minimap : UIC_MapBase
    {
        UIT_GridControllerMono<UIGI_MapEntityLocation> m_Enermys;
        public UIC_Minimap(Transform transform) : base(transform, LevelConst.I_UIMinimapSize)
        {
            m_Enermys = new UIT_GridControllerGridItem<UIGI_MapEntityLocation>(m_Map_Origin_Base.transform.Find("EnermyGrid"));
            TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
            TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
            TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, DoMapInit);
        }
        public void OnDestroy()
        {
            TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityRecycle);
            TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
            TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageFinished, DoMapInit);
        }
        
        void OnEntityActivate(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy)
                return;

            m_Enermys.AddItem(entity.m_EntityID).Play(entity);
        }

        void OnEntityRecycle(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy)
                return;

            m_Enermys.RemoveItem(entity.m_EntityID);
        }

        public void MinimapUpdate(EntityCharacterPlayer player)
        {
            UpdateMap(GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y));
            m_Map_Origin_Base.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(player.transform.position) * -m_MapScale;
            m_Enermys.m_Pool.m_ActiveItemDic.Traversal((int identity, UIGI_MapEntityLocation item) => { item.Tick(); });
        }
    }

    protected override void Init()
    {
        base.Init();
        m_Data = transform.Find("Data").GetComponent<Text>();
        Transform miniMap = transform.Find("Minimap");
        miniMap.GetComponent<Button>().onClick.AddListener(() => { GameUIManager.Instance.ShowPage<UI_Map>(true, true, .1f); });
        m_GameMinimap = new UIC_Minimap(miniMap);
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_GameMinimap.OnDestroy();
    }

    private void Update()
    {
        if (GameManager.Instance.m_GameLoading)
            return;
        m_Data.text = string.Format("{0},{1},{2}",(int)GameManager.Instance.m_GameLevel.m_TimeElapsed,GameManager.Instance.m_GameLevel.m_MinutesElapsed,GameManager.Instance.m_GameLevel.m_BattleTransmiting?"Battle":"Relax");
        m_GameMinimap.MinimapUpdate(GameManager.Instance.m_LocalPlayer);
    }
}
