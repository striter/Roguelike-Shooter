using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
using System;

public class EntityPlayerBase : EntityBase {
    public enum_PlayerWeapon TESTWEAPON1 = enum_PlayerWeapon.M16A4;
    public enum_PlayerWeapon TESTWEAPON2 = enum_PlayerWeapon.M82A1;
    public override bool B_IsPlayer => true;

    public float m_Coins { get; private set; } = 0;
    protected CharacterController m_CharacterController;
    protected PlayerAnimator m_Animator;
    protected Transform tf_WeaponHoldRight,tf_WeaponHoldLeft;
    protected List<WeaponBase> m_WeaponObtained=new List<WeaponBase>();
    protected SFXAimAssist m_Assist = null;
    public WeaponBase m_WeaponCurrent { get; private set; } = null;
    public InteractBase m_Interact { get; private set; }
    public EquipmentBase m_Equipment { get; private set; }
    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized* m_EntityInfo.F_MovementSpeed * time;
    public PlayerInfoManager m_PlayerInfo { get; private set; }
    protected override EntityInfoManager GetEntityInfo()
    {
        m_PlayerInfo = new PlayerInfoManager(this, OnReceiveDamage, OnInfoChange);
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
    public override void OnSpawn(int id,enum_EntityFlag _flag)
    {
        base.OnSpawn(id, enum_EntityFlag.Player);
        CameraController.Attach(this.transform);

        ObtainWeapon(ObjectManager.SpawnWeapon(TESTWEAPON1, this));
        ObtainWeapon(ObjectManager.SpawnWeapon(TESTWEAPON2, this));
#if UNITY_EDITOR
        PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
        PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
        PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Fire, OnMainButtonDown);
        PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Interact, OnSwitchWeapon);
        PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Reload, OnReloadDown);
#else
        UIManager.OnMainDown = OnMainButtonDown;
        UIManager.OnReload = OnReloadDown;
        UIManager.OnSwitch = OnSwitchWeapon;
        TouchDeltaManager.Instance.Bind(OnMovementDelta, OnRotateDelta);
#endif
    }
    protected override void OnDead()
    {
        base.OnDead();
        if (m_Assist)
            m_Assist.ForceRecycle();
        if (m_WeaponCurrent)
            m_WeaponCurrent.Detach();

        m_Animator.OnDead();
        m_MoveAxisInput = Vector2.zero;
        RemoveBinding();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        RemoveBinding();
    }
    void RemoveBinding()
    {
#if UNITY_EDITOR
        PCInputManager.Instance.DoBindingRemoval<EntityPlayerBase>();
        PCInputManager.Instance.RemoveMovementCheck();
        PCInputManager.Instance.RemoveRotateCheck();
#else
        UIManager.OnMainDown = null;
        UIManager.OnReload = null;
        UIManager.OnSwitch = null;
        TouchDeltaManager.Instance.Bind(null, null);
#endif
    }

    void OnMainButtonDown(bool down)
    {
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
        if (m_WeaponCurrent != null)
            m_WeaponCurrent.Trigger(down);
    }
#region WeaponControll
    void ObtainWeapon(WeaponBase weapon)
    {
        m_WeaponObtained.Add(weapon);
        weapon.Attach(I_EntityID,this,weapon.B_AttachLeft?tf_WeaponHoldLeft:tf_WeaponHoldRight, OnFireAddRecoil,m_Animator.Reload, m_EntityInfo.GetDamageBuffInfo, m_EntityInfo.F_FireRateTick,m_EntityInfo.F_ReloadRateTick);
        weapon.SetActivate(false);
        if (m_WeaponCurrent == null)
            OnSwitchWeapon();
    }
    void OnReloadDown()
    {
        if (m_WeaponCurrent == null)
            return;
        m_WeaponCurrent.TryReload();
    }
    void OnSwitchWeapon()
    {
        if (m_WeaponCurrent == null)
        {
            if (m_WeaponObtained != null)
                m_WeaponCurrent = m_WeaponObtained[0];
        }
        else
        {
            int index = m_WeaponObtained.IndexOf(m_WeaponCurrent);
            index++;
            if (index == m_WeaponObtained.Count)
                index = 0;

            m_WeaponCurrent.SetActivate(false);
            m_WeaponCurrent = m_WeaponObtained[index];
        }
        m_WeaponCurrent.SetActivate(true);

        if (m_Assist)
            m_Assist.ForceRecycle();

        m_Assist = ObjectManager.SpawnSFX<SFXAimAssist>(01);
        m_Assist.Play(I_EntityID,tf_Head, tf_Head, GameConst.F_AimAssistDistance,GameLayer.Mask.I_All,(Collider collider)=> {return GameManager.B_DoHitCheck(collider.Detect(),I_EntityID); });

        m_Animator.SwitchAnim(m_WeaponCurrent.E_Anim);
    }
