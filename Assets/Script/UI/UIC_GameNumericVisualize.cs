using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UIC_GameNumericVisualize : UIControlBase
{
    int visualize = 0;
    public UIT_GridControllerGridItem<UIGI_VisualizeHealth> m_HealthGrid { get; private set; }
    public UIT_GridControllerGridItem<UIGI_VisualizeDamage> m_DamageGrid { get; private set; }
    public  UIT_GridControllerGridItem<UIGI_VisualizePickup> m_PickupGrid { get; private set; }
    public UIT_GridControllerGridItem<UIGI_VisualizeAttackIndicate> m_AttackIndicateGrid { get; private set; }
    protected override void Init()
    {
        base.Init();
        m_HealthGrid = new UIT_GridControllerGridItem<UIGI_VisualizeHealth>(transform.Find("HealthGrid"));
        m_DamageGrid = new UIT_GridControllerGridItem<UIGI_VisualizeDamage>(transform.Find("DamageGrid"));
        m_PickupGrid = new UIT_GridControllerGridItem<UIGI_VisualizePickup>(transform.Find("PickupGrid"));
        m_AttackIndicateGrid = new UIT_GridControllerGridItem<UIGI_VisualizeAttackIndicate>(transform.Find("AttackIndicateGrid"));
    }

    private void Start()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_UIStatus>.Add<EntityCharacterAI>(enum_BC_UIStatus.UI_OnWillAIAttack,OnWillAIAttack);
        TBroadCaster<enum_BC_UIStatus>.Add<InteractPickup>(enum_BC_UIStatus.UI_PlayerInteractPickup,OnPlayerPickupAmount);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterHealthChange, OnCharacterHealthChange);
        TBroadCaster<enum_BC_UIStatus>.Remove<EntityCharacterAI>(enum_BC_UIStatus.UI_OnWillAIAttack, OnWillAIAttack);
        TBroadCaster<enum_BC_UIStatus>.Add<InteractPickup>(enum_BC_UIStatus.UI_PlayerInteractPickup, OnPlayerPickupAmount);
    }

    bool b_showEntityHealthInfo(EntityBase entity)
    {
        switch(entity.m_ControllType)
        {
            case enum_EntityType.AIWeaponModel:
            case enum_EntityType.AIWeaponHelper:
            case enum_EntityType.Device:
                return true;
        }
        return false;
    } 
    void OnEntityActivate(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.AddItem(entity.m_EntityID).Play(entity);
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

        m_DamageGrid.AddItem(visualize++).Play(damageEntity, damageInfo.m_CritcalHitted, applyAmount, m_DamageGrid.RemoveItem);

        if (!b_showEntityHealthInfo(damageEntity))
            return;

        m_HealthGrid.GetItem(damageEntity.m_EntityID).OnShow();
    }
    
    void OnPlayerPickupAmount(InteractPickup pickup)=> m_PickupGrid.AddItem(visualize++).Play(pickup,m_PickupGrid.RemoveItem);
    
    void OnWillAIAttack(EntityCharacterAI ai)=>m_AttackIndicateGrid.AddItem(visualize++).Play(ai.transform,m_AttackIndicateGrid.RemoveItem);

}
