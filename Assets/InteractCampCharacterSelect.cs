using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampCharacterSelect : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampCharaceterSelect;
    public Transform m_CameraPos { get; private set; }
    Transform m_CharacterPos;

    EntityCharacterPlayer m_Character;
    float rotation = 0;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterPos = transform.Find("CharacterPos");
        m_CameraPos = transform.Find("CameraPos");
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        CampManager.Instance.OnCharacterSelectInteract(this);
        return true;
    }

    public EntityCharacterBase ShowCharacter(enum_PlayerCharacter character)
    {
        RecycleCharacter();
        rotation = m_CameraPos.rotation.eulerAngles.y;
         m_Character = GameObjectManager.SpawnPlayerCharacter(character,m_CharacterPos.position,m_CharacterPos.rotation);
        return m_Character;
    }
    public void RecycleCharacter()
    {
        if (m_Character)
            m_Character.DoItemRecycle();
        m_Character = null;
    }

    public void RotateCharacter(Vector2 delta)
    {
        rotation += delta.x;
        m_Character.transform.rotation = Quaternion.Euler(0,rotation,0);
    }
}
