using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TSpecialClasses;
public class EntityPlayerBase : EntityBase {
    public enum_PlayerWeapon TESTWEAPON1 = enum_PlayerWeapon.M16A4;
    public enum_PlayerWeapon TESTWEAPON2 = enum_PlayerWeapon.MK10;
    public float m_Coins { get; private set; } = 0;
    public float m_Pitch { get; private set; } = 0;

    protected CharacterController m_CharacterController;
    protected PlayerAnimator m_Animator;
    protected Transform tf_WeaponHold;
    protected List<WeaponBase> m_WeaponObtained=new List<WeaponBase>();

    public WeaponBase m_WeaponCurrent { get; private set; } = null;

    public bool B_Interacting => m_InteractTarget != null;
    public InteractBase m_InteractTarget { get; private set; }


    public override Vector3 m_PrecalculatedTargetPos(float time) => tf_Head.position + (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized* m_EntityInfo.m_moveSpeed * time;
    public override void Init(SEntity entityInfo)
    {
        Init( entityInfo, true);
        m_CharacterController = GetComponent<CharacterController>();
        m_CharacterController.detectCollisions = false;
        gameObject.layer = GameLayer.I_MovementDetect;
        tf_WeaponHold = transform.Find("WeaponHold");
        m_Animator = new PlayerAnimator(tf_Model.GetComponent<Animator>());
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
    }
    public override void OnSpawn(int id)
    {
        base.OnSpawn(id);
        m_Pitch = 0;
        CameraController.Attach(this.transform);

        if (GameManager.Instance.B_TestMode)
        {
            PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
            PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Fire, OnMainButtonDown);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Interact, OnSwitchWeapon);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Reload, OnReload);
        }
        else
        {
            UIManager.OnMainDown = OnMainButtonDown;
            UIManager.OnReload = OnReload;
            UIManager.OnSwitch = OnSwitchWeapon;
            TouchDeltaManager.Instance.Bind(OnMovementDelta, OnRotateDelta);
        }
        ObtainWeapon(ObjectManager.SpawnWeapon(TESTWEAPON1, this));
        ObtainWeapon(ObjectManager.SpawnWeapon(TESTWEAPON2, this));
    }
    protected override void OnCostMana(float manaCost)
    {
        float manaMinus = m_CurrentMana - manaCost;
        if (manaMinus <= 0)
        {
            m_Coins += manaMinus;
            manaMinus = m_CurrentMana;
        }
        else
        {
            manaMinus = manaCost;
        }
        base.OnCostMana(manaMinus);
    }
    void OnMainButtonDown(bool down)
    {
        if (down)
        {
            if (m_InteractTarget != null)
                OnInteract(down);
            else
                OnTriggerDown(down);
        }
        else
        {
            if (m_InteractTarget != null)
                OnInteract(down);
            OnTriggerDown(down);
        }
    }
    #region WeaponControll
    void ObtainWeapon(WeaponBase weapon)
    {
        m_WeaponObtained.Add(weapon);
        weapon.Attach(I_EntityID,tf_WeaponHold, OnCostMana, AddRecoil, GetDamageBuffInfo);
        weapon.SetActivate(false);
        if (m_WeaponCurrent == null)
            OnSwitchWeapon();
    }
    void OnTriggerDown(bool down)
    {
        if (m_WeaponCurrent == null)
            return;
        m_WeaponCurrent.Trigger(down);
    }
    void OnReload()
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
    }
    #endregion
    #region PlayerControll
    Vector2 m_MoveAxisInput;
    void OnRotateDelta(Vector2 rotateDelta)
    {
        m_Pitch += (rotateDelta.y/Screen.height)*90f;
        m_Pitch = Mathf.Clamp(m_Pitch, 0, 0);
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        CameraController.Instance.RotateCamera(rotateDelta);
    }
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveAxisInput = moveDelta;
        m_Animator.SetRun(moveDelta.magnitude > .2f);
    }
    protected override void Update()
    {
        base.Update();
        m_WeaponCurrent.SetCanFire(!Physics.SphereCast(new Ray(tf_WeaponHold.position, tf_WeaponHold.forward), .1f,1f   , GameLayer.Physics.I_Static));
        tf_WeaponHold.localRotation = Quaternion.Euler(-m_Pitch,0,0);
        transform.rotation = Quaternion.Lerp(transform.rotation,CameraController.CameraXZRotation,GameConst.F_PlayerCameraSmoothParam);
        Vector3 direction = (transform.right * m_MoveAxisInput.x + transform.forward * m_MoveAxisInput.y).normalized;
        m_CharacterController.Move(direction*m_EntityInfo.m_moveSpeed * Time.deltaTime + Vector3.down * GameConst.F_PlayerFallSpeed*Time.deltaTime);
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerInfoChanged, this);
    }
    public void AddRecoil(Vector2 recoil)
    {
        m_Pitch += recoil.y;
        m_Pitch = Mathf.Clamp(m_Pitch, 0, 0);
        OnRotateDelta(new Vector2(Random.Range(-1f,1f)>0?1f:-1f *recoil.x,0));
    }
    #endregion
    #region PlayerInteract
    public void OnInteract(bool down)
    {
        if (m_InteractTarget == null)
        {
            Debug.LogError("Can't Interact With Null Target!");
            return;
        }

        if (down && m_InteractTarget.TryInteract())
            m_InteractTarget = null;
    }

    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (isEnter)
            m_InteractTarget = interactTarget;
        else if (m_InteractTarget = interactTarget)
            m_InteractTarget = null;
    }
    #endregion

    protected class PlayerAnimator : AnimatorClippingTime
    {
        static readonly int HS_B_Run = Animator.StringToHash("b_run");
        public PlayerAnimator(Animator _animator) : base(_animator)
        {

        }
        public void SetRun(bool run)
        {
            m_Animator.SetBool(HS_B_Run,run);
        }
    }
#if UNITY_EDITOR
    CapsuleCollider hitBox;
    private void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying && !GameManager.Instance.B_GizmosInGame)
            return;

        if (!hitBox)
            hitBox = transform.Find("Model").GetComponent<CapsuleCollider>();
        Gizmos.color = Color.green;
        Gizmos_Extend.DrawWireCapsule(transform.position+transform.up*hitBox.height/2*hitBox.transform.localScale.y,Quaternion.LookRotation(transform.forward,transform.up), hitBox.transform.localScale, hitBox.radius,hitBox.height);
    }
#endif
}
