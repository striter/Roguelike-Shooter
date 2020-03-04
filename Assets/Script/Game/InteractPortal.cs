using GameSetting;
using UnityEngine;
using TTiles;
using System;

public class InteractPortal : InteractGameBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.Portal;
    string m_localizeKey = "";
    public override string m_ExternalLocalizeKeyJoint => "_" + m_localizeKey;
    Action OnPortalInteract;
    public override bool B_InteractOnTrigger => true;
    public void Play(Action _OnPortalInteract, string _localizeKey)
    {
        base.Play();
        OnPortalInteract = _OnPortalInteract;
        m_localizeKey= _localizeKey;
    }
    
    protected override bool OnInteractOnceCanKeepInteract(EntityCharacterPlayer _interactor)
    {
        base.OnInteractOnceCanKeepInteract(_interactor);
        OnPortalInteract();
        return false;
    }
}
