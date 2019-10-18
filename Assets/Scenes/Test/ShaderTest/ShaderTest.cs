using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderTest : MonoBehaviour {
    public GameObject target;
	// Use this for initialization
	void Start () {
    GetComponent<CameraEffectManager>().GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
        PE_FocalDepth focal= GetComponent<CameraEffectManager>().GetOrAddCameraEffect<PE_FocalDepth>();
        focal.SetEffect(.1f, 2);
        focal.SetFocalTarget(target.transform.position);

    }
	
}
