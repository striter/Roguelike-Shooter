using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTestAreaScan : MonoBehaviour {
    public Texture m_ScanTex;
    public Color m_ScanColor;
    public float m_ScanScale=1f;
    public float m_Opacity=.7f;
    public float m_Width=1f;
    public float m_Duration = 1.5f;
    public float m_Range=20f;
    RaycastHit hit;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && CameraController.Instance.InputRayCheck(Input.mousePosition, 0, ref hit))
            CameraController.Instance.m_Effect.StartAreaScan(hit.point,m_ScanColor, m_ScanTex, m_ScanScale,m_Opacity,m_Width,m_Range,m_Duration);
    }
}
