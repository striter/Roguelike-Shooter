using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class TestGameManager : GameManagerBase
{
#if UNITY_EDITOR
    #region Test
    public bool B_PhysicsDebugGizmos = true;
    public int Z_TestEntitySpawn = 221;
    public enum_EntityFlag TestEntityFlag = enum_EntityFlag.Enermy;
    public int TestEntityBuffOnSpawn = 1;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            SetBulletTime(!m_BulletTiming, .1f);

        RaycastHit hit = new RaycastHit();
        if (Input.GetKeyDown(KeyCode.Z) && CameraController.Instance.InputRayCheck(Input.mousePosition, GameLayer.Mask.I_Static, ref hit))
        {
            EntityCharacterBase enermy = GameObjectManager.SpawnEntityCharacter(Z_TestEntitySpawn, hit.point,m_LocalPlayer.transform.position, TestEntityFlag);
            enermy.SetExtraDifficulty(GameExpression.GetAIBaseHealthMultiplier(1), GameExpression.GetAIMaxHealthMultiplier( enum_StageLevel.Veteran), GameExpression.GetEnermyGameDifficultyBuffIndex(1));
            if (TestEntityBuffOnSpawn > 0)
                enermy.m_HitCheck.TryHit(new DamageInfo(0, enum_DamageType.Basic, DamageDeliverInfo.BuffInfo(-1, TestEntityBuffOnSpawn)));
        }
        if (Input.GetKeyDown(KeyCode.N))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(20, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
        if (Input.GetKeyDown(KeyCode.M))
            m_LocalPlayer.m_HitCheck.TryHit(new DamageInfo(-50, enum_DamageType.Basic, DamageDeliverInfo.Default(-1)));
    }
    #endregion
#endif
    public enum_Style m_EnermiesType;
    public enum_PlayerCharacter m_Character;
    EntityCharacterPlayer m_LocalPlayer;
    protected override void Start()
    {
        base.Start();
        if(m_EnermiesType!= enum_Style.Invalid)
            GameObjectManager.RegistStyledInGamePrefabs(m_EnermiesType, enum_StageLevel.Veteran);

        CBattleSave testData = new CBattleSave();
        testData.m_character = m_Character;
        m_LocalPlayer = GameObjectManager.SpawnEntityPlayer(testData);
        AttachPlayerCamera( m_LocalPlayer.tf_CameraAttach);
        InitPostEffects( enum_Style.Iceland);
    }
}
