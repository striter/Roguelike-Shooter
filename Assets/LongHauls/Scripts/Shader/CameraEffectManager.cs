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
            effectBase.InitEffect(this);
            m_CameraEffects.Add(effectBase);
            ResetPostEffectParams();
            return effectBase;
        }
        return null;
    }

    public T GetCameraEffect<T>() where T : CameraEffectBase => m_CameraEffects.Find(p => p.GetType() ==typeof(T)) as T;
    public void RemoveCameraEffect<T>() where T : CameraEffectBase, new()
    {
        T effect = GetCameraEffect<T>();
        if (effect == null)
            return;

        m_CameraEffects.Remove(effect);
        effect.OnDestroy();
        ResetPostEffectParams();
    }
    public void RemoveAllPostEffect()
    {
        m_CameraEffects.Traversal((CameraEffectBase effect)=> { effect.OnDestroy(); });
        m_CameraEffects.Clear();
        ResetPostEffectParams();
    }

    public void SetCameraEffects( DepthTextureMode textureMode)
    {
        m_Camera.depthTextureMode = textureMode;
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
    List<CameraEffectBase> m_CameraEffects=new List<CameraEffectBase>();
    public Camera m_Camera { get; protected set; }
    public bool m_DepthToWorldMatrix { get; private set; } = false;
    public bool m_DoGraphicBlitz { get; private set; } = false;
    RenderTexture tempTexture1, tempTexture2;
    protected void Awake()
    {
        m_Camera = GetComponent<Camera>();
        m_Camera.depthTextureMode = DepthTextureMode.None;
        m_DepthToWorldMatrix = false;
    }
    
    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (m_DepthToWorldMatrix)
        {
            CalculateFrustumCornorsRay();
            CalculateViewProjectionMatrixInverse();
        }

        if(!m_DoGraphicBlitz)
        {
            Graphics.Blit(source, destination);
            return;
        }

        tempTexture1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
        Graphics.Blit(source, tempTexture1);
        for (int i = 0; i < m_CameraEffects.Count; i++)
        {
            if (! m_CameraEffects[i].m_Enabled)
                continue;

            tempTexture2 = RenderTexture.GetTemporary(Screen.width, Screen.height, 0);
            m_CameraEffects[i].OnRenderImage(tempTexture1,tempTexture2);
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

    static readonly int m_GlobalCameraDepthTextureMode = Shader.PropertyToID("_CameraDepthTextureMode");
    void ResetPostEffectParams()
    {
        Shader.SetGlobalInt(m_GlobalCameraDepthTextureMode, (int)m_Camera.depthTextureMode);
        m_DoGraphicBlitz = false;
        m_DepthToWorldMatrix = false;
        m_CameraEffects.Traversal((CameraEffectBase effectBase) =>
        {
            effectBase.OnCheckEffectTextureEnable(m_Camera.depthTextureMode);
            if (!effectBase.m_Enabled)
                return;

            m_DoGraphicBlitz |=effectBase.m_DoGraphicBlitz;
            m_DepthToWorldMatrix |= effectBase.m_DepthToWorldMatrix;
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
