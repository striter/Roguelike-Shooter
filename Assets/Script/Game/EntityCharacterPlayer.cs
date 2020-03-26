using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;
using UnityEngine.AI;

public class EntityCharacterPlayer : EntityCharacterBase {
    #region PresetData
    public int I_AbilityTimes = -1;
    public float F_AbilityCoolDown = 0f;
    #endregion
    public override enum_EntityController m_ControllType => enum_EntityController.Player;
    public virtual enum_PlayerCharacter m_Character => enum_PlayerCharacter.Invalid;
    protected PlayerAnimator m_Animator;
    protected virtual PlayerAnimator GetAnimatorController(Animator animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) => new PlayerAnimator(animator, _OnAnimEvent);
    public Transform tf_WeaponAim { get; private set; }
    protected Transform tf_WeaponHoldRight, tf_WeaponHoldLeft;
    protected SFXAimAssist m_Assist = null;
    public bool m_weaponEquipingFirst { get; private set; } = false;
    public WeaponBase m_WeaponCurrent => m_weaponEquipingFirst ? m_Weapon1 : m_Weapon2;
    public WeaponBase m_Weapon1 { get; private set; }
    public WeaponBase m_Weapon2 { get; private set; }
    public InteractBase m_Interact { get; private set; }
    public float m_EquipmentDistance { get; private set; }
    public Transform tf_UIStatus { get; private set; }
    public override Transform tf_Weapon => m_WeaponCurrent.m_Muzzle;
    public override MeshRenderer m_WeaponSkin => m_WeaponCurrent.m_WeaponSkin;
    protected Transform tf_AimAssistTarget=null;
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.F_MovementSpeed * time;
    public new PlayerInfoManager m_CharacterInfo { get; private set; }
    protected bool m_aiming = false;
    protected override enum_GameVFX m_DamageClip => enum_GameVFX.PlayerDamage;
    public new EntityPlayerHealth m_Health { get; private set; }
    protected override HealthBase GetHealthManager()
    {
        m_Health=new EntityPlayerHealth(this, OnHealthChanged);
        return m_Health;
    }
    
    NavMeshAgent m_Agent;
    CharacterController m_Controller;

    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected float m_BaseMovementSpeed;
    public override float m_baseMovementSpeed => m_BaseMovementSpeed;
    protected float f_reviveCheck = 0f;
    public CharacterAbility m_CharacterAbility { get; private set; }

