using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractAction : InteractGameBase {
    public override enum_Interaction m_InteractType => enum_Interaction.Action;
    public ActionBase m_Action { get; private set; }
    protected override bool B_RecycleOnInteract => true;
    Transform tf_PlayerEquipment, tf_WeaponAbility;
    public override void OnPoolItemInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolItemInit(identity, OnRecycle);
        tf_PlayerEquipment = transform.Find("Container/Model/PlayerEquipment");
        tf_WeaponAbility = transform.Find("Container/Model/WeaponAbility");
    }
    public InteractAction Play(ActionBase _action)
    {
        base.Play();
        m_Action = _action;
        tf_WeaponAbility.SetActivate(_action.m_ActionType == enum_ActionType.WeaponAbility);
        tf_PlayerEquipment.SetActivate(_action.m_ActionType == enum_ActionType.PlayerEquipment);
        return this;
    }
    protected override void OnInteractSuccessful(EntityCharacterPlayer _interactTarget)
    {
        base.OnInteractSuccessful(_interactTarget);
        _interactTarget.OnPickupAction(m_Action);
    }
}
