using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class GamePlayTest : GameManagerBase {

    protected void Start()
    {
        OnGameStartInit( enum_GameStyle.Invalid);
        AttachPlayerCamera(GameObjectManager.SpawnEntityPlayer(new CBattleSave(), Vector3.zero, Quaternion.identity).transform);
    }
}
