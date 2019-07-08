using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer),typeof(BoxCollider))]
public class SFXCastLaserBeam : SFXCastBox {
    LineRenderer m_Beam;
    public override void Init(int _sfxIndex)
    {
        base.Init(_sfxIndex);
        m_Beam = GetComponent<LineRenderer>();
    }
    protected override void Update()
    {
        base.Update();
        f_castLength = m_Collider.size.z;
        RaycastHit hit;
        if (Physics.BoxCast(transform.position, new Vector3(m_Collider.size.x / 2, m_Collider.size.y / 2, .01f), transform.forward, out hit, Quaternion.LookRotation(transform.forward, transform.up), f_castLength, GameLayer.Physics.I_Static))
            f_castLength = TCommon.GetXZDistance(transform.position, hit.point);

        m_Beam.SetPosition(0, transform.position);
        m_Beam.SetPosition(1, transform.position + transform.forward * f_castLength);
    }
}
