using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CameraEffectBase
{
    public virtual DepthTextureMode m_DepthTextureMode => DepthTextureMode.None;
    public virtual bool m_DepthToWorldMatrix => false;
    protected CameraEffectManager m_Manager { get; private set; }
    public bool m_Supported { get; private set; }
    public virtual bool m_IsPostEffect => false;
    public CameraEffectBase()
    {
        m_Supported=OnCreate();
    }
    protected virtual bool OnCreate()
    {
        return true;
    }
    public virtual void OnSetEffect(CameraEffectManager _manager)
    {
        m_Manager = _manager;
    }
    public virtual void OnRenderObject()
    {

    }
    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
    public virtual void OnDestroy()
    {
    }
}
#region PostEffect
public class PostEffectBase: CameraEffectBase
{
    const string S_ParentPath = "Hidden/PostEffect/";
    public Material m_Material { get; private set; }
    public override bool m_IsPostEffect => true;
    protected override bool OnCreate()
    {
        Shader shader = Shader.Find(S_ParentPath + this.GetType().ToString());
        if (shader == null)
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString() + " Not Found");
        if (!shader.isSupported)
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString() + " Is Not Supported");

        m_Material = new Material(shader) { hideFlags = HideFlags.DontSave };
        return shader != null && shader.isSupported;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_Material);
    }
    public override void OnDestroy()
    {
        GameObject.Destroy(m_Material);
    }
}
public class PE_ViewNormal : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.DepthNormals;
}
public class PE_ViewDepth : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
}
public class PE_BSC : PostEffectBase {      //Brightness Saturation Contrast
    
