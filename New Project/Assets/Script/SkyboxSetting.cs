using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkyboxSetting_", menuName = "GameAsset/SkyboxSetting")]
public class SkyboxSetting : ScriptableObject
{
    [SerializeField]
    public Material m_SkyboxMaterial;
    public Color m_DirectionalLightColor;
    public float m_DirectionalLightIntensity;
    public float m_RotationPerSecond;
}
