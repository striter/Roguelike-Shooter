﻿using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testScript : MonoBehaviour
{
    public int id = 115;
    int visualize = 1;
    public UIT_GridControllerGridItem<UIGI_VisualizeAttackIndicate> m_AttackIndicateGrid { get; private set; }
    // Use this for initialization
    void Start () {
        //m_AttackIndicateGrid = new UIT_GridControllerGridItem<UIGI_VisualizeAttackIndicate>(transform.Find("AttackIndicateGrid"));
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    int[] m_idLis = new int[] { 101, 102, 103, 104, 105, 201, 202, 203, 301, 302, 303, 401, 402, 403 };
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 50, 50), ""))
        {
            //new DamageInfo(CampManager.Instance.m_LocalPlayer.m_EntityID, enum_DamageIdentity.Expire).SetDamage(50, enum_DamageType.Basic);
            //int num = Random.Range(0, 14);

            //GameObjectManager.SpawnInteract<InteractPickupWeapon>(NavigationManager.NavMeshPosition(CampManager.Instance.m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(WeaponSaveData.New((enum_PlayerWeaponIdentity)m_idLis[num], 0), GameDataManager.m_CGameDrawWeaponData.m_currentValue);

            //GameDataManager.m_CGameDrawWeaponData.AddWeapon((enum_PlayerWeaponIdentity)m_idLis[num], NavigationManager.NavMeshPosition(CampManager.Instance.m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f)) ;
            //enum_PlayerWeaponIdentity m_weaponDrawing = GameDataManager.RandomWeaponDrawing();
            //Debug.Log( TLocalization.GetKeyLocalized(m_weaponDrawing.GetNameLocalizeKey()));


            //enum_PlayerWeaponIdentity m_weaponDrawing = enum_PlayerWeaponIdentity.DE;
            //Debug.Log(TLocalization.GetKeyLocalized(m_weaponDrawing.GetNameLocalizeKey()));
            //SWeaponInfos weaponInfo = GameDataManager.GetWeaponProperties(m_weaponDrawing);

            //GameDataManager.UnlockArmoryBlueprint(weaponInfo.m_Rarity);
            //GameObjectManager.SpawnInteract<InteractPickupWeapon>(NavigationManager.NavMeshPosition(CampManager.Instance.m_LocalPlayer.transform.position + TCommon.RandomXZSphere() * 5f), Quaternion.identity).Play(WeaponSaveData.New(enum_PlayerWeaponIdentity.M82A1, 5));

            //GameDataManager.m_CGameShopData.Random();
            //CampUIManager.Instance.ShowCoinsPage<UI_DailyTasks>(true, true, ResetPlayerCamera, .1f);
            //GameDataManager. OnCreditStatus(500f);
            //GameManagerBase.Instance.SetExtraTimeScale(10);
            //BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().OnDeductMoney(50);
            //if(BattleManager.Instance)
            //BattleManager.Instance.m_LocalPlayer.ObtainWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.New((enum_PlayerWeaponIdentity)id, 5)));
            //else
            //CampManager.Instance.m_LocalPlayer.ObtainWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.New((enum_PlayerWeaponIdentity)id, 5)));
        }
        if (GUI.Button(new Rect(50, 0, 50, 50), ""))
        {
            for (int i = 0; i < 10; i++)
            {
                if (GameDataManager.m_CGameDrawWeaponData.GetWeapon(i) != enum_PlayerWeaponIdentity.Invalid)
                {
                    GameObjectManager.SpawnInteract<InteractPickupWeapon>(GameDataManager.m_CGameDrawWeaponData.GetWeaponPos(i), Quaternion.identity).Play(WeaponSaveData.New(GameDataManager.m_CGameDrawWeaponData.GetWeapon(i), 0),i);
                }
            }
            //GameDataManager.m_GameTaskData.RandomTask();
            //GameDataManager.OnCGameTask(50);
            //GameDataManager.OnDiamondsStatus(50000);
            //GameObjectManager.SpawnPlayerCharacter(GameDataManager.m_GameProgressData.m_Character, Vector3.zero, new Quaternion(0,0,0,0)).OnPlayerActivate(GameDataManager.m_GameProgressData);
        }
    }
    void ResetPlayerCamera()
    {
        Debug.Log("退出");
    }
}