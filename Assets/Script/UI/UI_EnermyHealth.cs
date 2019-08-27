using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UI_EnermyHealth : SimpleSingletonMono<UI_EnermyHealth> {
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
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntitySpawn, OnEntitySpawn);
        TBroadCaster<enum_BC_GameStatus>.Add<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Add(enum_BC_GameStatus.OnStageFinish, OnStageFinish);
        TBroadCaster<enum_BC_GameStatus>.Add<DamageDeliverInfo, EntityBase, float>(enum_BC_GameStatus.OnEntityDamage, OnEntityDamage);
    }
    private void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntitySpawn, OnEntitySpawn);
        TBroadCaster<enum_BC_GameStatus>.Remove<EntityBase>(enum_BC_GameStatus.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatus>.Remove(enum_BC_GameStatus.OnStageFinish, OnStageFinish);
        TBroadCaster<enum_BC_GameStatus>.Remove<DamageDeliverInfo, EntityBase, float>(enum_BC_GameStatus.OnEntityDamage, OnEntityDamage);
    }
    int damageCount=0;
    void OnEntitySpawn(EntityBase entity)
    {
        if (entity.B_IsPlayer)
            return;

        m_HealthGrid.AddItem(entity.I_EntityID).AttachItem(entity);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        if (entity.B_IsPlayer)
            return;

        m_HealthGrid.RemoveItem(entity.I_EntityID);
    }
    void OnEntityDamage(DamageDeliverInfo damageDetail, EntityBase damageEntity, float damage)
    {
        m_DamageGrid.AddItem(damageCount++).Play(damageEntity,damage,OnDamageExpire);

        if (damageEntity.B_IsPlayer)
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
