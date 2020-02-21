using GameSetting;
using UnityEngine;
using UnityEngine.UI;
using LevelSetting;
using TSpecialClasses;
public class UIC_GameStatus : UIControlBase
{
    public AnimationCurve m_DyingCurve;
    RawImage img_Dying;

    Transform tf_Container;
    EntityCharacterPlayer m_Player;

    UIT_GridControllerGridItem<UIGI_ActionExpireInfo> m_ActionExpireGrid;
    UIT_GridControllerGridItem<UIGI_ActionBase> m_EquipmentGrid;
    
    Transform tf_StatusData;
    RectTransform rtf_AmmoData;
    GridLayoutGroup m_AmmoLayout;
    Image img_ReloadFill;
    float m_AmmoGridWidth;
    UIT_GridControllerGridItem<UIGI_AmmoItem> m_AmmoGrid;
    UIT_TextExtend m_AmmoAmount,m_AmmoAmountProjection;

    Transform tf_ArmorData;
    RectTransform rtf_ArmorFillHandle;
    Image img_ArmorFill;
    UIT_TextExtend m_ArmorAmount;

    Transform tf_HealthData;
    RectTransform rtf_HealthFillHandle;
    Image img_HealthFill;
    UIT_TextExtend m_HealthAmount;

    ValueLerpSeconds m_HealthLerp,  m_ArmorLerp;

    Transform tf_Minimap;
    UIC_Minimap m_GameMinimap;

    class UIC_Minimap : UIC_MapBase
    {
        UIT_GridControllerMono<UIGI_MapEntityLocation> m_Enermys;
        UIT_GridControllerMono<Image> m_Locations;
        public UIC_Minimap(Transform transform) : base(transform, LevelConst.I_UIMinimapSize)
        {
            m_Enermys = new UIT_GridControllerGridItem<UIGI_MapEntityLocation>(m_Map_Origin_Base.transform.Find("EnermyGrid"));
            m_Locations = new UIT_GridControllerMono<Image>(m_Map_Origin_Base.transform.Find("LocationGrid"));
            TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
            TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
            TBroadCaster<enum_BC_UIStatus>.Add(enum_BC_UIStatus.UI_ChunkTeleportUnlock, UpdateIconStatus);
        }
        public void OnDestroy()
        {
            TBroadCaster<enum_BC_UIStatus>.Remove(enum_BC_UIStatus.UI_ChunkTeleportUnlock, UpdateIconStatus);
            TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityRecycle);
            TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        }
        public override void DoMapInit()
        {
            base.DoMapInit();
            UpdateIconStatus();
        }
        void OnEntityActivate(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy || entity.m_ControllType != enum_EntityController.AI)
                return;

