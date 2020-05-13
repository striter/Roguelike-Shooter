using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class GamePlayTest : GameManagerBase {

    protected override void Start()
    {
        base.Start();
        AttachPlayerCamera(GameObjectManager.SpawnPlayerCharacter(  enum_PlayerCharacter.Assassin, Vector3.zero, Quaternion.identity).OnPlayerActivate(new CGameProgressSave( enum_GameDifficulty.Hell,enum_PlayerCharacter.Assassin, enum_PlayerWeapon.Grenade)).transform);
    }
}
