using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class EntityPlayerBase : EntityBase {
    Vector2 m_MoveDelta;
    float m_Pitch;
    CharacterController m_CharacterController;
    protected Transform tf_WeaponHold;
    protected WeaponBase m_WeaponCurrent = null;
    protected List<WeaponBase> m_WeaponObtained=new List<WeaponBase>();
    public override void Init(SEntity entityInfo)
    {
        base.Init(entityInfo);
        m_CharacterController = GetComponent<CharacterController>();
        tf_WeaponHold = transform.Find("WeaponHold");
    }
    protected override void Start()
    {
        m_Pitch = 0;
        CameraController.Attach(this.transform);

        //Test

        //PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
        //PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
        //PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Fire, OnTriggerDown);
        //PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Interact, OnSwitchWeapon);
        //PCInputManager.Instance.AddBinding<EntityPlayerBase>(enum_BindingsName.Reload, OnReload);

        UIManager.OnFire = OnTriggerDown;
        UIManager.OnReload = OnReload;
        UIManager.OnSwitch = OnSwitchWeapon;
        TouchDeltaManager.Instance.Bind(OnMovementDelta,OnRotateDelta) ;

        ObtainWeapon(ObjectManager.SpawnWeapon(enum_Weapon.Rifle, this));
        ObtainWeapon(ObjectManager.SpawnWeapon(enum_Weapon.SnipeRifle, this));
    }
    void ObtainWeapon(WeaponBase weapon)
    {
        m_WeaponObtained.Add(weapon);
        weapon.Attach(tf_WeaponHold, OnAmmoInfoChanged, AddRecoil);
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
        OnAmmoInfoChanged();
    }
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
    private void Update()
    {
        tf_WeaponHold.localRotation = Quaternion.Euler(-m_Pitch,0,0);
        OnPitchInfoChanged();
        transform.rotation = Quaternion.Lerp(transform.rotation,CameraController.CameraXZRotation,.1f);
        Vector3 direction = (transform.right * m_MoveDelta.x + transform.forward * m_MoveDelta.y).normalized;
        m_CharacterController.Move(direction*Time.deltaTime*m_EntityInfo.m_moveSpeed);
    }
    public void AddRecoil(Vector2 recoil)
    {
        m_Pitch += recoil.y;
        m_Pitch = Mathf.Clamp(m_Pitch, -45, 45);
        OnRotateDelta(new Vector2(Random.Range(-1f,1f)>0?1f:-1f *recoil.x,0));
    }
    #endregion


    void OnAmmoInfoChanged()
    {
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.AmmoLeftChanged,m_WeaponCurrent==null?-1:m_WeaponCurrent.I_AmmoLeft);
    }
    void OnPitchInfoChanged()
    {
        TBroadCaster<enum_BC_UIStatusChanged>.Trigger(enum_BC_UIStatusChanged.PitchChanged, m_Pitch);
    }
}
