using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CommandBufferTest : MonoBehaviour {

    private CommandBuffer commandBuffer = null;
    private Renderer targetRenderer = null;

void OnEnable()
    {
        targetRenderer = this.GetComponentInChildren<Renderer>();
        if (targetRenderer)
        {
            commandBuffer = new CommandBuffer();
            commandBuffer.DrawRenderer(targetRenderer, targetRenderer.sharedMaterial);
            //直接加入相机的CommandBuffer事件队列中,
            Camera.main.AddCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
            targetRenderer.enabled = false;
        }
    }

    private void Update()
    {

        transform.Rotate(1, 1, 1);
    }
    void OnDisable()
    {
        if (targetRenderer)
        {
            //移除事件，清理资源
            Camera.main.RemoveCommandBuffer(CameraEvent.AfterImageEffects, commandBuffer);
            commandBuffer.Clear();
            targetRenderer.enabled = true;
        }
    }
}
