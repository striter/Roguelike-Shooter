using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
using TTiles;
using System;

public class LevelItemBase : ObjectPoolMonoItem<LevelItemBase>,ISingleCoroutine {
    public LevelBase m_LevelParent { get; private set; }
    public int m_sizeXAxis = 1;
    public int m_sizeYAxis = 1;
    public enum_LevelItemType m_ItemType = enum_LevelItemType.Invalid;
    Transform tf_Model;
    float m_Health;
    Action OnLevelItemDestroyed;
    HitCheckStatic[] m_hitChecks;
    Renderer[] m_Renderers;
    public override void OnPoolItemInit(LevelItemBase _identity, Action<LevelItemBase, MonoBehaviour> _OnSelfRecycle)
    {
        base.OnPoolItemInit(_identity, _OnSelfRecycle);
        tf_Model = transform.Find("Model");
        m_Renderers = GetComponentsInChildren<Renderer>();
        m_hitChecks =GetComponentsInChildren<HitCheckStatic>();
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.Attach(OnHit); });
    }
    private void OnDisable()
    {
        this.StopSingleCoroutine(0);
    }

    public void InitItem(LevelBase levelParent, enum_TileDirection direction,Action _OnLevelItemDestroyed)
    {
        m_LevelParent = levelParent;
        ItemRecenter(direction == enum_TileDirection.Right || direction == enum_TileDirection.Left);
        tf_Model.localRotation = Quaternion.Euler(0, (int)direction * 45, 0);
        transform.SetActivate(true);

        OnLevelItemDestroyed = _OnLevelItemDestroyed;
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => {hitCheck.SetEnable(true); });
        if (OnLevelItemDestroyed == null)
            return;
        switch(m_ItemType)
        {
            case enum_LevelItemType.BorderLinear:
            case enum_LevelItemType.BorderOblique:
                m_Health = 0;
                break;
            default:
                m_Health = m_sizeXAxis * m_sizeYAxis * 20f;
                break;
        }
    }

    bool OnHit(DamageInfo damageInfo,Vector3 direction)
    {
        if (m_Health <= 0)
            return false;
        m_Health -= damageInfo.m_AmountApply;
        if (m_Health <= 0)
        {
            m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.SetEnable(false); });
            m_Renderers.Traversal((Renderer render) => {
                render.material.shader = TEffects.SD_Dissolve;
                render.material.SetTexture(TEffects.ID_NoiseTex,TEffects.TX_Noise);
                render.material.SetFloat(TEffects.ID_DissolveScale, .1f);
            });
            this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value)=>
            {
                m_Renderers.Traversal((Renderer render) => { render.material.SetFloat(TEffects.ID_Dissolve,value); });
            },0f,1f,2f,()=> { transform.SetActivate(false); }));
            OnLevelItemDestroyed?.Invoke();
        } 
        return true;
    }

    public void ItemRecenter(bool inverse=false)
    {
        transform.GetChild(0).localPosition = new Vector3(((inverse?m_sizeYAxis:m_sizeXAxis) - 1) * GameConst.F_LevelTileSize, 0f, ((inverse ? m_sizeXAxis:m_sizeYAxis) - 1) * GameConst.F_LevelTileSize) / 2;
    }
#if UNITY_EDITOR
    public bool b_showGizmos = true;
    public bool b_AutoCenter = true;
    public bool b_AutoRotation = true;
    public void OnDrawGizmos()
    {
        if (UnityEditor.EditorApplication.isPlaying||!b_showGizmos)
            return;
        
        transform.localPosition = Vector3.zero;

        if (UnityEditor.EditorApplication.isPlaying)
            return;

        Gizmos.color = TCommon.ColorAlpha(Color.blue,.3f);

        for (int i = 0; i < m_sizeXAxis; i++)
            for (int j = 0; j < m_sizeYAxis; j++)
                Gizmos.DrawCube(transform.position + new Vector3(i * GameConst.F_LevelTileSize, .5f, j * GameConst.F_LevelTileSize), new Vector3(GameConst.F_LevelTileSize, 1f, GameConst.F_LevelTileSize));

        if (b_AutoCenter)
            ItemRecenter();
    }

#endif
}
