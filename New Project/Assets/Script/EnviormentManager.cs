using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnviormentManager : SingletonMono<EnviormentManager>,ISingleCoroutine
{
    public enum_SkyboxType E_CurrentSkybox;
    public float F_OuterEnviormentScale=0;
    SkyboxSetting m_currentSkybox;
    Light m_DirectionalLight;
//    OuterEnviorment m_outerEnviorment;
    public enum enum_SkyboxType
    {
        Invalid=-1,
        BlackMesa,
        DayMountain,
        NightCloudless,
        NightCloudful,
    }
    protected override void Awake()
    {
        instance = this;
        m_DirectionalLight = transform.Find("DirectionalLight").GetComponent<Light>();
 //       m_outerEnviorment =new OuterEnviorment( transform.Find("OuterEnviorment"), F_OuterEnviormentScale);
        SetSkybox(E_CurrentSkybox);
    }
    void SetSkybox(enum_SkyboxType type)
    {
        E_CurrentSkybox = type;
        this.StartSingleCoroutine(0,LoadResourcesAsync("Skybox/"+type.ToString(),(SkyboxSetting setting)=> {
            m_currentSkybox = setting;
            RenderSettings.skybox = m_currentSkybox.m_SkyboxMaterial;
            m_DirectionalLight.intensity = m_currentSkybox.m_DirectionalLightIntensity;
            m_DirectionalLight.color = m_currentSkybox.m_DirectionalLightColor;
        }));
    }
    IEnumerator LoadResourcesAsync<T>(string path,Action<T> OnLoadFinished) where T:ScriptableObject
    {
        ResourceRequest request = Resources.LoadAsync(path);
        yield return request;
        OnLoadFinished(request.asset as T);
    }
    protected void Update()
    {
        if (m_currentSkybox != null)
            RenderSettings.skybox.SetFloat("_Rotation", m_currentSkybox.m_RotationPerSecond*Time.time);
    }
    //protected void LateUpdate()
    //{
    //    m_outerEnviorment.TickLateUpdate();
    //}
    public void RandomSkybox()
    {
        for (; ; )
        {
            enum_SkyboxType type = (enum_SkyboxType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(enum_SkyboxType)).Length - 1);
            if (type != E_CurrentSkybox)
            {
                SetSkybox(type);
                return;
            }
        }
    }
}

//public class OuterEnviorment
//{
//    public Transform transform { get; private set; }
//    Camera MainCamera;
//    float groundScale;
//    public OuterEnviorment(Transform _transform,float _groundScale=0f)
//    {
//        transform = _transform;
//        MainCamera = transform.Find("Camera").GetComponent<Camera>();
//        groundScale = _groundScale;
//    }

//    public void TickLateUpdate()
//    {
//        MainCamera.transform.rotation = CameraController.CameraRotation;
//        MainCamera.transform.localPosition = CameraController.MainCamera.transform.position * groundScale;
//        MainCamera.fieldOfView = CameraController.MainCamera.fieldOfView;
//    }
//}
