using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EMaterialExtractor : EditorWindow {
    [MenuItem("Tools/ChangeShader")]
    public static void ChangeGameObjectShader()
    {
        // Get existing open window or if none, make a new one:
        EMaterialExtractor window = GetWindow(typeof(EMaterialExtractor)) as EMaterialExtractor;
        window.Show();
    }
    enum_ShaderType shaderType;

    public enum enum_ShaderType
    {
        Invalid=-1,
        Diffuse=1,
    }

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical();
        EditorGUILayout.TextArea("Now Selecting");
        for (int i = 0; i < Selection.objects.Length; i++)
            EditorGUILayout.TextArea(Selection.objects[i].name);
        shaderType=(enum_ShaderType)EditorGUILayout.EnumPopup("Shader Type",shaderType);
        if (Selection.objects.Length > 0 && shaderType!= enum_ShaderType.Invalid)
            if (EditorGUILayout.DropdownButton(new GUIContent("Confirm"), FocusType.Passive))
                ExtractMaterial();
        EditorGUILayout.EndVertical();
    }
    void ExtractMaterial()
    {
        Shader shader=SelectShader(shaderType);

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            Renderer[] renderers = (Selection.objects[i] as GameObject).GetComponentsInChildren<Renderer>();
            for (int j = 0; j < renderers.Length; j++)
            {
                string folderPath = "Assets/Material/" + Selection.objects[i].name;
                string materialPath = folderPath+"/"+ renderers[j].sharedMaterial.name+".mat";
                if (!AssetDatabase.IsValidFolder(folderPath))
                    AssetDatabase.CreateFolder("Assets/Material", Selection.objects[i].name);

                if (AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) == null)
                    AssetDatabase.CreateAsset(new Material(renderers[j].sharedMaterial),materialPath);

                Material targetMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
                targetMaterial.shader = shader;
                renderers[j].material = targetMaterial;
            }
        }
        Debug.Log("Extract Successful");
    }

    Shader SelectShader(enum_ShaderType type)
    {
        Shader shader;
        switch (shaderType)
        {
            default:
                shader= null;
                break;
            case enum_ShaderType.Diffuse:
                shader= Shader.Find("Game/LowPoly_Diffuse");
                break;
        }
        if (shader == null)
        {
            Debug.LogWarning("Null Shader Found:"+type.ToString());
        }
        return shader;
    }
}
