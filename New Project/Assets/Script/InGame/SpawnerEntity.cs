using GameSetting;
using UnityEngine;

public class SpawnerEntity : SpawnerBase {
    public enum_Entity E_SpawnType = enum_Entity.Invalid;
    protected override void OnSpawn()
    {
        base.OnSpawn();

        if (E_SpawnType == enum_Entity.Invalid || E_SpawnType == enum_Entity.Player)
        {
            Debug.LogError("Can't Spawn Entity Which Type Of:"+E_SpawnType.ToString());
            return;
        }

        ObjectManager.SpawnEntity(E_SpawnType,transform);
    }
}
