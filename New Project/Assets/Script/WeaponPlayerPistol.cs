using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TSpecial;
public class WeaponPlayerPistol : WeaponPlayer
{
    public override enum_WeaponType E_WeaponType => enum_WeaponType.Pistol;
    public override enum_AmmoType E_AmmoType => enum_AmmoType.Dot45ACP;
    Transform tf_Silencer;

    protected override void Awake()
    {
        base.Awake();
        Transform tf_Weapon = tf_Model.Find("P_Hands_spine_end_jnt/cluster1/cluster3/gun");
        tf_Muzzle = tf_Weapon.Find("Muzzle");
        tf_ShellThrow = tf_Weapon.Find("ShellThrow");
        wa_current = new WeaponAnimator(tf_Model.GetComponent<Animator>(), new List<SAnimatorParam>(){
            new SAnimatorParam("fire1",WeaponAnimator.hs_fire,F_FireRate),
            new SAnimatorParam("reload", WeaponAnimator.hs_reload, F_ReloadTime),
        });
    }
    protected override void OnTriggerSet(bool down)
    {
        base.OnTriggerSet(down);
        if (B_Actionable&&down&&I_AmmoLeft>0)
        {
            SpawnShell();
            SpawnMuzzle();
            FireOnePellet();

            SetActionPause(F_FireRate);
            I_AmmoLeft--;
            OnWeaponStatusChanged();
        }
    }
}
