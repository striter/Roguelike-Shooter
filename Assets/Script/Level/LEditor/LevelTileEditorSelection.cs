using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;

public class LevelTileEditorSelection : LevelTileEditor
{
    public override void Init(TileAxis axis, ChunkTileData data,System.Random random)
    {
        Clear();
        base.Init(axis, data,random);
        SetSelecting(false);
    }

    public void SetSelecting(bool selecting)
    {
        m_EditorModel.SetActivate(selecting);
    }
}
