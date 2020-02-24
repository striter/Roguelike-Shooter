using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIC_Control : UIControlBase {
    protected TouchDeltaManager m_TouchDelta { get; private set; }
    Transform tf_InGame;
    Image m_AbilityBG,m_AbilityImg,m_AbilityCooldown;
    Image m_setting;
    ControlWeaponData m_weapon1Data, m_weapon2Data;
    Action OnReload,OnSwap, OnCharacterAbility;
    Action<bool> OnWeaponAction;
    Action<bool> OnMainDown,OnSubDown;
    TSpecialClasses.ValueChecker<bool> m_AbilityCooldownChecker;
    protected override void Init()
    {
        base.Init();
        tf_InGame = transform.Find("InGame");
        m_AbilityBG = transform.Find("Ability").GetComponent<Image>();
        m_AbilityImg = transform.Find("Ability/Image").GetComponent<Image>();
        m_AbilityCooldown = transform.Find("Ability/Cooldown").GetComponent<Image>();
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(OnReloadButtonDown);
        transform.Find("Ability").GetComponent<Button>().onClick.AddListener(OnAbilityButtonDown);
        transform.Find("Main").GetComponent<UIT_EventTriggerListener>().OnPressStatus = OnMainButtonDown;
        transform.Find("Sub").GetComponent<UIT_EventTriggerListener>().OnPressStatus = OnSubButtonDown;

        transform.Find("Settings").GetComponent<Button>().onClick.AddListener(OnSettingBtnClick);
        m_setting = transform.Find("Settings/Image").GetComponent<Image>();
        
        m_weapon1Data = new ControlWeaponData(tf_InGame.Find("Weapon1Data"),OnWeaponFirstActionClick, OnWeaponSwap);
        m_weapon2Data = new ControlWeaponData(tf_InGame.Find("Weapon2Data"),OnWeaponSecondActionClick, OnWeaponSwap);

        m_TouchDelta = transform.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;
        m_AbilityCooldownChecker = new TSpecialClasses.ValueChecker<bool>(true);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OncommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OncommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
    }
    public UIC_Control SetInGame(bool inGame)
    {
        tf_InGame.SetActivate(inGame);
        return this;
    }

    void OnOptionsChanged() => UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    bool CheckControlable() => !UIPageBase.m_PageOpening;
    
    void OncommonStatus(EntityCharacterPlayer player)
    {
        if(player.m_CharacterAbility.m_Cooldowning)
            m_AbilityCooldown.fillAmount = player.m_CharacterAbility.m_CooldownScale;

        if(m_AbilityCooldownChecker.Check(player.m_CharacterAbility.m_Cooldowning))
        {
            m_AbilityBG.sprite = UIManager.Instance.m_CommonSprites[UIConvertions.GetAbilityBackground(m_AbilityCooldownChecker.check1)];
            m_AbilityCooldown.SetActivate(m_AbilityCooldownChecker.check1);
        }
        
        m_weapon1Data.Tick(Time.deltaTime);
        m_weapon2Data.Tick(Time.deltaTime);
    }

    #region Controls
    public void DoBinding(EntityCharacterPlayer player,Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta, Action<bool> _OnMainDown, Action<bool> _OnSubDown, Action _OnSwap, Action _OnReload, Action _OnCharacterAbility)
    {
        m_TouchDelta.AddLRBinding(_OnLeftDelta, _OnRightDelta, CheckControlable);
        OnMainDown = _OnMainDown;
        OnSubDown = _OnSubDown;
        OnReload = _OnReload;
        OnSwap = _OnSwap;
        OnCharacterAbility = _OnCharacterAbility;
        m_AbilityImg.sprite = UIManager.Instance.m_CommonSprites[UIConvertions.GetAbilitySprite(player.m_Character)];
    }
    public void RemoveBinding()
    {
        m_TouchDelta.RemoveAllBinding();
        OnReload = null;
        OnMainDown = null;
        OnCharacterAbility = null;
        OnSwap = null;
    }

    protected void OnReloadButtonDown() => OnReload?.Invoke();
    protected void OnMainButtonDown(bool down, Vector2 pos) => OnMainDown?.Invoke(down);
    protected void OnSubButtonDown(bool down, Vector2 pos) => OnSubDown?.Invoke(down);
    protected void OnWeaponSwap() => OnSwap?.Invoke();
    protected void OnWeaponFirstActionClick() => OnWeaponAction?.Invoke(true);
    protected void OnWeaponSecondActionClick() => OnWeaponAction?.Invoke(false);
    protected void OnAbilityButtonDown() => OnCharacterAbility?.Invoke();
    public void AddDragBinding(Action<bool, Vector2> _OnDragDown, Action<Vector2> _OnDrag)
    {
        transform.localScale = Vector3.zero;
        m_TouchDelta.AddDragBinding(_OnDragDown,_OnDrag);
    }
    public void RemoveDragBinding()
    {
        transform.localScale = Vector3.one;
        m_TouchDelta.RemoveExtraBinding();
    }
    void OnSettingBtnClick()
    {
        if (OnSettingClick != null)
        {
            OnSettingClick();
            return;
        }
        UIManager.Instance.ShowPage<UI_Options>(true, 0f).SetInGame(GameManagerBase.Instance.B_InGame);
    }
    Action OnSettingClick;
    public void OverrideSetting(Action Override = null)
    {
        OnSettingClick = Override;
        m_setting.sprite = UIManager.Instance.m_CommonSprites[Override == null ? "icon_setting" : "icon_close"];
        m_setting.SetNativeSize();
    }
    #endregion

    void OnWeaponStatus(EntityCharacterPlayer _player)
    {
        m_weapon1Data.UpdateInfo(_player.m_Weapon1, _player.m_weaponEquipingFirst);
        m_weapon2Data.UpdateInfo(_player.m_Weapon2, !_player.m_weaponEquipingFirst);
    }

    class ControlWeaponData
    {
        WeaponBase m_weapon;
        Transform transform;
        Transform tf_Equiping, tf_Unequiping;
        UIC_WeaponData m_weaponData;
        TSpecialClasses.ValueChecker<int, int> m_AmmoStatusChecker;
        Action OnWeaponClick;
        public ControlWeaponData(Transform _transform,UnityAction OnActionClick,Action _OnWeaponClick)
        {
            transform = _transform;
            m_weaponData = new UIC_WeaponData(transform.Find("WeaponData"));
            tf_Equiping = m_weaponData.transform.Find("Equiping");
            tf_Unequiping = m_weaponData.transform.Find("Unequiping");
            m_AmmoStatusChecker = new TSpecialClasses.ValueChecker<int, int>(-1, -1);
            OnWeaponClick = _OnWeaponClick;
            m_weaponData.transform.GetComponent<UIT_EventTriggerListener>().SetOnPressDuration(.25f, OnWeaponDetailPressed);
        }
        public void UpdateInfo(WeaponBase weapon, bool equiping)
        {
            m_weapon = weapon;
            m_AmmoStatusChecker.Check(-1, -1);
            bool weaponInvalid = m_weapon == null;
            tf_Unequiping.SetActivate(!equiping);
            tf_Equiping.SetActivate(equiping);
            m_weaponData.transform.SetActivate(!weaponInvalid);
            if (weaponInvalid)
                return;
            m_weaponData.UpdateInfo(weapon);
            UpdateAmmoStatus();
        }
        
        public void Tick(float deltaTime)
        {
            if (m_weapon == null)
                return;
            UpdateAmmoStatus();
        }

        void UpdateAmmoStatus()
        {
            if (m_AmmoStatusChecker.Check(m_weapon.I_AmmoLeft, m_weapon.I_ClipAmount))
                m_weaponData.UpdateAmmoInfo(m_weapon.I_AmmoLeft, m_weapon.I_ClipAmount);
        }
        
        void OnWeaponDetailPressed(bool pressed)
        {
            if (pressed)
                UIManager.Instance.ShowPage<UI_WeaponStatus>(true, 0f).Play(m_weapon.m_WeaponInfo,null);
            else
                OnWeaponClick();
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnMainButtonDown(true, Vector2.zero);
        else if (Input.GetMouseButtonUp(0))
            OnMainButtonDown(false, Vector2.zero);

        if (Input.GetMouseButtonDown(1))
            OnSubButtonDown(true, Vector2.zero);
        else if (Input.GetMouseButtonUp(1))
            OnSubButtonDown(false, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.R))
            OnReloadButtonDown();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            OnAbilityButtonDown();
        if (Input.GetKeyDown(KeyCode.Tab))
            OnWeaponSwap();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnWeaponFirstActionClick();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnWeaponSecondActionClick();

    }
#endif
}
