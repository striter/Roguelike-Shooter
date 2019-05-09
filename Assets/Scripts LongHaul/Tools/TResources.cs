﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TResources
{

    #region Only For Test Or Start Of The Project
    public static T Instantiate<T>(string path,Transform toParent=null) where T : UnityEngine.Object
    {
        GameObject obj = Resources.Load<GameObject>(path);
        if (obj == null)
        {
            throw new Exception("Null Path Of :Resources/" + path.ToString());
        }
        return UnityEngine.Object.Instantiate(obj, toParent).GetComponent<T>() ;
    }
    public static T Load<T>(string path) where T:UnityEngine.Object
    {
        return Resources.Load<T>(path);
    }
    public static IEnumerator LoadAsync<T>(string resourcePath,Action<T> OnLoadFinished) where T:UnityEngine.Object
    {
        ResourceRequest request = Resources.LoadAsync<T>(resourcePath);
        yield return request;
        if (request.isDone && request.asset != null)
            OnLoadFinished(request.asset as T);
        else
            Debug.LogError("Null Path Of: Resources/"+resourcePath);
    }
    #endregion
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


        //only useable in pc???
        //static FileInfo[] GetAllFilesAtDirectory(string resourcePath)
        //{
        //    FileInfo[] files = new FileInfo[0];
        //    try
        //    {
        //        string path = resourcePath;
        //        string fullPath = Application.streamingAssetsPath + path;
        //        if (!Directory.Exists(fullPath))
        //        {
        //            throw (new Exception("No path of " + fullPath + " found"));
        //        }
        //        DirectoryInfo direction = new DirectoryInfo(fullPath);
        //        files = direction.GetFiles("*", SearchOption.AllDirectories);
        //        if (files.Length == 0)
        //        {
        //            throw (new Exception("No files exists at path " + fullPath));
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.LogError(e.Message);
        //    };
        //    return files;
        //}
    }

}