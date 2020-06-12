using GameSetting;
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
    private void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 50, 50), ""))
        {
            CampUIManager.Instance.ShowCoinsPage<UI_DailyTasks>(true, true, ResetPlayerCamera, .1f);
            //GameDataManager. OnCreditStatus(500f);
            //GameManagerBase.Instance.SetExtraTimeScale(10);
            //BattleUIManager.Instance.GetComponentInChildren<UIC_GameNumericVisualize>().OnDeductMoney(50);
            //if(BattleManager.Instance)
            //BattleManager.Instance.m_LocalPlayer.ObtainWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.New((enum_PlayerWeaponIdentity)id, 5)));
            //else
            //    CampManager.Instance.m_LocalPlayer.ObtainWeapon(GameObjectManager.SpawnWeapon(WeaponSaveData.New((enum_PlayerWeaponIdentity)id, 5)));
        }
        if (GUI.Button(new Rect(50, 0, 50, 50), ""))
        {
            GameDataManager.m_GameTaskData.RandomTask();
            //GameDataManager.OnCGameTask(50);
            //GameDataManager.OnDiamondsStatus(500);
            //GameObjectManager.SpawnPlayerCharacter(GameDataManager.m_GameProgressData.m_Character, Vector3.zero, new Quaternion(0,0,0,0)).OnPlayerActivate(GameDataManager.m_GameProgressData);
        }
    }
    void ResetPlayerCamera()
    {
        Debug.Log("退出");
    }
}
