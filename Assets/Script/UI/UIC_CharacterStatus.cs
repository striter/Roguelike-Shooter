using GameSetting;
using UnityEngine;
using UnityEngine.UI;
using TSpecialClasses;
public class UIC_CharacterStatus : UIControlBase
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
    Slider m_ArmorFill;
    UIT_TextExtend m_ArmorAmount;

    Transform tf_HealthData;
    Slider m_HealthFill;
    UIT_TextExtend m_HealthAmount;

    ValueLerpSeconds m_HealthLerp, m_ArmorLerp;

    Transform tf_MapData;
    UIT_TextExtend m_MapTitle;
    Image m_MapPrevious, m_MapCurrent, m_MapNext;
    TSpecialClasses.AnimationControlBase m_MapAnimation;
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
        m_ArmorFill = tf_ArmorData.Find("Slider").GetComponent<Slider>();
        m_ArmorAmount = tf_ArmorData.Find("ArmorAmount").GetComponent<UIT_TextExtend>();

        tf_HealthData = tf_StatusData.Find("HealthData");
        m_HealthFill = tf_HealthData.Find("Slider").GetComponent<Slider>();
        m_HealthAmount = tf_HealthData.Find("HealthAmount").GetComponent<UIT_TextExtend>();
        
        
        m_ActionExpireGrid = new UIT_GridControllerGridItem<UIGI_ActionExpireInfo>(tf_Container.Find("ActionExpireGrid"));
        m_EquipmentGrid = new UIT_GridControllerGridItem<UIGI_ActionBase>(tf_Container.Find("EquipmentGrid"));

        m_HealthLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => { m_HealthFill.value = value; });
        m_ArmorLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => { m_ArmorFill.value = value; });

        tf_MapData = tf_Container.Find("MapData");
        m_MapTitle = tf_MapData.Find("Title").GetComponent<UIT_TextExtend>();
        m_MapPrevious = tf_MapData.Find("Previous").GetComponent<Image>();
        m_MapCurrent = tf_MapData.Find("Current").GetComponent<Image>();
        m_MapNext = tf_MapData.Find("Next").GetComponent<Image>();
        m_MapAnimation =new AnimationControlBase( tf_MapData.GetComponent<Animation>(),false);

        m_DyingCheck = new ValueChecker<bool>(true);
    }

    private void OnEnable()
    {
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionExpireStatus, OnExpireListStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerEquipmentStatus, OnEquipmentStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerActionExpireStatus, OnExpireListStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerEquipmentStatus, OnEquipmentStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
    }

    public UIC_CharacterStatus SetInGame(bool inGame)
    {
        tf_MapData.SetActivate(inGame);
        return this;
    }

    void OnChangeLevel()
    {
        bool finalLevel = GameManager.Instance.m_GameLevel.B_IsFinalLevel;
        bool firstLevel = GameManager.Instance.m_GameLevel.B_IsFirstLevel;
        m_MapPrevious.SetActivate(!firstLevel);
        if (!firstLevel)
        {
            SBigmapLevelInfo previousInfo = LevelManager.Instance.m_previousLevel;
            m_MapPrevious.sprite = GameUIManager.Instance.m_InGameSprites[(previousInfo == null ? enum_LevelType.End : previousInfo.m_LevelType).GetUISprite()];
        }
        m_MapCurrent.sprite = GameUIManager.Instance.m_InGameSprites[GameManager.Instance.m_GameLevel.m_LevelType.GetUISprite()];
        m_MapNext.SetActivate(!finalLevel);
        m_MapTitle.formatText("UI_Map_Title",string.Format("<color=#FFCD00FF>{0}</color>", (int)(GameManager.Instance.m_GameLevel.m_GameStage-1)*10+LevelManager.Instance.m_currentLevelIndex+1));
        m_MapAnimation.Play(true);
    }
    
    
    private void Update()
    {
        if (!m_Player)
            return;

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

    void OnHealthStatus(EntityHealth _healthManager)
    {
        m_ArmorLerp.ChangeValue(_healthManager.m_CurrentArmor / UIConst.F_UIMaxArmor);
        m_HealthLerp.ChangeValue(_healthManager.F_HealthBaseScale);
        m_ArmorAmount.text=string.Format("{0}",(int)_healthManager.m_CurrentArmor);
        m_HealthAmount.text = string.Format("{0}", (int)_healthManager.m_CurrentHealth);
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

        Color ammoStatusColor = weaponInfo.F_AmmoStatus < .5f ? Color.Lerp(Color.red, Color.white, (weaponInfo.F_AmmoStatus / .5f)) : Color.white;
        if (weaponInfo.I_ClipAmount <= UIConst.I_AmmoCountToSlider)
        {
            for (int i = 0; i < weaponInfo.I_ClipAmount; i++)
                m_AmmoGrid.GetItem(i).Set((weaponInfo.B_Reloading || i > weaponInfo.I_AmmoLeft - 1) ? new Color(0, 0, 0, 1) : ammoStatusColor);
            img_ReloadFill.fillAmount = 0;
        }
        else
        {
            img_ReloadFill.fillAmount = weaponInfo.F_AmmoStatus;
            img_ReloadFill.color = ammoStatusColor;
        }

        if (weaponInfo.B_Reloading)
        {
            img_ReloadFill.fillAmount = weaponInfo.F_ReloadStatus;
            img_ReloadFill.color = Color.Lerp(Color.red, Color.white, weaponInfo.F_ReloadStatus);
        }
    }
    
    void OnExpireListStatus(PlayerInfoManager infoManager)
    {
        m_ActionExpireGrid.ClearGrid();
        for (int i = 0; i < infoManager.m_ActionPlaying.Count; i++)
        {
            if (infoManager.m_ActionPlaying[i].m_ActionType == enum_ActionType.Equipment)
                continue;
            m_ActionExpireGrid.AddItem(i).SetInfo(infoManager.m_ActionPlaying[i]);
        }
    }

    void OnEquipmentStatus(PlayerInfoManager infoManager)
    {
        m_EquipmentGrid.ClearGrid();
        for (int i = 0; i < infoManager.m_ActionEquipment.Count; i++)
            m_EquipmentGrid.AddItem(i).SetInfo(infoManager.m_ActionEquipment[i]);
    }
}
