using GameSetting;
using UnityEngine;
using UnityEngine.UI;
using LevelSetting;
using TSpecialClasses;
public class UIC_PlayerStatus : UIControlBase
{
    public AnimationCurve m_DyingCurve;
    RawImage img_Dying;
    
    EntityCharacterPlayer m_Player;
    
    Transform tf_StatusData;
    RectTransform tf_WeaponData;
    AmmoData m_Ammo1Data, m_Ammo2Data;

    Transform tf_ArmorData;
    RectTransform rtf_ArmorFillHandle;
    Image img_ArmorFill;
    UIT_TextExtend m_ArmorAmount;

    Transform tf_HealthData;
    RectTransform rtf_HealthFillHandle;
    Image img_HealthFill;
    UIT_TextExtend m_HealthAmount;

    ValueLerpSeconds m_HealthLerp,  m_ArmorLerp;
    

    TSpecialClasses.ValueChecker<bool> m_DyingCheck;
    protected override void Init()
    {
        base.Init();
        img_Dying = transform.Find("Dying").GetComponent<RawImage>();

        tf_StatusData = transform.Find("StatusData");
        tf_WeaponData = tf_StatusData.Find("WeaponData") as RectTransform;
        m_Ammo1Data = new AmmoData(tf_WeaponData.Find("Ammo1Data"));
        m_Ammo2Data = new AmmoData(tf_WeaponData.Find("Ammo2Data"));

        tf_ArmorData = tf_StatusData.Find("ArmorData");
        img_ArmorFill = tf_ArmorData.Find("Fill").GetComponent<Image>();
        rtf_ArmorFillHandle = img_ArmorFill.transform.Find("Handle").GetComponent<RectTransform>();
        m_ArmorAmount = tf_ArmorData.Find("Amount").GetComponent<UIT_TextExtend>();

        tf_HealthData = tf_StatusData.Find("HealthData");
        img_HealthFill = tf_HealthData.Find("Fill").GetComponent<Image>();
        rtf_HealthFillHandle = img_HealthFill.transform.Find("Handle").GetComponent<RectTransform>();
        m_HealthAmount = tf_HealthData.Find("Amount").GetComponent<UIT_TextExtend>();
        m_HealthAmount = tf_HealthData.Find("Amount").GetComponent<UIT_TextExtend>();

        m_HealthLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_HealthFill.fillAmount = value;
            rtf_HealthFillHandle.ReAnchorReposX(value);
        });
        m_ArmorLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => {
            img_ArmorFill.fillAmount = value;
            rtf_ArmorFillHandle.ReAnchorReposX(value);
        });

        m_DyingCheck = new ValueChecker<bool>(true);

        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonUpdate, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthUpdate, OnHealthStatus);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonUpdate, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityPlayerHealth>(enum_BC_UIStatus.UI_PlayerHealthUpdate, OnHealthStatus);
    }
    
    private void Update()
    {
        if (!m_Player)
            return;
        
        tf_WeaponData.SetWorldViewPortAnchor(m_Player.tf_UIStatus.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

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
        m_Ammo1Data.UpdateData(_player.m_Weapon1);
        m_Ammo2Data.UpdateData(_player.m_Weapon2);
    }

    void OnHealthStatus(EntityPlayerHealth _healthManager)
    {
        m_ArmorLerp.ChangeValue(_healthManager.F_ArmorMaxScale);
        m_HealthLerp.ChangeValue(_healthManager.F_HealthMaxScale);

        m_ArmorAmount.text=string.Format("{0} <color=#FFCB4e>/ {1}</color>", (int)_healthManager.m_CurrentArmor,(int)_healthManager.m_MaxArmor);
        m_HealthAmount.text = string.Format("{0} <color=#FFCB4e>/ {1}</color>", (int)_healthManager.m_CurrentHealth,(int)_healthManager.m_MaxHealth);
    }

    class AmmoData
    {
        RectTransform transform;
        GridLayoutGroup m_AmmoLayout;
        float m_AmmoGridWidth;
        UIT_GridControllerGridItem<UIGI_AmmoItem> m_AmmoGrid;
        UIT_TextExtend m_AmmoAmount, m_AmmoAmountProjection;
        ValueChecker<int, int> m_AmmoUpdate = new ValueChecker<int, int>(-1, -1);
        public AmmoData(Transform _transform)
        {
            transform = _transform as RectTransform;
            m_AmmoAmount = transform.Find("Container/Amount").GetComponent<UIT_TextExtend>();
            m_AmmoAmountProjection = transform.Find("Container/AmountProjection").GetComponent<UIT_TextExtend>();
            m_AmmoGrid = new UIT_GridControllerGridItem<UIGI_AmmoItem>(transform.Find("Container/AmmoGrid"));
            m_AmmoGridWidth = m_AmmoGrid.transform.GetComponent<RectTransform>().sizeDelta.x;
            m_AmmoLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        }


        public void UpdateData(WeaponBase weaponInfo)
        {
            transform.SetActivate(weaponInfo != null);
            if (weaponInfo == null || !m_AmmoUpdate.Check(weaponInfo.m_AmmoLeft, weaponInfo.m_ClipAmount))
                return;

            string ammoText = string.Format("{0} / {1}", weaponInfo.m_AmmoLeft, weaponInfo.m_ClipAmount);
            m_AmmoAmount.text = ammoText;
            m_AmmoAmountProjection.text = ammoText;

            if (m_AmmoGrid.m_Count != weaponInfo.m_ClipAmount)
            {
                m_AmmoGrid.ClearGrid();
                for (int i = 0; i < weaponInfo.m_ClipAmount; i++)
                    m_AmmoGrid.AddItem(i);

                float size = (m_AmmoGridWidth - m_AmmoLayout.padding.right - m_AmmoLayout.padding.left - (weaponInfo.m_ClipAmount - 1) * m_AmmoLayout.spacing.x) / weaponInfo.m_ClipAmount;
                m_AmmoLayout.cellSize = new Vector2(size, m_AmmoLayout.cellSize.y);
            }

            for (int i = 0; i < weaponInfo.m_ClipAmount; i++)
                m_AmmoGrid.GetItem(i).SetValid(i < weaponInfo.m_AmmoLeft);

        }
    }
}
