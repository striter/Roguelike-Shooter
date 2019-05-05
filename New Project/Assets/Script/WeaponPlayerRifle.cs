using System.Collections;
using System.Collections.Generic;
using TSpecial;
using UnityEngine;

public class WeaponPlayerRifle : WeaponPlayer
{
    public override enum_WeaponType E_WeaponType => enum_WeaponType.Rifle;
    public override enum_AmmoType E_AmmoType => enum_AmmoType.Rifle556MM;
    public float F_BurstFireRate;
    WeaponAnimatorRifle wrf_current;
    protected override void Awake()
    {
        base.Awake();
        Transform tf_Weapon = tf_Model.Find("P_Hands_spine_end_jnt/P_Hands_rt_clavicle_jnt/P_Hands_rt_upArm_jnt/P_Hands_rt_elbow_jnt/P_Hands_rt_wrist_jnt/P_Hands_rt_hand_jnt/rifle08_m4");     //BullShit
        tf_Muzzle = tf_Weapon.Find("Muzzle");
        tf_ShellThrow = tf_Weapon.Find("ShellThrow");
        wrf_current = new WeaponAnimatorRifle(transform.Find("Model").GetComponent<Animator>(), new List<SAnimatorParam>() {
            new SAnimatorParam("aimIN",WeaponAnimator.hs_aim,F_AimCoolDown),
            new SAnimatorParam("shotSingle",WeaponAnimator.hs_fire,F_FireRate),
            new SAnimatorParam("reload", WeaponAnimator.hs_reload, F_ReloadTime),
            new SAnimatorParam("shotBurst",WeaponAnimatorRifle.hs_burstFire,F_BurstFireRate),
        });
        wa_current = wrf_current;
    }
    protected override void OnTriggerSet(bool down)
    {
        if (down)
        {
            this.StartSingleCoroutine(2, TIEnumerators.Tick(()=> {
                if (I_AmmoLeft <= 0)
                {
                    this.StopSingleCoroutine(2);
                    return;
                }

                if (!B_Actionable)
                    return;
                SetActionPause(F_FireRate);

                SpawnMuzzle();
                SpawnShell();
                FireOnePellet();

                I_AmmoLeft--;
                OnWeaponStatusChanged();
            }));
        }
        else
        {
            this.StopSingleCoroutine(2);
        }
    }
}
public class WeaponAnimatorRifle : WeaponAnimator
{
    public static readonly int hs_burstFire = Animator.StringToHash("fm_burstFire");
    readonly int t_triggerBurst = Animator.StringToHash("t_burstFire");
    public WeaponAnimatorRifle(Animator _animator, List<SAnimatorParam> param) : base(_animator, param)
    {

    }
    public void TriggerBurst()
    {
        am_current.SetTrigger(t_triggerBurst);
    }
}