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
    Text m_Coins, m_Keys;
    ValueLerpSeconds m_CoinLerp;

    Transform m_Mission;
    UIT_TextExtend m_MissionData;

    public Texture2D m_ProgressRampSample;
    Transform m_Progress;
    Text m_Time, m_Attack, m_Health;
    ValueChecker<int> m_TimeValueChecker;
    ValueChecker<int> m_MinuteValueCheck;

    Transform m_EliteHealth;
    Text m_EliteHealthAmount;
    Image m_EliteHealthImage;
    UIT_TextExtend m_EliteName;
    ValueLerpSeconds m_EliteHealthLerp;
    ValueChecker<float> m_EliteHealthCheck;
    EntityCharacterBase m_Elite;

    Transform m_Rank;
    Image m_RankFill;
    Text m_RankAmount;
    ValueLerpSeconds m_RankLerp;

    protected override void Init()
    {
        base.Init();
        m_Minimap = transform.Find("Minimap");
        m_Minimap.GetComponent<Button>().onClick.AddListener(() => { GameUIManager.Instance.ShowPage<UI_Map>(true, true, .1f); });
        m_Map = new UIC_Minimap(m_Minimap.Find("Map"));

        transform.Find("EquipmentButton").GetComponent<Button>().onClick.AddListener(OnEquipmentBtnClick);

        m_Mission = transform.Find("Mission");
        m_MissionData = m_Mission.GetComponent<UIT_TextExtend>();

        m_Currency = transform.Find("Currency");
        m_Coins = m_Currency.Find("CoinData").GetComponent<Text>();
        m_Keys = m_Currency.Find("KeyData").GetComponent<Text>();
        m_CoinLerp = new ValueLerpSeconds(-1f, 20f, 1f, (float value) => { m_Coins.text = ((int)value).ToString(); });

        m_Progress = transform.Find("Progress");
        m_Time = m_Progress.Find("Time").GetComponent<Text>();
        m_Attack = m_Progress.Find("Attack").GetComponent<Text>();
        m_Health = m_Progress.Find("Health").GetComponent<Text>();
        m_TimeValueChecker = new ValueChecker<int>(-1);
        m_MinuteValueCheck = new ValueChecker<int>(-1);

        m_EliteHealth = transform.Find("EliteHealth");
        m_EliteHealthAmount = m_EliteHealth.Find("Amount").GetComponent<Text>();
        m_EliteHealthImage = m_EliteHealth.Find("Health").GetComponent<Image>();
        m_EliteName = m_EliteHealth.Find("Name").GetComponent<UIT_TextExtend>();
        m_EliteHealthLerp = new ValueLerpSeconds(0,.1f,1f,(float value)=> { m_EliteHealthImage.fillAmount = value; });
        m_EliteHealthCheck = new ValueChecker<float>(-1);

        m_Rank = transform.Find("Rank");
        m_RankFill = m_Rank.Find("Fill").GetComponent<Image>();
        m_RankAmount = m_Rank.Find("Amount").GetComponent<Text>();
        m_RankLerp = new ValueLerpSeconds(0, .1f, .3f, (float value) => {m_RankFill.fillAmount=value - Mathf.Floor(value); });

        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Add<bool>(enum_BC_GameStatus.OnGameTransmitStatus, OnTransmissionStatus);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityCharacterBase>(enum_BC_GameStatus.OnGameTransmitEliteStatus, OnTransmissionEliteStatus);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageStart, OnStageStart);
        TBroadCaster<enum_BC_GameStatus>.Remove<bool>(enum_BC_GameStatus.OnGameTransmitStatus, OnTransmissionStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityCharacterBase>(enum_BC_GameStatus.OnGameTransmitEliteStatus, OnTransmissionEliteStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, m_Map.OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, m_Map.OnEntityRecycle);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerExpireManager>(enum_BC_UIStatus.UI_PlayerCurrencyUpdate, OnCurrencyUpdate);
    }

    void OnEquipmentBtnClick()=> UIManager.Instance.ShowPage<UI_PlayerDetail>(true, true, 0f);

    void OnStageStart()
    {
        m_Map.OnPlay();
        m_MissionData.text = "UI_GAMESTATUS_MISSION".GetKeyLocalized() + "UI_MISSION_SEARCHSIGNAL".GetKeyLocalized();
        m_EliteHealth.SetActivate(false);
    }

    void OnCurrencyUpdate(PlayerExpireManager _playerInfo)
    {
        m_CoinLerp.SetLerpValue(_playerInfo.m_Coins);
        m_Keys.text = _playerInfo.m_Keys.ToString();
        m_RankAmount.text = string.Format("Lv.{0}", _playerInfo.m_RankManager.m_Rank.ToString());
        m_RankLerp.SetLerpValue(_playerInfo.m_RankManager.m_ExpCurRankScale+_playerInfo.m_RankManager.m_Rank);
    }

    void OnTransmissionStatus(bool transmiting)=>m_MissionData.text = "UI_GAMESTATUS_MISSION".GetKeyLocalized() + (transmiting ? "UI_MISSION_SURVIVE" : "UI_MISSION_ENTERPORTAL").GetKeyLocalized();

    void OnTransmissionEliteStatus(EntityCharacterBase elite)
    {
        m_Elite = elite;
        if (m_Elite)
        {
            m_EliteName.localizeKey = m_Elite.GetNameLocalizeKey();
            m_EliteHealthLerp.SetFinalValue(1f);
            m_EliteHealthCheck.Check(m_Elite.m_Health.m_MaxHealth);
            m_EliteHealthAmount.text = string.Format("{0} / {1}", (int)m_Elite.m_Health.m_CurrentHealth, (int)m_Elite.m_Health.m_MaxHealth);
        }
        m_EliteHealth.SetActivate(m_Elite);
    }

    private void Update()
    {
        if (BattleManager.Instance.m_GameLoading)
            return;

        float deltaTime = Time.deltaTime;

        m_CoinLerp.TickDelta(deltaTime);
        m_RankLerp.TickDelta(deltaTime);
        m_Map.MinimapUpdate(BattleManager.Instance.m_LocalPlayer);

        int secondPassed= (int)BattleManager.Instance.m_BattleEntity.m_TimeElapsed;
        if (m_TimeValueChecker.Check(secondPassed))
            m_Time.text = TTime.TTimeTools.GetMinuteSecond(m_TimeValueChecker.value1);

        int minutePassed = BattleManager.Instance.m_BattleEntity.m_MinutesElapsed;
        if (m_MinuteValueCheck.Check(minutePassed))
        {
            Color color = m_ProgressRampSample.GetPixel((int)(m_ProgressRampSample.width * ((float)minutePassed / UIConst.I_GameProgressDifficultyColorRampMaxMinutes)), 1);
            m_Attack.color = color;
            m_Health.color = color;

            m_Attack.text = string.Format("+{0}%", GameConst.F_EnermyDamageMultiplierPerMinutePassed * minutePassed * 100);
            m_Health.text = string.Format("+{0}%", GameConst.F_EnermyMaxHealthMultiplierPerMinutePassed * minutePassed * 100);
        }

        m_EliteHealthLerp.TickDelta(deltaTime);
        if(m_Elite && m_EliteHealthCheck.Check(m_Elite.m_Health.m_CurrentHealth))
        {
            m_EliteHealthLerp.SetLerpValue(m_Elite.m_Health.F_HealthMaxScale);
            m_EliteHealthAmount.text = string.Format("{0} / {1}", m_Elite.m_Health.m_CurrentHealth, m_Elite.m_Health.m_MaxHealth);
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

        public override void OnPlay()
        {
            base.OnPlay();
            UpdateMapRotation(GameLevelManager.Instance.GetMapAngle(CameraController.Instance.m_Yaw));

            m_Locations.ClearGrid();
            BattleManager.Instance.m_StageInteracts.Traversal((InteractBattleBase interactData) =>
            {
                Image image = m_Locations.AddItem(m_Locations.m_Count);
                image.sprite = GameUIManager.Instance.m_CommonSprites[interactData.m_InteractType.GetInteractIcon()];
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