    public void SetEffect(float _brightness = 1f, float _saturation = 1f, float _contrast = 1f)
    {
        m_Material.SetFloat("_Brightness", _brightness);
        m_Material.SetFloat("_Saturation", _saturation);
        m_Material.SetFloat("_Contrast", _contrast);
    }
}
public class PE_GaussianBlur : PostEffectBase       //Gassuain Blur
{
    float F_BlurSpread;
    int I_Iterations;
    RenderTexture buffer0, buffer1;
    int rtW, rtH;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        SetEffect();
    }
    public void SetEffect(float _blurSpread=2f, int _iterations=5, int _downSample = 4)
    {
        F_BlurSpread = _blurSpread;
        I_Iterations = _iterations;
        _downSample = _downSample > 0 ? _downSample : 1;
        rtW = m_Manager.m_Camera.scaledPixelWidth >> _downSample;
        rtH = m_Manager.m_Camera.scaledPixelHeight >> _downSample;
        if (buffer0) RenderTexture.ReleaseTemporary(buffer0);
        if (buffer1) RenderTexture.ReleaseTemporary(buffer1);
        buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;
        buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        RenderTexture.ReleaseTemporary(buffer0);
        RenderTexture.ReleaseTemporary(buffer1);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, buffer0);
        for (int i = 0; i < I_Iterations; i++)
        {
            m_Material.SetFloat("_BlurSpread", 1 + i * F_BlurSpread);
            Graphics.Blit(buffer0, buffer1, m_Material, 0);
            Graphics.Blit(buffer1, buffer0, m_Material, 1);
        }
        Graphics.Blit(buffer0, destination);
    }
}
public class PE_Bloom : PostEffectBase
{
    float F_BlurSpread = 2f;
    int I_Iterations = 3;
    int I_DownSample = 3;
    float F_LuminanceThreshold = .85f;
    float F_LuminanceMultiple = 10;
    public void SetEffect(float _blurSpread = 2f, int _iterations = 5, int _downSample = 4,float _luminanceThreshold=.85f,float _luminanceMultiple=10)
    {
        F_BlurSpread = _blurSpread;
        I_Iterations = _iterations;
        I_DownSample = _downSample;
        F_LuminanceThreshold = _luminanceThreshold;
        F_LuminanceMultiple = _luminanceMultiple;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (I_DownSample == 0)
            I_DownSample = 1;
        m_Material.SetFloat("_LuminanceThreshold", F_LuminanceThreshold);
        m_Material.SetFloat("_LuminanceMultiple", F_LuminanceMultiple);
        int rtW = source.width >> I_DownSample;
        int rtH = source.height >> I_DownSample;

        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;
        Graphics.Blit(source, buffer0,m_Material,0);
        for (int i = 0; i < I_Iterations; i++)
        {
            m_Material.SetFloat("BlurSize", 1f+i*F_BlurSpread);
            RenderTexture buffer1;
            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, m_Material, 1);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, m_Material, 2);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
        }

        m_Material.SetTexture("_Bloom", buffer0);
        RenderTexture.ReleaseTemporary(buffer0);
        Graphics.Blit(source, destination, m_Material, 3);
    }
}
public class PE_MotionBlur : PostEffectBase     //Camera Motion Blur ,Easiest
{
    private RenderTexture rt_Accumulation;
    public void SetEffect(float _BlurSize=1f)
    {
        m_Material.SetFloat("_BlurAmount", 1 - _BlurSize);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (rt_Accumulation == null || rt_Accumulation.width != source.width || rt_Accumulation.height != source.height)
        {
            GameObject.Destroy(rt_Accumulation);
            rt_Accumulation = new RenderTexture(source.width,source.height,0);
            rt_Accumulation.hideFlags = HideFlags.DontSave;
            Graphics.Blit(source, rt_Accumulation);
       }

        Graphics.Blit(source, rt_Accumulation, m_Material);
        Graphics.Blit(rt_Accumulation, destination);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if(rt_Accumulation!=null)
        GameObject.Destroy(rt_Accumulation);
    }
}
public class PE_MotionBlurDepth:PE_MotionBlur
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
    private Matrix4x4 mt_CurVP;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        mt_CurVP = m_Manager.m_Camera.projectionMatrix * m_Manager.m_Camera.worldToCameraMatrix;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        m_Material.SetMatrix("_PreviousVPMatrix", mt_CurVP);
        mt_CurVP = m_Manager.m_Camera.projectionMatrix * m_Manager.m_Camera.worldToCameraMatrix;
        m_Material.SetMatrix("_CurrentVPMatrixInverse", mt_CurVP.inverse);
        Graphics.Blit(source,destination,m_Material);
    }
}
public class PE_FogDepth : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
    public override bool m_DepthToWorldMatrix => true;
    public T SetEffect<T>(Color _fogColor,  float _fogDensity = .5f, float _fogYStart = -1f, float _fogYEnd = 5f) where T:PE_FogDepth
    {
        m_Material.SetFloat("_FogDensity", _fogDensity);
        m_Material.SetColor("_FogColor", _fogColor);
        m_Material.SetFloat("_FogStart", _fogYStart);
        m_Material.SetFloat("_FogEnd", _fogYEnd);
        return this as T;
    }
}
public class PE_DepthOutline:PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.DepthNormals;
    public void SetEffect(Color _edgeColor, float _sampleDistance = 1f, float _depthBias=.001f)
    {
        m_Material.SetColor("_EdgeColor", _edgeColor);
        m_Material.SetFloat("_SampleDistance", _sampleDistance);
        m_Material.SetFloat("_DepthBias", _depthBias);
    }
}
public class PE_FogDepthNoise : PE_FogDepth
{
    public void SetEffect(Texture noise, float _noiseLambert = .3f, float _noisePow = 1f,float _fogSpeedX=.02f,float _fogSpeedY=.02f)
    {
        m_Material.SetTexture("_NoiseTex", noise);
        m_Material.SetFloat("_NoiseLambert", _noiseLambert);
        m_Material.SetFloat("_NoisePow", _noisePow);
        m_Material.SetFloat("_FogSpeedX", _fogSpeedX);
        m_Material.SetFloat("_FogSpeedY", _fogSpeedY);
    }
}
public class PE_BloomSpecific : PostEffectBase //Need To Bind Shader To Specific Items
{
    Camera m_RenderCamera;
    RenderTexture m_RenderTexture;
    Shader m_RenderShader;
    public PE_GaussianBlur m_GaussianBlur { get; private set; }
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_GaussianBlur = new PE_GaussianBlur();
        m_GaussianBlur.OnSetEffect(_manager);
        m_RenderShader = Shader.Find("Hidden/PostEffect/PE_BloomSpecific_Render");
        if (m_RenderShader == null)
            Debug.LogError("Null Shader Found!");
        GameObject temp = new GameObject("Render Camera");
        temp.transform.SetParentResetTransform(m_Manager.m_Camera.transform);
        m_RenderCamera = temp.AddComponent<Camera>();
        m_RenderCamera.clearFlags = CameraClearFlags.SolidColor;
        m_RenderCamera.backgroundColor = Color.black;
        m_RenderCamera.orthographic = m_Manager.m_Camera.orthographic;
        m_RenderCamera.orthographicSize = m_Manager.m_Camera.orthographicSize;
        m_RenderCamera.nearClipPlane = m_Manager.m_Camera.nearClipPlane;
        m_RenderCamera.farClipPlane = m_Manager.m_Camera.farClipPlane;
        m_RenderCamera.fieldOfView = m_Manager.m_Camera.fieldOfView;
        m_RenderCamera.enabled = false;
        m_RenderTexture = new RenderTexture(m_Manager.m_Camera.scaledPixelWidth, m_Manager.m_Camera.scaledPixelHeight, 1);
        m_RenderCamera.targetTexture = m_RenderTexture;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_RenderCamera.RenderWithShader(m_RenderShader, "RenderType");
        m_GaussianBlur.OnRenderImage(m_RenderTexture, m_RenderTexture);     //Blur
        m_Material.SetTexture("_RenderTex", m_RenderTexture);
        Graphics.Blit(source, destination, m_Material, 1);        //Mix
    }
}

