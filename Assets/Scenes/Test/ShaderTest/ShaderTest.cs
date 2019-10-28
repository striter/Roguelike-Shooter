using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShaderTest : MonoBehaviour
{
    void Start ()
    {
        GetComponent<CameraEffectManager>().GetOrAddCameraEffect<CB_GenerateOpaqueTexture>();
    }

}