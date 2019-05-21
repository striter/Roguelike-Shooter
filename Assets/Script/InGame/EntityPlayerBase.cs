using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityPlayerBase : EntityBase {
    public enum_Weapon TESTWEAPON1 = enum_Weapon.M16A4;
    public enum_Weapon TESTWEAPON2 = enum_Weapon.MK10;
    public float m_Coins { get; private set; } = 0;
    public float m_Pitch { get; private set; } = 0;

    Vector2 m_MoveDelta;
    CharacterController m_CharacterController;
    protected Transform tf_WeaponHold;
    public WeaponBase m_WeaponCurrent { get; private set; } = null;
    protected List<WeaponBase> m_WeaponObtained=new List<WeaponBase>();
    public override void Init(int entityID,SEntity entityInfo)
    {
        base.Init(entityID,entityInfo);
        m_CharacterController = GetComponent<CharacterController>();
        tf_WeaponHold = transform.Find("WeaponHold");
    }
    protected override void Start()
    {
        m_Pitch = 0;
        CameraController.Attach(this.transform);

        if (GameManager.Instance.B_TestMode)
        {
            PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
            PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Fire, OnTriggerDown);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Interact, OnSwitchWeapon);
            PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Reload, OnReload);
        }
        else
        {
            UIManager.OnFire = OnTriggerDown;
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
    #region WeaponControll
    void ObtainWeapon(WeaponBase weapon)
    {
        m_WeaponObtained.Add(weapon);
        weapon.Attach(I_EntityID,tf_WeaponHold, OnCostMana, AddRecoil);
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
    #region PlayerMovement
    void OnRotateDelta(Vector2 rotateDelta)
    {
        m_Pitch += (rotateDelta.y/Screen.height)*90f;
        m_Pitch = Mathf.Clamp(m_Pitch, -45, 45);
        rotateDelta.y = 0;
        rotateDelta.x = (rotateDelta.x / Screen.width) * 180f;
        CameraController.Instance.RotateCamera(rotateDelta);
    }
    void OnMovementDelta(Vector2 moveDelta)
    {
        m_MoveDelta = moveDelta*.1f;
    }
    protected override void Update()
    {
        base.Update();
        tf_WeaponHold.localRotation = Quaternion.Euler(-m_Pitch,0,0);
        transform.rotation = Quaternion.Lerp(transform.rotation,CameraController.CameraXZRotation,.1f);
        Vector3 direction = (transform.right * m_MoveDelta.x + transform.forward * m_MoveDelta.y).normalized +Vector3.down*.98f;
        m_CharacterController.Move(direction*Time.deltaTime*m_EntityInfo.m_moveSpeed);

        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PlayerInfoChanged, this);


        if (GameManager.Instance.B_TestMode&&Input.GetKeyDown(KeyCode.Space))
        {
            TryTakeDamage(10);
        }
    }
    public void AddRecoil(Vector2 recoil)
    {
        m_Pitch += recoil.y;
        m_Pitch = Mathf.Clamp(m_Pitch, -45, 45);
        OnRotateDelta(new Vector2(Random.Range(-1f,1f)>0?1f:-1f *recoil.x,0));
    }
#endregion
}
