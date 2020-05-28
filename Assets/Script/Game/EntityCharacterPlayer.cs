using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;
using UnityEngine.AI;

public class EntityCharacterPlayer : EntityCharacterBase
{
    public int I_HealthPerRank=10;
    public int I_DefaultArmor;
    public int I_ArmorPerRank=10;

    public override enum_EntityType m_ControllType => enum_EntityType.Player;
    public virtual enum_PlayerCharacter m_Character => enum_PlayerCharacter.Invalid;
    protected override enum_BattleVFX m_DamageClip => enum_BattleVFX.PlayerDamage;

    public Transform tf_WeaponAim { get; private set; }
    protected Transform tf_WeaponHoldRight, tf_WeaponHoldLeft;
    protected SFXAimAssist m_AimAssist = null;
    public bool m_weaponEquipingFirst { get; private set; } = false;
    protected virtual PlayerCharacterBethAnimator m_Animator { get; private set; }

    public enum_PlayerCharacterEnhance m_Enhance { get; private set; }
    public WeaponBase m_WeaponCurrent => m_weaponEquipingFirst ? m_Weapon1 : m_Weapon2;
    public WeaponBase m_Weapon1 { get; private set; }
    public WeaponBase m_Weapon2 { get; private set; }
    public InteractBase m_Interact { get; private set; }
    public Transform tf_UIStatus { get; private set; }
    public new PlayerExpireManager m_CharacterInfo { get; private set; }
    public bool m_Aiming { get; private set; } = false;
    public new EntityPlayerHealth m_Health { get; private set; }

    NavMeshAgent m_Agent;
    CharacterController m_Controller;
    TimerBase m_ArmorRegenTimer = new TimerBase(GameConst.F_PlayerArmorRegenDuration);

    public override Transform tf_Weapon => m_WeaponCurrent.m_Muzzle;
    public override MeshRenderer m_WeaponSkin => m_WeaponCurrent.m_WeaponSkin;
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.GetCharacterMovementSpeed() * time;
    protected override HealthBase GetHealthManager()
    {
        m_Health=new EntityPlayerHealth(this, OnUIHealthChanged);
        return m_Health;
    }
    
    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected TimerBase m_ReviveTimer=new TimerBase(GameConst.F_PlayerReviveCheckAfterDead);

    public override float GetBaseMovementSpeed() => (base.GetBaseMovementSpeed()+(m_Enhance>= enum_PlayerCharacterEnhance.MovementSpeed?GameConst.F_PlayerEnhanceMovementSpeedAdditive:0f)) * CalculateMovementSpeedMultiple();
    public override float GetBaseCriticalRate()=> base.GetBaseCriticalRate()+(m_Enhance>= enum_PlayerCharacterEnhance.Critical?GameConst.F_PlayerEnhanceCriticalRateAdditive:0f);
    public float GetMaxHealthAdditive() =>  m_CharacterInfo.m_RankManager.m_Rank * I_HealthPerRank + m_CharacterInfo.F_MaxHealthAdditive+(m_Enhance>= enum_PlayerCharacterEnhance.Health?GameConst.I_PlayerEnhanceMaxHealthAdditive:0f);
    public float GetMaxArmorAdditive() => m_CharacterInfo.m_RankManager.m_Rank * I_ArmorPerRank + m_CharacterInfo.F_MaxArmorAdditive+(m_Enhance>=enum_PlayerCharacterEnhance.Armor?GameConst.I_PlayerEnhanceMaxArmorAddtive:0f);

    protected override CharacterExpireManager GetEntityInfo()
    {
        m_CharacterInfo = new PlayerExpireManager(this, OnExpireChange);
        return m_CharacterInfo;
    }

