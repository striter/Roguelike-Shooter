using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TAnimatorEvent : MonoBehaviour {
    Type enumType;
    Action<string> OnEventTrigger;
    public void Attach(Action<string> _OnEventTrigger)
    {
        OnEventTrigger = _OnEventTrigger;
    }
    void OnEvent(string _event)
    {
        OnEventTrigger(_event);
    }
}
