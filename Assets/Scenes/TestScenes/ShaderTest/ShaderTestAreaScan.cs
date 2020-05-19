using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTestAreaScan : MonoBehaviour
{
    public Color m_ScanColor;
    public float m_Width=1f;
    public float m_Duration = 1.5f;
    public float m_Range=20f;
    public Texture m_ScanTex;
    public float m_ScanScale = 1f;
    RaycastHit hit;

    private void Start()
    {
        CameraController.Instance.m_Effect.SetMainTextureCamera(true);
    }
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && CameraController.Instance.InputRayCheck(Input.mousePosition,1<<0, ref hit))
            CameraController.Instance.m_Effect.StartDepthScanCircle(hit.point,m_ScanColor,m_Width,m_Range,m_Duration).SetTexture(m_ScanTex, m_ScanScale);
    }
}
