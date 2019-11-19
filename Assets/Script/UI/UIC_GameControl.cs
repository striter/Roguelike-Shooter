using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIC_GameControl : UIControlBase
{
    EntityCharacterPlayer m_Player;
    Transform tf_WeaponData;
    UIT_TextExtend m_WeaponName;
    Image m_WeaponImage;
    UI_WeaponActionHUD m_WeaponActionHUD;

    Button btn_Bigmap;
    Button btn_ActionStorage;

    protected override void Init()
    {
        base.Init();
        btn_ActionStorage = transform.Find("ActionStorage").GetComponent<Button>();
        btn_ActionStorage.onClick.AddListener(OnActionStorageClick);
        btn_Bigmap = transform.Find("Bigmap").GetComponent<Button>();
        btn_Bigmap.onClick.AddListener(OnMapControlClick);

        tf_WeaponData = transform.Find("WeaponData");
        m_WeaponName = tf_WeaponData.Find("WeaponName").GetComponent<UIT_TextExtend>();
        m_WeaponImage = tf_WeaponData.Find("WeaponImage").GetComponent<Image>();
        m_WeaponActionHUD = new UI_WeaponActionHUD(tf_WeaponData);
        tf_WeaponData.Find("WeaponDetailBtn").GetComponent<Button>().onClick.AddListener(OnWeaponDetailClick);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Add<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_UIStatus>.Remove<WeaponBase>(enum_BC_UIStatus.UI_PlayerWeaponStatus, OnWeaponStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    void OnMapControlClick() => UIManager.Instance.ShowPage<UI_MapControl>(true);
    void OnActionStorageClick()
    {
        if (UIPageBase.Opening<UI_ActionPack>() || UIPageBase.Opening<UI_WeaponStatus>())
            return;
        UIManager.Instance.ShowPage<UI_ActionPack>(true).Show(m_Player.m_PlayerInfo);
    }
    void OnWeaponDetailClick()
    {
        if (UIPageBase.Opening<UI_ActionPack>() || UIPageBase.Opening<UI_WeaponStatus>())
            return;
        UIManager.Instance.ShowPage<UI_WeaponStatus>(true, 0f).SetInfo(m_Player.m_WeaponCurrent);
    }

    void OnCommonStatus(EntityCharacterPlayer _player) => m_Player = _player;
    public UIC_GameControl SetInGame(bool inGame)
    {
        btn_Bigmap.SetActivate(inGame);
        btn_ActionStorage.SetActivate(inGame);
        return this;
    }

    void OnBattleStart()
    {
        btn_Bigmap.SetActivate(false);
        btn_ActionStorage.SetActivate(false);
    }
    void OnBattleFinish()
    {
        btn_ActionStorage.SetActivate(true);
    }

    public void ShowBigmapBtn(bool active)
    {
        btn_Bigmap.SetActivate(active);
    }

    void OnWeaponStatus(WeaponBase weapon)
    {
        m_WeaponImage.sprite = UIManager.Instance.m_WeaponSprites[weapon.m_WeaponInfo.m_Weapon.GetSpriteName()];
        m_WeaponName.autoLocalizeText = weapon.m_WeaponInfo.m_Weapon.GetLocalizeNameKey();
        m_WeaponActionHUD.SetInfo(weapon.m_WeaponAction);
    }
}
