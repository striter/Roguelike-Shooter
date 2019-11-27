using GameSetting;
using UnityEngine;
using UnityEngine.UI;
using TSpecialClasses;
public class UIC_PlayerStatus : UIControlBase
{
    public AnimationCurve m_DyingCurve;

    Transform tf_Container;
    EntityCharacterPlayer m_Player;

    bool m_dying;
    RawImage img_Dying;

    Transform tf_ExtraControl;
    Transform tf_ActionControl, tf_MapControl;
    TSpecialClasses.AnimationControlBase m_ActionControlAnim,m_MapControlAnim;
    UIC_ActionEnergy m_ActionEnergy;
    Button  btn_ActionShuffle;
    Image img_ShuffleFill;
    UIT_GridControllerGridItem<UIGI_ActionItemHold> m_ActionGrid;
    Button btn_map;
    
    Transform tf_ExpireData;
    UIT_GridControllerGridItem<UIGI_ExpireInfoItem> m_ExpireGrid;

    RectTransform rtf_StatusData;
    GridLayoutGroup m_AmmoLayout;
    Transform tf_AmmoData;
    Image img_ReloadFill;
    float m_AmmoGridWidth;
    UIT_GridControllerGridItem<UIGI_AmmoItem> m_AmmoGrid;
    UIC_Numeric m_AmmoAmount, m_AmmoClipAmount;

    Transform tf_ArmorData;
    Slider m_ArmorFill;
    UIC_Numeric m_ArmorAmount;

    Transform tf_HealthData;
    Slider m_HealthFill;
    UIC_Numeric m_HealthAmount, m_MaxHealth;

