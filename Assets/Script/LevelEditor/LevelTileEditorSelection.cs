using LevelSetting;
using System.Collections;
using System.Collections.Generic;
using TTiles;
using UnityEngine;

public class LevelTileEditorSelection : LevelTileEditor
{
    public override void Init(TileAxis axis, LevelTileData data)
    {
        base.Init(axis, data);
        SetSelecting(false);
    }
    protected override void OnEditorModelChange(TileAxis size)
    {
        base.OnEditorModelChange(size);
        m_EditorModel.localPosition = TileTools.GetLocalPosBySizeAxis(size);
        m_EditorModel.localScale = TileTools.GetUnitScaleBySizeAxis(size, LevelConst.I_TileSize);
    }
    public void SetSelecting(bool selecting)
    {
        m_EditorModel.SetActivate(selecting);
    }
}
