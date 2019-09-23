using System;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

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
                if (connectedPrefab == null)
                    continue;

                PrefabUtility.ReplacePrefab(objects[i], connectedPrefab);
                Debug.Log("Prefab:" + connectedPrefab.name + " Replaced Successful!");
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

    public static class TUI
    {
        public class FontsHelpWindow : EditorWindow
        {
            [MenuItem("Work Flow/UI Tools/Fonts Help Window", false, 203)]
            public static void ShowWindow()
            {
                FontsHelpWindow window = GetWindow(typeof(FontsHelpWindow)) as FontsHelpWindow;
                window.Show();
            }

            Transform m_parent;
            Text[] m_text;
            Font m_Font;
            private void OnEnable()
            {
                m_Font = null;
                m_parent = null;
                m_text = null;
                EditorApplication.update += Update;
            }
            private void OnDisable()
            {
                EditorApplication.update -= Update;
            }
            private void Update()
            {
                if (EditorApplication.isPlaying)
                    Close();

                if (m_parent != Selection.activeTransform)
                {
                    m_parent = Selection.activeTransform;
                    m_text = m_parent ? m_parent.GetComponentsInChildren<Text>() : null;
                    EditorUtility.SetDirty(this);
                }
            }
            private void OnGUI()
            {
                EditorGUILayout.BeginVertical();
                if (m_text == null)
                {
                    EditorGUILayout.TextArea("Please Select Texts Parent In Hierarchy!");
                }
                else
                {
                    EditorGUILayout.TextArea("Current Selecting:"+m_parent.name+ ", Texts Counts:"+m_text.Length);
                    m_Font = (Font)EditorGUILayout.ObjectField("Fonts", m_Font, typeof(Font),false);
                    
                    if (m_Font&&GUILayout.Button("Set All Text Font To:"+m_Font.name))
                    {
                        for (int i = 0; i < m_text.Length; i++)
                            m_text[i].font = m_Font;
                    }
                }
                EditorGUILayout.EndVertical();
            }
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
