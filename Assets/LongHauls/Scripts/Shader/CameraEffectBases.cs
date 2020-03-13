using System;
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
    public bool m_Enabled { get; protected set; }
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
        m_Enabled = true;
    }
    public virtual void OnRenderObject()
    {

    }
    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination);
    }
    public virtual void OnCheckMobileCostEnable(bool enable) { }
    public virtual void OnDestroy()
    {
    }
}
#region PostEffect
public class PostEffectBase: CameraEffectBase
{
    const string S_ParentPath = "Hidden/PostEffect/";
    public Material m_Material { get; private set; }
    protected override bool OnCreate()
    {
        m_Material = CreateMaterial(this.GetType());
        return m_Material!=null;
    }

    public static Material CreateMaterial(Type type)
    {
        try
        {
            Shader shader = Shader.Find(S_ParentPath + type.ToString());
            if (shader == null)
                throw new Exception("Shader:" + S_ParentPath + type.ToString() + " Not Found");
            if (!shader.isSupported)
                throw new Exception("Shader:" + S_ParentPath + type.ToString() + " Is Not Supported");

            return new Material(shader) { hideFlags = HideFlags.DontSave };
        }
        catch(Exception e)
        {
            Debug.LogError("Post Effect Error:" + e.Message);
            return null;
        }
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
public class PE_DistortVortex : PostEffectBase
{
    static readonly int ID_DistortParam = Shader.PropertyToID("_DistortParam");
    public void SetTexture(Texture noise, float _noiseStrength = 1f)
    {
        m_Material.SetTexture("_NoiseTex", noise);
        m_Material.SetFloat("_NoiseStrength", _noiseStrength);
    }
    public void SetDistort(Vector2 playerViewPort, float distortFactor)
    {
        m_Material.SetVector(ID_DistortParam, new Vector4(playerViewPort.x, playerViewPort.y, distortFactor));
    }
}
public class PE_Blurs : PostEffectBase       //Blur Base Collection
{
    public enum enum_BlurType
    {
        Invalid=-1,
        AverageBlur,
        GaussianBlur,
    }
    enum enum_BlurPass
    {
        Invalid=-1,
        Average=0,
        GaussianHorizontal=1,
        GaussianVertical=2,
    }
    public enum_BlurType m_BlurType { get; private set; } = enum_BlurType.Invalid;
    float F_BlurSpread;
    int I_Iterations;
    RenderTexture buffer0, buffer1;
    int rtW, rtH;
    public void SetEffect(enum_BlurType blurType, float _blurSpread=2f, int _iterations=5, int _downSample = 4)
    {
        m_BlurType = blurType;
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
        if (m_BlurType == enum_BlurType.Invalid)
        {
            Debug.LogError("Invalid Blur Detected!");
            Graphics.Blit(source, destination);
            return;
        }

        RenderTexture target = null;
        Graphics.Blit(source, buffer0);
        for (int i = 0; i < I_Iterations; i++)
        {
            m_Material.SetFloat("_BlurSpread", 1 + i * F_BlurSpread);
            switch (m_BlurType)
            {
                case enum_BlurType.AverageBlur:
                    Graphics.Blit(buffer0, buffer1, m_Material, (int)enum_BlurPass.Average);
                    target = buffer1;
                    break;
                case enum_BlurType.GaussianBlur:
                    Graphics.Blit(buffer0, buffer1, m_Material,(int)enum_BlurPass.GaussianHorizontal);
                    Graphics.Blit(buffer1, buffer0, m_Material, (int)enum_BlurPass.GaussianVertical);
                    target = buffer0;
                    break;
            }
        }
        Graphics.Blit(target, destination);
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
public class PE_FogDepthNoise : PE_FogDepth
{
    public void SetEffect(Texture noise, float _noiseLambert = .3f, float _noisePow = 1f, float _fogSpeedX = .02f, float _fogSpeedY = .02f)
    {
        m_Material.SetTexture("_NoiseTex", noise);
        m_Material.SetFloat("_NoiseLambert", _noiseLambert);
        m_Material.SetFloat("_NoisePow", _noisePow);
        m_Material.SetFloat("_FogSpeedX", _fogSpeedX);
        m_Material.SetFloat("_FogSpeedY", _fogSpeedY);
    }
}
public class PE_FocalDepth : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
    public PE_Blurs m_Blur { get; private set; }
    RenderTexture m_TempTexture;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Blur = new PE_Blurs();
        m_Blur.OnSetEffect(_manager);
    }
    public void SetEffect(int downSample=2)
    {
        m_Blur.SetEffect( PE_Blurs.enum_BlurType.GaussianBlur,2, 3, downSample);
        m_TempTexture = RenderTexture.GetTemporary(m_Manager.m_Camera.scaledPixelHeight >> downSample, m_Manager.m_Camera.scaledPixelWidth >> downSample);
        m_Material.SetTexture("_BlurTex",m_TempTexture);
    }
    public void SetFocalTarget(Vector3 focalTarget, float focalWidth)
    {
        float _01Depth = m_Manager.Get01Depth(focalTarget);
        float _01Width = m_Manager.Get01DepthLength(focalWidth);
        m_Material.SetFloat("_FocalDepthStart",_01Depth-_01Width );
        m_Material.SetFloat("_FocalDepthEnd", _01Depth+_01Width);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_Blur.OnRenderImage(source, m_TempTexture);
        base.OnRenderImage(source, destination);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        RenderTexture.ReleaseTemporary(m_TempTexture);
        m_Blur.OnDestroy();
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
public class PE_BloomSpecific : PostEffectBase //Need To Bind Shader To Specific Items
{
    Camera m_RenderCamera;
    RenderTexture m_RenderTexture;
    Shader m_RenderBloomShader,m_RenderOcclusionShader;
    public PE_Blurs m_Blur { get; private set; }
    public bool m_OccludeEnabled { get; private set; }
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Blur = new PE_Blurs();
        m_Blur.OnSetEffect(_manager);
        m_RenderBloomShader = Shader.Find("Hidden/PostEffect/PE_BloomSpecific_Render_Bloom");
        m_RenderOcclusionShader = Shader.Find("Hidden/PostEffect/PE_BloomSpecific_Render_Occlusion");
        if (m_RenderBloomShader == null||m_RenderOcclusionShader==null)
            Debug.LogError("Null Blom Specific Shader Found!");

        GameObject temp = new GameObject("Render Camera");
        temp.transform.SetParentResetTransform(m_Manager.m_Camera.transform);
        m_RenderCamera = temp.AddComponent<Camera>();
        m_RenderCamera.backgroundColor = Color.black;
        m_RenderCamera.orthographic = m_Manager.m_Camera.orthographic;
        m_RenderCamera.orthographicSize = m_Manager.m_Camera.orthographicSize;
        m_RenderCamera.nearClipPlane = m_Manager.m_Camera.nearClipPlane;
        m_RenderCamera.farClipPlane = m_Manager.m_Camera.farClipPlane;
        m_RenderCamera.fieldOfView = m_Manager.m_Camera.fieldOfView;
        m_RenderCamera.depthTextureMode = DepthTextureMode.None;
        m_RenderCamera.enabled = false;
        m_RenderTexture = RenderTexture.GetTemporary(m_Manager.m_Camera.scaledPixelWidth, m_Manager.m_Camera.scaledPixelHeight, 1);
        m_RenderCamera.targetTexture = m_RenderTexture;
    }
    public override void OnCheckMobileCostEnable(bool enable)
    {
        base.OnCheckMobileCostEnable(enable);
        m_OccludeEnabled = enable;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_RenderCamera.clearFlags = CameraClearFlags.SolidColor;
        if (m_OccludeEnabled)
        {
            m_RenderCamera.SetReplacementShader(m_RenderOcclusionShader, "RenderType");
            m_RenderCamera.Render();
            m_RenderCamera.clearFlags = CameraClearFlags.Nothing;
        }
        m_RenderCamera.SetReplacementShader(m_RenderBloomShader, "RenderType");
        m_RenderCamera.Render();
        m_Blur.OnRenderImage(m_RenderTexture, m_RenderTexture);     //Blur
        m_Material.SetTexture("_RenderTex", m_RenderTexture);
        Graphics.Blit(source, destination, m_Material, 1);        //Mix
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        GameObject.Destroy(m_RenderCamera.gameObject);
        m_Blur.OnDestroy();
        RenderTexture.ReleaseTemporary(m_RenderTexture);
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
public class PE_DepthSSAO : PostEffectBase
{
    public override DepthTextureMode m_DepthTextureMode => DepthTextureMode.Depth;
    public void SetEffect(float strength=1f, float sphereRadius= 0.02f,float _fallOff=0.00001f,float _fallOffLimit= 0.003f, int _sampleCount=16)
    {
        Vector4[] array = new Vector4[16] {
            new Vector3( 0.5381f, 0.1856f,-0.4319f),  new Vector3( 0.1379f, 0.2486f, 0.4430f),new Vector3( 0.3371f, 0.5679f,-0.0057f),  new Vector3(-0.6999f,-0.0451f,-0.0019f),
            new Vector3( 0.0689f,-0.1598f,-0.8547f),  new Vector3( 0.0560f, 0.0069f,-0.1843f),new Vector3(-0.0146f, 0.1402f, 0.0762f),  new Vector3( 0.0100f,-0.1924f,-0.0344f),
            new Vector3(-0.3577f,-0.5301f,-0.4358f),  new Vector3(-0.3169f, 0.1063f, 0.0158f),new Vector3( 0.0103f,-0.5869f, 0.0046f),  new Vector3(-0.0897f,-0.4940f, 0.3287f),
            new Vector3( 0.7119f,-0.0154f,-0.0918f),  new Vector3(-0.0533f, 0.0596f,-0.5411f),new Vector3( 0.0352f,-0.0631f, 0.5460f),  new Vector3(-0.4776f, 0.2847f,-0.0271f)};
        for (int i = 0; i < array.Length; i++)
            array[i] *= sphereRadius;
        m_Material.SetInt("_SampleCount", _sampleCount);
        m_Material.SetVectorArray("_SampleSphere", array);

        m_Material.SetFloat("_Strength", strength);
        m_Material.SetFloat("_FallOff", _fallOff);
        m_Material.SetFloat("_FallOffLimit", _fallOffLimit);
    }
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

public class CB_GenerateOpaqueTexture:CommandBufferBase
{
    readonly int ID_GlobalOpaqueTexture = Shader.PropertyToID("_CameraOpaqueTexture");
    protected override CameraEvent m_BufferEvent =>CameraEvent.BeforeForwardAlpha;
    readonly int ID_TempTexture = Shader.PropertyToID("_OpaqueTempRT");
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Buffer.GetTemporaryRT(ID_TempTexture,-2,-2,0, FilterMode.Bilinear);
        m_Buffer.Blit(BuiltinRenderTextureType.CurrentActive,ID_TempTexture);
        m_Buffer.SetGlobalTexture(ID_GlobalOpaqueTexture, ID_TempTexture);
    }
}

public class CB_GenerateOverlayUIGrabBlurTexture : CommandBufferBase
{
    public PE_Blurs m_GaussianBlur { get; private set; }
    protected override CameraEvent m_BufferEvent => CameraEvent.BeforeImageEffects;
    readonly int ID_GlobalBlurTexure = Shader.PropertyToID("_CameraUIOverlayBlurTexture");
    readonly int ID_TempTexture1 = Shader.PropertyToID("_UIBlurTempRT1");
    readonly int ID_TempTexture2 = Shader.PropertyToID("_UIBlurTempRT2");
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_GaussianBlur = new PE_Blurs();
        m_GaussianBlur.OnSetEffect(_manager);
    }
    public void SetEffect(int iterations=3, float blurSpread=1.5f,int _downSample=2)
    {
        m_GaussianBlur.SetEffect( PE_Blurs.enum_BlurType.GaussianBlur, blurSpread);
        m_Buffer.GetTemporaryRT(ID_TempTexture1, -_downSample, -_downSample, 0, FilterMode.Bilinear);
        m_Buffer.GetTemporaryRT(ID_TempTexture2, -_downSample, -_downSample, 0, FilterMode.Bilinear);
        m_Buffer.Blit(BuiltinRenderTextureType.CurrentActive, ID_TempTexture1);
        for (int i = 0; i < iterations; i++)
        {
            m_Buffer.Blit(ID_TempTexture1, ID_TempTexture2, m_GaussianBlur.m_Material, 1);
            m_Buffer.Blit(ID_TempTexture2, ID_TempTexture1, m_GaussianBlur.m_Material, 2);
        }
        m_Buffer.SetGlobalTexture(ID_GlobalBlurTexure, ID_TempTexture1);
    }
    public override void OnDestroy()
    {
        m_Buffer.ReleaseTemporaryRT(ID_TempTexture1);
        m_Buffer.ReleaseTemporaryRT(ID_TempTexture2);
        base.OnDestroy();
        m_GaussianBlur.OnDestroy();
    }
}

public class CB_DepthOfFieldSpecificStatic : CommandBufferBase
{
    public PE_Blurs m_GaussianBlur { get; private set; }
    List<Renderer> m_targets = new List<Renderer>();
    protected override CameraEvent m_BufferEvent => CameraEvent.AfterImageEffects;
    public override void OnSetEffect(CameraEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_GaussianBlur = new PE_Blurs();
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