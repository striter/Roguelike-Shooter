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
    public bool m_RecoilCompensate;
    Vector3 v3_Recoil;
    Vector3 v3_Shake;
    float inverseCheck = 0;
    bool b_shakeReverse;
    protected override Vector3 V3_LocalPositionOffset
    {
        get
        {

            inverseCheck += Time.deltaTime;
            if (inverseCheck > F_ReverseCheck)
            {
                b_shakeReverse = !b_shakeReverse;
                inverseCheck -= F_ReverseCheck;
            }

            v3_Shake = Vector3.Lerp(v3_Shake, Vector3.zero, I_ShakeParam * Time.deltaTime);
            return v3_localOffset +(b_shakeReverse?-1:1)* v3_Shake;
        }
    }
    protected override Quaternion CalculateSelfRotation()
    {
        if (m_RecoilCompensate)
        {
            v3_Recoil = Vector3.Lerp(v3_Recoil, Vector3.zero, Time.deltaTime*5f);
            return Quaternion.Euler(f_Pitch + v3_Recoil.x, f_Yaw + v3_Recoil.y, f_Roll + v3_Recoil.z);
        }
        else
        {
            f_Pitch += v3_Recoil.x;
            f_Yaw += v3_Recoil.y;
            f_Roll += v3_Recoil.z;
            v3_Recoil = Vector2.zero;
            return Quaternion.Euler(f_Pitch, f_Yaw, f_Roll);
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

    public void AddRecoil(Vector3 _recoil)=> v3_Recoil += _recoil;
    public void AddShake(float shakeAmount)=> v3_Shake += TCommon.RandomVector3(shakeAmount);
    public void SetImpact(Vector3 impactDirection)
    {
        v3_Shake = impactDirection;
        b_shakeReverse = false;
    } 
}
