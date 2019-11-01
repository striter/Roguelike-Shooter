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
    public int m_EquipmentTimes { get; private set; }
    public float m_EquipmentDistance { get; private set; }
    public override Transform tf_Weapon => m_WeaponCurrent.m_Case;
    public override float m_baseMovementSpeed => F_MovementSpeed*( f_movementReductionCheck >0? (1-GameConst.F_AimMovementReduction*m_PlayerInfo.F_AimMovementStrictMultiply):1f);
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized* m_CharacterInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    protected bool m_aiming = false;
    protected float f_movementReductionCheck = 0f;
    protected override enum_GameAudioSFX m_DamageClip => enum_GameAudioSFX.PlayerDamage;
    protected override void ActivateHealthManager(float maxHealth) => m_Health.OnActivate(maxHealth, I_DefaultArmor, true);
    protected override CharacterInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, m_HitCheck.TryHit, OnExpireChange,OnActionsChange);
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
        m_Animator = new PlayerAnimator(tf_Model.GetComponent<Animator>(), OnAnimationEvent);
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        SetBinding(true);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleStart, OnBattleStart);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnBattleFinish, OnBattleFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnChangeLevel, OnChangeLevel); 
        SetBinding(false);
    }
   
    public void SetPlayerInfo(int coins, List<ActionBase> storedActions)
    {
        m_PlayerInfo.OnCoinsReceive(coins);
        m_PlayerInfo.InitActionInfo(storedActions);
    }

    protected override void OnDead()
    {
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

    void OnMainButtonDown(bool down)
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

    protected override void Update()
    {
        base.Update();
        if (m_Health.b_IsDead)
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
        m_Animator.OnActivate(m_WeaponCurrent.E_Anim);

        if (m_Assist) m_Assist.Recycle();
        m_Assist = GameObjectManager.SpawnSFX<SFXAimAssist>(101);
        m_Assist.Play(m_EntityID, tf_Head, tf_Head, GameConst.F_AimAssistDistance, GameLayer.Mask.I_All, (Collider collider) => { return GameManager.B_CanHitTarget(collider.Detect(), m_EntityID); });

        OnWeaponStatus();
        return previousWeapon;
    }
#endregion
    #region PlayerControll
    Vector2 m_MoveAxisInput;
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
    }

    void OnRotateDelta(Vector2 rotateDelta)
    {
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        TPSCameraController.Instance.RotateCamera(rotateDelta *OptionsManager.m_Sensitive);
    }

    public void OnFireAddRecoil(float recoil)
    {
        TPSCameraController.Instance.AddRecoil(new Vector3(0, (TCommon.RandomBool() ? 1 : -1)* recoil,0));
        m_Animator.Fire();
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
            m_Interact = interactTarget;
        else if (m_Interact == interactTarget)
            m_Interact = null;
    }
    public void OnInteract()
    {
        if (m_Interact == null)
        {
            Debug.LogError("Can't Interact With Null Target!");
            return;
        }

        if (m_Interact.TryInteract(this)&&!m_Interact.B_InteractEnable)
            m_Interact = null;
    }
    #endregion
    #region Equipment
    public void OnAddupEquipmentUseTime(int times) => m_EquipmentTimes += times;
    public EquipmentBase AcquireEquipment(int actionIndex, Func<DamageDeliverInfo> OnDamageBuff,float throwDistance=10f)
    {
        OnMainButtonDown(false);
        EquipmentBase targetEquipment = EquipmentBase.AcquireEquipment(GameExpression.GetPlayerEquipmentIndex(actionIndex), this, tf_WeaponHoldLeft, OnDamageBuff == null ? m_PlayerInfo.GetDamageBuffInfo : OnDamageBuff);
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
        m_PlayerInfo.TestUseAction(GameDataManager.CreateAction(actionIndex,level));
    }
    public void UpgradeWeaponPerk(ActionBase invalidPerk)
    {
        if (m_WeaponCurrent.m_WeaponAction == null)
        {
            m_WeaponCurrent.OnSpawn(invalidPerk);
            m_PlayerInfo.OnAttachWeapon(m_WeaponCurrent);
        }
        m_WeaponCurrent.m_WeaponAction.Upgrade();
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
            UIManager.Instance.DoBinding(OnMovementDelta, OnRotateDelta, OnReloadDown, OnMainButtonDown);
        else
            UIManager.Instance.RemoveBinding();
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
