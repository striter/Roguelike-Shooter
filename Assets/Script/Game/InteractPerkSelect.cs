using System;
using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkSelect : InteractBattleBase
{
    public override enum_Interaction m_InteractType => enum_Interaction.PerkSelect;
    List<int> m_PerkIDs;
    TSpecialClasses.ParticleControlBase m_Particles;
    public override void OnPoolInit(enum_Interaction identity, Action<enum_Interaction, MonoBehaviour> OnRecycle)
    {
        base.OnPoolInit(identity, OnRecycle);
        m_Particles = new TSpecialClasses.ParticleControlBase(transform.Find("Particles"));
    }

    public InteractPerkSelect Play(List<int> _perkID)
    {
        base.Play();
        m_PerkIDs = _perkID;
        m_Particles.Play();
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_Particles.Stop();
        Vector3[] v3List = new Vector3[3] { new Vector3 (-1.5f,0), new Vector3(1.5f, 0), new Vector3(0, 0, 1.5f) };
        int num = 0;
        InteractPerkPickup[] interactPerkPickupList = new InteractPerkPickup[3];
        m_PerkIDs.Traversal((int perk) => {
            interactPerkPickupList[num]=GameObjectManager.SpawnInteract<InteractPerkPickup>(transform.position+ v3List[num], Quaternion.identity).Play(perk);
            num++;
        });

        for (int i = 0; i < interactPerkPickupList.Length; i++)
        {
            interactPerkPickupList[i].m_interactPerkPickupList = new List<InteractPerkPickup> ();
            for (int j = 0; j < interactPerkPickupList.Length; j++)
            {
                if (i != j)
                {
                    interactPerkPickupList[i].m_interactPerkPickupList.Add(interactPerkPickupList[j]);
                }
            }
        }
        //BattleUIManager.Instance.ShowPage<UI_PerkSelect>(true,true, .5f).Show(m_PerkIDs,_interactor.m_CharacterInfo.OnActionPerkAcquire);
        return false;
    }
}
