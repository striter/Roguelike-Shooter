using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractBattleBase : InteractBase,IObjectPoolStaticBase<enum_Interaction> {

    protected virtual bool B_SelfRecycleOnInteract => false;
    public AudioClip AC_OnPlay, AC_OnInteract;
    public int I_MuzzleOnInteract;
    public UIGI_VisualizeItem m_visualizeItem;


    public int m_TradePrice { get; private set; } = -1;
    public int m_KeyRequire { get; private set; } = -1;
    protected void SetTradePrice(float tradePrice) => m_TradePrice = Mathf.RoundToInt(tradePrice);
    protected void SetKeyAcquire(int keyAcquire) => m_KeyRequire = keyAcquire;

    public virtual void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle) { }
    public void OnPoolSpawn() { }
    public void OnPoolRecycle() { }

    protected override void Play()
    {
        base.Play();
        m_TradePrice = -1;
        m_KeyRequire = -1;
        if (AC_OnPlay)
            AudioManager.Instance.Play3DClip(-1, AC_OnPlay, false, transform.position);

        if (m_InteractType != enum_Interaction.PickupWeapon
            && m_InteractType != enum_Interaction.PickupCoin
            && m_InteractType != enum_Interaction.TradeContainer
            && m_InteractType != enum_Interaction.PerkPickup)
        {
            if (m_visualizeItem != null)
            {
                m_visualizeItem.Play(TLocalization.GetKeyLocalized(GetUITitleKey()),
                this);
            }
            else
            {
                Debug.Log("创建" + TLocalization.GetKeyLocalized(GetUITitleKey()) + "*****" + m_InteractType);
                BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().CreateItemInformation(TLocalization.GetKeyLocalized(GetUITitleKey()),
                    this);
            }
            
        }
    }


    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor) =>
        (m_TradePrice <= 0 || _interactor.m_CharacterInfo.CanCostCoins(m_TradePrice)) &&
        (m_KeyRequire <= 0 || _interactor.m_CharacterInfo.CanCostKeys(m_KeyRequire));

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);

        if (_interactor != null)
        {
            if (m_TradePrice > 0)
                _interactor.m_CharacterInfo.OnCoinsCost(m_TradePrice);
            if (m_KeyRequire > 0)
                _interactor.m_CharacterInfo.OnKeyCost(m_KeyRequire);

            GameObjectManager.PlayMuzzle(_interactor.m_EntityID, transform.position, transform.up, I_MuzzleOnInteract, AC_OnInteract);
        }
        if (B_SelfRecycleOnInteract)
        {
            SetInteractable(false);
            GameObjectManager.RecycleInteract(this);
        }
        return false;
    }

}
