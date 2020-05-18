using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

namespace GameSetting
{
    #region Enum
    public enum enum_PlayerAnim
    {
        Invalid = -1,
        //Without Anim Trigger
        Rifle_1001 = 1001,
        Pistol_1002 = 1002,
        Crossbow_1003 = 1003,
        Heavy_1004 = 1004,

        //With Anim Trigger
        HeavySword_2001 = 2001,
        Katana_2002=2002,
        HammerOfDawn_2003=2003,
        Drink_2004=2004,
        LightSword_2005=2005,

        Block_3001=3001,
        Bow_3002=3002,
    }

    public enum enum_EnermyAnim
    {
        Invalid = -1,
        Axe_Dual_Pound_10 = 10,
        Spear_R_Stick_20 = 20,
        Sword_R_Swipe_30 = 30,
        Sword_R_Slash_31 = 31,
        Dagger_Dual_Twice_40 = 40,
        Staff_L_Cast_110 = 110,
        Staff_Dual_Cast_111 = 111,
        Staff_R_Cast_Loop_112 = 112,
        Staff_R_Cast_113 = 113,
        Bow_Shoot_130 = 130,
        CrossBow_Shoot_131 = 131,
        Bow_UpShoot_133 = 133,
        Rifle_HipShot_140 = 140,
        Rifle_AimShot_141 = 141,
        Throwable_Hips_150 = 150,
        Throwable_R_ThrowHip_151 = 151,
        Throwable_R_ThrowBack_152 = 152,
        Throwable_R_Summon_153 = 153,
        Heavy_HipShot_161 = 161,
        Heavy_Mortal_162 = 162,
        Heavy_Shield_Spear_163 = 163,
        Heavy_Remote_164 = 164,
    }
    #endregion
    #region EntityAnimator

    public class CharacterAnimator : AnimatorControlBase
    {
        protected static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_F_Forward = Animator.StringToHash("f_forward");
        static readonly int HS_FM_Movement = Animator.StringToHash("fm_movement");
        static readonly int HS_FM_Attack = Animator.StringToHash("fm_attack");
        static readonly int HS_I_WeaponType = Animator.StringToHash("i_weaponType");
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");
        static readonly int HS_B_Attack = Animator.StringToHash("b_attack");
        public CharacterAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator)
        {
            _animator.fireEvents = true;
            m_Animator.GetComponent<TAnimatorEvent>().Attach(_OnAnimEvent);
        }
        public void OnRevive()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
        protected void OnActivate(int index)
        {
            m_Animator.SetInteger(HS_I_WeaponType, index);
            m_Animator.SetTrigger(HS_T_Activate);
            m_Animator.Update(1f);
        }
        public void SetPause(bool stun)=>m_Animator.speed = stun ? 0 : 1;
        public void SetForward(float forward)=> m_Animator.SetFloat(HS_F_Forward, forward);
        public void SetMovementSpeed(float movementSpeed)=> m_Animator.SetFloat(HS_FM_Movement, movementSpeed);

        public void OnDead()=> m_Animator.SetTrigger(HS_T_Dead);

        protected void SetAttackSpeed(float fireSpeed) => m_Animator.SetFloat(HS_FM_Attack, fireSpeed);
        protected void PlayAttack()=> m_Animator.SetTrigger(HS_T_Attack);
        protected void SetAttacking(bool attacking)=> m_Animator.SetBool(HS_B_Attack, attacking);
    }

    public class GameCharacterAnimator : CharacterAnimator
    {
        public GameCharacterAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
            m_Animator.fireEvents = true;
        }
        public void OnActivate(enum_EnermyAnim _animIndex) => OnActivate((int)_animIndex);
        public void OnAttack(bool attacking)
        {
            SetAttacking(attacking);
            if (attacking)
                PlayAttack();
        }

        public void SetMovementFireSpeed(float movementSpeed, float fireSpeed)
        {
            SetMovementSpeed(movementSpeed);
            SetAttackSpeed(fireSpeed);
        }
    }

    public class PlayerCharacterAnimator : CharacterAnimator
    {
        static readonly int HS_I_Attack = Animator.StringToHash("i_attack");
        static readonly int HS_F_Strafe = Animator.StringToHash("f_strafe");
        static readonly int HS_B_Aim = Animator.StringToHash("b_aim");
        Vector2 v2_movement;
        public PlayerCharacterAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
            v2_movement = Vector2.zero;
        }
        public void OnActivate(enum_PlayerAnim animIndex) => OnActivate((int)animIndex);
        public void SetRun(Vector2 movement, float movementParam, bool aiming)
        {
            v2_movement = Vector2.Lerp(v2_movement, movement, Time.deltaTime * 5f);
            m_Animator.SetFloat(HS_F_Strafe, v2_movement.x);
            m_Animator.SetBool(HS_B_Aim, aiming);
            base.SetForward(v2_movement.y);
            base.SetMovementSpeed(movementParam);
        }
        public void Attacking(bool attacking) => SetAttacking(attacking);
        public void Attack(float fireRate,int animIndex)
        {
            m_Animator.SetInteger(HS_I_Attack, animIndex);
            SetAttackSpeed(1 / fireRate);
            PlayAttack();
        }
    }

    public class PlayerCharacterBethAnimator : PlayerCharacterAnimator
    {
        static readonly int HS_T_Roll = Animator.StringToHash("t_roll");
        static readonly int HS_F_RollSpeed = Animator.StringToHash("fm_roll");
        public PlayerCharacterBethAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator, _OnAnimEvent)
        {
        }
        public void BeginRoll(float rollDuration)
        {
            m_Animator.SetTrigger(HS_T_Roll);
            m_Animator.SetFloat(HS_F_RollSpeed, 1f / rollDuration);
        }
        public void EndRoll()
        {
            m_Animator.SetTrigger(HS_T_Activate);
        }
    }

    #endregion
}