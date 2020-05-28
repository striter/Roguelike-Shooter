using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTestDepthGodray : MonoBehaviour {

    [Range(0,1f)]
    public float m_BaseAttenuation = 1;
    public Light m_TargetLight;

    PE_DepthGodRay m_DepthGodRay;
    private void Start()
    {
        CameraController.Instance.m_Effect.SetMainTextureCamera(true);
        m_DepthGodRay=CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_DepthGodRay>().SetEffect(m_TargetLight).SetQuality( enum_CameraEffectQuality.High);
    }

    private void Update()
    {
        m_DepthGodRay.SetBaseAttenuation(m_BaseAttenuation);
    }
}
