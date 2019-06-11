using UnityEngine;

[RequireComponent(typeof(Camera))]
public class PostEffectManager : SingletonMono<PostEffectManager> {
    public static T SetPostEffect<T>() where T:PostEffectBase,new()
    {
        if (instance.peb_curEffect != null)
            instance.peb_curEffect.OnDestroy();

        T effetBase = new T();
        instance.peb_curEffect = effetBase;
        instance.peb_curEffect.OnSetCamera(instance.cam_cur);
        return effetBase;
    }
    PostEffectBase peb_curEffect;
    Camera cam_cur;
    protected override void Awake()
    {
        instance = this;
        cam_cur = GetComponent<Camera>();
    }
    public virtual void LateUpdate()
    {
        if(peb_curEffect!=null&&peb_curEffect.B_Supported)
            peb_curEffect.LateUpdate();
    }
    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (peb_curEffect != null && peb_curEffect.B_Supported)
            peb_curEffect.OnRenderImage(source, destination);
        else
            Graphics.Blit(source, destination);
    }

    #region Test
#if UNITY_EDITOR
    public bool b_testMode = true;
    public float F_Test1;
    public float F_Test2;
    public float F_Test3;
    public float F_Test4;
    public float F_Test5;
    public Color C_Test1;
    public Color C_Test2;
    public Texture Tx_Test1;
    protected void Update()
    {
        if (!b_testMode)
            return;
        if (peb_curEffect as PE_Bloom != null)
        {
            (peb_curEffect as PE_Bloom).I_DownSample = (int)F_Test1;
            (peb_curEffect as PE_Bloom).I_Iterations = (int)F_Test2;
            (peb_curEffect as PE_Bloom).F_BlurSpread = F_Test3;
            (peb_curEffect as PE_Bloom).F_LuminanceThreshold = F_Test4;
            (peb_curEffect as PE_Bloom).F_LuminanceMultiple = F_Test5;
        }
    }
#endif
    #endregion
}