    protected override CharacterInfoManager GetEntityInfo()
    {
        m_CharacterInfo = new PlayerInfoManager(this, m_HitCheck.TryHit, OnExpireChange);
        return m_CharacterInfo;
    }

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_WeaponAim = transform.Find("WeaponAim");
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        tf_UIStatus = transform.FindInAllChild("UIStatus");
        m_Animator = GetAnimatorController(tf_Model.GetComponent<Animator>(),OnAnimationEvent);
        m_CharacterAbility = new CharacterAbility(I_AbilityTimes, F_AbilityCoolDown, OnAbilityTrigger);
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
        gameObject.layer = GameLayer.I_MovementDetect;
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.enabled = false;
        m_Agent.updateRotation = false;
        m_Controller = GetComponent<CharacterController>();
    }
    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        SetBinding(true);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        SetBinding(false);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }

    public void OnPlayerActivate(CBattleSave m_saveData)
    {
        OnMainCharacterActivate(enum_EntityFlag.Player);
        m_CharacterRotation = transform.rotation;
        m_Agent.enabled = true;

        m_CharacterInfo.SetInfoData(m_saveData);
        m_Health.OnActivate(I_MaxHealth,I_DefaultArmor, m_saveData.m_Health >= 0 ? m_saveData.m_Health : I_MaxHealth);
        ObtainWeapon(GameObjectManager.SpawnWeapon(m_saveData.m_weapon1));
        if (m_saveData.m_weapon2.m_Weapon != enum_PlayerWeapon.Invalid)
            ObtainWeapon(GameObjectManager.SpawnWeapon(m_saveData.m_weapon2));
        OnSwapWeapon(m_saveData.m_weaponEquipingFirst);
    }

    public void Teleport(Vector3 position,Quaternion rotation)
    {
        CameraController.Instance.SetCameraPosition(position);
        m_Controller.enabled = false;           //Magic Spell 1
        m_Agent.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        m_Controller.enabled = true;        //Magic Spell 2
        m_Agent.enabled = true;
    }
    
    protected override void OnDead()
    {
        f_reviveCheck = GameConst.F_PlayerReviveCheckAfterDead;
        m_Animator.OnDead();
        if (m_WeaponCurrent) m_WeaponCurrent.OnPlay(false);
        m_MoveAxisInput = Vector2.zero;
        m_Assist.SetEnable(false);
        base.OnDead();
    }
    protected override void OnRevive()
    {
        base.OnRevive();
        if (m_WeaponCurrent) m_WeaponCurrent.OnPlay(true);
        m_Assist.SetEnable(true);
        m_Animator.OnRevive();

        AudioManager.Instance.Play2DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_ReviveClip));
    }
    public override void DoRecycle()
    {
        base.DoRecycle();
        if (m_Assist)
            m_Assist.Recycle();
    }

    void OnMainDown(bool down)=>OnWeaponTrigger(true,down);
    
    void OnSubDown(bool down) => OnWeaponTrigger(false, down);

    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        OnWeaponTick(deltaTime);
        OnMoveTick(deltaTime);
        m_Health.OnMaxChange(m_CharacterInfo.F_MaxHealthAdditive,m_CharacterInfo.F_MaxArmorAdditive);
        m_CharacterAbility.Tick(deltaTime);
        OnCommonStatus();
    }

    protected virtual float CalculateMovementSpeedBase() => (F_MovementSpeed - m_WeaponCurrent.m_WeaponInfo.m_Weight);
    protected virtual float CalculateMovementSpeedMultiple()=> m_aimingMovementReduction ? (1 - GameConst.F_AimMovementReduction * m_CharacterInfo.F_AimMovementStrictMultiply) : 1f;
    protected virtual Quaternion GetCharacterRotation() => m_CharacterRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);
    protected virtual bool CalculateWeaponFire() =>!Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);

    #region WeaponControll

    void OnWeaponTrigger(bool mainWeapon, bool down)
    {
        if (!mainWeapon && !m_Weapon2)
            return;

        if (m_aiming==down&& m_weaponEquipingFirst==mainWeapon)
            return;
        m_aiming = down;

        if (down && m_weaponEquipingFirst != mainWeapon)
        {
            OnSwapWeapon(mainWeapon);
            OnWeaponStatus();
        }

        if (m_WeaponCurrent)
            m_WeaponCurrent.Trigger(down);
    }
    public bool m_weaponCanFire { get; private set; } = false;
    void OnWeaponTick(float deltaTime)
    {
        if (m_WeaponCurrent == null)
            return;
        
        m_weaponCanFire = CalculateWeaponFire();
        tf_WeaponAim.rotation = GetCharacterRotation();
        m_Assist.SetEnable(m_weaponCanFire  && m_Target != null);

        float reloadDelta = m_CharacterInfo.F_ReloadRateTick(deltaTime);
        float fireDelta = m_CharacterInfo.F_FireRateTick(deltaTime);
        if (m_Weapon1) m_Weapon1.Tick( fireDelta, reloadDelta);
        if (m_Weapon2) m_Weapon2.Tick(fireDelta, reloadDelta);
    }

    public WeaponBase ObtainWeapon(WeaponBase _weapon)
    {
        _weapon.OnAttach(this, _weapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight, OnFireAddRecoil);
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

        OnWeaponStatus();
        return exchangeWeapon;
    }
    public WeaponBase Reforge(WeaponBase _weapon)
    {
        _weapon.OnAttach(this, _weapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight, OnFireAddRecoil);
        WeaponBase exchangeWeapon = m_WeaponCurrent;
        m_WeaponCurrent.OnDetach();
        if (m_weaponEquipingFirst)
        {
            m_Weapon1 = _weapon;
            OnSwapWeapon(true);
        }
        else
        {
            m_Weapon2 = _weapon;
            OnSwapWeapon(false);
        }
        OnWeaponStatus();
        return exchangeWeapon;
    }

    public void OnSwapClick()
    {
        if (!m_Weapon2)
            return;
        OnSwapWeapon(!m_weaponEquipingFirst);
        OnWeaponStatus();
    }

    void OnSwapWeapon(bool isFirst)
    {
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnPlay(false);
        m_weaponEquipingFirst = isFirst;
        m_WeaponCurrent.OnPlay(true);
        m_Animator.OnActivate(m_WeaponCurrent.E_Anim);
        if (m_Assist) m_Assist.Recycle();
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(101, tf_WeaponAim.position, tf_Weapon.forward);
        m_Assist.Play(m_EntityID, tf_WeaponAim, tf_WeaponAim, GameConst.F_AimAssistDistance, GameLayer.Mask.I_ProjectileMask, (Collider collider) => { return GameManager.B_CanSFXHitTarget(collider.Detect(), m_EntityID); });
    }
    #endregion
    #region CharacterControll
    protected Vector2 m_MoveAxisInput { get; private set; } = Vector2.zero;
    protected Quaternion m_CharacterRotation { get; private set; } = Quaternion.identity;
    protected EntityCharacterBase m_Target { get; private set; } = null;
    protected bool m_TargetAvailable => m_Target != null && GameManager.Instance.EntityTargetable(m_Target);
    float targetCheck = .3f;
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
    }
    
    public void OnFireAddRecoil(float recoil)
    {
        TPSCameraController.Instance.AddRecoil(recoil);
        m_Animator.Attack(m_CharacterInfo.F_FireRateTick(m_WeaponCurrent.m_WeaponInfo.m_FireRate));
    }

    void OnMoveTick(float deltaTime)
    {
        if (m_aimingMovementReduction) f_aimMovementReduction -= deltaTime;
        if (m_aiming) f_aimMovementReduction =  GameConst.F_MovementReductionDuration;

        TargetTick(deltaTime);
        
        m_BaseMovementSpeed = CalculateMovementSpeedBase() * CalculateMovementSpeedMultiple();
        if (m_Target)
            m_CharacterRotation = Quaternion.LookRotation(TCommon.GetXZLookDirection(tf_Head.position, m_Target.tf_Head.position),Vector3.up); 
        else  if (m_MoveAxisInput != Vector2.zero)
            m_CharacterRotation = Quaternion.LookRotation(m_MoveAxisInput.x * CameraController.CameraXZRightward + m_MoveAxisInput.y * CameraController.CameraXZForward,Vector3.up);

        Vector3 moveDirection = CalculateMoveDirection(m_MoveAxisInput);
        float finalMovementSpeed = m_CharacterInfo.F_MovementSpeed;
        m_Controller.Move(moveDirection * finalMovementSpeed * deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, GetCharacterRotation(), deltaTime * GameConst.I_PlayerRotationSmoothParam);

        m_Animator.SetRun(new Vector2(Vector3.Dot(transform.right, moveDirection), Vector3.Dot(transform.forward, moveDirection)), m_CharacterInfo.F_MovementSpeedMultiply,m_aiming);
    }

    void TargetTick(float deltaTime)
    {
        if (!m_TargetAvailable || targetCheck < 0f)
        {
            targetCheck = .3f;
            m_Target =GameManager.Instance?GameManager.Instance.GetNeariesCharacter(this, false, true,  GameConst.F_PlayerAutoAimRangeBase+ m_CharacterInfo.F_AimRangeAdditive):null;
        }
        targetCheck -= Time.deltaTime;
    }

    #endregion
    #region CharacterAbility
    public void OnAbilityClick()
    {
        if (m_IsDead)
            return;
        m_CharacterAbility.OnAbilityClick();
    }
    
    protected virtual void OnAbilityTrigger()
    {
        m_CharacterInfo.OnAbilityTrigger();
    }

    public class CharacterAbility
    {
        public float m_CooldownScale => m_abilityCooldownLeft / m_baseAbilityCooldown;
        public bool m_Cooldowning => m_abilityCooldownLeft > 0f;
        protected float m_abilityCooldownLeft = 0f;

        protected float m_baseAbilityCooldown;
        Action OnAbilityTrigger;
        public CharacterAbility(int abilityTime, float abilityCoolDown, Action _OnAbilityTrigger)
        {
            OnAbilityTrigger = _OnAbilityTrigger;
            m_baseAbilityCooldown = abilityCoolDown;
        }
        
        public void OnAbilityClick()
        {
            if (m_Cooldowning)
                return;
            m_abilityCooldownLeft = m_baseAbilityCooldown;
            OnAbilityTrigger();
        }

        public void Tick(float deltaTime)
        {
            if (m_Cooldowning)
                m_abilityCooldownLeft -= deltaTime;
        }
    }
    #endregion
    #region PlayerInteract
    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (!interactTarget.DnCheckInteractResponse(this))
            return;

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
    public void OnInteract()
    {
        if (m_Interact == null)
            return;

        if (!m_Interact.TryInteract(this))
            return;

        if (!m_Interact.DnCheckInteractResponse(this))
            m_Interact = null;

        OnInteractStatus();
    }
    public void OnInteractPickup(InteractPickupAmount pickup,int amount)
    {
        switch (pickup.m_InteractType)
        {
            default:
                Debug.LogError("Invalid Convertions Here!");
                return;
            case enum_Interaction.PickupArmor:
                m_HitCheck.TryHit(new DamageInfo(-amount, enum_DamageType.ArmorOnly, DamageDeliverInfo.Default(m_EntityID)));
                break;
            case enum_Interaction.PickupCoin:
                m_CharacterInfo.OnCoinsGain(amount,true);
                break;
            case enum_Interaction.PickupHealth:
            case enum_Interaction.PickupHealthPack:
                m_HitCheck.TryHit(new DamageInfo(-amount, enum_DamageType.HealthOnly, DamageDeliverInfo.Default(m_EntityID)));
                break;
        }
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerInteractPickup, pickup.transform.position, pickup.m_InteractType, amount);
    }
    #endregion
    #region Action
    
    protected void OnCharacterHealthWillChange(DamageInfo damageInfo, EntityCharacterBase damageEntity)
    {
        if (damageInfo.m_AmountApply <= 0)
            return;

        if (damageInfo.m_detail.I_SourceID == m_EntityID)
            m_CharacterInfo.OnWillDealtDamage(damageInfo, damageEntity);
        else if (damageEntity.m_EntityID == m_EntityID)
            m_CharacterInfo.OnWillReceiveDamage(damageInfo, damageEntity);
    }
    protected void OnEntityActivate(EntityBase targetEntity)
    {
        m_CharacterInfo.OnEntityActivate(targetEntity);
    }

    #endregion
    #region UI Indicator
    protected override bool OnReceiveDamage(DamageInfo damageInfo, Vector3 damageDirection)
    {
        if (base.OnReceiveDamage(damageInfo, damageDirection))
        {
            if(damageDirection!=Vector3.zero) GameManagerBase.Instance.SetEffect_Impact(transform.InverseTransformDirection(damageDirection));
            return true;
        }
        return false;
    }

    protected void OnCommonStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCommonStatus, this);
    }
    protected void OnInteractStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger( enum_BC_UIStatus.UI_PlayerInteractStatus,m_Interact);
    }
    protected void OnWeaponStatus()
    {
        m_CharacterInfo.RefreshEffects();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerWeaponStatus, this);
    }
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnHealthChanged(type);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerHealthStatus, m_Health);
    }
    #endregion
    #region PlayerRevive
    protected override void OnDeadTick(float deltaTime)
    {
        base.OnDeadTick(deltaTime);
        if (f_reviveCheck < 0)
            return;

        f_reviveCheck -= Time.deltaTime;
        if (f_reviveCheck < 0)
            OnCheckRevive();
    }
    void OnCheckRevive()
    {
        if (m_CharacterInfo.CheckRevive())
        {
            RevivePlayer();
            return;
        }
        GameManager.Instance.CheckRevive(RevivePlayer);
    }
    void RevivePlayer()
    {
        ReviveCharacter();
        m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1, SBuff.m_PlayerReviveBuff)));
    }
    #endregion
    void OnAnimationEvent(TAnimatorEvent.enum_AnimEvent animEvent)
    {
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnAnimEvent(animEvent);
    }

    void SetBinding(bool on)
    {
        if (on)
            UIManager.Instance.DoBindings(this, OnMovementDelta, null,  OnMainDown,OnSubDown, OnInteract, OnSwapClick, OnAbilityClick);
        else
            UIManager.Instance.RemoveBindings();
    }
    
    protected class PlayerAnimator : CharacterAnimator
    {
        static readonly int HS_T_Attack = Animator.StringToHash("t_attack");
        static readonly int HS_F_Strafe = Animator.StringToHash("f_strafe");
        static readonly int HS_B_Aim = Animator.StringToHash("b_aim");
        Vector2 v2_movement;
        public PlayerAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator,_OnAnimEvent)
        {
            v2_movement = Vector2.zero;
        }
        public void OnActivate(enum_PlayerAnim animIndex)=>OnActivate((int)animIndex);
        public void SetRun(Vector2 movement,float movementParam,bool aiming)
        {
            v2_movement = Vector2.Lerp(v2_movement,movement,Time.deltaTime*5f);
            m_Animator.SetFloat(HS_F_Strafe, v2_movement.x);
            m_Animator.SetBool(HS_B_Aim, aiming);
            base.SetForward(v2_movement.y);
            base.SetMovementSpeed(movementParam);
        }
        public void Attack(float fireRate)
        {
            SetFireSpeed(1/fireRate);
            m_Animator.SetTrigger(HS_T_Attack);
        }
    }
#if UNITY_EDITOR
    CapsuleCollider hitBox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && GameManager.Instance&&!GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if (!hitBox)
            hitBox = transform.GetComponentInChildren<CapsuleCollider>();
        Gizmos.color = Color.green;
        Gizmos_Extend.DrawWireCapsule(transform.position+transform.up*hitBox.height/2*hitBox.transform.localScale.y,Quaternion.LookRotation(transform.forward,transform.up), hitBox.transform.localScale, hitBox.radius,hitBox.height);
    }
#endif
}
