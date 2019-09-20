using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class EntityCharacterPlayer : EntityCharacterBase {
    public override enum_EntityController m_Controller => enum_EntityController.Player;
    protected CharacterController m_CharacterController;
    protected PlayerAnimator m_Animator;
    protected Transform tf_WeaponHoldRight,tf_WeaponHoldLeft;
    protected SFXAimAssist m_Assist = null;
    public WeaponBase m_WeaponCurrent { get; private set; } = null;
    public InteractBase m_Interact { get; private set; }
    public EquipmentBase m_Equipment { get; private set; }
    public override float m_baseMovementSpeed => F_MovementSpeed*(f_movementReductionCheck >0? GameConst.F_FireMovementReduction:1f);
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized* m_CharacterInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    public EntityHealth m_PlayerHealth { get; private set; }
    protected bool m_aiming = false;
    protected float f_movementReductionCheck = 0f;
    
    protected override HealthBase GetHealthManager()
    {
        m_PlayerHealth = new EntityHealth(this, OnHealthChanged, OnDead);
        return m_PlayerHealth;
    }
    protected override void ActivateHealthManager() => m_PlayerHealth.OnActivate(I_MaxHealth, I_DefaultArmor, true);
    protected override CharacterInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, OnReceiveDamage, OnExpireChange,OnActionsChange);
        return m_PlayerInfo;
    }
    public override void Init(int poolPresetIndex)
    {
        base.Init(poolPresetIndex);
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterController.detectCollisions = false;
        gameObject.layer = GameLayer.I_MovementDetect;
        tf_WeaponHoldRight = transform.FindInAllChild("WeaponHold_R");
        tf_WeaponHoldLeft = transform.FindInAllChild("WeaponHold_L");
        m_Animator = new PlayerAnimator(tf_Model.GetComponent<Animator>());
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
        
    }
    public void SetPlayerInfo(int coins, List<ActionBase> storedActions)
    {
        m_PlayerInfo.OnCoinsReceive(coins);
        m_PlayerInfo.InitActionInfo(storedActions);
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        CameraController.Attach(this.transform);
        SetBinding(true);
    }
    protected override void OnDead()
    {
        base.OnDead();
        if (m_Assist)
            m_Assist.ForceRecycle();
        if (m_WeaponCurrent)
            m_WeaponCurrent.OnDetach();

        m_Animator.OnDead();
        m_MoveAxisInput = Vector2.zero;
        SetBinding(false);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        SetBinding(false);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);;
    }
    void OnChangeLevel()
    {
        m_Interact = null;
    } 
    void OnBattleStart()
    {
        m_PlayerInfo.OnBattleStart();
    }
    void OnBattleFinish()
    {
        m_Equipment = null;
        m_PlayerInfo.OnBattleFinish();
        m_PlayerHealth.OnActivate(I_MaxHealth, I_DefaultArmor,false);
    }

    void OnMainButtonDown(bool down)
    {
        m_aiming = down;
        if (m_WeaponCurrent != null)
            m_WeaponCurrent.Trigger(down);


        if (m_Equipment != null)
        {
            OnEquipment(down);
            return;
        }

        if (m_Interact != null)
        {
            OnInteract(down);
            return;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (m_PlayerHealth.b_IsDead)
            return;
        

        bool canFire = !Physics.SphereCast(new Ray(tf_Head.position, tf_Head.forward), .3f, 1.5f, GameLayer.Mask.I_Static);
        m_WeaponCurrent.Tick(Time.deltaTime, canFire);
        m_Assist.SetEnable(canFire);

        if(f_movementReductionCheck>0) f_movementReductionCheck -= Time.deltaTime;
        if (m_aiming) f_movementReductionCheck=GameConst.F_MovementReductionDuration;

        transform.rotation = Quaternion.Lerp(transform.rotation, CameraController.CameraXZRotation, GameConst.F_PlayerCameraSmoothParam);

        Vector3 moveDirection = (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized;
        float movementSpeed = m_CharacterInfo.F_MovementSpeed;
        m_CharacterController.Move((moveDirection * movementSpeed+Vector3.down*GameConst.F_Gravity)*Time.deltaTime);
        m_Animator.SetRun(m_MoveAxisInput, movementSpeed / F_MovementSpeed);

        OnCommonStatus();
    }
    #region WeaponControll
    void OnReloadDown()
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
        m_Animator.SwitchAnim(m_WeaponCurrent.E_Anim);

        if (m_Assist) m_Assist.ForceRecycle();
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(01);
        m_Assist.Play(I_EntityID, tf_Head, tf_Head, GameConst.F_AimAssistDistance, GameLayer.Mask.I_All, (Collider collider) => { return GameManager.B_CanHitTarget(collider.Detect(), I_EntityID); });

        OnWeaponStatus();
        return previousWeapon;
    }
#endregion
    #region PlayerControll
    Vector2 m_MoveAxisInput;
    void OnMovementDelta(Vector2 moveDelta) => m_MoveAxisInput = moveDelta;
    void OnRotateDelta(Vector2 rotateDelta)
    {
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        CameraController.Instance.RotateCamera(rotateDelta);
    }
    public void OnFireAddRecoil(Vector2 recoil)
    {
        OnRotateDelta(new Vector2((TCommon.RandomBool()?1:-1) *recoil.x,0));
        m_Animator.Fire();
    }
    #endregion
    #region PlayerInteract
    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (!interactTarget.B_Interactable)
            return;

        if (interactTarget.B_InteractOnTrigger)
        {
            interactTarget.TryInteract(this);
            return;
        }

        if (isEnter)
            m_Interact = interactTarget;
        else if (m_Interact == interactTarget)
            m_Interact = null;
    }
    public void OnInteract(bool down)
    {
        if (m_Interact == null)
        {
            Debug.LogError("Can't Interact With Null Target!");
            return;
        }

        if (down && m_Interact.TryInteract(this)&&!m_Interact.B_Interactable)
            m_Interact = null;
    }
    #endregion
    #region Equipment
    public void OnUseEquipment<T>(int actionIndex, Action<T> OnItemCreated) where T:EquipmentBase
    {
        T target = EquipmentBase.AcquireEquipment(actionIndex * 10, this, tf_WeaponHoldLeft, m_PlayerInfo.GetDamageBuffInfo) as T;
        OnItemCreated?.Invoke(target);
        target.Play(this, transform.position + transform.forward * 10);
    }
    public T OnAcquireEquipment<T>(int actionIndex, Func<DamageDeliverInfo> OnDamageBuff=null) where T : EquipmentBase
    {
        OnMainButtonDown(false);
        m_Equipment = EquipmentBase.AcquireEquipment(actionIndex * 10, this, tf_WeaponHoldLeft, OnDamageBuff==null?m_PlayerInfo.GetDamageBuffInfo:OnDamageBuff);
        return m_Equipment as T;
    }

    void OnEquipment(bool down)
    {
        if (!down || m_Equipment == null)
            return;

        m_Equipment.Play(this, transform.position + transform.forward * 10);
        m_Equipment.OnDeactivate();
        m_Equipment = null;
    }
    #endregion
    #region Action
    public void TestUseAction(int actionIndex)
    {
        m_PlayerInfo.OnUseAcion(GameDataManager.CreateAction(actionIndex, enum_RarityLevel.L3));
    }
    #endregion

    #region UI Indicator
    protected void OnCommonStatus()
    {
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerAmmoStatus, m_WeaponCurrent);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerCommonStatus, this);
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
    protected override void OnHealthChanged(enum_HealthChangeMessage type)
    {
        base.OnHealthChanged(type);
        TBroadCaster<enum_BC_UIStatus>.Trigger(enum_BC_UIStatus.UI_PlayerHealthStatus, m_PlayerHealth);
    }
    #endregion

    void SetBinding(bool on)
    {
        if (on)
        {
#if UNITY_EDITOR
            PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
            PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
            PCInputManager.Instance.AddBinding<EntityCharacterPlayer>(enum_BindingsName.Fire, OnMainButtonDown);
            PCInputManager.Instance.AddBinding<EntityCharacterPlayer>(enum_BindingsName.Reload, OnReloadDown);
#else
        UIManager.OnMainDown = OnMainButtonDown;
        UIManager.OnReload = OnReloadDown;
        TouchDeltaManager.Instance.Bind(OnMovementDelta, OnRotateDelta);
#endif
            return;
        }
#if UNITY_EDITOR
        PCInputManager.Instance.DoBindingRemoval<EntityCharacterPlayer>();
        PCInputManager.Instance.RemoveMovementCheck();
        PCInputManager.Instance.RemoveRotateCheck();
#else
        UIManager.OnMainDown = null;
        UIManager.OnReload = null;
        TouchDeltaManager.Instance.Bind(null, null);
#endif
    }


    protected class PlayerAnimator : AnimatorClippingTime
    {
        static readonly int HS_F_Forward = Animator.StringToHash("f_forward");
        static readonly int HS_F_Strafe = Animator.StringToHash("f_strafe");
        static readonly int HS_I_WeaponType = Animator.StringToHash("i_weaponType");
        static readonly int HS_T_Activate = Animator.StringToHash("t_activate");
        static readonly int HS_T_Fire = Animator.StringToHash("t_attack");
        static readonly int HS_T_Dead = Animator.StringToHash("t_dead");
        static readonly int HS_T_Reload = Animator.StringToHash("t_reload");
        static readonly int HS_FM_Reload = Animator.StringToHash("fm_reload");
        static readonly int HS_FM_Movement = Animator.StringToHash("fm_movement");
        public PlayerAnimator(Animator _animator) : base(_animator)
        {
            _animator.fireEvents = true;
        }
        public void SetRun(Vector2 movement,float movementParam)
        {
            m_Animator.SetFloat(HS_F_Forward, movement.y);
            m_Animator.SetFloat(HS_F_Strafe, movement.x);
            m_Animator.SetFloat(HS_FM_Movement, movementParam);
        }
        public void SwitchAnim(enum_PlayerAnim animIndex)
        {
            m_Animator.SetInteger(HS_I_WeaponType, (int)animIndex);
            m_Animator.SetTrigger(HS_T_Activate);
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
        public void OnDead()
        {
            m_Animator.SetTrigger(HS_T_Dead);
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
