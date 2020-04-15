using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;
using UnityEngine.AI;

public class EntityCharacterPlayer : EntityCharacterBase {
    public override enum_EntityType m_ControllType => enum_EntityType.Player;
    public virtual enum_PlayerCharacter m_Character => enum_PlayerCharacter.Invalid;
    protected virtual PlayerCharacterAnimator m_Animator { get; private set; }
    public Transform tf_WeaponAim { get; private set; }
    protected Transform tf_WeaponHoldRight, tf_WeaponHoldLeft;
    protected SFXAimAssist m_Assist = null;
    public bool m_weaponEquipingFirst { get; private set; } = false;
    public WeaponBase m_WeaponCurrent => m_weaponEquipingFirst ? m_Weapon1 : m_Weapon2;
    public WeaponBase m_Weapon1 { get; private set; }
    public WeaponBase m_Weapon2 { get; private set; }
    public InteractBase m_Interact { get; private set; }
    public Transform tf_UIStatus { get; private set; }
    public override Transform tf_Weapon => m_WeaponCurrent.m_Muzzle;
    public override MeshRenderer m_WeaponSkin => m_WeaponCurrent.m_WeaponSkin;
    protected Transform tf_AimAssistTarget=null;
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.GetMovementSpeed * time;
    public new PlayerExpireManager m_CharacterInfo { get; private set; }
    public bool m_Aiming { get; private set; } = false;
    protected override enum_GameVFX m_DamageClip => enum_GameVFX.PlayerDamage;
    public new EntityPlayerHealth m_Health { get; private set; }
    protected override HealthBase GetHealthManager()
    {
        m_Health=new EntityPlayerHealth(this, OnHealthChanged);
        return m_Health;
    }
    
    NavMeshAgent m_Agent;
    CharacterController m_Controller;

    protected float m_BaseMovementSpeed;
    public override float m_baseMovementSpeed => m_BaseMovementSpeed;
    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected float f_reviveCheck = 0f;

