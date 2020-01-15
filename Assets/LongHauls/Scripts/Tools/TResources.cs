using GameSetting;
using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class TResources
{
    #region Extras
    public class ConstPath
    {
        public const string S_PlayerEntity = "Entity/0_Player";
        public const string S_StyleCustomization = "Level/Customization";

        public const string S_LevelPrefab = "Level/LevelPrefab";
        public const string S_LeveLItem = "Level/Item";

        public const string S_ChunkTile = "Chunk/Tile/";
        public const string S_ChunkTileEditor = "Chunk/Tile/Editor";
        public const string S_ChunkData = "Chunk/Data";

        public const string S_Texture_LevelBase = "Texture/Level/Texture_Base_";

        public const string S_PlayerWeapon = "WeaponModel/";
        public const string S_Entity = "Entity/";
        public const string S_SFXEffects = "SFX_Effects/";
        public const string S_SFXWeapon = "Weapon/";
        public const string S_InteractPortal = "Interact/Portal_";
        public const string S_InteractActionChest = "Interact/ActionChest_";
        public const string S_InteractCommon = "Interact/Interact_";

        public const string S_PETex_Noise = "Texture/PE_Noise";
        public const string S_PETex_Holograph = "Texture/PE_Holograph";

        public const string S_UI_Atlas_Numeric = "UI/Atlas/Atlas_Numeric";
        public const string S_UI_Atlas_Common = "UI/Atlas/Atlas_Common";
        public const string S_UI_Atlas_InGame = "UI/Atlas/Atlas_InGame";
        public const string S_UI_Atlas_Action = "UI/Atlas/Atlas_Action";
        public const string S_UI_Atlas_Weapon = "UI/Atlas/Atlas_Weapon";
        public const string S_UI_Manager = "UI/UIManager";

        public const string S_Audio_GameBGM = "Audio/Background/Game_";
        public const string S_Audio_CampBGM = "Audio/Background/Camp_";
        public const string S_GameAudio_VFX = "Audio/GameVFX/";
        public const string S_UIAudio_VFX = "Audio/UIVFX/";
    }

    static Texture m_NoiseTex=null;
    public static Texture GetNoiseTex()
    {
        if (m_NoiseTex == null)
            m_NoiseTex = Load<Texture>(ConstPath.S_PETex_Noise);
        return m_NoiseTex;
    }

    #region UI
    public static GameObject InstantiateUIManager() => Instantiate(ConstPath.S_UI_Manager);
    public static AtlasLoader GetUIAtlas_Numeric() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Numeric));
    public static AtlasLoader GetUIAtlas_Common() =>  new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Common));
    public static AtlasLoader GetUIAtlas_InGame() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_InGame));
    public static AtlasLoader GetUIAtlas_Action() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Action));
    public static AtlasLoader GetUIAtlas_Weapon() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Weapon));
    #endregion
    #region GamePrefab
    public static StyleColorData[] GetAllStyleCustomization(enum_LevelStyle levelStype) => LoadAll<StyleColorData>(ConstPath.S_StyleCustomization + "/" + levelStype);
    #region NewLevel
    public static LevelChunkData GetLevelData(string name) => Load<LevelChunkData>(ConstPath.S_ChunkData + "/" + name);
    public static Dictionary<enum_ChunkType, List<LevelChunkData>> GetChunkDatas()
    {
        LevelChunkData[] datas= LoadAll<LevelChunkData>(ConstPath.S_ChunkData);
        Dictionary<enum_ChunkType,List< LevelChunkData>> sortedDatas = new Dictionary<enum_ChunkType, List<LevelChunkData>>();
        datas.Traversal((LevelChunkData data) =>
        {
            if (!sortedDatas.ContainsKey(data.Type))
                sortedDatas.Add(data.Type,new List<LevelChunkData>());
            sortedDatas[data.Type].Add(data);
        });
        return sortedDatas;
    }
    public static TileItemBase[] GetChunkTiles(enum_LevelStyle _levelStyle) => LoadAll<TileItemBase>(ConstPath.S_ChunkTile+_levelStyle);
    public static TileItemBase[] GetChunkEditorTiles() => LoadAll<TileItemBase>(ConstPath.S_ChunkTileEditor);

    #endregion

    public static SFXWeaponBase GetDamageSource(int index) => Instantiate<SFXWeaponBase>(ConstPath.S_SFXWeapon+index.ToString());

    public static Dictionary<int, SFXBase> GetAllEffectSFX()
    {
        Dictionary<int, SFXBase> sfxsDic = new Dictionary<int, SFXBase>();
        LoadAll<SFXBase>(ConstPath.S_SFXEffects).Traversal((SFXBase sfx) => {
            sfxsDic.Add(int.Parse(sfx.name.Split('_')[0]), GameObject.Instantiate<SFXBase>(sfx));
            PreloadMaterials(sfx.gameObject);
        });
        return sfxsDic;
    }

    public static EntityCharacterPlayer GetPlayer(Transform parent) => Instantiate<EntityCharacterPlayer>(ConstPath.S_PlayerEntity,parent);
    public static Dictionary<int, EntityBase> GetCommonEntities()
    {
        Dictionary<int, EntityBase> entitisDic = new Dictionary<int, EntityBase>();
        EntityBase[] entities = LoadAll<EntityBase>(ConstPath.S_Entity + "Common");
        entities.Traversal((EntityBase entity) => {
            int index = int.Parse(entity.name.Split('_')[0]);
            entitisDic.Add(index, GameObject.Instantiate<EntityBase>(entity));
            PreloadMaterials(entity.gameObject);
        });
        return entitisDic;
    }
    public static Dictionary<int, EntityBase> GetEnermyEntities(enum_LevelStyle entityStyle)
    {
        Dictionary<int, EntityBase> entitisDic = new Dictionary<int, EntityBase>();
        EntityBase[] entities = LoadAll<EntityBase>(ConstPath.S_Entity +  entityStyle.ToString());
        entities.Traversal((EntityBase entity) => {
            int index = int.Parse(entity.name.Split('_')[0]);
            entitisDic.Add(index, GameObject.Instantiate<EntityBase>(entity));
            PreloadMaterials(entity.gameObject);
        });
        return entitisDic;
    }
    static void PreloadMaterials(GameObject targetObject)
    {
        targetObject.GetComponentsInChildren<Renderer>().Traversal((Renderer render) => { render.sharedMaterials.Traversal((Material material) => {if(material) material.hideFlags = material.hideFlags; }); });
    }
    public static WeaponBase GetPlayerWeapon(enum_PlayerWeapon weapon)=>Instantiate<WeaponBase>(ConstPath.S_PlayerWeapon + weapon.ToString());
    public static InteractGameBase GetInteractPortal(enum_LevelStyle portalStyle) => Instantiate<InteractGameBase>( ConstPath.S_InteractPortal + portalStyle.ToString());
    public static InteractGameBase GetInteract(enum_Interaction type) => Instantiate<InteractGameBase>(ConstPath.S_InteractCommon + type);
    #endregion
    #region Audio
    public static AudioClip GetGameBGM(enum_GameMusic music) => Load<AudioClip>(ConstPath.S_Audio_GameBGM+music);
    public static AudioClip GetGameBGM_Styled(enum_GameMusic music,enum_LevelStyle style) => Load<AudioClip>(ConstPath.S_Audio_GameBGM +style+"_" +music);
    public static AudioClip GetCampBGM(enum_CampMusic music) => Load<AudioClip>(ConstPath.S_Audio_CampBGM + music);
    public static AudioClip GetGameClip(enum_GameVFX vfx) => Load<AudioClip>(ConstPath.S_GameAudio_VFX + vfx.ToString());
    #endregion
    #endregion
    #region Will Be Replaced By AssetBundle If Needed
    public static GameObject Instantiate(string path, Transform toParent=null)
    {
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
            throw new Exception("Null Path Of :Resources/" + path.ToString());
        return UnityEngine.Object.Instantiate(obj, toParent);
    }
    public static T Instantiate<T>(string path, Transform toParent = null) where T : UnityEngine.Component=> Instantiate(path, toParent).GetComponent<T>();
    
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        T prefab = Resources.Load<T>(path);
        if (prefab == null)
            Debug.LogWarning("Invalid Item Found Of |"+typeof(T)+  "|At:" + path);
        return prefab;
    }

    public static T[] LoadAll<T>(string path) where T : UnityEngine.Object
    {
        T[] array = Resources.LoadAll<T>(path);

        if (array.Length == 0)
            Debug.LogWarning("No InnerItems At:" + path);
        return array;
    }

    public static IEnumerator LoadAsync<T>(string resourcePath, Action<T> OnLoadFinished) where T : UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
        yield return request;
        if (request.isDone && request.asset != null)
            OnLoadFinished(request.asset as T);
        else
            Debug.LogError("Null Path Of: Resources/" + resourcePath);
    }

    #endregion
    public static TextAsset GetExcelData(string dataSource, bool extraSheets = false)
    {
        TextAsset asset = Resources.Load<TextAsset>("Excel/" + dataSource);
        if (asset == null)
        {
            Debug.LogError("Path: Resources/Excel/" + dataSource + ".bytes Not Found");
            return null;
        }
        return asset;
    }

    public static T LoadResourceSync<T>(string bundlePath, string name) where T:UnityEngine.Object
    {
        T template = ResourceLoader.ResourcesLoader.LoadFromBundle<T>(bundlePath, name);
        if (template == null)
        {   
            Debug.LogError("Null Path Found Of Bundle Name:" + bundlePath + "/Item Name:" + name);
        }
        return template;
    }

    public static GameObject SpawnGameObjectAt( string bundlePath,string name, Transform parentTrans=null)
    {
#if UNITY_EDITOR            //Android Bundle Item In Editor Won't Show Proper Material, Load By AssetDataBase
        GameObject obj = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/BundleAssets/" + bundlePath + "/" + name + ".prefab");
#else
        GameObject obj = ResourceLoader.ResourcesLoader.LoadFromBundle<GameObject>( bundlePath , name );
#endif
        if (obj == null)
        {
            Debug.LogError("Null Path Of:" + bundlePath+"/"+name);
        }
        obj = GameObject.Instantiate(obj, parentTrans);
        return obj;
    }
}