    public override void OnPoolInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolInit(_identity, _OnRecycle);
        tf_WeaponAim = transform.Find("WeaponAim");
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        tf_UIStatus = transform.FindInAllChild("UIStatus");
        m_Animator = new PlayerCharacterBethAnimator(tf_Model.GetComponent<Animator>(),OnAnimationEvent);
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
        gameObject.layer = GameLayer.I_MovementDetect;
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.enabled = false;
        m_Agent.updateRotation = false;
        m_Controller = GetComponent<CharacterController>();
    }

    public  EntityCharacterPlayer OnPlayerActivate(CBattleSave _battleSave)
    {
        OnEntityActivate(enum_EntityFlag.Player);
        m_Enhance = _battleSave.m_Enhance;
        UIManager.Instance.DoBindings(this, OnMovementDelta, null, OnMainDown, OnSubDown, OnAbilityDown);

        m_Health.OnActivate(I_MaxHealth, I_DefaultArmor, _battleSave.m_Health >= 0 ? _battleSave.m_Health : I_MaxHealth);
        m_CharacterInfo.SetInfoData(_battleSave);

        m_CharacterRotation = transform.rotation;
        m_Agent.enabled = true;

        ObtainWeapon(GameObjectManager.SpawnWeapon(_battleSave.m_Weapon1));
        if (_battleSave.m_Weapon2.m_Weapon != enum_PlayerWeaponIdentity.Invalid)
            ObtainWeapon(GameObjectManager.SpawnWeapon(_battleSave.m_Weapon2));
        OnSwapWeapon(true);
        return this;
    }
    
    public override void OnPoolRecycle()
    {
        base.OnPoolRecycle();
        if (m_AimAssist)
        {
            m_AimAssist.Recycle();
            m_AimAssist = null;
        }

        if (m_Weapon1)
        {
            RecycleWeapon(m_Weapon1);
            m_Weapon1 = null;
        }

        if (m_Weapon2)
        {
            RecycleWeapon(m_Weapon2);
            m_Weapon2 = null;
        }

        m_Interact = null;
        OnInteractStatus();
        UIManager.Instance.RemoveBindings();
    }

    public void PlayAttackTriggering(bool attacking) => m_Animator.Attacking(attacking);
    public void PlayAttackAnim(float animSpeed, int animIndex) => m_Animator.Attack(animSpeed, animIndex);
    public void PlayRecoil(float recoil) => TPSCameraController.Instance.AddRecoil(recoil);
    public void PlayTeleport(Vector3 position,Quaternion rotation)
    {
        m_Controller.enabled = false;           //Magic Spell 1
        m_Agent.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        m_Controller.enabled = true;        //Magic Spell 2
        m_Agent.enabled = true;
    }

    protected override void OnDead()
    {
        m_ReviveTimer.Replay();

        m_Animator.OnDead();
        if (m_WeaponCurrent) m_WeaponCurrent.OnShow(false);
        m_MoveAxisInput = Vector2.zero;
        m_AimAssist.SetEnable(false);
        base.OnDead();
    }

    protected override void OnRevive()
    {
        base.OnRevive();
        if (m_WeaponCurrent) m_WeaponCurrent.OnShow(true);
        m_AimAssist.SetEnable(true);
        m_Animator.OnRevive();

        AudioManager.Instance.Play2DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_ReviveClip));
    }

    void OnMainDown(bool down)
    {
        if (down)
        {
            TrySwapWeapon(true);

            if (OnInteract())
                return;
        }

        OnWeaponTrigger(true,down);
    }

    void OnSubDown(bool down)
    {
        if(down)
        {
            TrySwapWeapon(false);

            if ( OnInteract())
                return;
        }

        OnWeaponTrigger(false,down);
    } 

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        m_ArmorRegenTimer.Tick(deltaTime);
        if(!m_ArmorRegenTimer.m_Timing&&!m_Health.m_ArmorFull)
            OnReceiveDamage(new DamageInfo(-1).SetDamage(-GameConst.F_PlayerArmorRegenPerSec*deltaTime, enum_DamageType.Armor),Vector3.zero);

        OnWeaponTick(deltaTime);
        OnMoveTick(deltaTime);
        m_Health.OnMaxChange(GetMaxHealthAdditive(), GetMaxArmorAdditive());
        OnUICommonStatus();
    }

    protected override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
        if (damageEntity.m_EntityID == m_EntityID && amountApply > 0)
            m_ArmorRegenTimer.Replay();

        if (amountApply > 0 && BattleManager.Instance.EntityOpposite(this, damageEntity))
        {
            if (damageInfo.m_IdentityType == enum_DamageIdentity.PlayerWeapon && damageInfo.m_IdentityID == m_WeaponCurrent.m_WeaponID)
                m_WeaponCurrent.OnDealtDamage(amountApply);

            if (damageEntity.m_IsDead)
                m_CharacterInfo.OnKilledEnermy(damageInfo, damageEntity);
        }
    }

    protected virtual float CalculateMovementSpeedMultiple()=> m_aimingMovementReduction ? (1 - GameConst.F_AimMovementReduction * m_CharacterInfo.F_AimMovementStrictMultiply) : 1f;
    protected virtual Quaternion GetCharacterRotation() => m_CharacterRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);
    protected virtual bool CheckWeaponFiring() =>!Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);

    #region Weapon Controll
    void TrySwapWeapon(bool mainWeapon)
    {
        if (!mainWeapon && !m_Weapon2)
            return;

        if (m_weaponEquipingFirst != mainWeapon)
            OnSwapWeapon(mainWeapon);
    }

    void OnWeaponTrigger(bool weaponFirst,  bool down)
    {
        if (weaponFirst && !m_Weapon1)
            return;
        if (!weaponFirst && !m_Weapon2)
            return;

        if (weaponFirst != m_weaponEquipingFirst)
            return;

        m_Aiming = down;

        if (m_WeaponCurrent)
            m_WeaponCurrent.Trigger(down);
    }
    public bool m_weaponFirePause { get; private set; } = false;
    void OnWeaponTick(float deltaTime)
    {
        if (m_WeaponCurrent == null)
            return;

        tf_WeaponAim.rotation = GetCharacterRotation();

        m_weaponFirePause = !CheckWeaponFiring();
        m_AimAssist.SetEnable(!m_weaponFirePause  && m_AimingTarget != null);
        m_WeaponCurrent.WeaponTick(m_weaponFirePause, deltaTime);

        float reloadTick = m_CharacterInfo.DoReloadRateTick(deltaTime);
        if (m_Weapon1) m_Weapon1.ReloadTick(reloadTick);
        if (m_Weapon2) m_Weapon2.ReloadTick(reloadTick);
    }

    public WeaponBase ObtainWeapon(WeaponBase _weapon)
    {
        _weapon.OnAttach(this, _weapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight);
        WeaponBase exchangeWeapon = null;
        if (m_Weapon1 != null && m_Weapon2 != null)
        {
            m_WeaponCurrent.OnDetach();
            exchangeWeapon = m_WeaponCurrent;
            if (m_weaponEquipingFirst)
                m_Weapon1 = _weapon;
            else
                m_Weapon2 = _weapon;
            OnSwapWeapon(m_weaponEquipingFirst);
        }
        else if (m_Weapon1 == null)
        {
            m_Weapon1 = _weapon;
            OnSwapWeapon(true);
        }
        else if (m_Weapon2 == null)
        {
            m_Weapon2 = _weapon;
            OnSwapWeapon(false);
        }
        
        return exchangeWeapon;
    }

    void RecycleWeapon(WeaponBase recycleWeapon)
    {
        recycleWeapon.OnShow(false);
        recycleWeapon.OnDetach();
        recycleWeapon.DoRecycle();
    }

    void OnSwapWeapon(bool isFirst)
    {
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnShow(false);
        m_weaponEquipingFirst = isFirst;
        m_WeaponCurrent.OnShow(true);
        m_Animator.OnActivate(m_WeaponCurrent.E_Anim);
        if (m_AimAssist) m_AimAssist.Recycle();
        m_AimAssist = GameObjectManager.SpawnSFX<SFXAimAssist>(101, tf_WeaponAim.position, tf_Weapon.forward);
        m_AimAssist.Play(m_EntityID, tf_WeaponAim, tf_WeaponAim, GameConst.F_AimAssistDistance, GameLayer.Mask.I_ProjectileMask, (Collider collider) => { return BattleManager.B_CanSFXHitTarget(collider.Detect(), m_EntityID); });
        OnUIWeaponStatus();
    }

    public enum_Rarity GameWeaponRecycle()
    {
        WeaponBase recycleWeapon = m_WeaponCurrent;
        if(m_weaponEquipingFirst)
            m_Weapon1 = m_Weapon2;
        m_Weapon2 = null;
        OnSwapWeapon(true);
        RecycleWeapon(recycleWeapon);
        return recycleWeapon.m_WeaponInfo.m_Rarity;
    }


    public void GameWeaponReforge(WeaponBase _reforgeWeapon)
    {
        _reforgeWeapon.OnAttach(this, _reforgeWeapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight);
        RecycleWeapon(m_WeaponCurrent);
        if (m_weaponEquipingFirst)
        {
            m_Weapon1 = _reforgeWeapon;
            OnSwapWeapon(true);
        }
        else
        {
            m_Weapon2 = _reforgeWeapon;
            OnSwapWeapon(false);
        }
    }
    
    #endregion
    #region Character Controll
    protected Vector2 m_MoveAxisInput { get; private set; } = Vector2.zero;
    protected Quaternion m_CharacterRotation { get; private set; } = Quaternion.identity;
    protected EntityCharacterBase m_AimingTarget { get; private set; } = null;
    TimerBase m_TargetCheckTimer = new TimerBase(.3f, false);
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
    }


    void OnMoveTick(float deltaTime)
    {
        if (m_aimingMovementReduction) f_aimMovementReduction -= deltaTime;
        if (m_Aiming) f_aimMovementReduction =  GameConst.F_MovementReductionDuration;

        TargetTick(deltaTime);
        
        if (m_AimingTarget)
            m_CharacterRotation = Quaternion.LookRotation(TCommon.GetXZLookDirection(tf_Head.position, m_AimingTarget.tf_Head.position),Vector3.up); 
        else  if (m_MoveAxisInput != Vector2.zero)
            m_CharacterRotation = Quaternion.LookRotation(m_MoveAxisInput.x * CameraController.CameraXZRightward + m_MoveAxisInput.y * CameraController.CameraXZForward,Vector3.up);

        Vector3 moveDirection = CalculateMoveDirection(m_MoveAxisInput);
        float finalMovementSpeed = m_CharacterInfo.GetCharacterMovementSpeed();
        m_Controller.Move(moveDirection * finalMovementSpeed * deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, GetCharacterRotation(), deltaTime * GameConst.I_PlayerRotationSmoothParam);
        m_Animator.SetRun(new Vector2(Vector3.Dot(transform.right, moveDirection), Vector3.Dot(transform.forward, moveDirection)), m_CharacterInfo.m_MovementSpeedMultiply,m_Aiming);
    }

    void TargetTick(float deltaTime)
    {
        m_TargetCheckTimer.Tick(deltaTime);
        if (m_AimingTarget == null || !m_AimingTarget.m_TargetAvailable || !m_TargetCheckTimer.m_Timing)
        {
            m_AimingTarget =BattleManager.Instance?BattleManager.Instance.GetNeariesCharacter(this, false, true,  GameConst.F_PlayerAutoAimRangeBase):null;
            m_TargetCheckTimer.Replay();
        }
    }

    public Vector3 GetAimingPosition(bool projectile) => m_AimingTarget ? m_AimingTarget.transform.position : (tf_WeaponAim.position + tf_WeaponAim.forward * (projectile?GameConst.I_ProjectileInvalidDistance:GameConst.I_ProjectileParacurveInvalidDistance));
    #endregion
    #region Character Ability
    public virtual float m_AbilityCooldownScale => 0f;
    public virtual bool m_AbilityAvailable => false;
    public virtual void OnAbilityDown(bool down)
    {
    }
    #endregion
    #region Player Interact
    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (interactTarget.B_InteractOnTrigger)
        {
            interactTarget.TryInteract(this);
            return;
        }

        if (isEnter)
        {
            m_Interact = interactTarget;
            OnInteractStatus();
        }
        else if (m_Interact == interactTarget)
        {
            m_Interact = null;
            OnInteractStatus();
        }
    }
    protected bool OnInteract()
    {
        if (m_Interact == null)
            return false;

        InteractBase interactTarget = m_Interact;

        if (!interactTarget.TryInteract(this))
            return false;

        if (!interactTarget.m_InteractEnable)
            m_Interact = null;

        OnInteractStatus();
        return true;
    }

    protected override float OnReceiveDamageAmount(DamageInfo damageInfo, Vector3 direction)
    {
        if (damageInfo.m_IsDamage&&m_WeaponCurrent.OnDamageBlockCheck(damageInfo))
            return 0;

        float amount = base.OnReceiveDamageAmount(damageInfo, direction);
        GameManagerBase.Instance.SetEffect_Impact(transform.InverseTransformDirection(direction));
        return amount;
    }
    
    #endregion
    #region Indicator

    protected void OnUICommonStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCommonUpdate, this);
    }
    protected void OnInteractStatus()
    {
        GameManagerBase.Instance.SetExtraTimeScale(m_Interact==null?1f:.2f);
        TBroadCaster<enum_BC_UIStatus>.Trigger( enum_BC_UIStatus.UI_PlayerInteractUpdate,this);
    }
    protected void OnUIWeaponStatus()
    {
        m_CharacterInfo.RefreshEffects();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerWeaponUpdate, this);
    }
    protected override void OnUIHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnUIHealthChanged(type);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerHealthUpdate, m_Health);
    }
    #endregion
    #region PlayerRevive
    protected override void OnDeadTick(float deltaTime)
    {
        base.OnDeadTick(deltaTime);

        if (m_ReviveTimer.m_Timing)
        {
            m_ReviveTimer.Tick(deltaTime);
            if(!m_ReviveTimer.m_Timing)
                OnCheckRevive();
        }
    }
    void OnCheckRevive()
    {
        if (m_CharacterInfo.CheckRevive())
        {
            RevivePlayer();
            return;
        }
        BattleManager.Instance.CheckPlayerRevive(RevivePlayer);
    }
    void RevivePlayer()
    {
        ReviveCharacter();
        m_HitCheck.TryHit(new DamageInfo(-1).AddPresetBuff(101));
    }
    #endregion
    void OnAnimationEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnAnimEvent(animEvent);
    }
    
#if UNITY_EDITOR
    CapsuleCollider hitBox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && BattleManager.Instance&&!BattleManager.Instance.B_PhysicsDebugGizmos)
            return;

        if (!hitBox)
            hitBox = transform.GetComponentInChildren<CapsuleCollider>();
        Gizmos.color = Color.green;
         Gizmos_Extend.DrawWireCapsule(transform.position+transform.up*hitBox.height/2*hitBox.transform.localScale.y,Quaternion.LookRotation(transform.forward,transform.up), hitBox.transform.localScale, hitBox.radius,hitBox.height);
    }
#endif
}
