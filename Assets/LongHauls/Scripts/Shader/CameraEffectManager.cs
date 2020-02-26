using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffectManager :MonoBehaviour, ICoroutineHelperClass
{
    #region Interact
    public T GetOrAddCameraEffect<T>() where T: CameraEffectBase, new()
    {
        T existingEffect = GetCameraEffect<T>();
        if (existingEffect != null)
            return existingEffect;

        T effectBase = new T();
        if(effectBase.m_Supported)
        {
            effectBase.OnSetEffect(this);
            m_PostEffects.Add(effectBase);
            ResetPostEffectParams();
            return effectBase;
        }
        return null;
    }

    public T GetCameraEffect<T>() where T : CameraEffectBase => m_PostEffects.Find(p => p.GetType() ==typeof(T)) as T;
    public void RemoveCameraEffect<T>() where T : CameraEffectBase, new()
    {
        T effect = GetCameraEffect<T>();
        if (effect == null)
            return;

        m_PostEffects.Remove(effect);
        ResetPostEffectParams();
    }
    public void RemoveAllPostEffect()
    {
        m_PostEffects.Traversal((CameraEffectBase effect)=> { effect.OnDestroy(); });
        m_PostEffects.Clear();
        ResetPostEffectParams();
    }

    public void SetCostyEffectEnable(bool mobileCostEnable)
    {
        m_MobileCostEnable = mobileCostEnable;
        ResetPostEffectParams();
    }

    public void StartAreaScan(Vector3 startPoint,Color scanColor, Texture scanTex=null,float scale=1f, float lerp=.7f,float width=1f,float range=20,float duration=1.5f)
    {
        if (GetCameraEffect<PE_AreaScanDepth>() != null)
            RemoveCameraEffect<PE_AreaScanDepth>();

        PE_AreaScanDepth areaScan= GetOrAddCameraEffect<PE_AreaScanDepth>();
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
    public bool m_PostEffectEnabled { get; private set; } = false;
    public bool m_MobileCostEnable { get; private set; } = true;
    public bool m_calculateDepthToWorldMatrix { get; private set; } = false;
    RenderTexture tempTexture1, tempTexture2;
    protected void Awake()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode = DepthTextureMode.None;
        m_calculateDepthToWorldMatrix = false;
    }
    
    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_calculateDepthToWorldMatrix)
        {
            CalculateFrustumCornorsRay();
            CalculateViewProjectionMatrixInverse();
        }

        if(!m_PostEffectEnabled)
        {
            Graphics.Blit(source, destination);
            return;
        }

        tempTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        Graphics.Blit(source, tempTexture1);
        for (int i = 0; i < m_PostEffects.Count; i++)
        {
            if (! m_PostEffects[i].m_Enabled)
                continue;

            tempTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            m_PostEffects[i].OnRenderImage(tempTexture1,tempTexture2);
            Graphics.Blit(tempTexture2, tempTexture1);
            RenderTexture.ReleaseTemporary(tempTexture2);
        }
        Graphics.Blit(tempTexture1,destination);
        RenderTexture.ReleaseTemporary(tempTexture1);
    }
    private void OnDestroy()
    {
        RemoveAllPostEffect();
    }
    private void OnRenderObject()
    {
        for (int i = 0; i < m_PostEffects.Count; i++)
            m_PostEffects[i].OnRenderObject();
    }

    void ResetPostEffectParams()
    {
        m_PostEffectEnabled = false;
        m_Camera.depthTextureMode = DepthTextureMode.None;
        m_calculateDepthToWorldMatrix = false;
        
        m_PostEffects.Traversal((CameraEffectBase effectBase) =>
        {
            effectBase.OnCheckMobileCostEnable(m_MobileCostEnable);
            if (!effectBase.m_Enabled)
                return;

            m_PostEffectEnabled |= effectBase.m_IsPostEffect;
            m_Camera.depthTextureMode |= effectBase.m_DepthTextureMode;
            m_calculateDepthToWorldMatrix |= effectBase.m_DepthToWorldMatrix;
        });
    }

    #region Calculations

    public float Get01Depth(Vector3 target) => m_Camera.WorldToViewportPoint(target).z / (m_Camera.farClipPlane - m_Camera.nearClipPlane);
    public float Get01DepthLength(float length) => length / (m_Camera.farClipPlane - m_Camera.nearClipPlane);
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
