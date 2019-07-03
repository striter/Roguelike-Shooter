using System;
using UnityEngine;

public class TAnimatorEvent : MonoBehaviour {
    Action<string> OnEventTrigger;
    public void Attach(Action<string> _OnEventTrigger)
    {
        OnEventTrigger = _OnEventTrigger;
    }
    protected void OnEvent(string eventName)
    {
        OnEventTrigger(eventName);
    }
}
