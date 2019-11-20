using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class EntityCharacterPlayer : EntityCharacterBase {
    #region Preset
    public int I_AbilityCost = 0;
    public float F_AbilityCoolDown = 0f;
    #endregion
    public override enum_EntityController m_Controller => enum_EntityController.Player;
    public virtual enum_PlayerCharacter m_Character => enum_PlayerCharacter.Invalid;
    protected CharacterController m_CharacterController;
    protected PlayerAnimator m_Animator;
    protected virtual PlayerAnimator GetAnimatorController(Animator animator, Action<TAnimatorEvent.enum_AnimEvent> _OnAnimEvent) => new PlayerAnimator(animator, _OnAnimEvent);
    public Transform tf_WeaponAim { get; private set; }
    protected Transform tf_WeaponHoldRight, tf_WeaponHoldLeft;
    protected SFXAimAssist m_Assist = null;
    public WeaponBase m_WeaponCurrent { get; private set; } = null;
    public InteractBase m_Interact { get; private set; }
    public EquipmentBase m_Equipment { get; private set; }
    public int m_EquipmentTimes { get; private set; }
    public float m_EquipmentDistance { get; private set; }
    public override Transform tf_Weapon => m_WeaponCurrent.m_Case;
    public Transform tf_Status { get; private set; }
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    protected bool m_aiming = false;
    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected override enum_GameAudioSFX m_DamageClip => enum_GameAudioSFX.PlayerDamage;
    protected float f_abilityCoolDown = 0f;
    protected bool m_AbilityCooldowning => f_abilityCoolDown > 0f;
    protected override void ActivateHealthManager(float maxHealth) => m_Health.OnActivate(maxHealth, I_DefaultArmor, true);
    protected float m_BaseMovementSpeed;
    public override float m_baseMovementSpeed => m_BaseMovementSpeed;
    protected override CharacterInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, m_HitCheck.TryHit, OnExpireChange, OnActionsChange);
        return m_PlayerInfo;
    }

    public override void OnPoolItemInit(int poolPresetIndex)
    {
        base.OnPoolItemInit(poolPresetIndex);
        gameObject.layer = GameLayer.I_MovementDetect;
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterController.detectCollisions = false;
        tf_WeaponAim = transform.Find("WeaponAim");
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        tf_Status = transform.FindInAllChild("Status");
        m_Animator = GetAnimatorController(tf_Model.GetComponent<Animator>(),OnAnimationEvent);
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
    }
    protected override void OnPoolItemEnable()
    {
        base.OnPoolItemEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        SetBinding(true);
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        SetBinding(false);
    }

    public void SetPlayerInfo(int coins,float health, List<ActionBase> storedActions)
    {
        m_PlayerInfo.OnCoinsReceive(coins);
        m_PlayerInfo.InitActionInfo(storedActions);
        m_Health.OnRevive(health>0?health:I_MaxHealth,I_DefaultArmor);
    }

    protected override void OnDead()
    {
        f_abilityCoolDown = 0f;
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

        GameAudioManager.Instance.PlayClip(m_EntityID, GameAudioManager.Instance.GetSFXClip(m_ReviveClip), false);
    }
    protected override void OnRecycle()
    {
        base.OnRecycle();
        if (m_Assist)
            m_Assist.Recycle();
    }
    void OnChangeLevel()
    {
        m_Interact = null;
        OnInteractStatus();
    }
    void OnBattleStart()
    {
        m_PlayerInfo.OnBattleStart();
    }
    void OnBattleFinish()
    {
        m_Equipment = null;
        m_EquipmentTimes = 0;
        m_Health.OnRestoreArmor();
        m_PlayerInfo.OnBattleFinish();
    }

    void OnMainDown(bool down)
    {
        m_aiming = down;

        if (down)
        {
            if (m_Equipment != null)
            {
                OnEquipment();
                return;
            }
            if (m_Interact != null)
            {
                OnInteract();
                return;
            }
        }

        if (m_WeaponCurrent != null)
            m_WeaponCurrent.Trigger(down);
    }
    
    protected override void OnCharacterUpdate(float deltaTime)
    {
        base.OnCharacterUpdate(deltaTime);
        OnWeaponTick(deltaTime);
        OnMoveTick(deltaTime);
        OnAbilityTick(deltaTime);
        OnCommonStatus();
    }

    protected virtual float CalculateMovementSpeedBase() => (F_MovementSpeed - m_WeaponCurrent.m_WeaponInfo.m_Weight);
    protected virtual float CalculateMovementSpeedMultiple()=> m_aimingMovementReduction ? (1 - GameConst.F_AimMovementReduction * m_PlayerInfo.F_AimMovementStrictMultiply) : 1f;
    protected virtual Quaternion CalculateTargetRotation() => CameraController.CameraXZRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);

    #region WeaponControll
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

    protected virtual bool CalculateCanFire() => !Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);
    void OnWeaponTick(float deltaTime)
    {
        if (m_WeaponCurrent == null)
            return;
        tf_WeaponAim.rotation = CalculateTargetRotation();
        bool canFire = CalculateCanFire();
        m_WeaponCurrent.Tick(Time.deltaTime, canFire);
        m_Assist.SetEnable(canFire&&!m_WeaponCurrent.B_Reloading);
    }

    public WeaponBase ObtainWeapon(WeaponBase _weapon)
    {
        WeaponBase previousWeapon = m_WeaponCurrent;

        if (m_WeaponCurrent)
        {
            m_WeaponCurrent.OnDetach();
            m_PlayerInfo.OnDetachWeapon();
        }
        m_WeaponCurrent = _weapon;
        m_WeaponCurrent.OnAttach(this, _weapon.B_AttachLeft ? tf_WeaponHoldLeft : tf_WeaponHoldRight, OnFireAddRecoil, OnReload);
        m_PlayerInfo.OnAttachWeapon(m_WeaponCurrent);
        m_Animator.OnActivate(m_WeaponCurrent.E_Anim);

        if (m_Assist) m_Assist.Recycle();
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(101);
        m_Assist.Play(m_EntityID, tf_WeaponAim, tf_WeaponAim, GameConst.F_AimAssistDistance, GameLayer.Mask.I_All, (Collider collider) => { return GameManager.B_CanHitTarget(collider.Detect(), m_EntityID); });

        OnWeaponStatus();
        return previousWeapon;
    }

    #endregion
    #region CharacterControll
    protected Vector2 m_MoveAxisInput { get; private set; }
    protected Vector2 m_RotateAxisInput{get;private set; }

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
        transform.rotation = Quaternion.Lerp(transform.rotation, CalculateTargetRotation(),deltaTime*GameConst.I_PlayerRotationSmoothParam);

        m_BaseMovementSpeed = CalculateMovementSpeedBase() * CalculateMovementSpeedMultiple();

        float finalMovementSpeed = m_CharacterInfo.F_MovementSpeed;
        m_CharacterController.Move((CalculateMoveDirection(m_MoveAxisInput) * finalMovementSpeed + Vector3.down * GameConst.F_Gravity) * deltaTime);
        m_Animator.SetRun(m_MoveAxisInput, finalMovementSpeed / F_MovementSpeed);
    }

    #endregion
    #region CharacterAbility
    protected void OnAbilityTick(float deltaTime)
    {
        if (m_AbilityCooldowning)
            f_abilityCoolDown -= deltaTime;
    }

    void OnAbilityClick()
    {
        if (m_AbilityCooldowning||m_Health.b_IsDead||!m_PlayerInfo.TryCostEnergy(I_AbilityCost))
            return;
        f_abilityCoolDown = F_AbilityCoolDown;
        OnAbilityTrigger();
    }

    protected virtual void OnAbilityTrigger()
    {
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
    #region Equipment
    public void OnAddupEquipmentUseTime(int times) => m_EquipmentTimes += times;
    public EquipmentBase AcquireEquipment(int actionIndex, Func<DamageDeliverInfo> OnDamageBuff,float throwDistance=10f)
    {
        OnMainDown(false);
        EquipmentBase targetEquipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(actionIndex), this, OnDamageBuff == null ? m_PlayerInfo.GetDamageBuffInfo : OnDamageBuff);
        m_EquipmentTimes = (m_Equipment == null || m_Equipment.I_Index == targetEquipment.I_Index) ? m_EquipmentTimes + 1 : 1;
        m_Equipment = targetEquipment;
        m_EquipmentDistance = throwDistance;
        return m_Equipment;
    }
    public T AcquireEquipment<T>(int actionIndex, Func<DamageDeliverInfo> OnDamageBuff=null) where T : EquipmentBase=> AcquireEquipment(actionIndex,OnDamageBuff) as T;

    void OnEquipment()
    {
        if (m_EquipmentTimes<=0||m_Equipment == null)
            return;

        m_Equipment.Play(this, transform.position + transform.forward * m_EquipmentDistance);
        m_Equipment.OnDeactivate();

        m_EquipmentTimes--;
        if (m_EquipmentTimes <= 0)
            m_Equipment = null;
    }
    #endregion
    #region Action
    public void TestUseAction(int actionIndex,enum_RarityLevel level)
    {
        m_PlayerInfo.TestUseAction(ActionDataManager.CreateAction(actionIndex,level));
    }
    public void UpgradeWeaponPerk(ActionBase invalidPerk)
    {
        if (m_WeaponCurrent.m_WeaponAction == null)
        {
            m_WeaponCurrent.OnSpawn(invalidPerk);
            m_PlayerInfo.OnAttachWeapon(m_WeaponCurrent);
        }
        else
        {
            m_WeaponCurrent.m_WeaponAction.Upgrade();
        }
        OnWeaponStatus();
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
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerAmmoStatus, m_WeaponCurrent);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCommonStatus, this);
    }
    protected void OnInteractStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger( enum_BC_UIStatus.UI_PlayerInteractStatus,m_Interact);
    }
    protected void OnWeaponStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerWeaponStatus, m_WeaponCurrent);
    }
    protected override void OnExpireChange()
    {
        base.OnExpireChange();
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerExpireStatus, m_PlayerInfo);
    }
    protected void OnActionsChange()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerActionStatus, m_PlayerInfo);
    }
    protected override void OnHealthStatus(enum_HealthChangeMessage type)
    {
        base.OnHealthStatus(type);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerHealthStatus, m_Health);
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
            UIManager.Instance.m_CharacterControl.DoBinding(this, OnMovementDelta, OnRotateDelta, OnReloadClick, OnMainDown,OnAbilityClick);
        else
            UIManager.Instance.m_CharacterControl.RemoveBinding();
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
        public void OnActivate(enum_PlayerAnim animIndex) => OnActivate((int)animIndex);
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
