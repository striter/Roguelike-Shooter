using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesViewerManager : MonoBehaviour {

    private void Awake()
    {
        GameObjectManager.Init();
        GameObjectManager.PresetRegistCommonObject();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Space))
            return;
        ObjectPoolManager<int, SFXBase>.RecycleAll();
        int muzzleIndex = 0;
        int indicatorIndex = 0;
        int impactIndex = 0;
        Quaternion rotation = Quaternion.LookRotation(Vector3.up,Vector3.left);
        ObjectPoolManager<int, SFXBase>.GetRegistedList().Traversal((int identity) =>
        {
            SFXBase particle = ObjectPoolManager<int, SFXBase>.GetRegistedSpawnItem(identity);
            if ((particle as SFXMuzzle) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, null, new Vector3(-5, 0, muzzleIndex++*2), rotation) as SFXMuzzle).PlayOnce(-1);
            if ((particle as SFXIndicator) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, null, new Vector3(0, 0, indicatorIndex++*2), rotation) as SFXIndicator).PlayOnce(-1);
            if ((particle as SFXImpact) != null)
                (ObjectPoolManager<int, SFXBase>.Spawn(identity, null, new Vector3(5, 0, impactIndex++*2), rotation) as SFXImpact).PlayOnce(-1);
        });
    }
}
