using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTestDepthScanArea : MonoBehaviour
{
    public Color fillColor =Color.grey;
    public Color edgeColor = Color.blue;
    public float radius = 10f;
    public float width = 1f;
    public float duration = 2f;
    public Texture tex =null;
    public float texScale = 1;
    public Vector2 texFlow=Vector2.one;

    bool set = false;
    private void Start()
    {
        CameraController.Instance.m_Effect.SetMainTextureCamera(true);
    }

    RaycastHit hit;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && CameraController.Instance.InputRayCheck(Input.mousePosition, 1 << 0, ref hit))
        {
            set = !set;
            CameraController.Instance.m_Effect.SetDepthAreaCircle(set,hit.point, radius,width, duration).SetColor(fillColor, edgeColor).SetTexture(tex,texScale,texFlow);
        }
    }
}
