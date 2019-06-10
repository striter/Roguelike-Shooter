using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostEffectManager : SingletonMono<PostEffectManager> {
    #region Test
    //public float F_Test1;
    //public float F_Test2;
    //public float F_Test3;
    //public float F_Test4;
    //public Color C_Test1;
    //public Color C_Test2;
    //public Texture Tx_Test1;
    //protected void Update()
    //{
    //    if (peb_curEffect as PE_Bloom != null)
    //    {
    //        (peb_curEffect as PE_Bloom).I_DownSample = (int)F_Test1;
    //        (peb_curEffect as PE_Bloom).I_Iterations = (int)F_Test2;
    //        (peb_curEffect as PE_Bloom).F_BlurSpread = F_Test3;
    //        (peb_curEffect as PE_Bloom).F_LuminanceThreshold = F_Test4;

    //    }
    //}
    #endregion

    public T SetPostEffect<T>() where T:PostEffectBase,new()
    {

        if (peb_curEffect != null)
            peb_curEffect.OnDestroy();

        T effetBase = new T();
        peb_curEffect = effetBase;
        peb_curEffect.OnSetCamera(cam_cur);
        return effetBase;
    }
    PostEffectBase peb_curEffect;
    Camera cam_cur;
    protected override void Awake()
    {
        instance = this;
        cam_cur = GetComponent<Camera>();
    }
    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (peb_curEffect != null && peb_curEffect.B_Supported)
            peb_curEffect.OnRenderImage(source, destination);
        else
            Graphics.Blit(source, destination);
    }
}
