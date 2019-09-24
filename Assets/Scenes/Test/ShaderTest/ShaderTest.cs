using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
        CameraController.Instance.m_Effect.AddCameraEffect<CB_GenerateGlobalGaussianBlurTexture>().SetEffect(3,2f,2);
	}
	
}