    ValueLerpSeconds m_HealthLerp, m_ArmorLerp, m_EnergyLerp;
    protected override void Init()
    {
        base.Init();
        tf_Container = transform.Find("Container");

        img_Dying = tf_Container.Find("Dying").GetComponent<RawImage>();

        tf_ExtraControl = tf_Container.Find("ExtraControl");
        tf_ActionControl = tf_ExtraControl.Find("Action");
        m_ActionControlAnim = new AnimationControlBase(tf_ActionControl.GetComponent<Animation>());
        m_ActionEnergy = new UIC_ActionEnergy(tf_ActionControl.Find("ActionEnergy"));
        m_ActionGrid = new UIT_GridControllerGridItem<UIGI_ActionItemHold>(tf_ActionControl.Find("ActionGrid"));
        btn_ActionShuffle = tf_ActionControl.Find("ActionShuffle").GetComponent<Button>();
        btn_ActionShuffle.onClick.AddListener(OnActionShuffleClick);
        img_ShuffleFill = btn_ActionShuffle.transform.Find("ShuffleFill").GetComponent<Image>();
        tf_MapControl = tf_ExtraControl.Find("Map");
        m_MapControlAnim = new AnimationControlBase(tf_MapControl.GetComponent<Animation>());
        btn_map = tf_MapControl.Find("MapBtn").GetComponent<Button>();
        btn_map.onClick.AddListener(OnMapControlClick);

        rtf_StatusData = tf_Container.Find("StatusData").GetComponent<RectTransform>();
        tf_AmmoData = rtf_StatusData.Find("Container/AmmoData");
        m_AmmoGridWidth = tf_AmmoData.GetComponent<RectTransform>().sizeDelta.x;
        m_AmmoAmount = new UIC_Numeric(tf_AmmoData.Find("AmmoAmount"));
        m_AmmoClipAmount = new UIC_Numeric(m_AmmoAmount.transform.Find("ClipAmount"));
        m_AmmoGrid = new UIT_GridControllerGridItem<UIGI_AmmoItem>(tf_AmmoData.Find("AmmoGrid"));
        m_AmmoLayout = m_AmmoGrid.transform.GetComponent<GridLayoutGroup>();
        img_ReloadFill = m_AmmoGrid.transform.Find("Reload").GetComponent<Image>();

        tf_ArmorData = rtf_StatusData.Find("Container/ArmorData");
        m_ArmorFill = tf_ArmorData.Find("Slider").GetComponent<Slider>();
        m_ArmorAmount = new UIC_Numeric(tf_ArmorData.Find("ArmorAmount"));
        tf_HealthData = rtf_StatusData.Find("Container/HealthData");
        m_HealthFill = tf_HealthData.Find("Slider").GetComponent<Slider>();
        m_HealthAmount = new UIC_Numeric(tf_HealthData.Find("HealthAmount"));
        m_MaxHealth = new UIC_Numeric(m_HealthAmount.transform.Find("MaxHealth"));
        
        tf_ExpireData = tf_Container.Find("ExpireData");
        m_ExpireGrid = new UIT_GridControllerGridItem<UIGI_ExpireInfoItem>(tf_ExpireData.Find("ExpireGrid"));

        m_HealthLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => { m_HealthFill.value = value; });
        m_ArmorLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => { m_ArmorFill.value = value; });
        m_EnergyLerp = new ValueLerpSeconds(0f, 4f, 2f, (float value) => { m_ActionEnergy.SetValue(value); });

        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireListStatus, OnExpireListStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerBattleActionStatus, OnBattleActionStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);

        SetActionShow(false,false);
        SetMapInBattle(false, false);
        img_Dying.SetActivate(false);
        m_dying = false;
    }
    
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityHealth>(enum_BC_UIStatus.UI_PlayerHealthStatus, OnHealthStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerAmmoStatus, OnAmmoStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerExpireListStatus, OnExpireListStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<PlayerInfoManager>(enum_BC_UIStatus.UI_PlayerBattleActionStatus, OnBattleActionStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    public UIC_PlayerStatus SetInGame(bool inGame)
    {
        tf_ExtraControl.SetActivate(inGame);
        return this;
    }


    void OnBattleStart()
    {
        SetActionShow(true, true);
        SetMapInBattle(true, true);
    }
    void OnBattleFinish()
    {
        SetActionShow(false, true);
        if (GameManager.Instance.m_GameLevel.B_HaveNextStage)
            SetMapInBattle(false, true);
    }

    void SetActionShow(bool show,bool animate)
    {
        m_ActionGrid.ClearGrid();
        if (animate) m_ActionControlAnim.Play(show);
        else m_ActionControlAnim.SetPlayPosition(show);
        btn_ActionShuffle.interactable = show;
    }

    void SetMapInBattle(bool inbattle,bool animate)
    {
        if (animate) m_MapControlAnim.Play(inbattle);
        else m_MapControlAnim.SetPlayPosition(inbattle);
        btn_map.interactable = !inbattle;
    }
    
    private void Update()
    {
        if (!m_Player)
            return;

        rtf_StatusData.SetWorldViewPortAnchor(m_Player.tf_UIStatus.position, CameraController.Instance.m_Camera, Time.deltaTime * 10f);

        m_EnergyLerp.TickDelta(Time.unscaledDeltaTime);
        m_HealthLerp.TickDelta(Time.unscaledDeltaTime);
        m_ArmorLerp.TickDelta(Time.unscaledDeltaTime);

        if (m_dying)
        {
            float dyingValue = 1-Mathf.InverseLerp(UIConst.I_PlayerDyingMinValue, UIConst.I_PlayerDyingMaxValue, m_Player.m_Health.m_CurrentHealth) ;
            img_Dying.color = TCommon.ColorAlpha(img_Dying.color,dyingValue+m_DyingCurve.Evaluate(Time.time)*.3f);
        }
    }

    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        m_Player = _player;
        bool dying = !m_Player.m_Health.b_IsDead&& m_Player.m_Health.m_CurrentHealth < UIConst.I_PlayerDyingMaxValue;
        if(m_dying!= dying)
        {
            m_dying = dying;
            img_Dying.SetActivate(m_dying);
        }

        m_EnergyLerp.ChangeValue(m_Player.m_PlayerInfo.m_ActionEnergy);
        img_ShuffleFill.fillAmount = m_Player.m_PlayerInfo.f_shuffleScale;
    }

    void OnHealthStatus(EntityHealth _healthManager)
    {
        m_ArmorLerp.ChangeValue(_healthManager.m_CurrentArmor / UIConst.F_UIMaxArmor);
        m_HealthLerp.ChangeValue(_healthManager.F_HealthMaxScale);
        m_ArmorAmount.SetAmount((int)_healthManager.m_CurrentArmor);
        m_HealthAmount.SetAmount((int)_healthManager.m_CurrentHealth);
        m_MaxHealth.SetAmount((int)_healthManager.m_MaxHealth);
    }

    void OnAmmoStatus(WeaponBase weaponInfo)
    {
        tf_AmmoData.transform.SetActivate(weaponInfo != null);
        if (weaponInfo == null)
            return;

        m_AmmoAmount.SetAmount(weaponInfo.B_Reloading?0:weaponInfo.I_AmmoLeft);
        m_AmmoClipAmount.SetAmount(weaponInfo.I_ClipAmount);
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

    void OnMapControlClick() => UIManager.Instance.ShowPage<UI_MapControl>(true);

    void OnBattleActionStatus(PlayerInfoManager playerInfo)
    {
        m_ActionGrid.ClearGrid();
        for (int i = 0; i < playerInfo.m_BattleActionPicking.Count; i++)
            m_ActionGrid.AddItem(i).SetInfo(playerInfo,playerInfo.m_BattleActionPicking[i],OnActionClick, OnActionPressDuration);
    }

    void OnActionClick(int index)=> m_Player.m_PlayerInfo.TryUsePickingAction(index);
    void OnActionPressDuration()=> UIManager.Instance.ShowPage<UI_ActionBattle>(false,0f).Show(m_Player.m_PlayerInfo) ;
    void OnActionShuffleClick()=>m_Player.m_PlayerInfo.TryShuffle();

    void OnExpireListStatus(PlayerInfoManager expireInfo)
    {
        m_ExpireGrid.ClearGrid();
        for (int i = 0; i < expireInfo.m_Expires.Count; i++)
        {
            if (expireInfo.m_Expires[i].m_ExpireType != enum_ExpireType.Action)
                continue;
            ActionBase action = (expireInfo.m_Expires[i] as ActionBase);

            if (action .m_ActionType == enum_ActionType.WeaponPerk)
                    continue;

            m_ExpireGrid.AddItem(i).SetInfo(action);
        }
    }
}
