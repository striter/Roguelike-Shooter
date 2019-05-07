using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSetting;
using System;

public class UIManager : SingletonMono<UIManager> {

    Text txt_AmmoLeft;
    RectTransform rtf_Pitch;
    Text txt_Pitch;
    public static Action OnSwitch, OnReload;
    public static Action<bool> OnFire;
    protected override void Awake()
    {
        instance = this;
        txt_AmmoLeft = transform.Find("AmmoLeft").GetComponent<Text>();
        rtf_Pitch = transform.Find("Pitch/Pitch").GetComponent<RectTransform>();
        txt_Pitch = rtf_Pitch.Find("Pitch").GetComponent<Text>();
        transform.Find("Switch").GetComponent<Button>().onClick.AddListener(()=> { OnSwitch?.Invoke(); });
        transform.Find("Reload").GetComponent<Button>().onClick.AddListener(() => { OnReload?.Invoke(); });
        transform.Find("Fire").GetComponent<UIT_EventTriggerListener>().D_OnPress+=(bool down,Vector2 pos) => { OnFire?.Invoke(down); };
    }
    private void Start()
    {
        TBroadCaster<enum_BC_UIStatusChanged>.Add<float>(enum_BC_UIStatusChanged.PitchChanged, OnPitchChanged);
        TBroadCaster<enum_BC_UIStatusChanged>.Add<int>(enum_BC_UIStatusChanged.AmmoLeftChanged, OnAmmoChanged);
    }
    private void OnDisable()
    {
        TBroadCaster<enum_BC_UIStatusChanged>.Remove<float>(enum_BC_UIStatusChanged.PitchChanged, OnPitchChanged);
        TBroadCaster<enum_BC_UIStatusChanged>.Remove<int>(enum_BC_UIStatusChanged.AmmoLeftChanged, OnAmmoChanged);
    }
    void OnAmmoChanged(int ammo)
    {
        txt_AmmoLeft.text = ammo.ToString();
    }
    void OnPitchChanged(float pitch)
    {
        txt_Pitch.text = ((int)pitch).ToString();
        rtf_Pitch.anchoredPosition =new Vector2( 0, (pitch / 45f) * 900);
    }

}