#endregion
#region PlayerControll
    Vector2 m_MoveAxisInput;
    void OnRotateDelta(Vector2 rotateDelta)
    {
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        CameraController.Instance.RotateCamera(rotateDelta);
    }
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
        m_Animator.SetRun(m_MoveAxisInput);
    }
    protected override void Update()
    {
        base.Update();
        bool canFire = !Physics.SphereCast(new Ray(tf_Head.position, tf_Head.forward), .3f, 1.5f, GameLayer.Mask.I_Static);
        m_WeaponCurrent.SetCanFire(canFire);
        m_Assist.SetEnable(canFire);
        transform.rotation = Quaternion.Lerp(transform.rotation,CameraController.CameraXZRotation,GameConst.F_PlayerCameraSmoothParam);
        Vector3 direction = (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized;
        m_CharacterController.Move(direction*m_EntityInfo.F_MovementSpeed * Time.deltaTime + Vector3.down * GameConst.F_PlayerFallSpeed*Time.deltaTime);
        TBroadCaster<enum_BC_GameStatusChanged>.Trigger(enum_BC_GameStatusChanged.PlayerInfoChanged, this);
    }
    public void OnFireAddRecoil(Vector2 recoil)
    {
        OnRotateDelta(new Vector2(TCommon.RandomBool()?1:-1 *recoil.x,0));
        m_Animator.Fire();
    }
    #endregion
    #region PlayerInteract/Equipment

    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (isEnter)
            m_Interact = interactTarget;
        else if (m_Interact = interactTarget)
            m_Interact = null;
    }
    public void OnInteract(bool down)
    {
        if (m_Interact == null)
        {
            Debug.LogError("Can't Interact With Null Target!");
            return;
        }

        if (down && m_Interact.TryInteract())
            m_Interact = null;
    }

    public T OnAcquireEquipment<T>(int actionIndex, Func<DamageBuffInfo> OnDamageBuff) where T:EquipmentBase
    {
        m_Equipment = EquipmentBase.AcquireEquipment(actionIndex*10, this, tf_WeaponHoldLeft, OnDamageBuff, OnDead);
        return m_Equipment as T;
    }

    void OnEquipment(bool down)
    {
        if (!down || m_Equipment == null)
            return;

        m_Equipment.Play(null,transform.position+transform.forward*10);
        m_Equipment.OnDeactivate();
        m_Equipment = null;
    }
    #endregion

    public void TestUseAction(int actionIndex)
    {
        m_PlayerInfo.TestUseAction(actionIndex);
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
        public PlayerAnimator(Animator _animator) : base(_animator)
        {
            _animator.fireEvents = true;
        }
        public void SetRun(Vector2 movement)
        {
            m_Animator.SetFloat(HS_F_Forward, movement.y);
            m_Animator.SetFloat(HS_F_Strafe, movement.x);
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
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_PhysicsDebugGizmos)
            return;

        if (!hitBox)
            hitBox = transform.GetComponentInChildren<CapsuleCollider>();
        Gizmos.color = Color.green;
        Gizmos_Extend.DrawWireCapsule(transform.position+transform.up*hitBox.height/2*hitBox.transform.localScale.y,Quaternion.LookRotation(transform.forward,transform.up), hitBox.transform.localScale, hitBox.radius,hitBox.height);
    }
#endif
}