public class PE_AreaScanDepth : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
    public override bool m_DepthToWorldMatrix => true;
    static readonly int ID_ScanElapse =Shader.PropertyToID("_ScanElapse");
    public void SetElapse(float elapse)
    {
        m_Material.SetFloat(ID_ScanElapse,elapse);
    }
    public void SetEffect(Vector3 origin,Color scanColor,Texture scanTex=null, float _scanTexScale = 15f, float colorLerp=.7f,float width=1f)
    {
        m_Material.SetVector("_ScanOrigin", origin);
        m_Material.SetColor("_ScanColor", scanColor);
        m_Material.SetTexture("_ScanTex", scanTex);
        m_Material.SetFloat("_ScanWidth", width);
        m_Material.SetFloat("_ScanTexScale", _scanTexScale);
        m_Material.SetFloat("_ScanLerp", colorLerp);
    }
}

public class PE_DepthSSAO : PostEffectBase      //Test Currently Uncomplete
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
}
#endregion
#region CommandBuffer
public class CommandBufferBase:CameraEffectBase
{
    protected CommandBuffer m_Buffer;
    protected virtual CameraEvent m_BufferEvent => 0;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Buffer = new CommandBuffer();
        m_Buffer.name = this.GetType().ToString();
        m_Manager.m_Camera.AddCommandBuffer(m_BufferEvent, m_Buffer);
    }
    public override void OnDestroy()
    {
        m_Buffer.Clear();
        m_Manager.m_Camera.RemoveCommandBuffer(m_BufferEvent,m_Buffer);
    }
}

public class CB_GenerateGlobalGaussianBlurTexture : CommandBufferBase
{
    public PE_GaussianBlur m_GaussianBlur { get; private set; }
    protected override CameraEvent m_BufferEvent => CameraEvent.BeforeImageEffects;
    readonly int ID_GlobalBlurTexure = Shader.PropertyToID("_GlobalBlurTexture");
    RenderTexture m_BlurTexture;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_GaussianBlur = new PE_GaussianBlur();
        m_GaussianBlur.OnSetEffect(_manager);
        m_BlurTexture = RenderTexture.GetTemporary(Screen.width,Screen.height);
        Shader.SetGlobalTexture(ID_GlobalBlurTexure, m_BlurTexture);
        m_Buffer.Blit(BuiltinRenderTextureType.CurrentActive , m_BlurTexture);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        //m_GaussianBlur.OnRenderImage(m_BlurTexture, m_BlurTexture);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        RenderTexture.ReleaseTemporary(m_BlurTexture);
    }
}

public class CB_DepthOfFieldSpecificStatic : CommandBufferBase
{
    public PE_GaussianBlur m_GaussianBlur { get; private set; }
    List<Renderer> m_targets = new List<Renderer>();
    protected override CameraEvent m_BufferEvent => CameraEvent.AfterImageEffects;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_GaussianBlur = new PE_GaussianBlur();
        m_GaussianBlur.OnSetEffect(_manager);
    }
    public void SetStaticTarget(params Renderer[] targets)
    {
        targets.Traversal((Renderer renderer)=> {
            if (m_targets.Contains(renderer))
                return;
            m_targets.Add(renderer);
            renderer.enabled = false;
            m_Buffer.DrawRenderer(renderer, renderer.sharedMaterial);
        });
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_GaussianBlur.OnRenderImage(source, destination);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        m_GaussianBlur.OnDestroy();
        m_targets.Traversal((Renderer renderer) =>{
            renderer.enabled = true;
        });
        m_targets.Clear();
    }
}
#endregion