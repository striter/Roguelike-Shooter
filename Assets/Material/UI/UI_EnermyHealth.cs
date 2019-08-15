using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class UI_EnermyHealth : SimpleSingletonMono<UI_EnermyHealth> {
    UIT_GridControllerMono<UIGI_HealthBar> m_HealthGrid;
    protected override void Awake()
    {
        base.Awake();
        m_HealthGrid = new UIT_GridControllerMono<UIGI_HealthBar>(transform.Find("HealthGrid"));
    }
    private void Start()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnEntitySpawn);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<int,EntityBase,float>(enum_BC_GameStatusChanged.OnEntityDamage, OnEntityDamage);
    }
    private void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntitySpawn, OnEntitySpawn);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnEntityRecycle, OnEntityRecycle);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<int,EntityBase,float>(enum_BC_GameStatusChanged.OnEntityDamage, OnEntityDamage);
    }

    void OnEntitySpawn(EntityBase entity)
    {
        if (entity.m_Flag!= enum_EntityFlag.Enermy)
            return;

        m_HealthGrid.AddItem(entity.I_EntityID).AttachItem(entity);
    }

    void OnEntityRecycle(EntityBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy)
            return;

        m_HealthGrid.RemoveItem(entity.I_EntityID);
    }
    void OnEntityDamage(int sourceID, EntityBase damageEntity, float damage)
    {
        if (damageEntity.m_Flag != enum_EntityFlag.Enermy)
            return;

        m_HealthGrid.GetItem(damageEntity.I_EntityID).OnShow();
    }
}
