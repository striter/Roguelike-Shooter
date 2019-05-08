using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffectBase {
    const string S_ParentPath = "PostEffect/";
    Camera cam_Cur;
    Material mat_Cur;
    Shader sd_Cur;
    bool b_supported;
    public PostEffectBase()
    {
        b_supported = true;
        sd_Cur = Shader.Find(S_ParentPath+this.GetType().ToString());
        if (sd_Cur == null)
        {
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString()+" Not Found");
            b_supported = false;
        }
        else if (!sd_Cur.isSupported)
        {
            Debug.LogError("Shader:" + S_ParentPath + this.GetType().ToString() + " Is Not Supported");
            b_supported = false;
        }
        mat_Cur = new Material(sd_Cur);
        mat_Cur.hideFlags = HideFlags.DontSave;
    }
    public virtual void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

    }
    public virtual void OnSetCamera(Camera cam)
    {
        cam_Cur = cam;
    }
    public virtual void OnDestroy()
    {
        GameObject.Destroy(mat_Cur);
    }
    #region Get
    protected Material Mat_Cur
    {
        get
        {
            return mat_Cur;
        }
    }
    protected Shader Sd_Cur
    {
        get
        {
            return sd_Cur;
        }
    }
    public bool B_Supported
    {
        get
        {
            return b_supported;
        }
    }
    public Camera Cam_Cur
    {
        get
        {
            return cam_Cur;
        }
    }
    #endregion
}

public class PE_BSC : PostEffectBase {      //Brightness Saturation Contrast
    public float F_Brightness = 1f;
    public float F_Saturation = 1f;
    public float F_Contrast = 1f;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        Mat_Cur.SetFloat("_Brightness", F_Brightness);
        Mat_Cur.SetFloat("_Saturation", F_Saturation);
        Mat_Cur.SetFloat("_Contrast", F_Contrast);

        Graphics.Blit(source, destination, Mat_Cur);
    }
}
public class PE_EdgeDetection : PostEffectBase  //Edge Detection Easiest
{
    public float F_ShowEdge = 1;
    public Color C_EdgeColor = Color.green;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        Mat_Cur.SetColor("_EdgeColor", C_EdgeColor);
        Mat_Cur.SetFloat("_ShowEdge", F_ShowEdge);
        Graphics.Blit(source,destination,Mat_Cur);
    }
}
public class PE_GaussianBlur : PostEffectBase       //Gassuain Blur
{
    public float F_BlurSpread = 2f;
    public int I_Iterations = 5;
    public int I_DownSample = 4;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        if (I_DownSample == 0)
            I_DownSample = 1;

        int rtW = source.width / I_DownSample;
        int rtH = source.height / I_DownSample;

        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;

        Graphics.Blit(source, buffer0);
        for (int i = 0; i < I_Iterations; i++)
        {
            Mat_Cur.SetFloat("_BlurSpread", 1 + i * F_BlurSpread);

            RenderTexture buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, Mat_Cur, 0);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW,rtH,0);
            Graphics.Blit(buffer0, buffer1, Mat_Cur, 1);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
        }

        Graphics.Blit(buffer0, destination);
        RenderTexture.ReleaseTemporary(buffer0);
    }
}
public class PE_Bloom : PostEffectBase
{
    public int I_Iterations = 3;
    public float F_BlurSpread = .6f;
    public int I_DownSample = 2;
    public float F_LuminanceThreshold = .6f;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        if (I_DownSample == 0)
            I_DownSample = 1;
        Mat_Cur.SetFloat("_LuminanceThreshold", F_LuminanceThreshold);
        int rtW = source.width / I_DownSample;
        int rtH = source.height / I_DownSample;

        RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 0);
        buffer0.filterMode = FilterMode.Bilinear;
        Graphics.Blit(source, buffer0,Mat_Cur,0);
        for (int i = 0; i < I_Iterations; i++)
        {
            Mat_Cur.SetFloat("BlurSize", 1f+i*F_BlurSpread);
            RenderTexture buffer1;
            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, Mat_Cur, 1);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;

            buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);
            Graphics.Blit(buffer0, buffer1, Mat_Cur, 2);
            RenderTexture.ReleaseTemporary(buffer0);
            buffer0 = buffer1;
        }

        Mat_Cur.SetTexture("_Bloom", buffer0);
        Graphics.Blit(source, destination, Mat_Cur, 3);
        RenderTexture.ReleaseTemporary(buffer0);
    }
}
public class PE_MotionBlur : PostEffectBase     //Camera Motion Blur ,Easiest
{
    public float F_BlurSize=.5f;
    private RenderTexture rt_Accumulation;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        if (rt_Accumulation == null || rt_Accumulation.width != source.width || rt_Accumulation.height != source.height)
        {
            GameObject.Destroy(rt_Accumulation);
            rt_Accumulation = new RenderTexture(source.width,source.height,0);
            rt_Accumulation.hideFlags = HideFlags.DontSave;
            Graphics.Blit(source, rt_Accumulation);
       }

        rt_Accumulation.MarkRestoreExpected();
        Mat_Cur.SetFloat("_BlurAmount", 1 -F_BlurSize);

        Graphics.Blit(source, rt_Accumulation, Mat_Cur);
        Graphics.Blit(rt_Accumulation, destination);
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        if(rt_Accumulation!=null)
        GameObject.Destroy(rt_Accumulation);
    }
}

