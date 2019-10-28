﻿using GameSetting;
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
        public const string S_LevelPrefab = "Level/LevelPrefab";
        public const string S_LeveLItem = "Level/Item";
        public const string S_StyleCustomization = "Level/Customization";

        public const string S_Texture_LevelBase = "Texture/Level/Texture_Base_";

        public const string S_PlayerWeapon = "WeaponModel/";
        public const string S_Entity = "Entity/";
        public const string S_SFXEffects = "SFX_Effects/";
        public const string S_SFXEquipment = "Equipment/";
        public const string S_InteractPortal = "Interact/Portal_";
        public const string S_InteractActionChest = "Interact/ActionChest_";
        public const string S_InteractCommon = "Interact/Interact_";

        public const string S_PETex_Noise = "Texture/PE_Noise";
        public const string S_PETex_Holograph = "Texture/PE_Holograph";

        public const string S_UI_Atlas_Numeric = "UI/Atlas/Atlas_Numeric";
        public const string S_UI_Atlas_Common = "UI/Atlas/Atlas_Common";
        public const string S_UI_Atlas_InGame = "UI/Atlas/Atlas_InGame";
        public const string S_UI_Atlas_Action = "UI/Atlas/Atlas_Action";
        public const string S_UI_Manager = "UI/UIManager";

        public const string S_Audio_Background = "Audio/Background/";
        public const string S_Audio_SFX = "Audio/SFX/";
    }

    static Texture m_NoiseTex=null;
    public static Texture GetNoiseTex()
    {
        if (m_NoiseTex == null)
            m_NoiseTex = Load<Texture>(ConstPath.S_PETex_Noise);
        return m_NoiseTex;
    }

    #region UI
    public static UIManager InstantiateUIManager() => Instantiate<UIManager>(ConstPath.S_UI_Manager);
    public static AtlasLoader GetUIAtlas_Numeric() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Numeric));
    public static AtlasLoader GetUIAtlas_Common() =>  new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Common));
    public static AtlasLoader GetUIAtlas_InGame() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_InGame));
    public static AtlasLoader GetUIAtlas_Action() => new AtlasLoader(Load<SpriteAtlas>(ConstPath.S_UI_Atlas_Action));
    #endregion
    #region GamePrefab
    public static StyleColorData[] GetAllStyleCustomization(enum_Style levelStype) => LoadAll<StyleColorData>(ConstPath.S_StyleCustomization + "/" + levelStype);
    public static Dictionary<enum_LevelItemType,List<LevelItemBase>>  GetAllLevelItems(enum_Style _levelStyle, Transform parent)
    {
        LevelItemBase[] styledLevelItemPrefabs = LoadAll<LevelItemBase>(ConstPath.S_LeveLItem + "/" + _levelStyle);
        foreach (LevelItemBase levelItem in styledLevelItemPrefabs)
        {
            if (levelItem.m_ItemType == enum_LevelItemType.Invalid)
                Debug.LogError("Please Edit Level Item(Something invalid): Resources/" + ConstPath.S_LeveLItem + _levelStyle + "/" + levelItem.name);
        }

        Dictionary<enum_LevelItemType, List<LevelItemBase>> itemPrefabDic = new Dictionary<enum_LevelItemType, List<LevelItemBase>>();
        foreach (LevelItemBase levelItem in styledLevelItemPrefabs)
        {
            if (!itemPrefabDic.ContainsKey(levelItem.m_ItemType))
                itemPrefabDic.Add(levelItem.m_ItemType, new List<LevelItemBase>());
            itemPrefabDic[levelItem.m_ItemType].Add(levelItem);
        }

        if (!itemPrefabDic.ContainsKey(enum_LevelItemType.BorderLinear))
            Debug.LogError("Level Style Item Not Contains LinaerBorder Type!");
        else if(!itemPrefabDic.ContainsKey( enum_LevelItemType.BorderOblique))
            Debug.LogError("Level Style Item Not Contains ObliqueBorder Type!");

        return itemPrefabDic;
    }
    public static LevelBase GetLevelBase(enum_Style levelStyle)
    {
        LevelBase level = Instantiate<LevelBase>(ConstPath.S_LevelPrefab);
        Renderer matRenderer = level.GetComponentInChildren<Renderer>();
        matRenderer.sharedMaterial.SetTexture("_MainTex", Load<Texture>(ConstPath.S_Texture_LevelBase + levelStyle));
        return level;
    }
    
    public static SFXBase GetDamageSource(int index) => Instantiate<SFXBase>(ConstPath.S_SFXEquipment+index.ToString());

    public static Dictionary<int, SFXBase> GetAllEffectSFX()
    {
        Dictionary<int, SFXBase> sfxsDic = new Dictionary<int, SFXBase>();
        LoadAll<SFXBase>(ConstPath.S_SFXEffects).Traversal((SFXBase sfx) => {
            sfxsDic.Add(int.Parse(sfx.name.Split('_')[0]), GameObject.Instantiate<SFXBase>(sfx));
            sfx.GetComponentsInChildren<Renderer>().Traversal((Renderer render) => { if (render.sharedMaterials != null) render.sharedMaterials.Traversal((Material material) => {if(material!=null) material.enableInstancing = false; }); });
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
    public static Dictionary<int, EntityBase> GetEnermyEntities(enum_Style entityStyle)
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
    public static WeaponBase GetPlayerWeapon(enum_PlayerWeapon weapon)
    {
        WeaponBase target;
        try
        {
            target= Instantiate<WeaponBase>(ConstPath.S_PlayerWeapon + weapon.ToString());
        }
        catch(Exception e)       //Error Check
        {
            Debug.LogWarning("Model Null Weapon Model Found:Resources/PlayerWeapon/" + weapon+","+e.Message);

            target = TResources.Instantiate<WeaponBase>(ConstPath.S_PlayerWeapon+"Error");
        }
        return target;
    }
    public static InteractBase GetInteractPortal(enum_Style portalStyle) => Instantiate<InteractBase>( ConstPath.S_InteractPortal + portalStyle.ToString());
    public static InteractBase GetInteractActionChest(enum_StageLevel stageLevel) => Instantiate<InteractBase>(ConstPath.S_InteractActionChest + stageLevel.ToString());
    public static InteractBase GetInteract(enum_Interaction type) => Instantiate<InteractBase>(ConstPath.S_InteractCommon + type);
    #endregion
    #region Audio
    public static AudioClip GetAudioClip_Background(bool inGame,enum_GameMusic music) => Load<AudioClip>(ConstPath. S_Audio_Background+(inGame?"Game_":"Camp_")+music.ToString());
    public static AudioClip GetAudioClip_SFX(enum_GameAudioSFX sfx) => Load<AudioClip>(ConstPath.S_Audio_SFX + sfx.ToString());
    #endregion
    #endregion
    #region Will Be Replaced By AssetBundle If Needed
    public static T Instantiate<T>(string path, Transform toParent = null) where T : UnityEngine.Object
    {
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
            throw new Exception("Null Path Of :Resources/" + path.ToString());
        return UnityEngine.Object.Instantiate(obj, toParent).GetComponent<T>();
    }
    public static T Load<T>(string path) where T : UnityEngine.Object
    {
        T prefab = Resources.Load<T>(path);
        if (prefab == null)
            Debug.LogError("Invalid Item Found Of |"+typeof(T)+  "|At:" + path);
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