using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UI_EntityHealth : SimpleSingletonMono<UI_EntityHealth> {
    UIT_GridControllerMono<UIGI_HealthBar> m_HealthGrid;
    UIT_GridControllerMono<UIGI_Damage> m_DamageGrid;
    protected override void Awake()
    {
        base.Awake();
        m_HealthGrid = new UIT_GridControllerMono<UIGI_HealthBar>(transform.Find("HealthGrid"));
        m_DamageGrid = new UIT_GridControllerMono<UIGI_Damage>(transform.Find("DamageGrid"));
    }
    private void Start()
    {
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityDeactivate);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageFinish, OnStageFinish);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageDeliverInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterDamage, OnCharacterDamage);
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityActivate, OnEntityActivate);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityDeactivate, OnEntityDeactivate);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageFinish, OnStageFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageDeliverInfo, EntityCharacterBase, float>(enum_BC_GameStatus.OnCharacterDamage, OnCharacterDamage);
    }

    bool b_showEntityHealthInfo(EntityBase entity) => (entity as EntityCharacterPlayer) == null;
    int damageCount=0;
    void OnEntityActivate(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.AddItem(entity.I_EntityID).AttachItem(entity);
    }

    void OnEntityDeactivate(EntityBase entity)
    {
        if (!b_showEntityHealthInfo(entity))
            return;

        m_HealthGrid.RemoveItem(entity.I_EntityID);
    }
    void OnCharacterDamage(DamageDeliverInfo damageDetail, EntityCharacterBase damageEntity, float damage)
    {
        m_DamageGrid.AddItem(damageCount++).Play(damageEntity,damage,OnDamageExpire);

        if (!b_showEntityHealthInfo(damageEntity))
            return;

        m_HealthGrid.GetItem(damageEntity.I_EntityID).OnShow();
    }

    void OnDamageExpire(int index)
    {
        m_DamageGrid.RemoveItem(index);
    }

    void OnStageFinish()
    {
        m_DamageGrid.ClearGrid();
    }
}
