using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXEffect : SFXParticles
{
    public bool B_IsMeshGlow;
    public void Play(EntityCharacterBase entity)
    {
        base.Play(entity.I_EntityID,0f,0f);
        AttachTo(entity.transform);
    }
    protected override void OnStop()
    {
        base.OnStop();
        AttachTo(null);
    }

}
