using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPSCameraController : CameraController
{
    protected static TPSCameraController ninstance;
    public static new TPSCameraController Instance => ninstance;
    public Vector3 TPSOffset=new Vector3(6,3,1);
    public int I_YawMin = -90, I_YawMax = 90;
    public int I_ShakeParam;
    public float F_ReverseCheck;
    Vector3 v3_Shake;
    float inverseCheck = 0;
    bool b_reverse;
    protected override Vector3 V3_LocalPositionOffset
    {
        get
        {

            inverseCheck += Time.deltaTime;
            if (inverseCheck > F_ReverseCheck)
            {
                b_reverse = !b_reverse;
                inverseCheck -= F_ReverseCheck;
            }

            v3_Shake = Vector3.Lerp(v3_Shake, Vector3.zero, I_ShakeParam * Time.deltaTime);
            return v3_localOffset +(b_reverse?1:-1)* v3_Shake;
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
        v3_Shake += TCommon.RandomVector(shakeAmount);
    }
}
