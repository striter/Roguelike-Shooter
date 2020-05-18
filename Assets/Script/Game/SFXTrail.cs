using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXTrail : SFXParticles {
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Particle.SetActive(true);
    }
    protected override void OnStop()
    {
        base.OnStop();
        m_Particle.SetActive(false);
    }
}
