using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameControl : UIControlBase
{
    TSpecialClasses.AnimationControlBase m_Animation;
    EntityCharacterPlayer m_Player;
    Transform tf_WeaponData;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponBackground;
    Image m_WeaponImage;
    UI_WeaponActionHUD m_WeaponActionHUD;
    Button btn_ActionStorage;

    protected override void Init()
    {
        base.Init();
        m_Animation = new TSpecialClasses.AnimationControlBase(transform.GetComponent<Animation>());
        btn_ActionStorage = transform.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);

        tf_WeaponData = transform.Find("WeaponData");
        m_WeaponBackground = tf_WeaponData.GetComponent<Image>();
        m_WeaponName = tf_WeaponData.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponData.Find("WeaponImage").GetComponent<Image>();
        m_WeaponActionHUD = new UI_WeaponActionHUD(tf_WeaponData);
        tf_WeaponData.Find("WeaponDetailBtn").GetComponent<Button>().onClick.AddListener(OnWeaponDetailClick);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        SetInBattle(false,false);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    void OnActionStorageClick()=> UIManager.Instance.ShowPage<UI_ActionPack>(true).Show(m_Player.m_PlayerInfo);
    void OnWeaponDetailClick()=>  UIManager.Instance.ShowPage<UI_WeaponStatus>(true, 0f).SetInfo(m_Player.m_WeaponCurrent.m_WeaponInfo,m_Player.m_WeaponCurrent.m_WeaponAction);
    void OnCommonStatus(EntityCharacterPlayer _player) => m_Player = _player;
    void OnWeaponStatus(WeaponBase weapon)
    {
        m_WeaponBackground.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_UIRarity.GetUIGameControlBackground()];
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
        m_WeaponName.autoLocalizeText = weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
        m_WeaponActionHUD.SetInfo(weapon.m_WeaponAction);
    }

    public UIC_GameControl SetInGame(bool inGame)
    {
        btn_ActionStorage.SetActivate(inGame);
        return this;
    }

    void SetInBattle(bool inBattle,bool anim)
    {
        if (anim) m_Animation.Play(inBattle);
        else m_Animation.SetPlayPosition(inBattle);

        btn_ActionStorage.interactable = !inBattle;
    }
    void OnBattleStart() => SetInBattle(true, true);
    void OnBattleFinish() => SetInBattle(false, true);
}
