using GameSetting;
using TSpecialClasses;
using UnityEngine;
using UnityEngine.UI;

public class UIC_CharacterEnergy : UIControlBase {

    ValueLerpSeconds m_EnergyLerp;
    Image img_Full, img_Fill;
    Text txt_amount;
    float m_value;
    TSpecialClasses.AnimationControlBase m_Animation;
    protected override void Init()
    {
        base.Init();
        Transform tf_container = transform.Find("Container");
        txt_amount = tf_container.Find("Amount").GetComponent<Text>();
        img_Full = tf_container.Find("Full").GetComponent<Image>();
        img_Fill = tf_container.Find("Fill").GetComponent<Image>();
        m_EnergyLerp = new ValueLerpSeconds(0f, 4f, 2f,  SetValue);
        m_Animation = new AnimationControlBase(GetComponent<Animation>(), false);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterPlayer>(enum_BC_UIStatus.UI_PlayerCommonStatus, OnCommonStatus);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
    }
    void OnCommonStatus(EntityCharacterPlayer _player)
    {
        m_EnergyLerp.ChangeValue(_player.m_PlayerInfo.m_ActionEnergy);
    }

    void OnBattleStart() => m_Animation.Play(true);
    void OnBattleFinish() => m_Animation.Play(false);

    public void SetValue(float value)
    {
        if (m_value == value)
            return;

        m_value = value;
        float detail = m_value % 1f;
        bool full = m_value == GameConst.F_MaxActionEnergy;
        img_Full.SetActivate(full);
        img_Fill.SetActivate(!full);
        txt_amount.text = ((int)m_value).ToString();
        if (!full) img_Fill.fillAmount = detail;
    }
    private void Update()
    {
        m_EnergyLerp.TickDelta(Time.unscaledDeltaTime);
    }

}
