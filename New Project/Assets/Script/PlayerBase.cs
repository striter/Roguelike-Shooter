using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : LivingBase
{
    public override enum_LivingType E_Type => enum_LivingType.Player;
    public override enum_Flags E_Flag => E_CurrentFlag;
    public enum_Flags E_CurrentFlag = enum_Flags.Invalid;
    public float F_MovementSpeed = 1f;
    Transform tf_Head, tf_WeaponAttach, tf_HandHold;
    CharacterController cc_current;
    DetectorPickup m_pickupDetector;
    Action OnPlayerAnimStatusChanged;
    protected PlayerFlashLight m_FlashLight;

    //Knapsack System Temporary
    Dictionary<enum_AmmoType, int> dic_packAmmos = new Dictionary<enum_AmmoType, int>();

    protected HitCheckDynamic m_HoldingItem = null;
    public WeaponPlayer wb_current { get; private set; } = null;
    public bool b_sprint { get; private set; }
    public bool b_aim { get; private set; }
    public bool b_FlashLightOn => m_FlashLight.b_flashLightOn;
    protected override void Awake()
    {
        base.Awake();
        tf_Head = transform.Find("Head");
        tf_WeaponAttach = tf_Head.Find("WeaponAttach");
        tf_HandHold = tf_WeaponAttach.Find("HandHold");
        m_FlashLight = new PlayerFlashLight(tf_WeaponAttach.Find("FlashLight").GetComponent<Light>());
        cc_current = GetComponent<CharacterController>();

        m_pickupDetector = transform.Find("PickupDetector").GetComponent<DetectorPickup>();
        m_pickupDetector.Init(OnPickupDetected);

        b_sprint = false;
        b_aim = false;
        b_sprintDown = false;
        b_aimDown = false;
        //Init Knapsack
        TCommon.TraversalEnum((enum_AmmoType type) => { dic_packAmmos.Add(type, 0); });
    }
    private void Start()
    {
        CameraController.Attach(tf_Head);

        PCInputManager.Instance.AddRotateCheck(OnCameraRotate);
        PCInputManager.Instance.AddMovementCheck(OnMove);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Reload, Reload);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Fire, LeftTrigger);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Aim, Aim);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Sprint, Sprint);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Throw, Throw);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Interact, Interact);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.Jump, Jump);
        PCInputManager.Instance.AddBinding<PlayerBase>(enum_BindingsName.FlashLight, m_FlashLight.OnSwitch);

        OnPlayerHealthStatusChanged();

        OnWeaponInfoChanged();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        PCInputManager.Instance.DoBindingRemoval<PlayerBase>();
    }
    private void Update()
    {
        OnPlayerActionStatus();
        OnPlayerMovement();
        OnPlayerHoldItem();
        m_FlashLight.Update();
        OnPlayerAnimation();
    }
    protected override void OnDead()
    {
        isDead = true;
        Debug.Log("Player Dead");
    }
    public override bool? TakeDamage(float damage, enum_DamageType damageType, LivingBase damageSource)
    {
        bool? takeDamage = base.TakeDamage(damage, damageType,damageSource);
        OnPlayerHealthStatusChanged();
        FPSCameraController.Instance.DoDamageAnimation(GetDamageCameraAnimationParam(damage,damageType));
        return takeDamage;
    }
    Vector3 GetDamageCameraAnimationParam(float damage, enum_DamageType damageType)
    {
        switch (damageType)
        {
            default:
                Debug.LogError("Add More Convertion Here!" + damageType.ToString());
                return Vector3.zero;
            case enum_DamageType.Melee:
                return 2   *damage * (new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)));
            case enum_DamageType.Range:
                return damage * (new Vector3(UnityEngine.Random.Range(-.2f, .2f), UnityEngine.Random.Range(-.5f, .5f), 0));
            case enum_DamageType.DangerZone:
                return Vector3.zero;
        }
    }
    #region MoveMent/CameraRotation
    float f_groundedDetectedCheck;
    float f_downSpeed, f_gravitySpeed;
    public Vector2 v2_deltaXZMovement { get; private set; }
    Vector3 v3_gravity;
    Vector3 v3_slopeSlide = Vector3.zero;
    Vector3 v3_inhertia;
    bool b_sprintDown;
    bool b_aimDown;
    protected float f_playerGroundMoveCheck;
    bool b_leftStep;
    public bool B_IsGrounded { get; private set; }
    public bool B_IsSlopeSliding { get; private set; }
    public bool B_HoldingItem => m_HoldingItem != null;
    void OnMove(Vector2 delta)
    {
        v2_deltaXZMovement = delta;
    }
    void Jump()
    {
        if (Time.time > f_groundedDetectedCheck && B_IsGrounded)
        {
            f_downSpeed = -GameSettings.CF_PlayerJumpSpeed;
            f_groundedDetectedCheck = Time.time + .2f;
        }
    }
    void OnGravity()
    {
        if (B_IsGrounded && Time.time > f_groundedDetectedCheck)
        {
            f_downSpeed = 0f;
            f_gravitySpeed = GameSettings.CF_NormalGravity;
        }
        else
        {
            f_downSpeed += GameSettings.CF_NormalGravity * Time.deltaTime;
            f_gravitySpeed = f_downSpeed;
        }
    }
    void OnPlayerMovement()
    {
        B_IsGrounded = cc_current.isGrounded;

        OnGravity();

        Vector3 v3_inputMovement = (CameraController.CameraXZForward * v2_deltaXZMovement.y + CameraController.CameraXZRightward * v2_deltaXZMovement.x).normalized * F_MovementSpeed *
            MoveSpeedParam() * Time.deltaTime;

        Vector3 v3_direction = Vector3.down * f_gravitySpeed;
        if (B_IsSlopeSliding)
            v3_direction += v3_slopeSlide;

        if (B_IsGrounded)
        {
            v3_inhertia = v3_inputMovement;
            v3_direction += v3_inputMovement;
            f_playerGroundMoveCheck += v3_inputMovement.magnitude;
            if (f_playerGroundMoveCheck > (b_sprint ? GameSettings.CF_StepDistanceSprinting : GameSettings.CF_StepDistanceWalking))
            {
                f_playerGroundMoveCheck -= (b_sprint ? GameSettings.CF_StepDistanceSprinting : GameSettings.CF_StepDistanceWalking);
                b_leftStep = !b_leftStep;
                if (b_sprint)
                    FPSCameraController.Instance.OnSprintAnimation((b_leftStep ? 1 : -1) * GameSettings.CI_SprintCameraRollAnimation);
            }
        }
        else
        {
            v3_direction += v3_inhertia;
            f_playerGroundMoveCheck = 0f;
        }
        cc_current.Move(v3_direction);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float f_hitAngle = Vector3.Angle(hit.normal, Vector3.up);
        B_IsSlopeSliding = f_hitAngle > cc_current.slopeLimit && f_hitAngle <= 90f;
        if (B_IsSlopeSliding)
        {
            v3_slopeSlide.x = ((1f - hit.normal.y) * hit.normal.x) * GameSettings.CF_PlayerSlopeSliderParam;
            v3_slopeSlide.z = ((1f - hit.normal.y) * hit.normal.z) * GameSettings.CF_PlayerSlopeSliderParam;
            v3_slopeSlide *= Time.deltaTime;
        }
    }

    float MoveSpeedParam()
    {
        return b_sprint ? 2f : b_aim ? .7f : 1f;
    }

    void OnCameraRotate(Vector2 delta)
    {
        CameraController.Instance.RotateCamera(delta);
        tf_WeaponAttach.rotation = Quaternion.Lerp(tf_WeaponAttach.rotation, CameraController.CameraRotation, Time.deltaTime * 20);

        tf_WeaponAttach.localPosition = b_aim ? Vector3.zero : new Vector3(0, Mathf.Abs(TCommon.GetAngle180(tf_WeaponAttach.rotation.eulerAngles.x) / 90f / 4f), 0);
    }
    #endregion
    #region PlayerAction/WeaponAction
    public int I_AmmoLeft => wb_current == null ? -1 : dic_packAmmos[wb_current.E_AmmoType];
    public void OnAmmoUsed(int ammoCount)
    {
        dic_packAmmos[wb_current.E_AmmoType] -= ammoCount;
        OnWeaponInfoChanged();
    }

    void LeftTrigger(bool down)
    {
        if (B_HoldingItem)
        {
            m_HoldingItem.StopHold();
            m_HoldingItem.Throw(GameSettings.CI_ThrowForce, FPSCameraController.MainCamera.transform.forward);
            m_HoldingItem = null;
            return;
        }

        if (wb_current == null || B_HoldingItem)
            return;
        if (down)
            wb_current.StopReload();
        if (b_sprint)
        {
            b_sprintDown = false;
            return;
        }

        wb_current.Trigger(down);
    }
    void Reload()
    {
        if (wb_current == null || B_HoldingItem)
            return;

        if (wb_current.StartReload())
        {
            b_sprint = false;
            b_aim = false;
        }
    }
    void Aim(bool down)
    {
        b_aimDown = down;
        if (down)
        {
            if (b_sprint)
                b_sprintDown = false;
            if (wb_current != null)
                wb_current.StopReload();
        }
    }
    void Sprint(bool down)
    {
        b_sprintDown = down;
        if (down)
        {
            if (b_aim)
                b_aimDown = false;
            if (wb_current != null)
                wb_current.StopReload();
        }
    }
    void OnPlayerActionStatus()
    {
        if (B_HoldingItem)
        {
            b_sprint = false;
            b_aim = false;
            return;
        }

        if (b_sprintDown)
        {
            if (!b_sprint && !b_sprint && (wb_current == null || wb_current.B_Actionable))
            {
                b_sprint = true;
            }
        }
        else if (b_sprint)
        {
            b_sprint = false;
        }
        if (b_aimDown)
        {
            if (!b_aim && !b_sprint && (wb_current == null || wb_current.B_Actionable))
            {
                b_aim = true;
                FPSCameraController.Instance.SetZoom(b_aim);
            }
        }
        else if (b_aim)
        {
            b_aim = false;
        }
    }
    void OnPlayerAnimation()
    {
        FPSCameraController.Instance.SetZoom(b_aim);
        if (wb_current != null)
            OnPlayerAnimStatusChanged?.Invoke();
    }
    #endregion
    #region Throw/PickUp/Interact/HoldItem
    void Throw()
    {
        if (wb_current == null)
            return;

        EntityManager.SpawnPickup<PickupWeapon>(wb_current.Detach(), tf_HandHold).Throw(GameSettings.CI_ThrowForce, tf_WeaponAttach.forward, GameSettings.CI_DurationThrowWeaponCanBePickedUpAgain);
        wb_current = null;
        b_aim = false;
        OnWeaponInfoChanged();
        m_pickupDetector.CheckPickups();
    }
    bool OnPickupDetected(PickupBase pickUp)
    {
        switch (pickUp.E_Type)
        {
            default:
                Debug.LogError("Add More Pickup Detect Here");
                break;
            case enum_PickupType.Weapon:
                if (wb_current == null)
                {
                    OnPickupWeapon(pickUp);
                    return true;
                }
                break;
            case enum_PickupType.Ammo:
                {
                    OnPickupAmmo(pickUp);
                    return true;
                }
        }
        return false;
    }

    void OnPickupWeapon(PickupBase pickupWeapon)
    {
        PickupInfoBase pickUp = pickupWeapon.PickUp();
        wb_current = EntityManager.SpawnPlayerWeapon(pickUp.E_PickupType.ToWeaponType(), tf_WeaponAttach);
        OnPlayerAnimStatusChanged = wb_current.Attach(this, pickupWeapon.m_PickUpInfo as PickupInfoWeapon, OnWeaponInfoChanged,OnWeaponHitEnermy);
        OnInteractInfo("You've Pickuped:"+wb_current.E_WeaponType.ToWeaponName());
    }

    void OnPickupAmmo(PickupBase pickUpAmmo)
    {
        PickupInfoAmmo info = (pickUpAmmo.PickUp() as PickupInfoAmmo);
        dic_packAmmos[info.E_Type] += info.I_AmmoCount;
        OnWeaponInfoChanged();
        OnInteractInfo("You've Pickuped:" + info.E_Type.ToString()+" x"+info.I_AmmoCount.ToString());
    }

    void OnWeaponInfoChanged()
    {
        TBroadCaster<enum_BC_PlayerMessage>.Trigger(enum_BC_PlayerMessage.PlayerWeaponStatusChanged, this);
    }
    void OnWeaponHitEnermy(bool targetDead)
    {
        TBroadCaster<enum_BC_PlayerMessage>.Trigger( enum_BC_PlayerMessage.PlayerHitEnermy,targetDead);
    }
    void OnInteractInfo(string interactInfo)
    {
        TBroadCaster<enum_BC_PlayerMessage>.Trigger(enum_BC_PlayerMessage.PlayerInteractInfo, interactInfo);
    }
    void OnPlayerHealthStatusChanged()
    {
        TBroadCaster<enum_BC_PlayerMessage>.Trigger(enum_BC_PlayerMessage.PlayerHealthChanegd, this);
    }
    RaycastHit rh_hitInfo;
    void Interact()
    {
        if (B_HoldingItem)
        {
            m_HoldingItem.StopHold();
            m_HoldingItem = null;
            return;
        }

        Vector3 start = CameraController.MainCamera.transform.position;
        Vector3 end = start + CameraController.MainCamera.transform.forward * 100;

        if (Physics.Raycast(start, end - start, out rh_hitInfo, GameSettings.CI_PlayerInteractRange, GameLayersPhysics.IL_Interact))
        {
            HitCheckBase hit = rh_hitInfo.collider.GetComponent<HitCheckBase>();
            if (hit!=null)
            hit.OnHitCheck(-1, enum_DamageType.Interact, Vector3.one, this);

            if (rh_hitInfo.collider.gameObject.layer == GameLayers.IL_Dynamic)
            {
                m_HoldingItem =(hit as HitCheckDynamic).StartHold();
            }
        }
    }

    void OnPlayerHoldItem()
    {
        if (B_HoldingItem)
        {
            if (!m_HoldingItem.gameObject.activeInHierarchy)
            {
                m_HoldingItem.StopHold();
                m_HoldingItem = null;
                return;
            }

            m_HoldingItem.transform.position = Vector3.Lerp(m_HoldingItem.transform.position, tf_HandHold.position, .3f);
        }
    }
    #endregion
    #region FlashLight
    protected class PlayerFlashLight:SimpleMonoLifetime
    {
        Light m_FlashLight;
        float m_startIntensity;
        float m_switchTime;
        float m_switchTimeCheck;

        public bool b_flashLightOn { get; private set; }
        public PlayerFlashLight(Light light,float switchTime=2f)
        {
            m_FlashLight = light;
            b_flashLightOn = false;
            m_startIntensity = m_FlashLight.intensity;
            m_FlashLight.intensity = b_flashLightOn ? m_startIntensity : 0;
            m_switchTime = switchTime;
        }
        public override void Update()
        {
            base.Update();
            if (Time.time < m_switchTimeCheck+m_switchTime)
            {
                float timeParam = (m_switchTimeCheck + m_switchTime - Time.time) / m_switchTime;
                m_FlashLight.intensity = Mathf.Lerp(m_FlashLight.intensity, b_flashLightOn ? m_startIntensity : 0, timeParam);
            }
        }
        public void OnSwitch()
        {
            b_flashLightOn = !b_flashLightOn;
            m_switchTimeCheck = Time.time;
        }
    }
    #endregion
}
