using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class EntityCharacterPlayer : EntityCharacterBase {
    #region PresetData
    public int I_AbilityTimes = -1;
    public float F_AbilityCoolDown = 0f;
    #endregion
    public override enum_EntityController m_Controller => enum_EntityController.Player;
    public virtual enum_PlayerCharacter m_Character => enum_PlayerCharacter.Invalid;
    protected CharacterController m_CharacterController;
    protected PlayerAnimator m_Animator;
    protected virtual PlayerAnimator GetAnimatorController(Animator animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) => new PlayerAnimator(animator, _OnAnimEvent);
    public Transform tf_WeaponAim { get; private set; }
    public Transform tf_CameraAttach { get; private set; }
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
    public override Transform tf_WeaponModel => m_WeaponCurrent.m_Case;
    protected Transform tf_AimAssistTarget=null;
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    protected override void ActivateHealthManager(float maxHealth) => m_Health.OnActivate(maxHealth, I_DefaultArmor, true);
    protected bool m_aiming = false;
    protected override enum_GameVFX m_DamageClip => enum_GameVFX.PlayerDamage;
    public new EntityPlayerHealth m_Health { get; private set; }
    protected override HealthBase GetHealthManager()
    {
        m_Health=new EntityPlayerHealth(this, OnHealthChanged);
        return m_Health;
    } 

    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected float m_BaseMovementSpeed;
    public override float m_baseMovementSpeed => m_BaseMovementSpeed;
    protected float f_reviveCheck = 0f;
    public CharacterAbility m_Ability { get; private set; }

    protected override CharacterInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, m_HitCheck.TryHit, OnExpireChange,OnPlayerActionChange);
        return m_PlayerInfo;
    }

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        gameObject.layer = GameLayer.I_MovementDetect;
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterController.detectCollisions = false;
        tf_WeaponAim = transform.Find("WeaponAim");
        tf_CameraAttach = transform.Find("CameraAttach");
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        tf_UIStatus = transform.FindInAllChild("Status");
        m_Animator = GetAnimatorController(tf_Model.GetComponent<Animator>(),OnAnimationEvent);
        m_Ability = new CharacterAbility(I_AbilityTimes, F_AbilityCoolDown, OnAbilityTrigger);
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
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
    public void SetSpawnPosRot(Vector3 position,Quaternion rotation)
    {
        transform.position = position;
        transform.rotation = rotation;
        m_CharacterRotation = rotation;
        CameraController.Instance.SetCameraRotation(-1,transform.rotation.eulerAngles.y);
    }

    public void SetPlayerInfo(CBattleSave m_saveData)
    {
        m_PlayerInfo.SetInfoData(m_saveData.m_coins, ActionDataManager.CreateActions(m_saveData.m_actionEquipment));
        m_Health.SetInfoData(m_saveData.m_curHealth>=0?m_saveData.m_curHealth:I_MaxHealth,m_saveData.m_curArmor>=0?m_saveData.m_curArmor:I_DefaultArmor,m_saveData.m_maxArmorAdditive>=0?m_saveData.m_maxArmorAdditive:0);

        ObtainWeapon(GameObjectManager.SpawnWeapon(m_saveData.m_weapon1));
        if (m_saveData.m_weapon2.m_Weapon != enum_PlayerWeapon.Invalid)
            ObtainWeapon(GameObjectManager.SpawnWeapon(m_saveData.m_weapon2));
        SwapWeapon(m_saveData.m_weaponEquipingFirst);
    }

    protected override void OnDead()
    {
        f_reviveCheck = GameConst.F_PlayerReviveCheckAfterDead;
        m_Animator.OnDead();
        m_MoveAxisInput = Vector2.zero;
        m_Assist.SetEnable(false);
        base.OnDead();
    }
    protected override void OnRevive()
    {
        base.OnRevive();
        m_Assist.SetEnable(true);
        m_Animator.OnRevive();

        AudioManager.Instance.Play2DClip(m_EntityID, AudioManager.Instance.GetGameSFXClip(m_ReviveClip));
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        if (m_Assist)
            m_Assist.Recycle();
    }

    void OnMainDown(bool down)
    {
        m_aiming = down;

        if (down)
        {
            if (m_Interact != null)
            {
                OnInteract();
                return;
            }
        }

        OnWeaponTrigger(down);
    }
    
    protected override void OnAliveTick(float deltaTime)
    {
        base.OnAliveTick(deltaTime);
        OnWeaponTick(deltaTime);
        OnMoveTick(deltaTime);
        m_Ability.Tick(deltaTime);
        OnCommonStatus();
    }

    protected virtual float CalculateMovementSpeedBase() => (F_MovementSpeed - m_WeaponCurrent.m_WeaponInfo.m_Weight);
    protected virtual float CalculateMovementSpeedMultiple()=> m_aimingMovementReduction ? (1 - GameConst.F_AimMovementReduction * m_PlayerInfo.F_AimMovementStrictMultiply) : 1f;
    protected virtual Quaternion GetCharacterRotation() => m_CharacterRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);
    protected virtual bool CalculateWeaponFire() =>!Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);

    #region WeaponControll
    void OnWeaponTrigger(bool down)
    {
        if (m_Weapon1) m_Weapon1.Trigger(down);
        if (m_Weapon2) m_Weapon2.Trigger(down);
    }
    public void OnWeaponEnergy(float energy)
    {
        if (m_Weapon1) m_Weapon1.OnEnergyReceive(energy);
        if (m_Weapon2) m_Weapon2.OnEnergyReceive(energy);
    }
    public bool m_weaponCanFire { get; private set; } = false;
    void OnReloadClick()
    {
        if (m_WeaponCurrent == null)
            return;
        m_WeaponCurrent.TryReload();
    }

    void OnReload(bool start, float reloadTime)
    {
        if (start)
            m_Animator.Reload(reloadTime);
        else
            m_PlayerInfo.OnReloadFinish();
    }
    void OnWeaponTick(float deltaTime)
    {
        m_weaponCanFire = CalculateWeaponFire();
        if (m_WeaponCurrent == null)
            return;
        tf_WeaponAim.rotation = GetCharacterRotation();
        m_Assist.SetEnable(m_weaponCanFire && !m_WeaponCurrent.B_Reloading && m_Target != null);
        m_WeaponCurrent.AmmoTick(m_PlayerInfo.F_ReloadRateTick(deltaTime));
        if (m_weaponCanFire)
            m_WeaponCurrent.FireTick(m_PlayerInfo.F_FireRateTick( deltaTime));
    }

    public WeaponBase ObtainWeapon(WeaponBase _weapon)
    {
        _weapon.OnAttach(this, _weapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight, OnFireAddRecoil, OnReload);
        WeaponBase exchangeWeapon = null;
        if (m_Weapon1 != null&&m_Weapon2!=null)
        {
            m_WeaponCurrent.OnDetach();
            exchangeWeapon = m_WeaponCurrent;
            if (m_weaponEquipingFirst)
                m_Weapon1 = _weapon;
            else
                m_Weapon2 = _weapon;
            SwapWeapon(m_weaponEquipingFirst);
        }
        else if (m_Weapon1 == null)
        {
            m_Weapon1 = _weapon;
            SwapWeapon(true);
        }
        else if(m_Weapon2==null)
        {
            m_Weapon2 = _weapon;
            SwapWeapon(false);
        }
        OnWeaponStatus();
        return exchangeWeapon;
    }

    public void OnSwapClick()
    {
        if (!m_Weapon2)
            return;
        SwapWeapon(!m_weaponEquipingFirst);
        OnWeaponStatus();
    }

    void SwapWeapon(bool isFirst)
    {
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnShow(false);
        m_weaponEquipingFirst = isFirst;
        m_WeaponCurrent.OnShow(true);
        m_Animator.OnActivate(m_WeaponCurrent.E_Anim);
        if (m_Assist) m_Assist.Recycle();
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(101, tf_WeaponAim.position, tf_Weapon.forward);
        m_Assist.Play(m_EntityID, tf_WeaponAim, tf_WeaponAim, GameConst.F_AimAssistDistance, GameLayer.Mask.I_All, (Collider collider) => { return GameManager.B_CanSFXHitTarget(collider.Detect(), m_EntityID); });
    }
    #endregion
    #region CharacterControll
    protected Vector2 m_MoveAxisInput { get; private set; } = Vector2.zero;
    protected Vector2 m_RotateAxisInput { get; private set; } = Vector2.zero;
    protected Quaternion m_CharacterRotation { get; private set; } = Quaternion.identity;
    protected EntityCharacterBase m_Target { get; private set; } = null;
    protected bool m_TargetAvailable => m_Target != null && GameManager.Instance.CheckEntityTargetable(m_Target);
    float targetCheck = .3f;
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
    }

    void OnRotateDelta(Vector2 rotateDelta)
    {
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        m_RotateAxisInput = rotateDelta;
    }

    public void OnFireAddRecoil(float recoil)
    {
        TPSCameraController.Instance.AddRecoil(new Vector3(0, (TCommon.RandomBool() ? 1 : -1)* recoil,0));
        m_Animator.Fire();
    }

    void OnMoveTick(float deltaTime)
    {
        if (m_aimingMovementReduction) f_aimMovementReduction -= deltaTime;
        if (m_aiming) f_aimMovementReduction = GameConst.F_MovementReductionDuration;
        TPSCameraController.Instance.RotateCamera(m_RotateAxisInput * OptionsManager.m_Sensitive);

        if (!m_TargetAvailable || targetCheck<0f)
        {
            targetCheck = .3f;
            m_Target = GameManager.Instance.GetAvailableEntity(this, false, true, GameConst.F_PlayerAutoAimRange);
        }
        targetCheck -= Time.deltaTime;

        if (m_Target)
        {
            Vector3 m_targetOffset = m_Target.tf_Head.position - tf_Head.position;
            m_CharacterRotation = Quaternion.LookRotation(m_targetOffset.normalized, Vector3.up);

            tf_CameraAttach.position = transform.position+ m_targetOffset/3;
        }
        else
        {
            tf_CameraAttach.position = transform.position;
            if (m_MoveAxisInput != Vector2.zero)
                m_CharacterRotation = Quaternion.LookRotation(m_MoveAxisInput.x * CameraController.CameraXZRightward + m_MoveAxisInput.y * CameraController.CameraXZForward, Vector3.up);
        }


        transform.rotation = Quaternion.Lerp(transform.rotation, GetCharacterRotation(),deltaTime*GameConst.I_PlayerRotationSmoothParam);

        m_BaseMovementSpeed = CalculateMovementSpeedBase() * CalculateMovementSpeedMultiple();

        float finalMovementSpeed = m_CharacterInfo.F_MovementSpeed;
        m_CharacterController.Move((CalculateMoveDirection(m_MoveAxisInput) * finalMovementSpeed + Vector3.down * GameConst.F_Gravity) * deltaTime);
        m_Animator.SetRun(m_MoveAxisInput, finalMovementSpeed / F_MovementSpeed);
    }

    #endregion
    #region CharacterAbility
    public void OnAbilityClick()
    {
        if (m_IsDead)
            return;
        m_Ability.OnAbilityClick();
    }
    
    protected virtual void OnAbilityTrigger()
    {
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
        if (!interactTarget.B_InteractEnable)
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
        {
            Debug.LogError("Can't Interact With Null Target!");
            return;
        }

        if (!m_Interact.TryInteract(this))
            return;

        if (!m_Interact.B_InteractEnable)
            m_Interact = null;

        OnInteractStatus();
    }
    #endregion
    #region Action
    public void OnPickupAction(ActionBase action)
    {
        switch (action.m_ActionType)
        {
            case enum_ActionType.WeaponAbility:
                m_WeaponCurrent.SetWeaponAction(action,0f);
                OnWeaponStatus();
                break;
            case enum_ActionType.PlayerEquipment:
                m_PlayerInfo.OnUseAction(action);
                break;
        }
    }
    public void OnWeaponAbilityClick(bool isFirstWeapon)
    {
        ActionBase targetAction = isFirstWeapon ? m_Weapon1.GetAbilityACtion() : m_Weapon2.GetAbilityACtion();
        if(targetAction!=null)
        m_PlayerInfo.OnUseAction(targetAction);
    }
    public void UpgradeActionPerk(ActionBase _weaponAction)
    {
        if (m_WeaponCurrent.m_WeaponAction == null)
            m_WeaponCurrent.SetWeaponAction(_weaponAction,0f);
        else
            m_WeaponCurrent.m_WeaponAction.Upgrade();
        OnWeaponStatus();
    }
    protected void OnCharacterHealthWillChange(DamageInfo damageInfo, EntityCharacterBase damageEntity)
    {
        if (damageInfo.m_AmountApply <= 0)
            return;

        if (damageInfo.m_detail.I_SourceID == m_EntityID)
        {
            m_PlayerInfo.OnWillDealtDamage(damageInfo, damageEntity);

        }
        else if (damageEntity.m_EntityID == m_EntityID)
        {
            m_PlayerInfo.OnWillReceiveDamage(damageInfo, damageEntity);
        }
    }
    protected void OnEntityActivate(EntityBase targetEntity)
    {
        m_PlayerInfo.OnEntityActivate(targetEntity);
    }
    protected override void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        base.OnCharacterHealthChange(damageInfo, damageEntity, amountApply);
        
        if (amountApply <= 0 || damageEntity.b_isSubEntity || !GameManager.Instance.EntityExists(damageInfo.m_detail.I_SourceID))
            return;

        if (damageInfo.m_detail.I_SourceID == m_EntityID || GameManager.Instance.GetEntity(damageInfo.m_detail.I_SourceID).m_SpawnerEntityID == m_EntityID)
            OnWeaponEnergy(GameExpression.GetActionEnergyRevive(amountApply));
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
        m_PlayerInfo.RefreshEffects();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerWeaponStatus, this);
    }
    protected void OnPlayerActionChange()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerExpireListStatus, m_PlayerInfo);
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
    RangeFloat m_PlayerReviveAmount;
    void OnCheckRevive()
    {
        m_PlayerReviveAmount = new RangeFloat(m_Health.m_BaseHealth, m_Health.m_StartArmor);
        if (m_PlayerInfo.CheckRevive(ref m_PlayerReviveAmount))
        {
            RevivePlayer();
            return;
        }
        GameManager.Instance.CheckRevive(RevivePlayer);
    }
    void RevivePlayer()
    {
        ReviveCharacter(m_PlayerReviveAmount.start, m_PlayerReviveAmount.length);
        m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1, SBuff.SystemPlayerReviveInfo(GameConst.F_PlayerReviveBuffDuration, GameExpression.I_PlayerReviveBuffIndex))));
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
            UIManager.Instance.m_UIControl.DoBinding(this, OnMovementDelta, OnRotateDelta,  OnMainDown, OnSwapClick, OnReloadClick, OnWeaponAbilityClick, OnAbilityClick);
        else
            UIManager.Instance.m_UIControl.RemoveBinding();
    }
    
    protected class PlayerAnimator : CharacterAnimator
    {
        static readonly int HS_T_Fire = Animator.StringToHash("t_attack");
        static readonly int HS_T_Reload = Animator.StringToHash("t_reload");
        static readonly int HS_FM_Reload = Animator.StringToHash("fm_reload");
        static readonly int HS_F_Strafe = Animator.StringToHash("f_strafe");
        Vector2 v2_movement;
        public PlayerAnimator(Animator _animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) : base(_animator,_OnAnimEvent)
        {
            v2_movement = Vector2.zero;
        }
        public void OnActivate(enum_PlayerAnim animIndex)
        {
            OnActivate((int)animIndex);
        }
        public void SetRun(Vector2 movement,float movementParam)
        {
            v2_movement = Vector2.Lerp(v2_movement,movement,Time.deltaTime*5f);
            m_Animator.SetFloat(HS_F_Strafe, v2_movement.x);
            base.SetForward(v2_movement.y);
            base.SetMovementSpeed(movementParam);
        }
        public void Fire()
        {
            m_Animator.SetTrigger(HS_T_Fire);
        }
        public void Reload(float reloadTime)
        {
            m_Animator.SetTrigger(HS_T_Reload);
            m_Animator.SetFloat(HS_FM_Reload, 1 / reloadTime);
        }
    }
#if UNITY_EDITOR
    CapsuleCollider hitBox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && GameManager.Instance&&GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if (!hitBox)
            hitBox = transform.GetComponentInChildren<CapsuleCollider>();
        Gizmos.color = Color.green;
        Gizmos_Extend.DrawWireCapsule(transform.position+transform.up*hitBox.height/2*hitBox.transform.localScale.y,Quaternion.LookRotation(transform.forward,transform.up), hitBox.transform.localScale, hitBox.radius,hitBox.height);
    }
#endif
}
