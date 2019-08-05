﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : CameraController
{
    protected static TPSCameraController ninstance;
    public static new TPSCameraController Instance => ninstance;
    public Vector3 TPSOffset=new Vector3(6,3,1);
    public int I_YawMin = -90, I_YawMax = 90;
    Vector3 f_shake;
    protected override Vector3 V3_LocalPositionOffset
    {
        get
        {
            return v3_localOffset + f_shake;
        }
    }
    protected override void Awake()
    {
        ninstance = this;
        base.Awake();
        SetCameraOffset(TPSOffset);
        B_SelfRotation = true;
        B_SmoothCamera = true;
        B_CameraOffsetWallClip = true;
        SetCameraYawClamp(I_YawMin, I_YawMax);
    }
    public void AddShake(float shakeAmount)
    {

    }
}
