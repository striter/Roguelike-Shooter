using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXParticlesPlayOnce : SFXParticles {
    protected override bool m_Loop => false;
    protected override bool m_AutoStop => false;
}
