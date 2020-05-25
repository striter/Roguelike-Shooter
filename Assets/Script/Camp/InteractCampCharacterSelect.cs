using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampCharacterSelect : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampCharaceterSelect;
    public Transform m_CameraPos { get; private set; }
    Transform m_CharacterPos;

    public EntityCharacterPlayer m_CharacterModel { get; private set; }
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

    public void ShowCharacter(enum_PlayerCharacter character)
    {
        if (m_CharacterModel && character == m_CharacterModel.m_Character)
            return;

        if (m_CharacterModel)
            m_CharacterModel.DoRecycle();

        rotation = m_CameraPos.rotation.eulerAngles.y;
        m_CharacterModel = GameObjectManager.SpawnPlayerCharacter(character,m_CharacterPos.position,m_CharacterPos.rotation);
    }

    public void RotateCharacter(Vector2 delta)
    {
        rotation += delta.x;
        m_CharacterModel.transform.rotation = Quaternion.Euler(0,rotation,0);
    }
}
