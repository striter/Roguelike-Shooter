using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIC_GameNumericVisualize : UIControlBase
{
    int visualize = 0;
    UIT_GridControllerGridItem<UIGI_VisualizeHealthBar> m_HealthGrid;
    UIT_GridControllerGridItem<UIGI_VisualizeDamage> m_DamageGrid;
    UIT_GridControllerGridItem<UIGI_VisualizePickup> m_PickupGrid;
    protected override void Init()
    {
        base.Init();
        m_HealthGrid = new UIT_GridControllerGridItem<UIGI_VisualizeHealthBar>(transform.Find("HealthGrid"));
        m_DamageGrid = new UIT_GridControllerGridItem<UIGI_VisualizeDamage>(transform.Find("DamageGrid"));
        m_PickupGrid = new UIT_GridControllerGridItem<UIGI_VisualizePickup>(transform.Find("PickupGrid"));
    }

    private void Start()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageFinish, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnGameExit, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_UIStatus>.Add<Vector3, enum_Interaction,int>(enum_BC_UIStatus.UI_PlayerInteractPickup,OnPlayerPickupAmount);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageFinish, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnGameExit, ClearAll);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_UIStatus>.Add<Vector3, enum_Interaction, int>(enum_BC_UIStatus.UI_PlayerInteractPickup, OnPlayerPickupAmount);
    }

    bool b_showEntityHealthInfo(EntityBase entity)
    {
        switch(entity.m_Controller)
        {
            case enum_EntityController.AI:
            case enum_EntityController.Device:
                return true;
        }
        return false;
    } 
    void OnEntityActivate(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.AddItem(entity.m_EntityID).AttachItem(entity);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.RemoveItem(entity.m_EntityID);
    }
    void OnCharacterHealthChange(DamageInfo damageInfo, EntityCharacterBase damageEntity, float applyAmount)
    {
        if (applyAmount <= 0)
            return;

        m_DamageGrid.AddItem(visualize++).Play(damageEntity,applyAmount, m_DamageGrid.RemoveItem);

        if (!b_showEntityHealthInfo(damageEntity))
            return;

        m_HealthGrid.GetItem(damageEntity.m_EntityID).OnShow();
    }
    
    void OnPlayerPickupAmount(Vector3 position,enum_Interaction interaction,int amount)
    {
        m_PickupGrid.AddItem(visualize++).Play(position,interaction,amount,m_PickupGrid.RemoveItem);
    }

    void ClearAll()
    {
        m_HealthGrid.ClearGrid();
        m_DamageGrid.ClearGrid();
        m_PickupGrid.ClearGrid();
    }
}