            m_Enermys.AddItem(entity.m_EntityID).Play(entity);
        }

        void OnEntityRecycle(EntityBase entity)
        {
            if (entity.m_Flag != enum_EntityFlag.Enermy || entity.m_ControllType != enum_EntityController.AI)
                return;

            m_Enermys.RemoveItem(entity.m_EntityID);
        }

        public void MinimapUpdate(EntityCharacterPlayer player)
        {
            UpdateMap(GameLevelManager.Instance.GetMapAngle(CameraController.CameraRotation.eulerAngles.y));
            m_Map_Origin_Base.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(player.transform.position) * -m_MapScale;
            m_Enermys.m_Pool.m_ActiveItemDic.Traversal((int identity, UIGI_MapEntityLocation item) => { item.Tick(); });
            m_Locations.m_Pool.m_ActiveItemDic.Traversal((int identity, Image image) => { image.transform.rotation = Quaternion.identity; });
        }
        public void UpdateIconStatus()
        {
            m_Locations.ClearGrid();
            GameManager.Instance.m_GameChunkData.Traversal((GameChunk chunkData) =>
            {
                if (!chunkData.GetChunkMapIconShow)
                    return;

                Image image = m_Locations.AddItem(m_Locations.I_Count);
                image.sprite = GameUIManager.Instance.m_InGameSprites[chunkData.GetChunkMapLocationSprite];
                image.rectTransform.anchoredPosition = GameLevelManager.Instance.GetOffsetPosition(chunkData.GetChunkMapLocationPosition);
            });
        }

    }

    TSpecialClasses.ValueChecker<bool> m_DyingCheck;
    protected override void Init()
    {
        base.Init();
        tf_Container = transform.Find("Container");

        img_Dying = tf_Container.Find("Dying").GetComponent<RawImage>();

        tf_StatusData = tf_Container.Find("StatusData");
        rtf_AmmoData = tf_StatusData.Find("AmmoData") as RectTransform;
        m_AmmoAmount =  rtf_AmmoData.Find("Container/Amount").GetComponent<UIT_TextExtend>();
        m_AmmoAmountProjection = rtf_AmmoData.Find("Container/AmountProjection").GetComponent<UIT_TextExtend>();
        m_AmmoGrid = new UIT_GridControllerGridItem<UIGI_AmmoItem>(rtf_AmmoData.Find("Container/AmmoGrid"));
        m_AmmoGridWidth = m_AmmoGrid.transform.GetComponent<RectTransform>().sizeDelta.x;
        m_AmmoLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        img_ReloadFill = m_AmmoGrid.transform.Find("Reload").GetComponent<Image>();

        tf_ArmorData = tf_StatusData.Find("ArmorData");
        img_ArmorFill = tf_ArmorData.Find("Fill").GetComponent<Image>();
        rtf_ArmorFillHandle = img_ArmorFill.transform.Find("Handle").GetComponent<RectTransform>();
        m_ArmorAmount = tf_ArmorData.Find("Amount").GetComponent<UIT_TextExtend>();

        tf_HealthData = tf_StatusData.Find("HealthData");
        img_HealthFill = tf_HealthData.Find("Fill").GetComponent<Image>();
        rtf_HealthFillHandle = img_HealthFill.transform.Find("Handle").GetComponent<RectTransform>();
        m_HealthAmount = tf_HealthData.Find("Amount").GetComponent<UIT_TextExtend>();
        m_HealthAmount = tf_HealthData.Find("Amount").GetComponent<UIT_TextExtend>();
        
        m_ActionExpireGrid = new UIT_GridControllerGridItem<UIGI_ActionExpireInfo>(tf_Container.Find("ActionExpireGrid"));
        m_EquipmentGrid = new UIT_GridControllerGridItem<UIGI_ActionBase>(tf_Container.Find("EquipmentGrid"));
        m_EquipmentGrid.transform.GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.ShowPage<UI_EquipmentPack>(true, 0f).Show(m_Player.m_CharacterInfo));

        m_HealthLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_HealthFill.fillAmount = value;
            rtf_HealthFillHandle.ReAnchorReposX(value);
        });
        m_ArmorLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_ArmorFill.fillAmount = value;
            rtf_ArmorFillHandle.ReAnchorReposX(value);
        });

        tf_Minimap = tf_Container.Find("Minimap");
        tf_Minimap.GetComponent<Button>().onClick.AddListener(()=> { if (!GameManager.Instance.m_Battling) GameUIManager.Instance.ShowPage<UI_Map>(true);  });
        m_GameMinimap = new UIC_Minimap(tf_Minimap.Find("Map"));

        m_DyingCheck = new ValueChecker<bool>(true);

        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerPerkStatus, OnEquipmentStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnGameBegin, m_GameMinimap.DoMapInit);
    }

    protected override void OnDestroy()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerPerkStatus, OnEquipmentStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnGameBegin, m_GameMinimap.DoMapInit);
        m_GameMinimap.OnDestroy();
    }
    
    private void Update()
    {
        if (!m_Player)
            return;

        m_GameMinimap.MinimapUpdate(m_Player);

        rtf_AmmoData.SetWorldViewPortAnchor(m_Player.tf_UIStatus.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

        m_HealthLerp.TickDelta(Time.unscaledDeltaTime);
        m_ArmorLerp.TickDelta(Time.unscaledDeltaTime);

        if (m_DyingCheck.check1)
        {
            float dyingValue = 1-Mathf.InverseLerp(UIConst.I_PlayerDyingMinValue, UIConst.I_PlayerDyingMaxValue, m_Player.m_Health.m_CurrentHealth) ;
            img_Dying.color = TCommon.ColorAlpha(img_Dying.color,dyingValue+m_DyingCurve.Evaluate(Time.time)*.3f);
        }
    }

    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        m_Player = _player;
        bool dying = !m_Player.m_IsDead && m_Player.m_Health.m_CurrentHealth < UIConst.I_PlayerDyingMaxValue;
        if(m_DyingCheck.Check(dying))
            img_Dying.SetActivate(dying);
        OnAmmoStatus(_player.m_WeaponCurrent);
    }

    void OnHealthStatus(EntityPlayerHealth _healthManager)
    {
        m_ArmorLerp.ChangeValue(_healthManager.F_ArmorMaxScale);
        m_HealthLerp.ChangeValue(_healthManager.F_HealthMaxScale);
        m_ArmorAmount.text=string.Format("{0} <color=#FFCB4e>/ {1}</color>", (int)_healthManager.m_CurrentArmor,(int)_healthManager.m_MaxArmor);
        m_HealthAmount.text = string.Format("{0} <color=#FFCB4e>/ {1}</color>", (int)_healthManager.m_CurrentHealth,(int)_healthManager.m_MaxHealth);
    }

    void OnAmmoStatus(WeaponBase weaponInfo)
    {
        rtf_AmmoData.transform.SetActivate(weaponInfo != null);
        if (weaponInfo == null)
            return;

        string ammoText = string.Format("{0} / {1}", (int)weaponInfo.I_AmmoLeft, weaponInfo.I_ClipAmount);
        m_AmmoAmount.text = ammoText;
        m_AmmoAmountProjection.text = ammoText;
        if (m_AmmoGrid.I_Count != weaponInfo.I_ClipAmount)
        {
            m_AmmoGrid.ClearGrid();
            if (weaponInfo.I_ClipAmount <= UIConst.I_AmmoCountToSlider)
            {
                for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                    m_AmmoGrid.AddItem(i);

                float size = (m_AmmoGridWidth - m_AmmoLayout.padding.right - m_AmmoLayout.padding.left - (weaponInfo.I_ClipAmount - 1) * m_AmmoLayout.spacing.x) / weaponInfo.I_ClipAmount;
                m_AmmoLayout.cellSize = new Vector2(size, m_AmmoLayout.cellSize.y);
            }
        }
    }
    
    void OnEquipmentStatus(PlayerInfoManager infoManager)
    {
        m_EquipmentGrid.ClearGrid();
        for (int i = 0; i < infoManager.m_ExpirePerks.Count; i++)
            m_EquipmentGrid.AddItem(i).SetInfo(infoManager.m_ExpirePerks[i]);
    }
}
