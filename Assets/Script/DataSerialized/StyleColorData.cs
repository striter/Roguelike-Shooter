using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StyleColorData : ScriptableObject {
    public Color c_directionnal=Color.white;
    public Vector3 v3_eulerAngle = new Vector3(45, 60, 0);
    public float f_directionalIntensity=.8f;
    public Color c_ambientSky=Color.grey,c_ambientEquator=Color.black,c_ambientGround=Color.black;
    public Color c_ocean=Color.blue;
    public static StyleColorData Default()
    {
        return CreateInstance<StyleColorData>();
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
