using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class GamePlayTest : GameManagerBase {

    protected override void Start()
    {
        base.Start();
        AttachPlayerCamera(GameObjectManager.SpawnEntityPlayer( new CPlayerBattleSave( enum_PlayerCharacter.Assassin,enum_PlayerWeapon.DE,new List<EquipmentSaveData>()), Vector3.zero, Quaternion.identity).transform);
    }
}
