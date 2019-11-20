using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXEquipmentBase : SFXParticles {


    protected override void DoSFXRecycle() => ObjectPoolManager<int, SFXEquipmentBase>.Recycle(I_SFXIndex,this);
}
