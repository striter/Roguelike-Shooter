﻿using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;
using GameSetting;
public class TileObjectBase : TileItemBase,IObjectpool<enum_TileObjectType>,ICoroutineHelperClass {
    public enum_TileObjectType m_ObjectType= enum_TileObjectType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Object;
    Action<enum_TileObjectType, MonoBehaviour> OnRecycle;
    public override TileAxis GetDirectionedSize(enum_TileDirection direction) => m_ObjectType.GetSizeAxis(direction);

    HitCheckStatic[] m_hitChecks;
    Renderer[] m_Renderers;
    Material[] m_Materials;
    Action OnItemDestroy;
    float m_Health = -1;

    public void OnPoolItemInit(enum_TileObjectType identity, Action<enum_TileObjectType, MonoBehaviour> OnRecycle)
    {
        Init();
        m_ObjectType = identity;
        m_Renderers = GetComponentsInChildren<Renderer>();
        m_Materials = new Material[m_Renderers.Length];
        m_Renderers.Traversal((int index, Renderer renderer) => { m_Materials[index] = renderer.sharedMaterial; });
        m_hitChecks = GetComponentsInChildren<HitCheckStatic>();
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.Attach(OnObjectHit); });
        this.OnRecycle = OnRecycle;
    }
    private void OnDestroy()
    {
        this.StopSingleCoroutine(0);
    }
    public override void DoRecycle()=> OnRecycle(m_ObjectType, this);

    public override void OnGenerateItem(ChunkTileData _data, System.Random random)
    {
        base.OnGenerateItem(_data, random);
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.SetEnable(true); });
        m_Renderers.Traversal((int index, Renderer renderer) => { renderer.material = m_Materials[index]; });
        m_Health = -1;
    }

    public virtual void GameInit(float health,Action OnDestroy)
    {
        m_Health = health;
        OnItemDestroy = OnDestroy;
    }

    bool OnObjectHit(DamageInfo damage,Vector3 direction)
    {
        if (m_Health <= 0)
            return false;

        m_Health -= damage.m_AmountApply;
        if (m_Health <= 0)
            OnObjectDead();
        return true;
    }
    
    void OnObjectDead()
    {
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.SetEnable(false); });
        m_Renderers.Traversal((Renderer render) => {
            render.material.shader = TEffects.SD_ExtraEffects[ enum_Effects.Death];
            render.material.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            render.material.SetFloat(TEffects.ID_DissolveScale, .1f);
        });
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{
            m_Renderers.Traversal((Renderer render) => { render.material.SetFloat(TEffects.ID_Dissolve, value); });
        }, 0f, 1f, 1f, DoRecycle));
        OnItemDestroy();
    }

}
