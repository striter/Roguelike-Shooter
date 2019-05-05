using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : CameraController
{
    public Vector3 TPSOffset=new Vector3(6,3,1);
    protected override void Awake()
    {
        base.Awake();
    }
}
