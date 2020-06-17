using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using System;

public class InteractPickupWeapon : InteractPickup {
    Transform tf_ModelContainer;
    int m_storageNumber = -1;
    public WeaponBase m_Weapon { get; private set; }
    public override enum_Interaction m_InteractType => enum_Interaction.PickupWeapon;
    protected override bool OnTryInteractCheck(EntityCharacterPlayer _interactor) => base.OnTryInteractCheck(_interactor)&& m_Weapon != null;
    protected override bool B_SelfRecycleOnInteract => false;
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        tf_ModelContainer = transform.Find("Container/Model");
    }
    public InteractPickupWeapon Play(WeaponSaveData data,int storageNumber = -1)
    {
        base.Play();
        m_storageNumber = storageNumber;
        m_Weapon = GameObjectManager.SpawnWeapon(data,tf_ModelContainer);
        m_Weapon.transform.localPosition = Vector3.zero;
        m_Weapon.transform.localRotation = Quaternion.identity;
        if (m_visualizeItem != null)
        {
            m_visualizeItem.Play(TLocalization.GetKeyLocalized(m_Weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey()),
            this);
        }
        else
        {
            Debug.Log("创建" + TLocalization.GetKeyLocalized(m_Weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey()));
            //BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().CreateItemInformation(TLocalization.GetKeyLocalized(m_Weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey()),
            //   this);
        }
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactTarget)
    {
        int Score = (int)m_Weapon.m_WeaponInfo.m_Rarity+ m_Weapon.m_EnhanceLevel;
        if (GameDataManager.m_getWeapons < Score)
        {
            GameDataManager.m_getWeapons = Score;
        }
        Debug.Log("武器星星" + m_Weapon.m_WeaponInfo.m_Weapon + "00000" + Score);
        if (m_storageNumber != -1)
        {
            GameDataManager.m_CGameDrawWeaponData.DeleteWeapon(enum_PlayerWeaponIdentity.Invalid, m_storageNumber);
        }
        base.OnInteractedContinousCheck(_interactTarget);
        m_Weapon = _interactTarget.ObtainWeapon(m_Weapon);
        if (!m_Weapon)
            return false;
        m_Weapon.transform.SetParentResetTransform(tf_ModelContainer);
        if (m_storageNumber!=-1)
            GameDataManager.m_CGameDrawWeaponData.AddWeapon(m_Weapon.m_Identity,transform.position,m_storageNumber);
        if (m_visualizeItem != null)
        {
            m_visualizeItem.Play(TLocalization.GetKeyLocalized(m_Weapon.m_WeaponInfo.m_Weapon.GetNameLocalizeKey()),
            this);
        }
        return true;
    }
}
