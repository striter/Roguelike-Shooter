using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;

public class LevelTileEditorSelection : LevelTileEditor
{
    protected Transform m_EditorModel { get; private set; }
    public override void InitEditorTile(TileAxis axis, ChunkTileData data, System.Random random)
    {
        m_EditorModel = transform.Find("EditorModel");
        base.InitEditorTile(axis, data, random);
        SetSelecting(false);
    }

    public void SetSelecting(bool selecting)  {
        m_EditorModel.SetActivate(selecting);
    }
}
