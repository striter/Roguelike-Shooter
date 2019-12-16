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
        v3_Recoil = Vector3.Lerp(v3_Recoil, Vector3.zero, Time.deltaTime * 5f);
        return Quaternion.Euler(f_Pitch + v3_Recoil.x, f_Yaw + v3_Recoil.y, f_Roll + v3_Recoil.z);
    }
    protected override void Awake()
    {
        ninstance = this;
        base.Awake();
        SetCameraOffset(TPSOffset);
        m_SelfRotation = true;
        SetCameraYawClamp(I_YawMin, I_YawMax);
    }
    
    public void Attach(Transform target,bool selfRotation,bool useOffset)
    {
        base.SetCameraOffset(useOffset ? TPSOffset : Vector3.zero);
        base.Attach(target,selfRotation);
    }
    public void AddRecoil(float recoilAmount)=>v3_Recoil +=new Vector3(0,( TCommon.RandomBool() ? 1 : -1) * recoilAmount, 0);
    public void AddShake(float shakeAmount) => v3_Shake += Random.insideUnitSphere * shakeAmount;
    public void SetImpact(Vector3 impactDirection)
    {
        v3_Shake = impactDirection;
        b_shakeReverse = false;
    } 
}
