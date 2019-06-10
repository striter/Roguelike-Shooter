﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleColorData : ScriptableObject
{
    [SerializeField]
    public Color c_directionnal;
    [SerializeField]
    public Vector3 v3_eulerAngle;
    [SerializeField]
    public float f_directionalIntensity;
    [SerializeField]
    public Color c_ambientSky;
    [SerializeField]
    public Color c_ambientEquator;
    [SerializeField]
    public Color c_ambientGround;
    [SerializeField]
    public Color c_ocean;
    public static StyleColorData Default()
    {
        StyleColorData defaultData = CreateInstance<StyleColorData>();
        defaultData.c_directionnal = Color.white;
        defaultData.v3_eulerAngle = new Vector3(45, 60, 0);
        defaultData.f_directionalIntensity = .8f;
        defaultData.c_ambientSky = Color.grey;
        defaultData.c_ambientEquator = Color.black;
        defaultData.c_ambientGround = Color.black;
        defaultData.c_ocean = Color.blue;
        return defaultData;
    }

    public void DataInit(Light directionalLight, Material oceanMat)
    {
        directionalLight.color = c_directionnal;
        directionalLight.intensity = f_directionalIntensity;
        directionalLight.transform.rotation = Quaternion.Euler(v3_eulerAngle);
        RenderSettings.ambientSkyColor = c_ambientSky;
        RenderSettings.ambientEquatorColor = c_ambientEquator;
        RenderSettings.ambientGroundColor = c_ambientGround;
    }

#if UNITY_EDITOR
    public void SaveData(Light directional, Material ocean)
    {
        c_directionnal = directional.color;
        f_directionalIntensity = directional.intensity;
        v3_eulerAngle = directional.transform.eulerAngles;
        c_ocean = ocean.GetColor("_Color");
        c_ambientSky = RenderSettings.ambientSkyColor;
        c_ambientEquator = RenderSettings.ambientEquatorColor;
        c_ambientGround = RenderSettings.ambientGroundColor;
    }
#endif
}