public static class TEffects
{
    public static readonly Shader SD_Dissolve = Shader.Find("Game/Effect/BloomSpecific/Bloom_Dissolve");
    public static readonly Shader SD_Scan = Shader.Find("Game/Extra/ScanEffect");
    public static readonly Shader SD_Ice = Shader.Find("Game/Effect/Ice");
    public static readonly Shader SD_Cloak = Shader.Find("Game/Effect/Cloak");
    public static readonly Texture TX_Noise = TResources.GetNoiseTex();
    public static readonly int ID_Color = Shader.PropertyToID("_Color");
    public static readonly int ID_Dissolve = Shader.PropertyToID("_DissolveAmount");
    public static readonly int ID_DissolveScale = Shader.PropertyToID("_DissolveScale");
    public static readonly int ID_Opacity = Shader.PropertyToID("_Opacity");
    public static readonly int ID_NoiseTex = Shader.PropertyToID("_NoiseTex");
}

namespace ResourceLoader
{
    public class ResourcesLoader
    {
        static Dictionary<string, AssetBundle> bundleDic = new Dictionary<string, AssetBundle>();
        public static T LoadFromBundle<T>(string subPath,string name) where T:UnityEngine.Object
        {
            AssetBundle bundle;
            if (bundleDic.ContainsKey(subPath))
              {
                bundle = bundleDic[subPath];
            }
            else
            {
                bundle = AssetBundle.LoadFromFile(Application.streamingAssetsPath + "/" + subPath.ToLower());
                bundleDic.Add(subPath, bundle);
            }
            return bundle.LoadAsset<T>(name);
    }
    public static IEnumerator LoadAllResourcesAsync<T>(string subPath,Action<List<T>> templateDel) where T : UnityEngine.Object
        {
            AssetBundle bundle = null;
            AssetBundleRequest request = null;
            List<T> templateList = new List<T>();
            string bundlePath = Application.streamingAssetsPath + "/" + subPath.ToLower();
            WWW bundleInfo = null;
            if (bundleDic.ContainsKey(subPath))
            {
                bundle = bundleDic[subPath];
            }
            else
            {
                bundleInfo= new WWW(bundlePath); ;
            }
            for (; ; )
            {
                while (bundleInfo!=null&&!bundleInfo.isDone)
                    yield return null;
                
                if (bundleInfo != null)
                {
                    bundle = bundleInfo.assetBundle;
                    bundleDic.Add(subPath, bundle);
                }
                if (request == null)
                {
                    request = bundle.LoadAllAssetsAsync<T>();
                }
                while (!request.isDone)
                    yield return null;

                for (int i=0 ; i < request.allAssets.Length; i++)
                {
                    templateList.Add(request.allAssets[i] as T);
                }
                
                templateDel(templateList);
                yield break;
            }
    }
    }

}