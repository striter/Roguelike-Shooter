using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderTest : MonoBehaviour
{
    public Renderer m_renderer1, m_renderer2, m_renderer3;
    public Color m_Color;
    MaterialPropertyBlock m_Block;
    void Start ()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        m_Block = new MaterialPropertyBlock();
        MaterialPropertyBlock m_block2 = new MaterialPropertyBlock();
        MaterialPropertyBlock m_block3 = new MaterialPropertyBlock();
        m_block2.SetColor("_Color", Color.red);
        m_block3.SetColor("_Color", Color.green);
        m_renderer2.SetPropertyBlock(m_block2);
        m_renderer3.SetPropertyBlock(m_block3);

       // GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_BloomSpecific>().m_Blur.SetEffect( PE_Blurs.enum_BlurType.AverageBlur);
    }

    private void Update()
    {;
        m_Block.SetColor("_Color",m_Color);
        m_renderer1.SetPropertyBlock(m_Block);
    }
}