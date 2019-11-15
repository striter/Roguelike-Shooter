using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class TestGameManager : GameManagerBase
{
    public enum_Style m_EnermiesType;
    public enum_PlayerCharacter m_Character;
    protected override void Start()
    {
        base.Start();
        if(m_EnermiesType!= enum_Style.Invalid)
            GameObjectManager.RegistStyledIngameEnermies(m_EnermiesType, enum_StageLevel.Veteran);

        CBattleSave testData = new CBattleSave();
        testData.m_character = m_Character;
        AttachPlayerCamera( GameObjectManager.SpawnEntityPlayer(testData).tf_Head);
        InitPostEffects( enum_Style.Iceland);
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1f;
    }
}
