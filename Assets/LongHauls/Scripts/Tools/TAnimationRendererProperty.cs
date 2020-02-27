using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode,RequireComponent(typeof(Renderer))]
public class TAnimationRendererProperty : MonoBehaviour
{
    public float m_alpha;
    Renderer m_Renderer;
    MaterialPropertyBlock m_PropertyBlock;
    int id_Color = Shader.PropertyToID("_Color");
    private void Awake()
    {
        m_Renderer = GetComponent<Renderer>();
        m_alpha = m_Renderer.sharedMaterial.color.a;
    }

    private void Update()
    {
        Color materialColor = m_Renderer.sharedMaterial.color;
        materialColor.a = m_alpha;
        m_PropertyBlock = new MaterialPropertyBlock();
        m_PropertyBlock.SetColor(id_Color, materialColor);
        m_Renderer.SetPropertyBlock(m_PropertyBlock);
    }

}
