using System;
using UnityEngine;

public class TAnimatorEvent : MonoBehaviour
{
    public enum enum_AnimEvent
    {
        Invalid = -1,
        Fire = 1,
        FootL,
        FootR,
        Reload1,
        Reload2,
        Reload3,
        Death,
    }
    Action<enum_AnimEvent> OnEventTrigger;
    public void Attach(Action<enum_AnimEvent> _OnEventTrigger)
    {
        OnEventTrigger = _OnEventTrigger;
    }
    protected void OnEvent(string eventName)
    {
        OnEventTrigger?.Invoke((enum_AnimEvent)Enum.Parse(typeof(enum_AnimEvent),eventName));
    }
}
