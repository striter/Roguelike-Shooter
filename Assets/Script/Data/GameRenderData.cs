using LPWAsset;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRenderData : ScriptableObject
{
    public float f_pitch;
    public float f_yaw;
    public float f_lightItensity;
    public float f_lambert;
    public Color c_lightColor;
    public Color c_ambient;
    public Color c_skyColor;
    public Color c_shadowColor;

    public static GameRenderData Default()
    {
        GameRenderData defaultData = CreateInstance<GameRenderData>();
        defaultData.c_lightColor = Color.white;
        defaultData.f_pitch = 45;
        defaultData.f_yaw = 60;
        defaultData.f_lightItensity = .8f;
        defaultData.f_lambert = .8f;
        defaultData.c_ambient = Color.grey;
        defaultData.c_skyColor = Color.white;
        defaultData.c_shadowColor = Color.black;
        return defaultData;
    }

    public void DataInit(Light directionalLight,Camera camera)
    {
        directionalLight.color = c_lightColor;
        directionalLight.intensity = f_lightItensity;
        directionalLight.transform.rotation = Quaternion.Euler(f_pitch,f_yaw,0);
        RenderSettings.ambientSkyColor = c_ambient;
        Shader.SetGlobalFloat("_Lambert", f_lambert);
        Shader.SetGlobalColor("_SkyColor", c_skyColor);
        Shader.SetGlobalColor("_ShadowColor", c_shadowColor);
        camera.backgroundColor = c_skyColor;
    }
}
