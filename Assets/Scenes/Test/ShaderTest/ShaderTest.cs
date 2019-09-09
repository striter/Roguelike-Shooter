using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
        CameraEffectManager.AddCameraEffect<CB_DepthOfFieldSpecific>().SetSpecificTarget(target.GetComponentsInChildren<Renderer>());	
	}
	
}
