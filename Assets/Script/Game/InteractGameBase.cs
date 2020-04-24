﻿using GameSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractGameBase : InteractBase,IObjectpool<enum_Interaction> {

    protected virtual bool B_SelfRecycleOnInteract => false;
    public AudioClip AC_OnPlay, AC_OnInteract;
    public int m_TradePrice { get; protected set; } = -1;
    public int m_KeyRequire { get; protected set; } = -1;
    public int I_MuzzleOnInteract;
    public virtual void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
    }

    protected override void Play()
    {
        base.Play();
        m_TradePrice = -1;
        m_KeyRequire = -1;
        if (AC_OnPlay)
            AudioManager.Instance.Play3DClip(-1, AC_OnPlay, false, transform.position);
    }

    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor) =>
        (m_TradePrice <= 0 || _interactor.m_CharacterInfo.CanCostCoins(m_TradePrice)) &&
        (m_KeyRequire <= 0 || _interactor.m_CharacterInfo.CanCostKeys(m_KeyRequire));

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);

        if (m_TradePrice > 0)
            _interactor.m_CharacterInfo.OnCoinsCost(m_TradePrice);
        if (m_KeyRequire > 0)
            _interactor.m_CharacterInfo.OnKeyCost(m_KeyRequire);

        GameObjectManager.PlayMuzzle(_interactor.m_EntityID, transform.position, transform.up, I_MuzzleOnInteract, AC_OnInteract);
        if (B_SelfRecycleOnInteract)
        {
            SetInteractable(false);
            GameObjectManager.RecycleInteract(this);
        }
        return false;
    }


}
