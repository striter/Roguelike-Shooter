using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMapData))]
public class TileMapEditor : Editor {
    public override void OnInspectorGUI()
    {
        TileMapData data = target as TileMapData;
        GUILayout.BeginVertical();
        GUILayout.TextArea("Tile Map Size:"+data.I_Width+ "x"+data.I_Height);
        GUILayout.TextArea("Contains:"+data.m_MapData.Count+" Tiles");
        GUILayout.TextArea("Offset:" + data.m_Offset);
        GUILayout.EndVertical();
    }
}
