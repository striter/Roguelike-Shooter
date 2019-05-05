using System;
using System.Collections.Generic;
using UnityEngine;
using TSpecial;
public class WeaponPlayer : WeaponBase,ISingleCoroutine
{
    protected WeaponAnimator wa_current;
    protected Transform tf_ZoomPos;
    protected Transform tf_UnzoomPos;
    protected Transform tf_Model;
    public float F_ReloadStartTime = .3f;
    public float F_ReloadEndTime = .3f;
    public float F_AimCoolDown = .8f;
    public float F_SprintCoolDown = .3f;
    public float F_RecoilPitchPerPellet = 1f;
    public float F_RecoilYawPerPellet = 1f;
    public float F_AimSpreadMultiple = .3f;
    protected float f_actionCooldown;
    public bool b_reloading { get; protected set; }
    protected PlayerBase m_attachPlayer => (base.m_attacher as PlayerBase);
    protected override void Awake()
    {
        tf_Model = transform.Find("Model");
        tf_UnzoomPos = transform.Find("UnzoomPos");
        tf_ZoomPos = transform.Find("ZoomPos");
    }
    protected virtual void OnDisable()
    {
        this.StopSingleCoroutines(0,1,2); 
    }
    void Init()
    {
        wa_current.Reset();
        b_reloading = false;
        tf_Model.localPosition = tf_UnzoomPos.localPosition;
        tf_Model.localRotation = tf_UnzoomPos.localRotation;
    }
    public virtual Action Attach(PlayerBase _attacher, PickupInfoWeapon _info, Action _OnWeaponStatusChanged, Action<bool> _OnWeaponHitEnermy)
    { 
        base.Attach(_attacher,_info,_OnWeaponStatusChanged,_OnWeaponHitEnermy);
        Init();
        OnWeaponStatusChanged();
        SetAnimatorStatus();
        return SetAnimatorStatus;
    }
    #region Fire
    public bool Trigger(bool down)
    {
        if (B_Actionable&&I_AmmoLeft<=0)
            return false;

        OnTriggerSet(down);
        return true;
    }
    protected virtual void OnTriggerSet(bool down)
    {
    }
    RaycastHit rh_hitInfo;
    protected void FireOnePellet()
    {
        FireAt(CameraController.MainCamera.transform.position, CameraController.MainCamera.transform.forward,(m_attachPlayer.b_aim ? F_AimSpreadMultiple : 1f));
        FPSCameraController.Instance.AddRecoil(-F_RecoilPitchPerPellet,UnityEngine.Random.Range(-F_RecoilYawPerPellet, F_RecoilYawPerPellet) ,F_FireRate);
    }
    #endregion
    #region Reload
    public bool StartReload()
    {
        if (!B_Actionable||m_attachPlayer.I_AmmoLeft <= 0||I_AmmoLeft>=I_MagCapacity)
            return false;

        b_reloading = true;
        wa_current.StartReload();
        SetActionPause(F_ReloadStartTime);
        this.StartSingleCoroutine(1,TIEnumerators.PauseDel(F_ReloadStartTime,   OnReloading));
        return true;
    }
    protected void OnReloading()
    {
        wa_current.Reloading(b_reloading);
        SetActionPause(F_ReloadTime);
        this.StartSingleCoroutine(1, TIEnumerators.PauseDel(F_ReloadTime, OnReloadFinished));
    }
    public void StopReload()
    {
        if (!b_reloading||I_AmmoLeft<=0)
        {
            return;
        }
        b_reloading = false;
        this.StopSingleCoroutine(1);
        wa_current.Reloading(b_reloading);
        SetActionable();
    }
    protected virtual void OnReloadFinished()
    {
        b_reloading = false;
        wa_current.Reloading(b_reloading);
        SetActionPause(F_ReloadEndTime);
        int i_ammoToRefill = I_AmmoLeft==0?I_MagCapacity:I_MagCapacity - I_AmmoLeft + 1;
        if (m_attachPlayer.I_AmmoLeft < i_ammoToRefill)
        {
            i_ammoToRefill = m_attachPlayer.I_AmmoLeft;
        }
        I_AmmoLeft += i_ammoToRefill;
        m_attachPlayer.OnAmmoUsed(i_ammoToRefill);
    }
    #endregion
    #region AimZoom
    public virtual void Zoom(bool zoomIn)
    {
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            tf_Model.localPosition = Vector3.Lerp(tf_Model.localPosition, zoomIn?tf_ZoomPos.localPosition : tf_UnzoomPos.localPosition,  value);
            tf_Model.localRotation = Quaternion.Lerp(tf_Model.localRotation, zoomIn ? tf_ZoomPos.localRotation : tf_UnzoomPos.localRotation, value);
        }, 0,1,zoomIn? 2f: F_AimCoolDown));
    }
    #endregion
    protected void SetActionPause(float time)
    {
        f_actionCooldown = Time.time + time;
    }
    protected void SetActionable()
    {
        f_actionCooldown = 0f;
    }
    public bool B_Actionable
    {
        get
        {
            return Time.time > f_actionCooldown&&!b_reloading;
        }
    }
    void SetAnimatorStatus()        //Temporaryly using Animator To Save Params
    {
        if (wa_current.Holster(m_attachPlayer.B_HoldingItem))
        {
            SetActionPause(GameSettings.CF_PlayerHoldingItemColldown);
        }

        bool b_sprint = m_attachPlayer.b_sprint && m_attachPlayer.v2_deltaXZMovement.magnitude > .5f&&m_attachPlayer.B_IsGrounded;
        if (wa_current.Sprint(b_sprint))
        {
            if (b_sprint)
            {
                wa_current.StartSprint();
                SetActionPause(F_SprintCoolDown);
            }
        }

        if (wa_current.Aim(m_attachPlayer.b_aim))
        {
            if (m_attachPlayer.b_aim)
            {
                wa_current.StartAim();
                SetActionPause(F_AimCoolDown);
            }
            Zoom(m_attachPlayer.b_aim);
        }
    }
}

