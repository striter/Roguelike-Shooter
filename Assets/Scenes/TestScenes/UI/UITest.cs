
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour {
    public Mesh m_Mesh;
    private void Awake()
    {
        CanvasRenderer renderer = GetComponent<CanvasRenderer>();
        renderer.SetMesh(m_Mesh);
    }

    private void Update()
    {
        Canvas.ForceUpdateCanvases();
    }
}
