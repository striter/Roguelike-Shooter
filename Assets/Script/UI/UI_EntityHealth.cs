using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UI_EntityHealth : UIToolsBase {
    UIT_GridControllerMonoItem<UIGI_HealthBar> m_HealthGrid;
    UIT_GridControllerMonoItem<UIGI_Damage> m_DamageGrid;
    protected override void Init()
    {
        base.Init();
        m_HealthGrid = new UIT_GridControllerMonoItem<UIGI_HealthBar>(transform.Find("HealthGrid"));
        m_DamageGrid = new UIT_GridControllerMonoItem<UIGI_Damage>(transform.Find("DamageGrid"));
    }

    private void Start()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageFinish, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnGameExit, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageFinish, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnGameExit, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
    }

    bool b_showEntityHealthInfo(EntityBase entity) => entity.m_Controller != enum_EntityController.Player;
    int damageCount=0;
    void OnEntityActivate(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.AddItem(entity.I_EntityID).AttachItem(entity);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.RemoveItem(entity.I_EntityID);
    }
    void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float applyAmount)
    {
        if (applyAmount <= 0)
            return;

        m_DamageGrid.AddItem(damageCount++).Play(damageEntity,applyAmount,OnDamageExpire);

        if (!b_showEntityHealthInfo(damageEntity))
            return;

        m_HealthGrid.GetItem(damageEntity.I_EntityID).OnShow();
    }

    void OnDamageExpire(int index)
    {
        m_DamageGrid.RemoveItem(index);
    }

    void ClearAll()
    {
        m_HealthGrid.ClearGrid();
        m_DamageGrid.ClearGrid();
    }
}