public  class WeaponAnimator:TSpecial.AnimatorClippingTime
{
    #region Readonly
    public static readonly int hs_fire = Animator.StringToHash("fm_fire");
    public static readonly int hs_aim = Animator.StringToHash("fm_aim");
    public static readonly int hs_sprint = Animator.StringToHash("fm_sprint");
    public static readonly int hs_reload = Animator.StringToHash("fm_reload");

    readonly int t_trigger = Animator.StringToHash("t_fire");
    readonly int t_reload = Animator.StringToHash("t_reload");
    readonly int t_sprint = Animator.StringToHash("t_sprint");
    readonly int t_aim = Animator.StringToHash("t_aim");

    readonly int b_reloading = Animator.StringToHash("b_reloading");
    readonly int b_holster = Animator.StringToHash("b_holster");
    readonly int b_sprint = Animator.StringToHash("b_sprint");
    readonly int b_aim = Animator.StringToHash("b_aim");
    #endregion
    public WeaponAnimator(Animator _animator, List<SAnimatorParam> _animatorParams):base(_animator,_animatorParams)
    {
    }
    public override void Reset( )
    {
        base.Reset();
        Reloading(false);
        Holster(false);
        Sprint(false);
        Aim(false);
    }
    public virtual void Fire()
    {
        am_current.SetTrigger(t_trigger);
    }
    public virtual void StartReload()
    {
        am_current.SetTrigger(t_reload);
    }
    public void Reloading(bool reloading)
    {
        am_current.SetBool(b_reloading, reloading);
    }
    public virtual bool Holster(bool holster)
    {
        if (am_current.GetBool(b_holster) != holster)
        {
            am_current.SetBool(b_holster, holster);
            return true;
        }
        return false;
    }
    public virtual void StartSprint()
    {
        am_current.SetTrigger(t_sprint);
    }
    public virtual bool Sprint(bool sprint)
    {
        if (am_current.GetBool(b_sprint) != sprint)
        {
            am_current.SetBool(b_sprint, sprint);
            return true;
        }
        return false;
    }
    public virtual void StartAim()
    {
        am_current.SetTrigger(t_aim);
    }
    public virtual bool Aim(bool aim)
    {
        if (am_current.GetBool(b_aim) != aim)
        {
            am_current.SetBool(b_aim, aim);
            return true;
        }
        return false;
    }

}