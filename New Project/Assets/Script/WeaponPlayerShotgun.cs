using System;
using System.Collections.Generic;
using UnityEngine;
using TSpecial;
public class WeaponPlayerShotgun : WeaponPlayer
{
    public override enum_AmmoType E_AmmoType => enum_AmmoType.Shotgun12GUAGE;
    public override enum_WeaponType E_WeaponType => enum_WeaponType.Shotgun;
    public float F_PumpTime = .5f;
    public int I_PelletsEachShot = 12;
    protected WeaponAnimatorSG wsg_current;

    public bool b_needPump { get; private set; } = false;
    public bool b_pumping { get; private set; } = false;
    protected override void Awake()
    {
        base.Awake();
        Transform tf_weapon = tf_Model.Find("spas13");
        tf_ShellThrow = tf_weapon.Find("ShellThrow");
        tf_Muzzle = tf_weapon.Find("Muzzle");

        wsg_current = new WeaponAnimatorSG(transform.Find("Model").GetComponent<Animator>(), new List<SAnimatorParam>() {
            new SAnimatorParam("aimIn",WeaponAnimator.hs_aim,F_AimCoolDown),
            new SAnimatorParam("fire",WeaponAnimator.hs_fire,F_FireRate),
            new SAnimatorParam("runStart",WeaponAnimator.hs_sprint,F_SprintCoolDown),
            new SAnimatorParam("pump3",WeaponAnimatorSG.hs_pump,F_PumpTime),
            new SAnimatorParam("reloadStart", WeaponAnimatorSG.hs_reloadStart, F_ReloadStartTime),
            new SAnimatorParam("reloadCycle", WeaponAnimatorSG.hs_reload, F_ReloadTime),
            new SAnimatorParam("reloadStop", WeaponAnimatorSG.hs_reloadEnd, F_ReloadEndTime),
        });
        wa_current = wsg_current;
    }
    public override Action Attach(PlayerBase attacher, PickupInfoWeapon info, Action _OnWeaponStatusChanged, Action<bool> _OnWeaponHitEnermy)
    {
        b_needPump = (info as PickupInfoWeaponShotgun).B_Pump;
        b_pumping = false;
        return base.Attach(attacher, info, _OnWeaponStatusChanged, _OnWeaponHitEnermy);
    }
    public override PickupInfoWeapon Detach()
    {
        base.Detach();
        return new PickupInfoWeaponShotgun(this);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        this.StopSingleCoroutine(101);
    }
    protected override void OnTriggerSet(bool down)
    {
        if (B_Actionable)
        {
            if (b_needPump)
            {
                if(!b_pumping)
                    Pump();
            }
            else
            {
                wa_current.Fire();
                SpawnMuzzle();
                I_AmmoLeft--;
                OnWeaponStatusChanged();
                SetActionPause(F_FireRate+F_PumpTime);
                for (int i = 0; i < I_PelletsEachShot; i++)
                {
                    FireOnePellet();
                }
                b_needPump = true;
                this.StartSingleCoroutine(101, TIEnumerators.PauseDel(F_FireRate, Pump));
            }
        }
    }
    void Pump()
    {
        b_pumping = true;
        wsg_current.Pump();
        SpawnShell();
        SetActionPause(F_PumpTime);
        this.StartSingleCoroutine(101, TIEnumerators.PauseDel(F_PumpTime, () => { b_needPump = false; b_pumping = false; }));
    }

    protected override void OnReloadFinished()
    {
        I_AmmoLeft++;
        m_attachPlayer.OnAmmoUsed(1);
        if (m_attachPlayer.I_AmmoLeft != 0 && I_AmmoLeft < I_MagCapacity)
        {
            OnReloading();
        }
        else
        {
            b_reloading = false;
            wsg_current.Reloading(b_reloading);
            SetActionPause(F_ReloadEndTime);
        }
        OnWeaponStatusChanged();
    }
    
}
public class WeaponAnimatorSG : WeaponAnimator
{
    public static readonly int hs_reloadStart = Animator.StringToHash("fm_reloadStart");
    public static readonly int hs_reloadEnd = Animator.StringToHash("fm_reloadEnd");
    public static readonly int hs_pump = Animator.StringToHash("fm_pump");
    readonly int t_pump = Animator.StringToHash("t_pump");
    readonly int f_pump = Animator.StringToHash("f_pump");
    public WeaponAnimatorSG(Animator _animator, List<SAnimatorParam> param) : base(_animator, param)
    {
        
    }
    public void Pump()
    {
        am_current.SetFloat(f_pump, UnityEngine.Random.Range(0, 3));
        am_current.SetTrigger(t_pump);
    }
}
