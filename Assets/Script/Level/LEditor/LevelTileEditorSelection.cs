using LevelSetting;
using System;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;

public class LevelTileEditorSelection : LevelTileEditor
{
    protected Transform m_EditorModel { get; private set; }
    public override void InitTile(TileAxis axis, ChunkTileData data, System.Random random)
    {
        base.InitTile(axis, data, random);
        m_EditorModel = transform.Find("EditorModel");
        SetSelecting(false);
    }

    public void SetSelecting(bool selecting)  {
        m_EditorModel.SetActivate(selecting);
    }
}
