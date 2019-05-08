using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class WeaponBase : MonoBehaviour,ISingleCoroutine {
    protected SWeapon m_WeaponInfo;
    public bool B_TriggerDown { get; private set; }
    public bool B_Reloading { get; private set; }
    public int I_AmmoLeft { get; private set; }
    float f_actionCheck=0;
    protected Transform tf_Muzzle;
    Action OnAmmoInfoChanged;
    Action<Vector2> OnRecoil;
    public void Init(SWeapon weaponInfo)
    {
        tf_Muzzle = transform.Find("Muzzle");
        m_WeaponInfo = weaponInfo;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
    }
    protected virtual void Start()
    {
        if (m_WeaponInfo.m_Type == 0)
            Debug.LogError("Please Init Entity Info!" + gameObject.name.ToString());
    }
    protected virtual void OnDisable()
    {
        this.StopAllSingleCoroutines();
        B_Reloading = false;
        B_TriggerDown = false;
    }
    bool b_actionAble => Time.time > f_actionCheck;
    void SetActionPause(float pauseDuration)
    {
        f_actionCheck = Time.time + pauseDuration;
    }
    public void Attach(Transform attachTarget,Action _OnAmmoInfoChanged,Action<Vector2> _OnRecoil)
    {
        transform.SetParent(attachTarget);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        OnAmmoInfoChanged = _OnAmmoInfoChanged;
        OnRecoil = _OnRecoil;
    }
    public bool Trigger(bool down)
    {
        if (B_Reloading)
            return false;
        B_TriggerDown = down;
        if (down)
            this.StartSingleCoroutine(0, TriggerOn());
        else
            this.StopSingleCoroutine(0);
        return true;
    }

    IEnumerator TriggerOn()
    {
        if (I_AmmoLeft <= 0)
            yield break;

        for (; ; )
        {
            if (b_actionAble)
                FireOnce();
            if (I_AmmoLeft <= 0)
                yield break;
            yield return null;
        }
    }
    void FireOnce()
    {
        I_AmmoLeft--;
        (ObjectManager.SpawnSFX(enum_SFX.Bullet, tf_Muzzle) as SFXBullet).Play(m_WeaponInfo.m_Damage,tf_Muzzle.forward);
        OnRecoil(new Vector2(m_WeaponInfo.m_RecoilHorizontal,m_WeaponInfo.m_RecoilVertical));
        SetActionPause(m_WeaponInfo.m_FireRate);
        OnAmmoInfoChanged();
    }


    public bool TryReload()
    {
        if (!b_actionAble)
            return false;

        StartReload();

        return true;
    }
    void StartReload()
    {
        B_Reloading = true;
        SetActionPause(m_WeaponInfo.m_ReloadTime);
        this.StartSingleCoroutine(1,TIEnumerators.PauseDel(m_WeaponInfo.m_ReloadTime,OnReloadFinished));
    }
    void OnReloadFinished()
    {
        B_Reloading = false;
        I_AmmoLeft = m_WeaponInfo.m_ClipAmount;
        OnAmmoInfoChanged();
    }
}