public class PE_ViewNormal : PostEffectBase {
    public override void OnSetCamera(Camera cam)
    {
        base.OnSetCamera(cam);
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        Graphics.Blit(source, destination, Mat_Cur);
    }
}
public class PE_ViewDepth : PostEffectBase{
    public override void OnSetCamera(Camera cam)
    {
        base.OnSetCamera(cam);
        cam.depthTextureMode = DepthTextureMode.Depth;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        Graphics.Blit(source,destination,Mat_Cur);
    }
}
public class PE_MotionBlurDepth:PE_MotionBlur
{
    private Matrix4x4 mt_CurVP;
    public override void OnSetCamera(Camera cam)
    {
        base.OnSetCamera(cam);
        cam.depthTextureMode = DepthTextureMode.Depth;
        mt_CurVP = Cam_Cur.projectionMatrix * Cam_Cur.worldToCameraMatrix;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);
        Mat_Cur.SetFloat("_BlurSize", F_BlurSize);
        Mat_Cur.SetMatrix("_PreviousVPMatrix", mt_CurVP);
        mt_CurVP = Cam_Cur.projectionMatrix * Cam_Cur.worldToCameraMatrix;
        Mat_Cur.SetMatrix("_CurrentVPMatrixInverse", mt_CurVP.inverse);
        Graphics.Blit(source,destination,Mat_Cur);
    }
}
public class PE_FogDepth : PostEffectBase
{
    Transform tra_Cam;
    public float F_FogDensity = 1f;
    public Color C_FogColor = Color.white;
    public float F_FogStart = 0f;
    public float F_FogEnd = 2f;
    public override void OnSetCamera(Camera cam)
    {
        base.OnSetCamera(cam);
        cam.depthTextureMode = DepthTextureMode.Depth;
        tra_Cam = Cam_Cur.transform;
    }

    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        base.OnRenderImage(source, destination);

        float fov = Cam_Cur.fieldOfView;
        float near = Cam_Cur.nearClipPlane;
        float far = Cam_Cur.farClipPlane;
        float aspect = Cam_Cur.aspect;

        float halfHeight = near * Mathf.Tan(fov * .5f * Mathf.Deg2Rad) ;
        Vector3 toRight = tra_Cam.right * halfHeight * aspect;
        Vector3 toTop = tra_Cam.up * halfHeight ;

        Vector3 topLeft = tra_Cam.forward * near + toTop - toRight;
        float scale = topLeft.magnitude / near;
        topLeft.Normalize();
        topLeft *= scale;

        Vector3 topRight = tra_Cam.forward * near + toTop + toRight;
        topRight.Normalize();
        topRight *= scale;

        Vector3 bottomLeft = tra_Cam.forward * near - toTop - toRight;
        bottomLeft.Normalize();
        bottomLeft *= scale;
        Vector3 bottomRight = tra_Cam.forward * near - toTop + toRight;
        bottomRight.Normalize();
        bottomRight *= scale;

        Matrix4x4 frustumCornersRay = Matrix4x4.identity;
        frustumCornersRay.SetRow(0, bottomLeft);
        frustumCornersRay.SetRow(1, bottomRight);
        frustumCornersRay.SetRow(2, topLeft);
        frustumCornersRay.SetRow(3, topRight);

        Mat_Cur.SetMatrix("_FrustumCornersRay", frustumCornersRay);
        Mat_Cur.SetMatrix("_VPMatrixInverse", (Cam_Cur.projectionMatrix * Cam_Cur.worldToCameraMatrix).inverse);
        Mat_Cur.SetFloat("_FogDensity", F_FogDensity);
        Mat_Cur.SetColor("_FogColor", C_FogColor);
        Mat_Cur.SetFloat("_FogStart", F_FogStart);
        Mat_Cur.SetFloat("_FogEnd", F_FogEnd);
        Graphics.Blit(source,destination,Mat_Cur);
    }
}
public class PE_EdgeDetectionDepth:PE_EdgeDetection
{
    public float F_SampleDistance = 1f;
    public float F_SensitivityDepth = 1f;
    public float F_SensitivityNormals = 1f;
    public override void OnSetCamera(Camera cam)
    {
        base.OnSetCamera(cam);
        cam.depthTextureMode = DepthTextureMode.DepthNormals;
    }
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {

        Mat_Cur.SetFloat("_SampleDistance", F_SampleDistance);
        Mat_Cur.SetFloat("_SensitivityDepth", F_SensitivityDepth);
        Mat_Cur.SetFloat("_SensitivityNormals", F_SensitivityNormals);
        base.OnRenderImage(source, destination);
    }
}
public class PE_FogDepthNoise : PE_FogDepth
{
    public Texture TX_Noise;
    public float F_FogSpeedX=.1f;
    public float F_FogSpeedY=.1f;
    public float F_NoiseAmount=1;
    public override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Mat_Cur.SetTexture("_NoiseTex", TX_Noise);
        Mat_Cur.SetFloat("_FogSpeedX", F_FogSpeedX);
        Mat_Cur.SetFloat("_FogSpeedY", F_FogSpeedY);
        Mat_Cur.SetFloat("_NoiseAmount", F_NoiseAmount);
        base.OnRenderImage(source, destination);
    }
}