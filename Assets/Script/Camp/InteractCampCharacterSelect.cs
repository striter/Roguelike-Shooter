using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampCharacterSelect : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampCharaceterSelect;
    public Transform m_CameraPos { get; private set; }
    Transform m_CharacterPos;

    public EntityCharacterPlayer m_CharacterModel { get; private set; }
    float m_YawRotation, m_YawOrigin;
    bool m_RotateDraging;

    protected override void Awake()
    {
        base.Awake();
        m_CharacterPos = transform.Find("CharacterPos");
        m_CameraPos = transform.Find("CameraPos");
        m_YawOrigin = m_CharacterPos.rotation.eulerAngles.y;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_CharacterModel = null;
        CampManager.Instance.OnCharacterSelectInteract(this);
        return true;
    }

    public void ShowCharacter(enum_PlayerCharacter character)
    {
        if (m_CharacterModel && character == m_CharacterModel.m_Character)
            return;

        if (m_CharacterModel)
            m_CharacterModel.DoRecycle();

        m_YawRotation = m_YawOrigin;
        m_CharacterModel = GameObjectManager.SpawnPlayerCharacter(character,m_CharacterPos.position,m_CharacterPos.rotation);
    }

    private void Update()
    {
        if (!m_CharacterModel)
            return;

        if (m_RotateDraging)
        {
            m_RotateDraging = false;
        }
        else
        {
            float offset = Mathf.Abs(m_YawOrigin - m_YawRotation);
            int deltaMultiply = offset > 360 ? 10 : 1;
            float delta = Time.deltaTime * 90f * deltaMultiply;

            if (offset <= delta)
                m_YawRotation = m_YawOrigin;
            else
                m_YawRotation = m_YawRotation + (m_YawRotation > m_YawOrigin ? -1 : 1) * delta;
        }
        m_CharacterModel.transform.rotation = Quaternion.Euler(0, m_YawRotation, 0);
    }

    public void RotateCharacter(Vector2 delta)
    {
        m_YawRotation += delta.x / (m_YawRotation > 360 ? 1f : 5f);
        m_RotateDraging = true;
    }
}
