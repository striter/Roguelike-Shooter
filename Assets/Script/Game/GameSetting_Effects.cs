using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
namespace GameSetting
{
    public enum enum_Effects
    {
        Invalid = -1,
        Cloak,
        Freeze,
        Scan,
        Death,
    }

    public static class TEffects
    {
        public static readonly Dictionary<enum_Effects, Shader> SD_ExtraEffects = new Dictionary<enum_Effects, Shader>()
    {
        { enum_Effects.Cloak,Shader.Find("Game/Effect/Cloak")},
        { enum_Effects.Death,Shader.Find("Game/Effect/BloomSpecific/Bloom_Dissolve")},
        { enum_Effects.Scan,Shader.Find("Game/Extra/ScanEffect")},
        { enum_Effects.Freeze, Shader.Find("Game/Effect/Ice")}
    };
        public static readonly Texture TX_Noise = TResources.GetNoiseTex();
        public static readonly int ID_Color = Shader.PropertyToID("_Color");
        public static readonly int ID_Dissolve = Shader.PropertyToID("_DissolveAmount");
        public static readonly int ID_DissolveScale = Shader.PropertyToID("_DissolveScale");
        public static readonly int ID_Opacity = Shader.PropertyToID("_Opacity");
        public static readonly int ID_NoiseTex = Shader.PropertyToID("_NoiseTex");
    }
}