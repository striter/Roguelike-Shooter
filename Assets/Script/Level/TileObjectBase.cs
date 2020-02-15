﻿using System;
using System.Collections;
using System.Collections.Generic;
using LevelSetting;
using TTiles;
using UnityEngine;
using GameSetting;
public class TileObjectBase : TileItemBase,IObjectpool<enum_TileObjectType>,ICoroutineHelper {
    public enum_TileObjectType m_ObjectType= enum_TileObjectType.Invalid;
    public override enum_TileSubType m_Type => enum_TileSubType.Object;
    Action<enum_TileObjectType, MonoBehaviour> OnRecycle;
    public override TileAxis GetDirectionedSize(enum_TileDirection direction) => m_ObjectType.GetSizeAxis(direction);
    protected override int GetTexSelection(System.Random random) => m_ObjectType.GetTexSelection(random);

    HitCheckStatic[] m_hitChecks;
    Renderer[] m_Renderers;
    Action OnItemDestroy;
    float m_Health = -1;

    public void OnPoolItemInit(enum_TileObjectType identity, Action<enum_TileObjectType, MonoBehaviour> OnRecycle)
    {
        m_ObjectType = identity;
        m_Renderers = GetComponentsInChildren<Renderer>();
        m_hitChecks = GetComponentsInChildren<HitCheckStatic>();
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.Attach(OnObjectHit); });
        this.OnRecycle = OnRecycle;
    }

    public override void DoRecycle()=> OnRecycle(m_ObjectType, this);

    public override void Init(ChunkTileData _data, System.Random random)
    {
        base.Init(_data, random);
        m_hitChecks.Traversal((HitCheckStatic hitCheck) => { hitCheck.SetEnable(true); });
        m_Health = -1;
    }

    public void GameInit(float health,Action OnDestroy)
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
            render.material.shader = TEffects.SD_Dissolve;
            render.material.SetTexture(TEffects.ID_NoiseTex, TEffects.TX_Noise);
            render.material.SetFloat(TEffects.ID_DissolveScale, .1f);
        });
        this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) =>{
            m_Renderers.Traversal((Renderer render) => { render.material.SetFloat(TEffects.ID_Dissolve, value); });
        }, 0f, 1f, 1f, DoRecycle));
        OnItemDestroy();
    }

}
