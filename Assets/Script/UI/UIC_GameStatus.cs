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

    Transform tf_Level;
    UIT_TextExtend m_Level_Info, m_Level_Index;
    Image m_Level_Icon;

    ValueLerpSeconds m_HealthLerp,  m_ArmorLerp;
    

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

        tf_Level = tf_Container.Find("Level");
        m_Level_Icon = tf_Level.Find("Icon").GetComponent<Image>();
        m_Level_Index = tf_Level.Find("Index").GetComponent<UIT_TextExtend>();
        m_Level_Info = tf_Level.Find("Info").GetComponent<UIT_TextExtend>();

        m_ActionExpireGrid = new UIT_GridControllerGridItem<UIGI_ActionExpireInfo>(tf_Container.Find("ActionExpireGrid"));
        m_HealthLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_HealthFill.fillAmount = value;
            rtf_HealthFillHandle.ReAnchorReposX(value);
        });
        m_ArmorLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_ArmorFill.fillAmount = value;
            rtf_ArmorFillHandle.ReAnchorReposX(value);
        });

        m_DyingCheck = new ValueChecker<bool>(true);

        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnLevelStart, OnLevelStart);
    }

    protected override void OnDestroy()
    {
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnLevelStart, OnLevelStart);
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

    void OnLevelStart()
    {
        m_Level_Icon.sprite = GameUIManager.Instance.m_CommonSprites[GameManager.Instance.m_GameLevel.GetLevelIconSprite()];
        m_Level_Index.formatText("UI_Level_Index", "<Color=#F8C64BFF>" + (GameManager.Instance.m_GameLevel.m_LevelPassed+1) +"</Color>");
        m_Level_Info.localizeKey = GameManager.Instance.m_GameLevel.GetLevelInfoKey();
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
    
}
