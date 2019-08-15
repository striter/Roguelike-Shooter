using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
namespace EToolsEditor
{
    public static class TMenuItem
    {
        [MenuItem("Work Flow/Selected Prefabs/Apply All",false,101)]
        static void ApplyAllPrefabs()
        {
            GameObject[] objects = Selection.gameObjects;
            for (int i = 0; i < objects.Length; i++)
            {
                UnityEngine.Object connectedPrefab = PrefabUtility.GetPrefabParent(objects[i]);
                if (connectedPrefab != null)
                {

                    PrefabUtility.ReplacePrefab(objects[i], connectedPrefab);
                    Debug.Log("Prefab:" + connectedPrefab.name + " Replaced Successful!");
                }
            }
        }
        [MenuItem("Work Flow/Take Screen Shot",false, 102)]
        static void TakeScreenShot()
        {
            DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath+"/ScreenShots");
            string path = Path.Combine(directory.Parent.FullName, string.Format("Screenshot_{0}.png", DateTime.Now.ToString("yyyyMMdd_Hmmss")));
            Debug.Log("Sceen Shots At " + path);
            ScreenCapture.CaptureScreenshot(path);
        }
    }

    public static class TEditor
    {
        public const string S_AssetDataBaseResources = "Assets/Resources/";
    }


    public class EAudio
    {
        static AudioClip curClip;
        //Reflection Target  UnityEditor.AudioUtil;
        public static void AttachClipTo(AudioClip clip)
        {
            curClip = clip;
        }
        public static bool IsAudioPlaying()
        {
            if (curClip != null)
                return (bool)GetClipMethod("IsClipPlaying").Invoke(null, new object[] { curClip });
            return false;
        }
        public static int GetSampleDuration()
        {
            if(curClip!=null)
              return(int)GetClipMethod("GetSampleCount").Invoke(null, new object[] { curClip });
            return -1;
        }
        public static int GetCurSample()
        {
            if (curClip != null)
                return (int)GetClipMethod("GetClipSamplePosition").Invoke(null, new object[] { curClip });
            return -1;
        }
        public static float GetCurTime()
        {
            if (curClip != null)
                return (float)GetClipMethod("GetClipPosition").Invoke(null, new object[] { curClip});
            return -1;
        }
        public static void PlayClip()
        {
            if (curClip != null)
                GetClipMethod("PlayClip").Invoke(null, new object[] { curClip });
        }
        public static void PauseClip()
        {
            if (curClip != null)
                GetClipMethod("PauseClip").Invoke( null,  new object[] { curClip } );
        }
        public static void StopClip()
        {
            if(curClip!=null)
            GetClipMethod("StopClip").Invoke(null,  new object[] { curClip } );
        }
        public static void ResumeClip()
        {
            if (curClip != null)
                GetClipMethod("ResumeClip").Invoke(null, new object[] { curClip });
        }
        public static void SetSamplePosition(int startSample)
        {
            GetMethod<AudioClip, int>("SetClipSamplePosition").Invoke(null, new object[] { curClip, startSample });
        }
        static MethodInfo GetClipMethod(string methodName)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
           return  audioUtilClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(AudioClip) }, null);
        }
        static MethodInfo GetMethod<T, U>(string methodName)
        {
            Assembly unityEditorAssembly = typeof(AudioImporter).Assembly;
            Type audioUtilClass = unityEditorAssembly.GetType("UnityEditor.AudioUtil");
            return audioUtilClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public, null, new System.Type[] { typeof(T), typeof(U) }, null);

        }
    }

}
