using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTerrainData : ScriptableObject {
    public class Node
    {
        Vector3 pos;
    }
    public GameObject SubTerrainPrefab;
    public int I_Width=10, I_Height=10;
    public float F_CellOffsetHorizontal=1f, F_CellOffsetVertical=1f;
}
