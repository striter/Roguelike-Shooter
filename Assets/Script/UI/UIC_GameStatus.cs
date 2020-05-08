using GameSetting;
using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameStatus : UIControlBase {
    Transform m_Minimap;
    UIC_Minimap m_Map;

    Transform m_Currency;
    Text m_Coins, m_Keys, m_Ranks;
    ValueLerpSeconds m_CoinLerp;

    Transform m_Mission;
    UIT_TextExtend m_MissionData;

    public Texture2D m_ProgressRampSample;
    Transform m_Progress;
    Text m_Time, m_Attack, m_Health;
    ValueChecker<int> m_TimeValueChecker;
    ValueChecker<int> m_MinuteValueCheck;

    protected override void Init()
    {
        base.Init();
        m_Minimap = transform.Find("Minimap");
        m_Minimap.GetComponent<Button>().onClick.AddListener(() => { GameUIManager.Instance.ShowPage<UI_Map>(true, true, .1f); });
        m_Map = new UIC_Minimap(m_Minimap.Find("Map"));

        transform.Find("EquipmentButton").GetComponent<Button>().onClick.AddListener(OnEquipmentBtnClick);

        m_Mission = transform.Find("Mission");
        m_MissionData = m_Mission.Find("Text").GetComponent<UIT_TextExtend>();

        m_Currency = transform.Find("Currency");
        m_Coins = m_Currency.Find("CoinData").GetComponent<Text>();
        m_Keys = m_Currency.Find("KeyData").GetComponent<Text>();
        m_Ranks = m_Currency.Find("RankData").GetComponent<Text>();
        m_CoinLerp = new ValueLerpSeconds(-1f, 20f, 1f, (float value) => { m_Coins.text = ((int)value).ToString(); });

        m_Progress = transform.Find("Progress");
        m_Time = m_Progress.Find("Time").GetComponent<Text>();
        m_Attack = m_Progress.Find("Attack").GetComponent<Text>();
        m_Health = m_Progress.Find("Health").GetComponent<Text>();
        m_TimeValueChecker = new ValueChecker<int>(-1);
        m_MinuteValueCheck = new ValueChecker<int>(-1);

        TBroadCaster<enum_BC_UIStatus>.Add<string>(enum_BC_UIStatus.UI_GameMissionUpdate, OnMissionUpdate);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
    }
    

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<string>(enum_BC_UIStatus.UI_GameMissionUpdate, OnMissionUpdate);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
    }

    void OnEquipmentBtnClick()
    {
        UIManager.Instance.ShowPage<UI_PlayerDetail>(true, true, 0f).Show();
    }

    void OnStageStart()
    {
        m_Map.DoMapInit();
    }

    void OnCurrencyUpdate(PlayerExpireManager _playerInfo)
    {
        m_CoinLerp.ChangeValue(_playerInfo.m_Coins);
        m_Keys.text = _playerInfo.m_Keys.ToString();
        m_Ranks.text = _playerInfo.m_RankManager.m_ExpCurRank + "|" + _playerInfo.m_RankManager.m_ExpToNextRank + "," + _playerInfo.m_RankManager.m_Rank;
    }

    void OnMissionUpdate(string missionKey)=>m_MissionData.text = TLocalization.GetKeyLocalized("UI_GAMESTATUS_MISSION") + TLocalization.GetKeyLocalized(missionKey);

    private void Update()
    {
        if (GameManager.Instance.m_GameLoading)
            return;

        m_CoinLerp.TickDelta(Time.deltaTime);
        m_Map.MinimapUpdate(GameManager.Instance.m_LocalPlayer);

        int secondPassed= (int)GameManager.Instance.m_GameLevel.m_TimeElapsed;
        if (m_TimeValueChecker.Check(secondPassed))
            m_Time.text = TTime.TTimeTools.GetMinuteSecond(m_TimeValueChecker.check1);

        int minutePassed = GameManager.Instance.m_GameLevel.m_MinutesElapsed;
        if (m_MinuteValueCheck.Check(minutePassed))
        {
            Color color = m_ProgressRampSample.GetPixel((int)(m_ProgressRampSample.width * ((float)minutePassed / UIConst.I_GameProgressDifficultyColorRampMaxMinutes)), 1);
            m_Attack.color = color;
            m_Health.color = color;

            m_Attack.text = string.Format("+{0}%", GameConst.F_EnermyDamageMultiplierPerMinutePassed * minutePassed * 100);
            m_Health.text = string.Format("+{0}%", GameConst.F_EnermyMaxHealthMultiplierPerMinutePassed * minutePassed * 100);
        }
    }


    class UIC_Minimap : UIC_MapBase
    {
        UIT_GridControllerGridItem<UIGI_MapEntityLocation> m_Enermys;
        UIT_GridControllerComponent<Image> m_Locations;
        public UIC_Minimap(Transform transform) : base(transform, LevelConst.I_UIMinimapSize)
        {
            m_Enermys = new UIT_GridControllerGridItem<UIGI_MapEntityLocation>(m_Map_Origin_Base.transform.Find("EnermyGrid"));
            m_Locations = new UIT_GridControllerComponent<Image>(m_Map_Origin_Base.transform.Find("LocationGrid"));
        }
        public override void DoMapInit()
        {
            base.DoMapInit();
            UpdateMapRotation(GameLevelManager.Instance.GetMapAngle(CameraController.Instance.m_Yaw));

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

}
