using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXProjectileSmoke : SFXProjectile {
    LineRenderer m_Smoke;
    public float F_SmokeDuration;
    float f_smokeCheck;
    public override void OnPoolItemInit(int identity)
    {
        base.OnPoolItemInit(identity);
        m_Smoke = transform.Find("Smoke").GetComponent<LineRenderer>();
    }
    protected override void OnPlay()
    {
        base.OnPlay();
        m_Smoke.SetPosition(0, transform.position);
        m_Smoke.SetPosition(1, transform.position);
    }
    protected override void OnStop()
    {
        base.OnStop();
        f_smokeCheck = F_SmokeDuration;
    }
    protected override void Update()
    {
        base.Update();

        if (B_Playing)
        {
            m_Smoke.SetPosition(1, transform.position);
            return;
        }
        f_smokeCheck -= Time.deltaTime;
        m_Smoke.startColor = Color.Lerp(Color.white, Color.black, (f_smokeCheck+1f) / F_SmokeDuration);
        m_Smoke.endColor=Color.Lerp(Color.white, Color.black, f_smokeCheck / F_SmokeDuration);

        m_Smoke.SetPosition(1, transform.position);
    }
}
