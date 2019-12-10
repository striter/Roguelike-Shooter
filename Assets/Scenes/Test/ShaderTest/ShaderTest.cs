using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderTest : MonoBehaviour
{
    public Renderer m_renderer1, m_renderer2, m_renderer3;
    void Start ()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        MaterialPropertyBlock m_block1=new MaterialPropertyBlock();
        MaterialPropertyBlock m_block2 = new MaterialPropertyBlock();
        MaterialPropertyBlock m_block3 = new MaterialPropertyBlock();
        m_block1.SetColor("_Color", Color.yellow);
        m_block2.SetColor("_Color", Color.red);
        m_block3.SetColor("_Color", Color.green);
        m_renderer1.SetPropertyBlock(m_block1);
        m_renderer2.SetPropertyBlock(m_block2);
        m_renderer3.SetPropertyBlock(m_block3);

    }

}