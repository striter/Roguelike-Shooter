using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera)),ExecuteInEditMode]
public class CameraEffectManager :MonoBehaviour, ISingleCoroutine {
    public bool B_TestMode=false;
    #region Interact
    public T AddCameraEffect<T>() where T: CameraEffectBase, new()
    {
        if (GetCameraEffect<T>() != null)
        {
            Debug.LogError("Effect Already Exist");
            return null;
        }
        T effectBase = new T();
        if (effectBase.m_Supported)
        {
            effectBase.OnSetEffect(this);
            m_PostEffects.Add(effectBase);
            m_Camera.depthTextureMode |= effectBase.m_DepthTextureMode;
            m_calculateDepthToWorldMatrix |= effectBase.m_DepthToWorldMatrix;
        }
        return effectBase;
    }
    public T GetCameraEffect<T>() where T : CameraEffectBase => m_PostEffects.Find(p => p.GetType() ==typeof(T)) as T;
    public void RemoveCameraEffect<T>() where T : CameraEffectBase, new()
    {
        T effect = GetCameraEffect<T>();
        if (effect != null)
            m_PostEffects.Remove(effect);
    }
    public void RemoveAllPostEffect()
    {
        m_PostEffects.Traversal((CameraEffectBase effect)=> { effect.OnDestroy(); });
        m_PostEffects.Clear();
    }

    public void StartAreaScan(Vector3 startPoint,Color scanColor, Texture scanTex=null,float scale=1f, float lerp=.7f,float width=1f,float range=20,float duration=1.5f)
    {
        if (GetCameraEffect<PE_AreaScanDepth>() != null)
            RemoveCameraEffect<PE_AreaScanDepth>();

        PE_AreaScanDepth areaScan= AddCameraEffect<PE_AreaScanDepth>();
        areaScan.SetEffect(startPoint, scanColor, scanTex,scale, lerp, width);
        this.StartSingleCoroutine(0,TIEnumerators.ChangeValueTo((float value)=> {
            areaScan.SetElapse(range*value);
        },0,1,duration,()=> {
            RemoveCameraEffect<PE_AreaScanDepth>();
        }));
    }
    
    #endregion
    List<CameraEffectBase> m_PostEffects=new List<CameraEffectBase>();
    public Camera m_Camera { get; protected set; }
    public bool m_calculateDepthToWorldMatrix { get; set; } = false;
    RenderTexture tempTexture1, tempTexture2;
    protected void Awake()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode = DepthTextureMode.None;
        m_calculateDepthToWorldMatrix = false;
    }
    
    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        tempTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        Graphics.Blit(source, tempTexture1);

        if (m_calculateDepthToWorldMatrix)
        {
            CalculateFrustumCornorsRay();
            CalculateViewProjectionMatrixInverse();
        }

        for (int i = 0; i < m_PostEffects.Count; i++)
        {
            if (B_TestMode)
            {   
                m_PostEffects[i].OnRenderImage(tempTexture1, tempTexture1);
                continue;
            }

            tempTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            m_PostEffects[i].OnRenderImage(tempTexture1,tempTexture2);
            Graphics.Blit(tempTexture2, tempTexture1);
            RenderTexture.ReleaseTemporary(tempTexture2);
        }
        Graphics.Blit(tempTexture1,destination);
        RenderTexture.ReleaseTemporary(tempTexture1);
    }
    private void OnRenderObject()
    {
        for (int i = 0; i < m_PostEffects.Count; i++)
            m_PostEffects[i].OnWillRenderObject();
    }
    #region Matrix
    static readonly int ID_VPMatrixInverse = Shader.PropertyToID("_VPMatrixInverse");
    static readonly int ID_FrustumCornersRayBL = Shader.PropertyToID("_FrustumCornersRayBL");
    static readonly int ID_FrustumCornersRayBR = Shader.PropertyToID("_FrustumCornersRayBR");
    static readonly int ID_FrustumCornersRayTL = Shader.PropertyToID("_FrustumCornersRayTL");
    static readonly int ID_FrustumCornersRayTR = Shader.PropertyToID("_FrustumCornersRayTR");
    protected void CalculateViewProjectionMatrixInverse()=>Shader.SetGlobalMatrix(ID_VPMatrixInverse, (m_Camera.projectionMatrix * m_Camera.worldToCameraMatrix).inverse);
    protected void CalculateFrustumCornorsRay()
    {
        float fov = m_Camera.fieldOfView;
        float near = m_Camera.nearClipPlane;
        float far = m_Camera.farClipPlane;
        float aspect = m_Camera.aspect;

        Transform cameraTrans = m_Camera.transform;
        float halfHeight = near * Mathf.Tan(fov * .5f * Mathf.Deg2Rad);
        Vector3 toRight = cameraTrans.right * halfHeight * aspect;
        Vector3 toTop = cameraTrans.up * halfHeight;

        Vector3 topLeft = cameraTrans.forward * near + toTop - toRight;
        float scale = topLeft.magnitude / near;
        topLeft.Normalize();
        topLeft *= scale;

        Vector3 topRight = cameraTrans.forward * near + toTop + toRight;
        topRight.Normalize();
        topRight *= scale;

        Vector3 bottomLeft = cameraTrans.forward * near - toTop - toRight;
        bottomLeft.Normalize();
        bottomLeft *= scale;
        Vector3 bottomRight = cameraTrans.forward * near - toTop + toRight;
        bottomRight.Normalize();
        bottomRight *= scale;
        

        Shader.SetGlobalVector(ID_FrustumCornersRayBL, bottomLeft);
        Shader.SetGlobalVector(ID_FrustumCornersRayBR, bottomRight);
        Shader.SetGlobalVector(ID_FrustumCornersRayTL, topLeft);
        Shader.SetGlobalVector(ID_FrustumCornersRayTR, topRight);
        
    }
    #endregion
}
