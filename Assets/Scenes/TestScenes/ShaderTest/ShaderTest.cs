using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderTest : MonoBehaviour
{
    public float m_Stength = 1;
    public int m_SphereRadius = 10;
    [Range(0.02f,0.001f)]
    public float m_FallOff = 0.002f;
    public Texture m_NoiseTex = null;
    PE_DepthSSAO m_DepthSSAO;
    void Start ()
    {
        //GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_BloomSpecific>().m_Blur.SetEffect( PE_Blurs.enum_BlurType.AverageBlur);
        m_DepthSSAO = GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_DepthSSAO>();
        //GetComponent<CameraEffectManager>().SetCameraEffects(DepthTextureMode.Depth);
    }

    void Update()
    {
        m_DepthSSAO.SetEffect(Color.black, m_Stength,m_SphereRadius,m_FallOff, m_NoiseTex,16);
    }

}