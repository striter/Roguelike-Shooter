using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectBase  {
    const string S_ParentPath = "Hidden/PostEffect/";
    public Material m_Material { get; private set; }
    protected PostEffectManager m_Manager { get; private set; }
    public bool m_Supported { get; private set; }
    public PostEffectBase()
    {
        m_Supported = true;
        Shader shader = Shader.Find(S_ParentPath+this.GetType().ToString());
        if (shader == null)
        {
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString()+" Not Found");
            m_Supported = false;
        }
        else if (!shader.isSupported)
        {
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString() + " Is Not Supported");
            m_Supported = false;
        }
        m_Material = new Material(shader)
        {
            hideFlags = HideFlags.DontSave
        };
    }
    protected bool RenderDefaultImage(RenderTexture source, RenderTexture destination)
    {
        if (!m_Supported)
        {
            Graphics.Blit(source, destination);
            return true;
        }
        return false;
    }
    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (RenderDefaultImage(source, destination)) 
            return;
        Graphics.Blit(source, destination, m_Material);
    }
    public virtual void OnSetEffect(PostEffectManager _manager)
    {
        m_Manager = _manager;
    }
    public virtual void OnDestroy()
    {
        GameObject.Destroy(m_Material);
    }
}
public class PE_ViewNormal : PostEffectBase
{
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.m_Camera.depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
public class PE_ViewDepth : PostEffectBase
{
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
    }
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
    float F_BlurSpread = 2f;
    int I_Iterations = 5;
    int I_DownSample = 4;
    public void SetEffect(float _blurSpread=2f, int _iterations=5, int _downSample = 4)
    {
        F_BlurSpread = _blurSpread;
        I_Iterations = _iterations;
        I_DownSample = _downSample;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (RenderDefaultImage(source,destination))
            return;


        if (I_DownSample == 0)
            I_DownSample = 1;

        int rtW = source.width >> I_DownSample;
        int rtH = source.height >> I_DownSample;

        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, buffer0);
        for (int i = 0; i < I_Iterations; i++)
        {
            m_Material.SetFloat("_BlurSpread", 1 + i * F_BlurSpread);

            RenderTexture buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, m_Material, 0);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW,rtH,0);
            Graphics.Blit(buffer0, buffer1, m_Material, 1);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
        }
        Graphics.Blit(buffer0, destination);
        RenderTexture.ReleaseTemporary(buffer0);
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
        if (RenderDefaultImage(source, destination))
            return;

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

        rt_Accumulation.MarkRestoreExpected();

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
    private Matrix4x4 mt_CurVP;
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
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

    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.calculateDepthWorldPos |= true;
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
        SetEffect(TCommon.ColorAlpha( Color.white,.5f));
    }
    public PE_FogDepth SetEffect(Color _fogColor,  float _fogDensity = .5f, float _fogYStart = -1f, float _fogYEnd = 5f)
    {
        m_Material.SetFloat("_FogDensity", _fogDensity);
        m_Material.SetColor("_FogColor", _fogColor);
        m_Material.SetFloat("_FogStart", _fogYStart);
        m_Material.SetFloat("_FogEnd", _fogYEnd);
        return this;
    }
    public void SetTexture(Texture noise,float _noiseLambert=.3f,float _noisePow=1f)
    {
        m_Material.SetTexture("_NoiseTex", noise);
        m_Material.SetFloat("_NoiseLambert", _noiseLambert);
        m_Material.SetFloat("_NoisePow", _noisePow);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (RenderDefaultImage(source,destination))
            return;
        
        m_Material.SetMatrix("_FrustumCornersRay",m_Manager.m_FrustumCornorsRay);
        m_Material.SetMatrix("_VPMatrixInverse",m_Manager.m_ViewProjectionMatrixInverse);
        Graphics.Blit(source,destination,m_Material);
    }
}
public class PE_DepthOutline:PostEffectBase
{
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.DepthNormals;
        SetEffect(Color.black);
    }
    public void SetEffect(Color _edgeColor, float _sampleDistance = 1f, float _depthBias=.001f)
    {
        m_Material.SetColor("_EdgeColor", _edgeColor);
        m_Material.SetFloat("_SampleDistance", _sampleDistance);
        m_Material.SetFloat("_DepthBias", _depthBias);
    }
}
public class PE_FogDepthNoise : PE_FogDepth
{
    public Texture TX_Noise;
    public float F_FogSpeedX=.02f;
    public float F_FogSpeedY=.02f;
    public float F_NoiseAmount=.8f;

    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Material.SetTexture("_NoiseTex", TX_Noise);
        m_Material.SetFloat("_FogSpeedX", F_FogSpeedX);
        m_Material.SetFloat("_FogSpeedY", F_FogSpeedY);
        m_Material.SetFloat("_NoiseAmount", F_NoiseAmount);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
    }
}
public class PE_BloomSpecific : PostEffectBase //Need To Bind Shader To Specific Items
{
    Camera m_RenderCamera;
    RenderTexture m_RenderTexture;
    Shader m_RenderShader;
    public PE_GaussianBlur m_GaussianBlur { get; private set; }

    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_RenderShader = Shader.Find("Hidden/PostEffect/PE_BloomSpecific_Render");
        if (m_RenderShader == null)
            Debug.LogError("Null Shader Found!");
        m_GaussianBlur = new PE_GaussianBlur();
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
        SetEffect();
    }
    public void SetEffect(float _blueSpread=1f, int _iterations = 10, int _downSample=2)
    {
        m_GaussianBlur.SetEffect(_blueSpread, _iterations, _downSample);
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (RenderDefaultImage(source, destination))
            return;
        m_RenderCamera.RenderWithShader(m_RenderShader, "RenderType");
        m_GaussianBlur.OnRenderImage(m_RenderTexture, m_RenderTexture);     //Blur
        m_Material.SetTexture("_RenderTex", m_RenderTexture);
        Graphics.Blit(source, destination, m_Material, 1);        //Mix
    }
}
public class PE_DepthSSAO : PostEffectBase
{
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
    }
}
public class PE_AreaScanDepth : PostEffectBase
{
    static readonly int ID_ScanElapse =Shader.PropertyToID("_ScanElapse");
    public override void OnSetEffect(PostEffectManager _manager)
    {
        base.OnSetEffect(_manager);
        m_Manager.calculateDepthWorldPos |= true;
        m_Manager.m_Camera.depthTextureMode |= DepthTextureMode.Depth;
    }
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
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);

        m_Material.SetMatrix("_FrustumCornersRay", m_Manager.m_FrustumCornorsRay);
        m_Material.SetMatrix("_VPMatrixInverse", m_Manager.m_ViewProjectionMatrixInverse);
    }
}