    protected override CharacterExpireManager GetEntityInfo()
    {
        m_CharacterInfo = new PlayerExpireManager(this, m_HitCheck.TryHit, OnExpireChange);
        return m_CharacterInfo;
    }

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        tf_WeaponAim = transform.Find("WeaponAim");
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        tf_UIStatus = transform.FindInAllChild("UIStatus");
        m_Animator = new PlayerCharacterAnimator(tf_Model.GetComponent<Animator>(),OnAnimationEvent);
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
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnLevelFinished, m_CharacterInfo.OnLevelFinish);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        SetBinding(false);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnLevelFinished, m_CharacterInfo.OnLevelFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase>(enum_BC_GameStatus.OnCharacterHealthWillChange, OnCharacterHealthWillChange);
    }

    public void OnPlayerActivate(CPlayerBattleSave _battleSave)
    {
        OnMainCharacterActivate(enum_EntityFlag.Player);

        m_Health.OnActivate(I_MaxHealth, I_DefaultArmor, _battleSave.m_Health >= 0 ? _battleSave.m_Health : I_MaxHealth);
        m_CharacterInfo.SetInfoData(_battleSave);

        m_CharacterRotation = transform.rotation;
        m_Agent.enabled = true;

        ObtainWeapon(GameObjectManager.SpawnWeapon(_battleSave.m_Weapon1));
        if (_battleSave.m_Weapon2.m_Weapon != enum_PlayerWeapon.Invalid)
            ObtainWeapon(GameObjectManager.SpawnWeapon(_battleSave.m_Weapon2));
        OnSwapWeapon(true);
    }

    public void Teleport(Vector3 position,Quaternion rotation)
    {
        m_Controller.enabled = false;           //Magic Spell 1
        m_Agent.enabled = false;
        transform.position = position;
        transform.rotation = rotation;
        m_Controller.enabled = true;        //Magic Spell 2
        m_Agent.enabled = true;
    }
    
    protected override void OnBattleFinish()
    {
        base.OnBattleFinish();
        m_Health.OnBattleFinishResetArmor();
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
        OnWeaponTick(deltaTime);
        OnMoveTick(deltaTime);
        m_Health.OnMaxChange(m_CharacterInfo.F_MaxHealthAdditive,m_CharacterInfo.F_MaxArmorAdditive);
        OnCommonStatus();
    }

    protected virtual float CalculateMovementSpeedBase() => (F_MovementSpeed - m_WeaponCurrent.m_WeaponInfo.m_Weight);
    protected virtual float CalculateMovementSpeedMultiple()=> m_aimingMovementReduction ? (1 - GameConst.F_AimMovementReduction * m_CharacterInfo.F_AimMovementStrictMultiply) : 1f;
    protected virtual Quaternion GetCharacterRotation() => m_CharacterRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);
    protected virtual bool CheckWeaponFiring() =>!Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);

    #region WeaponControll
    
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
        m_Assist.SetEnable(!m_weaponFirePause  && m_AimingTarget != null);
        float reloadDelta = m_CharacterInfo.DoReloadRateTick(deltaTime);
        float fireDelta = m_CharacterInfo.DoFireRateTick(deltaTime);
        if (m_Weapon1) m_Weapon1.Tick(m_weaponFirePause,  fireDelta, reloadDelta);
        if (m_Weapon2) m_Weapon2.Tick(m_weaponFirePause, fireDelta, reloadDelta);
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
        
        return exchangeWeapon;
    }

    public WeaponBase RecycleWeapon()
    {
        WeaponBase recycleWeapon = m_WeaponCurrent;
        if(m_weaponEquipingFirst)
            m_Weapon1 = m_Weapon2;
        m_Weapon2 = null;
        OnSwapWeapon(true);
        return recycleWeapon;
    }

    public void ReforgeWeapon(WeaponBase _reforgeWeapon)
    {
        _reforgeWeapon.OnAttach(this, _reforgeWeapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight, OnFireAddRecoil);
        WeaponBase exchangeWeapon = m_WeaponCurrent;
        m_WeaponCurrent.OnDetach();
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
        exchangeWeapon.DoItemRecycle();
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
        OnWeaponStatus();
    }
    #endregion
    #region CharacterControll
    protected Vector2 m_MoveAxisInput { get; private set; } = Vector2.zero;
    protected Quaternion m_CharacterRotation { get; private set; } = Quaternion.identity;
    protected EntityCharacterBase m_AimingTarget { get; private set; } = null;
    TimerBase m_TargetCheckTimer = new TimerBase(.3f, false);
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
    }
    
    public void OnFireAddRecoil(float recoil)
    {
        TPSCameraController.Instance.AddRecoil(recoil);
        m_Animator.Attack(m_WeaponCurrent.m_WeaponInfo.m_FireRate/m_CharacterInfo.m_FireRateMultiply);
    }

    void OnMoveTick(float deltaTime)
    {
        if (m_aimingMovementReduction) f_aimMovementReduction -= deltaTime;
        if (m_Aiming) f_aimMovementReduction =  GameConst.F_MovementReductionDuration;

        TargetTick(deltaTime);
        
        m_BaseMovementSpeed = CalculateMovementSpeedBase() * CalculateMovementSpeedMultiple();
        if (m_AimingTarget)
            m_CharacterRotation = Quaternion.LookRotation(TCommon.GetXZLookDirection(tf_Head.position, m_AimingTarget.tf_Head.position),Vector3.up); 
        else  if (m_MoveAxisInput != Vector2.zero)
            m_CharacterRotation = Quaternion.LookRotation(m_MoveAxisInput.x * CameraController.CameraXZRightward + m_MoveAxisInput.y * CameraController.CameraXZForward,Vector3.up);

        Vector3 moveDirection = CalculateMoveDirection(m_MoveAxisInput);
        float finalMovementSpeed = m_CharacterInfo.GetMovementSpeed;
        m_Controller.Move(moveDirection * finalMovementSpeed * deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, GetCharacterRotation(), deltaTime * GameConst.I_PlayerRotationSmoothParam);

        m_Animator.SetRun(new Vector2(Vector3.Dot(transform.right, moveDirection), Vector3.Dot(transform.forward, moveDirection)), m_CharacterInfo.m_MovementSpeedMultiply,m_Aiming);
    }

    void TargetTick(float deltaTime)
    {
        m_TargetCheckTimer.Tick(deltaTime);
        if (m_AimingTarget == null || !m_AimingTarget.m_TargetAvailable || !m_TargetCheckTimer.m_Timing)
        {
            m_AimingTarget =GameManager.Instance?GameManager.Instance.GetNeariesCharacter(this, false, true,  GameConst.F_PlayerAutoAimRangeBase+ m_CharacterInfo.F_AimRangeAdditive):null;
            m_TargetCheckTimer.Replay();
        }
    }

    #endregion
    #region CharacterAbility
    public virtual float m_AbilityCooldownScale => 0f;
    public virtual bool m_AbilityAvailable => false;
    public virtual void OnAbilityDown(bool down)
    {
    }
    #endregion
    #region PlayerInteract
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

        if (!m_Interact.TryInteract(this))
            return false;

        if (!m_Interact.m_InteractEnable)
            m_Interact = null;

        OnInteractStatus();
        return true;
    }
    public void OnInteractPickup(InteractPickupAmount pickup,int amount)
    {
        switch (pickup.m_InteractType)
        {
            default:
                Debug.LogError("Invalid Convertions Here!");
                return;
            case enum_Interaction.PickupArmor:
                m_HitCheck.TryHit(new DamageInfo(m_EntityID ,- amount, enum_DamageType.Armor));
                break;
            case enum_Interaction.PickupCoin:
                m_CharacterInfo.OnCoinsGain(amount);
                break;
            case enum_Interaction.PickupHealth:
            case enum_Interaction.PickupHealthPack:
                m_HitCheck.TryHit(new DamageInfo(m_EntityID ,- amount, enum_DamageType.Health));
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

        if (damageInfo.m_SourceID == m_EntityID)
            m_CharacterInfo.OnWillDealtDamage(damageInfo, damageEntity);
        else if (damageEntity.m_EntityID == m_EntityID)
            m_CharacterInfo.OnWillReceiveDamage(damageInfo, damageEntity);
    }

    protected void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float amountApply)
    {
        if(damageInfo.m_SourceID==m_EntityID)
        {
            if(damageEntity.m_IsDead&&GameManager.Instance.EntityOpposite(this,damageEntity))
                m_CharacterInfo.OnKillOpposite();
        }

        if (damageEntity.m_EntityID == m_EntityID)
        {
            if (amountApply > 0)
                m_CharacterInfo.OnAfterReceiveDamage(damageInfo,damageEntity,amountApply);
            else
                m_CharacterInfo.OnReceiveHealing(damageInfo, damageEntity, amountApply);
        }
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
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCommonUpdate, this);
    }
    protected void OnInteractStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger( enum_BC_UIStatus.UI_PlayerInteractUpdate,this);
    }
    protected void OnWeaponStatus()
    {
        m_CharacterInfo.RefreshEffects();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerWeaponUpdate, this);
    }
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnHealthChanged(type);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerHealthUpdate, m_Health);
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
        m_HitCheck.TryHit(new DamageInfo(-1, 101));
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
            UIManager.Instance.DoBindings(this, OnMovementDelta, null,  OnMainDown,OnSubDown, OnAbilityDown);
        else
            UIManager.Instance.RemoveBindings();
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
