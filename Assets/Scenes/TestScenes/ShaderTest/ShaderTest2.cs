using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderTest2 : MonoBehaviour {

    private RenderTexture depthRT;
    private RenderTexture colorRT;
    private RenderTexture depthTex;

    private CommandBuffer _cbDepth = null;

    private Camera _Camera = null;

    private void Awake()
    {
        _Camera = Camera.main;

        depthRT = new RenderTexture(_Camera.pixelWidth, _Camera.pixelHeight, 24, RenderTextureFormat.Depth);
        depthRT.name = "MainDepthBuffer";
        colorRT = new RenderTexture(_Camera.pixelWidth, _Camera.pixelHeight, 0, RenderTextureFormat.RGB111110Float);
        colorRT.name = "MainColorBuffer";

        int Width = _Camera.pixelWidth;
        int Height = _Camera.pixelHeight;

        //depthTex = new RenderTexture(Width, Height, 0, RenderTextureFormat.RHalf);
        //depthTex.name = "SceneDepthTex";

        _Camera.SetTargetBuffers(colorRT.colorBuffer, depthRT.depthBuffer);

        Shader.SetGlobalTexture("_CameraFreeDepthTexture", depthRT);
        //_cbDepth = new CommandBuffer();
        //_cbDepth.name = "CommandBuffer_DepthBuffer";
        //_cbDepth.Blit(depthRT.depthBuffer, depthTex.colorBuffer);
        //_cbDepth.SetGlobalTexture("_CameraDepthTexture", depthTex);
        //_Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, _cbDepth);

    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(colorRT, destination);
    }

    private void OnDestroy()
    {
        _Camera.targetTexture = null; 
    }
}
