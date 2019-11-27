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
    protected Transform tf_WeaponHoldRight, tf_WeaponHoldLeft;
    protected SFXAimAssist m_Assist = null;
    public WeaponBase m_WeaponCurrent { get; private set; } = null;
    public InteractBase m_Interact { get; private set; }
    public float m_EquipmentDistance { get; private set; }
    public Transform tf_UIStatus { get; private set; }
    public override Transform tf_Weapon => m_WeaponCurrent.m_Muzzle;
    public override Transform tf_WeaponModel => m_WeaponCurrent.m_Case;
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized * m_CharacterInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    protected override void ActivateHealthManager(float maxHealth) => m_Health.OnActivate(maxHealth, I_DefaultArmor, true);
    protected bool m_aiming = false;
    protected override enum_GameAudioSFX m_DamageClip => enum_GameAudioSFX.PlayerDamage;

    protected float f_aimMovementReduction = 0f;
    protected bool m_aimingMovementReduction => f_aimMovementReduction > 0f;
    protected float m_BaseMovementSpeed;
    public override float m_baseMovementSpeed => m_BaseMovementSpeed;
    protected float f_reviveCheck = 0f;
    public CharacterAbility m_Ability { get; private set; }

    protected override CharacterInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, m_HitCheck.TryHit, OnExpireChange,OnExpireListChange, OnBattleActionsChange);
        return m_PlayerInfo;
    }

    public override void OnPoolItemInit(int _identity, Action<int, MonoBehaviour> _OnRecycle)
    {
        base.OnPoolItemInit(_identity, _OnRecycle);
        gameObject.layer = GameLayer.I_MovementDetect;
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterController.detectCollisions = false;
        tf_WeaponAim = transform.Find("WeaponAim");
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
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        SetBinding(true);
    }
    protected override void OnPoolItemDisable()
    {
        base.OnPoolItemDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
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
        f_reviveCheck = GameConst.F_PlayerReviveCheckAfterDead;
        m_Animator.OnDead();
        m_MoveAxisInput = Vector2.zero;
        m_Assist.SetEnable(false);
        base.OnDead();
    }
    protected override void OnRevive()
    {
        base.OnRevive();
        m_Ability.SetEnable(GameManager.Instance.m_GameLevel.m_LevelType == enum_TileType.Battle);      //?
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
        m_Ability.SetEnable(GameManager.Instance.m_GameLevel.m_LevelType== enum_TileType.Battle);       //?
    }

    protected override void OnBattleFinish()
    {
        base.OnBattleFinish();
        m_Health.OnRestoreArmor();
        m_PlayerInfo.OnBattleFinish();
        m_Ability.SetEnable(false);
    }

    void OnMainDown(bool down)
    {
        m_aiming = down;

        if (down)
        {
            if (m_PlayerInfo.TryUseDevice())
                return;

            if (m_Interact != null)
            {
                OnInteract();
                return;
            }
        }

        if (m_WeaponCurrent != null)
            m_WeaponCurrent.Trigger(down);
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
    protected virtual Quaternion CalculateTargetRotation() => CameraController.CameraXZRotation;
    protected virtual Vector3 CalculateMoveDirection(Vector2 axisInput) => Vector3.Normalize(CameraController.CameraXZRightward * axisInput.x + CameraController.CameraXZForward * axisInput.y);
    protected virtual bool CalculateWeaponFire() => !Physics.SphereCast(new Ray(tf_WeaponAim.position, tf_WeaponAim.forward), .3f, 1.5f, GameLayer.Mask.I_Static);

    #region WeaponControll
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
        tf_WeaponAim.rotation = CalculateTargetRotation();
        m_Assist.SetEnable(m_weaponCanFire && !m_WeaponCurrent.B_Reloading);
        m_WeaponCurrent.AmmoTick(m_PlayerInfo.F_ReloadRateTick(deltaTime));
        if (m_weaponCanFire)
            m_WeaponCurrent.FireTick(m_PlayerInfo.F_FireRateTick( deltaTime));
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
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(101,tf_WeaponAim.position,tf_Weapon.forward);
        m_Assist.Play(m_EntityID, tf_WeaponAim, tf_WeaponAim, GameConst.F_AimAssistDistance, GameLayer.Mask.I_All, (Collider collider) => { return GameManager.B_CanSFXHitTarget(collider.Detect(), m_EntityID); });

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
    public void OnAbilityClick()
    {
        if (m_Health.b_IsDead)
            return;
        m_Ability.OnAbilityClick();
    }
    
    protected virtual void OnAbilityTrigger()
    {
    }

    public class CharacterAbility
    {
        public int m_AbilityTimes { get; private set; } = -1;
        public float m_AbilityCoolDownScale => m_abilityCooldownLeft / m_baseAbilityCooldown;
        public bool m_AbilityCooldowning => m_abilityCooldownLeft > 0f;
        public bool m_AbilityRunsOut => m_AbilityRunsOutable&&m_AbilityTimes == 0;
        public bool m_AbilityRunsOutable => m_baseAbilityTimes > 0;
        public bool m_AbilityEnable { get; private set; } = false;
        protected float m_abilityCooldownLeft = 0f;

        protected float m_baseAbilityCooldown;
        protected int m_baseAbilityTimes;
        Action OnAbilityTrigger;
        public CharacterAbility(int abilityTime, float abilityCoolDown, Action _OnAbilityTrigger)
        {
            OnAbilityTrigger = _OnAbilityTrigger;
            m_baseAbilityCooldown = abilityCoolDown;
            m_baseAbilityTimes = abilityTime;
            SetEnable(false);
        }

        public void SetEnable(bool enable)
        {
            m_AbilityEnable = enable;
            m_abilityCooldownLeft = 0;
            m_AbilityTimes = m_AbilityRunsOutable ? m_baseAbilityTimes : -1;
        }

        public void OnAbilityClick()
        {
            if (!m_AbilityEnable||m_AbilityCooldowning || m_AbilityRunsOut)
                return;
            m_abilityCooldownLeft = m_baseAbilityCooldown;
            m_AbilityTimes--;
            OnAbilityTrigger();
        }

        public void Tick(float deltaTime)
        {
            if (m_AbilityCooldowning)
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
    protected void OnExpireListChange()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerExpireListStatus, m_PlayerInfo);
    }
    protected void OnBattleActionsChange()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerBattleActionStatus, m_PlayerInfo);
    }
    protected override void OnHealthStatus(enum_HealthChangeMessage type)
    {
        base.OnHealthStatus(type);
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
        m_PlayerReviveAmount = new RangeFloat(m_Health.m_MaxHealth, m_Health.m_DefaultArmor);
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
