using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class GamePlayTest : GameManagerBase {

    protected override void Start()
    {
        base.Start();
        AttachPlayerCamera(GameObjectManager.SpawnEntityPlayer(new CBattleSave(), Vector3.zero, Quaternion.identity).transform);
    }
}
