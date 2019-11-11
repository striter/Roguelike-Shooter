using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : SimpleSingletonMono<CameraController>  {
    public bool B_SmoothCamera = true;
    public float F_CameraSmoothParam = .3f;
    public bool B_InvertCamera = false;
    public float F_RotateSensitive = 1;
    public bool B_CameraOffsetWallClip = true;

    public bool B_DrawDebugLine = false;

    protected bool m_SelfRotation ;
    protected Vector3 v3_localOffset;
    protected int I_YawAngleMin = 0;
    protected int I_YawAngleMax = 30;
    Ray ray_temp;
    Vector3 v3_temp;
    RaycastHit rh_temp;
    float f_CameraDistance;
    public Camera m_Camera { get; private set; }
    public Transform tf_AttachTo { get; private set; }
    protected Transform tf_MainCamera;
    protected Vector3 v3_CameraPos;
    protected Transform tf_CameraOffset;
    protected Quaternion qt_CameraRot=Quaternion.identity;
    protected Transform tf_CameraYawBase;
    protected Transform tf_CameraLookAt;

    protected float f_Yaw = 0;
    protected float f_Pitch = 0;
    protected float f_Roll = 0;

    public CameraEffectManager m_Effect;
    #region Preset
    protected override void Awake()
    {
        base.Awake();
        m_Camera = Camera.main;
        tf_MainCamera = m_Camera.transform;
        tf_CameraYawBase = transform.FindOrCreateNewTransform("CameraTrans");
        tf_CameraOffset = tf_CameraYawBase.FindOrCreateNewTransform("CameraPos");
        m_Effect = m_Camera.GetComponent<CameraEffectManager>();
    }

    protected void SetCameraOffset(Vector3 offset)
    {
        v3_localOffset = offset;
        f_CameraDistance = tf_CameraOffset.localPosition.magnitude;
    }
    protected void SetCameraYawClamp(int minRotationClamp = -1, int maxRotationClamp = -1)
    {
        I_YawAngleMin = minRotationClamp;
        I_YawAngleMax = maxRotationClamp;
        f_Pitch = Mathf.Clamp(f_Pitch, I_YawAngleMin, I_YawAngleMax);
    }
    #endregion
    #region Interact Apis
    public void Attach(Transform toTransform,bool selfRotation)
    {
        m_SelfRotation = selfRotation;
       tf_AttachTo = toTransform;
        qt_CameraRot = toTransform.rotation;
    }
    public void LookAt(Transform lookAtTrans) => tf_CameraLookAt = lookAtTrans;
    public void SetCameraSmoothParam(float smoothParam)=> F_CameraSmoothParam = smoothParam;
    public void SetCameraRotation(int pitch = -1, int yaw = -1)
    {
        if (pitch != -1)
            f_Pitch = m_SelfRotation? Mathf.Clamp(f_Pitch, I_YawAngleMin, I_YawAngleMax):pitch;
        if (yaw != -1)
            f_Yaw = yaw;
    }
    public void RotateCamera(Vector2 _input)
    {
        f_Yaw += _input.x * F_RotateSensitive;
        f_Pitch += (B_InvertCamera ? _input.y : -_input.y) * F_RotateSensitive;
        f_Pitch = Mathf.Clamp(f_Pitch, I_YawAngleMin, I_YawAngleMax);
    }

    public bool InputRayCheck(Vector2 inputPos, int layerMask, ref RaycastHit rayHit)
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return false;

        Ray r = m_Camera.ScreenPointToRay(inputPos);

        if (B_DrawDebugLine)
            Debug.DrawRay(r.origin, r.direction * 1000, Color.red);

        return Physics.Raycast(r, out rayHit, 1000, layerMask);
    }
    #endregion
    #region Calculate
    protected virtual Quaternion CalculateSelfRotation() => Quaternion.Euler(f_Pitch, f_Yaw, f_Roll);
    protected virtual Vector3 V3_LocalPositionOffset => v3_localOffset;
    protected virtual void LateUpdate()
    {
        if (tf_AttachTo == null)
            return;

        if (tf_CameraLookAt != null)
            qt_CameraRot = Quaternion.LookRotation(tf_CameraLookAt.position - tf_MainCamera.position, Vector3.up);
        else
            qt_CameraRot = m_SelfRotation ? CalculateSelfRotation() : tf_AttachTo.rotation;

        tf_CameraYawBase.position = tf_AttachTo.position;
        tf_CameraYawBase.rotation = Quaternion.Euler(0, qt_CameraRot.eulerAngles.y, 0);

        tf_CameraOffset.localPosition = V3_LocalPositionOffset;

        if (B_CameraOffsetWallClip && v3_localOffset != Vector3.zero)
        {
            v3_temp = Vector3.Normalize(tf_CameraOffset.position - tf_AttachTo.position);
            ray_temp = new Ray(tf_AttachTo.position, v3_temp);
            if (Physics.Raycast(ray_temp, out rh_temp, f_CameraDistance))
                tf_CameraOffset.position = rh_temp.point + v3_temp * .2f;

            if (B_DrawDebugLine)
                Debug.DrawRay(ray_temp.origin, ray_temp.direction);
        }

        tf_MainCamera.position = B_SmoothCamera ? Vector3.Lerp(tf_MainCamera.position, tf_CameraOffset.position, F_CameraSmoothParam) : tf_MainCamera.position;
        tf_MainCamera.rotation = B_SmoothCamera ? Quaternion.Lerp(tf_MainCamera.rotation, qt_CameraRot, F_CameraSmoothParam) : qt_CameraRot;
    }
    #endregion
    #region Get/Set
    public static Camera MainCamera => Instance.m_Camera;
    public static Quaternion CameraRotation=> Instance.qt_CameraRot;
    public static Quaternion CameraXZRotation=> Quaternion.LookRotation(CameraXZForward, Vector3.up);
    public static Vector3 CameraXZForward
    {
        get
        {
            Vector3 forward = Instance.tf_CameraYawBase.forward;
            forward.y = 0;
            forward = forward.normalized;
            return forward;
        }
    }
    public static Vector3 CameraXZRightward
    {
        get
        {
            Vector3 rightward = Instance.tf_CameraYawBase.right;
            rightward.y = 0;
            rightward = rightward.normalized;
            return rightward;
        }
    }
    #endregion
}
