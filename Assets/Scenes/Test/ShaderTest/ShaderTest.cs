using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
    GetComponent<CameraEffectManager>().GetOrAddCameraEffect<CB_GenerateOpaqueTexture>().SetEffect(3,4f,4);
	}
	
}
