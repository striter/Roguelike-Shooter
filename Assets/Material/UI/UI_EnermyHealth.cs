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
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Add<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
    }
    private void OnDestroy()
    {
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnSpawnEntity, OnSpawnEntity);
        TBroadCaster<enum_BC_GameStatusChanged>.Remove<EntityBase>(enum_BC_GameStatusChanged.OnRecycleEntity, OnRecycleEntity);
    }

    void OnSpawnEntity(EntityBase entity)
    {
        if (entity.m_Flag!= enum_EntityFlag.Enermy)
            return;

        m_HealthGrid.AddItem(entity.I_EntityID).AttachItem(entity);
    }
    void OnRecycleEntity(EntityBase entity)
    {
        if (entity.m_Flag != enum_EntityFlag.Enermy)
            return;

        m_HealthGrid.RemoveItem(entity.I_EntityID);
    }
}
