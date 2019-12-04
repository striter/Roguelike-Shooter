using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_Control : UIControlBase {
    protected TouchDeltaManager m_TouchDelta { get; private set; }
    Transform tf_InGame;
    Image m_MainImg;
    Image m_AbilityBG,m_AbilityImg,m_AbilityCooldown;
    EntityCharacterPlayer m_Player;
    Button btn_ActionStorage;
    Image m_setting;
    WeaponData m_weapon1Data, m_weapon2Data;
    Action OnReload,OnSwap, OnCharacterAbility;
    Action<bool> OnWeaponAction;
    Action<bool> OnMainDown;
    protected override void Init()
    {
        base.Init();
        tf_InGame = transform.Find("InGame");
        m_MainImg = transform.Find("Main/Image").GetComponent<Image>();
        m_AbilityBG = transform.Find("Ability").GetComponent<Image>();
        m_AbilityImg = transform.Find("Ability/Image").GetComponent<Image>();
        m_AbilityCooldown = transform.Find("Ability/Cooldown").GetComponent<Image>();
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(OnReloadButtonDown);
        transform.Find("Swap").GetComponent<Button>().onClick.AddListener(OnSwapButtonDown);
        transform.Find("Ability").GetComponent<Button>().onClick.AddListener(OnAbilityButtonDown);
        transform.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress += OnMainButtonDown;

        transform.Find("Settings").GetComponent<Button>().onClick.AddListener(OnSettingBtnClick);
        m_setting = transform.Find("Settings/Image").GetComponent<Image>();

        btn_ActionStorage = tf_InGame.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);
        m_weapon1Data = new WeaponData(tf_InGame.Find("Weapon1Data"));
        m_weapon2Data = new WeaponData(tf_InGame.Find("Weapon2Data"));

        m_TouchDelta = transform.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;
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
        btn_ActionStorage.SetActivate(inGame);
        return this;
    }

    void OnOptionsChanged() => UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    bool CheckControlable() => !UIPageBase.m_PageOpening;

    InteractBase m_Interact;
    bool m_cooldowning=true;
    void OncommonStatus(EntityCharacterPlayer player)
    {
        m_Player = player;
        if(player.m_Ability.m_Cooldowning)
            m_AbilityCooldown.fillAmount = player.m_Ability.m_CooldownScale;

        if(player.m_Ability.m_Cooldowning!=m_cooldowning)
        {
            m_cooldowning = player.m_Ability.m_Cooldowning;
            m_AbilityBG.sprite = UIManager.Instance.m_CommonSprites[UIConvertions.GetAbilityBackground(m_cooldowning)];
            m_AbilityCooldown.SetActivate(m_cooldowning);
        }

        if (player.m_Interact != m_Interact)
        {
            m_Interact = player.m_Interact;
            m_MainImg.sprite = UIManager.Instance.m_CommonSprites[UIConvertions.GetMainSprite(m_Interact)];
        }

        m_weapon1Data.UpdateAmmoStatus();
        m_weapon2Data.UpdateAmmoStatus();
    }
    #region Controls
    public void DoBinding(EntityCharacterPlayer player,Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta, Action<bool> _OnMainDown,Action _OnSwap, Action _OnReload, Action<bool> _OnWeaponAction, Action _OnCharacterAbility)
    {
        m_TouchDelta.AddLRBinding(_OnLeftDelta, _OnRightDelta, CheckControlable);
        OnMainDown = _OnMainDown;
        OnReload = _OnReload;
        OnSwap = _OnSwap;
        OnWeaponAction = _OnWeaponAction;
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
    protected void OnSwapButtonDown() => OnSwap?.Invoke();
    protected void OnWeaponFirstActionClick() => OnWeaponAction?.Invoke(true);
    protected void OnWeaponSecondActionClick() => OnWeaponAction?.Invoke(false);
    protected void OnAbilityButtonDown() => OnCharacterAbility?.Invoke();
    protected void OnActionStorageClick() => UIManager.Instance.ShowPage<UI_ActionPack>(true).Show(m_Player.m_PlayerInfo);
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
        m_weapon1Data.UpdateInfo(_player.m_Weapon1, _player.m_weaponEquipingFirst,OnWeaponFirstActionClick);
        m_weapon2Data.UpdateInfo(_player.m_Weapon2, !_player.m_weaponEquipingFirst,OnWeaponSecondActionClick);
    }

    class WeaponData
    {
        WeaponBase m_weapon;
        Transform transform;
        UIT_TextExtend m_Name;
        Image m_Background;
        Image m_Image;
        Transform m_Equiping, m_unEquiping;
        Text m_Clip, m_Total;
        UIGI_ActionItemWeapon m_Action;
        public WeaponData(Transform _transform)
        {
            transform = _transform;
            m_Background = _transform.GetComponent<Image>();
            m_Name = _transform.Find("Name").GetComponent<UIT_TextExtend>();
            m_Image = _transform.Find("Image").GetComponent<Image>();
            m_Equiping = transform.Find("Equiping");
            m_unEquiping = transform.Find("UnEquiping");
            _transform.Find("DetailBtn").GetComponent<Button>().onClick.AddListener(OnWeaponDetailClick);
            m_Clip = transform.Find("AmmoStatus/Clip").GetComponent<Text>();
            m_Total = transform.Find("AmmoStatus/Total").GetComponent<Text>();
            m_Action = transform.Find("ActionStatus").GetComponent<UIGI_ActionItemWeapon>();
            m_Action.Init();
        }
        public void UpdateInfo(WeaponBase weapon, bool equiping, Action OnWeaponActionClick)
        {
            m_weapon = weapon;
            bool invalid = m_weapon == null;
            m_Image.SetActivate(!invalid);
            m_Name.SetActivate(!invalid);
            m_Action.Play(invalid ? null : m_weapon.m_WeaponAction, OnWeaponActionClick);
            if (invalid)
            {
                m_Background.sprite = UIManager.Instance.m_WeaponSprites[enum_WeaponRarity.Invalid.GetUIGameControlBackground()];
                return;
            }

            m_Equiping.SetActivate(equiping);
            m_unEquiping.SetActivate(!equiping);

            m_Background.sprite = UIManager.Instance.m_WeaponSprites[m_weapon.m_WeaponInfo.m_Rarity.GetUIGameControlBackground()];
            m_Image.sprite = UIManager.Instance.m_WeaponSprites[m_weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
            m_Name.autoLocalizeText = m_weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
            UpdateAmmoStatus();
            LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
        }

        public void UpdateAmmoStatus()
        {
            if (m_weapon == null)
                return;
            m_Clip.text = m_weapon.I_AmmoLeft.ToString();
            m_Total.text = m_weapon.I_ClipAmount.ToString();
            m_Action.Tick(m_weapon);
        }

        void OnWeaponDetailClick()
        {
            UIManager.Instance.ShowPage<UI_WeaponStatus>(true, 0f).Play(m_weapon.m_WeaponInfo,m_weapon.m_WeaponAction);
        }
        
    }
#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            OnMainButtonDown(true, Vector2.zero);
        else if (Input.GetMouseButtonUp(0))
            OnMainButtonDown(false, Vector2.zero);

        if (Input.GetKeyDown(KeyCode.R))
            OnReloadButtonDown();
        if (Input.GetKeyDown(KeyCode.LeftShift))
            OnAbilityButtonDown();
        if (Input.GetKeyDown(KeyCode.Tab))
            OnSwapButtonDown();
        if (Input.GetKeyDown(KeyCode.Alpha1))
            OnWeaponFirstActionClick();
        if (Input.GetKeyDown(KeyCode.Alpha2))
            OnWeaponSecondActionClick();

    }
#endif
}
