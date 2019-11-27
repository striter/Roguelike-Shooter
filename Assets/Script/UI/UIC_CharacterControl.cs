using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_CharacterControl : UIControlBase {
    protected TouchDeltaManager m_TouchDelta { get; private set; }
    Image m_MainImg;
    Image m_AbilityBG,m_AbilityImg,m_AbilityRunsOut,m_AbilityTimesDecorate,m_AbilityCooldown,m_AbilityInfinite;
    Text m_AbilityTimeCounts;
    Action OnReload, OnAbility;
    Action<bool> OnMainDown;
    protected override void Init()
    {
        base.Init();
        m_MainImg = transform.Find("Main/Image").GetComponent<Image>();
        m_AbilityBG = transform.Find("Ability").GetComponent<Image>();
        m_AbilityImg = transform.Find("Ability/Image").GetComponent<Image>();
        m_AbilityTimeCounts = transform.Find("Ability/TimesCount").GetComponent<Text>();
        m_AbilityCooldown = transform.Find("Ability/Cooldown").GetComponent<Image>();
        m_AbilityRunsOut = transform.Find("Ability/RunsOut").GetComponent<Image>();
        m_AbilityTimesDecorate = transform.Find("Ability/TimesImage").GetComponent<Image>();
        m_AbilityInfinite = transform.Find("Ability/Infinite").GetComponent<Image>();
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(OnReloadButtonDown);
        transform.Find("Ability").GetComponent<Button>().onClick.AddListener(OnAbilityClick);
        transform.Find("Main").GetComponent<UIT_EventTriggerListener>().D_OnPress += OnMainButtonDown;

        m_TouchDelta = transform.GetComponent<TouchDeltaManager>();
        OnOptionsChanged();
        OptionsManager.event_OptionChanged += OnOptionsChanged;
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        OptionsManager.event_OptionChanged -= OnOptionsChanged;
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnPlayerStatusChanged);
    }
    void OnOptionsChanged() => UIT_JoyStick.Instance.SetMode(OptionsManager.m_OptionsData.m_JoyStickMode);
    bool CheckControlable() => !UIPageBase.m_PageOpening;

    InteractBase m_Interact;
    bool m_cooldowning;
    int m_abilityTimes;
    void OnPlayerStatusChanged(EntityCharacterPlayer player)
    {
        if(player.m_Ability.m_AbilityCooldowning)
            m_AbilityCooldown.fillAmount = player.m_Ability.m_AbilityCoolDownScale;
        if(player.m_Ability.m_AbilityCooldowning!=m_cooldowning||player.m_Ability.m_AbilityTimes!=m_abilityTimes)
        {
            m_cooldowning = player.m_Ability.m_AbilityCooldowning;
            m_abilityTimes = player.m_Ability.m_AbilityTimes;
            bool runsOut = player.m_Ability.m_AbilityRunsOut;
            bool infinite = !player.m_Ability.m_AbilityRunsOutable;

            m_AbilityTimeCounts.SetActivate(!infinite);
            m_AbilityInfinite.SetActivate(infinite);
            if(!infinite) m_AbilityTimeCounts.text = m_abilityTimes.ToString();
            m_AbilityCooldown.SetActivate(!runsOut && m_cooldowning);
            m_AbilityImg.color = TCommon.ColorAlpha(m_AbilityImg.color, runsOut?.6f:1f);
            m_AbilityBG.sprite = UIManager.Instance.m_CommonSprites[UIEnumConvertions.GetAbilityBackground(runsOut ,m_cooldowning)];
            m_AbilityRunsOut.SetActivate(runsOut);
            m_AbilityTimesDecorate.SetActivate(!runsOut);
        }

        if (player.m_Interact != m_Interact)
        {
            m_Interact = player.m_Interact;
            m_MainImg.sprite = UIManager.Instance.m_CommonSprites[UIEnumConvertions.GetMainSprite(m_Interact)];
        }
    }

    public void DoBinding(EntityCharacterPlayer player,Action<Vector2> _OnLeftDelta, Action<Vector2> _OnRightDelta, Action _OnReload, Action<bool> _OnMainDown, Action _OnAbility)
    {
        m_TouchDelta.AddLRBinding(_OnLeftDelta, _OnRightDelta, CheckControlable);
        OnReload = _OnReload;
        OnAbility = _OnAbility;
        OnMainDown = _OnMainDown;
        m_AbilityImg.sprite = UIManager.Instance.m_CommonSprites[UIEnumConvertions.GetAbilitySprite(player.m_Character)];
    }
    public void RemoveBinding()
    {
        m_TouchDelta.RemoveAllBinding();
        OnReload = null;
        OnMainDown = null;
        OnAbility = null;
    }

    protected void OnReloadButtonDown() => OnReload?.Invoke();
    protected void OnMainButtonDown(bool down, Vector2 pos) => OnMainDown?.Invoke(down);
    protected void OnAbilityClick() => OnAbility?.Invoke();

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
            OnAbilityClick();
    }
#endif
}
