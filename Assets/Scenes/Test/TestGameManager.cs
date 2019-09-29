using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGameManager : MonoBehaviour {
    protected void Start()
    {
        //        PostEffectManager.AddPostEffect<PE_ViewDepth>();
        CameraController.Instance.m_Effect.AddCameraEffect<PE_BloomSpecific>().m_GaussianBlur.SetEffect(2, 10, 4);
    }
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.BackQuote))
            Time.timeScale = Time.timeScale == 1 ? .1f : 1f;
    }
}
