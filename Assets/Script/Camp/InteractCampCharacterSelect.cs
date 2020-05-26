using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractCampCharacterSelect : InteractCampBase {
    public override enum_Interaction m_InteractType => enum_Interaction.CampCharaceterSelect;
    public Transform m_CameraPos { get; private set; }
    Transform m_CharacterPos;

    public EntityCharacterPlayer m_CharacterModel { get; private set; }
    float m_YawRotation, m_YawOrigin,m_YawRotationSpeed;
    bool m_RotateDraging;
    PE_FocalDepth m_FocalDepth;
    public override void Init()
    {
        base.Init();
        m_CharacterPos = transform.Find("CharacterPos");
        m_CameraPos = transform.Find("CameraPos");
        m_YawOrigin = m_CharacterPos.rotation.eulerAngles.y;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_CharacterModel = null;
        CampManager.Instance.OnCharacterSelectInteract(this);
        m_FocalDepth = CameraController.Instance.m_Effect.GetOrAddCameraEffect<PE_FocalDepth>().SetEffect(2);
        return true;
    }

    public void OnGenerateCharacter(enum_PlayerCharacter character)
    {
        if (m_CharacterModel && character == m_CharacterModel.m_Character)
            return;

        if (m_CharacterModel)
            m_CharacterModel.DoRecycle();

        m_YawRotation = m_YawOrigin;
        m_CharacterModel = GameObjectManager.SpawnPlayerCharacter(character,m_CharacterPos.position,m_CharacterPos.rotation);
    }

    public void OnCharacterSelected()
    {
        m_CharacterModel = null;
        m_FocalDepth = null;
        CameraController.Instance.m_Effect.RemoveCameraEffect<PE_FocalDepth>();
    }

    private void Update()
    {
        if (!m_CharacterModel)
            return;

        m_FocalDepth.SetFocalTarget(m_CharacterModel.tf_Head.position, 2f);
        float delta = Time.deltaTime * 60f;
        if (Mathf.Abs(m_YawRotationSpeed) <= delta)
            m_YawRotationSpeed = 0;
        else
            m_YawRotationSpeed -= Mathf.Sign(m_YawRotationSpeed) * delta;

        m_YawRotation += m_YawRotationSpeed;
        m_CharacterModel.transform.rotation = Quaternion.Euler(0, m_YawRotation, 0);
    }

    public void RotateCharacter(Vector2 delta)
    {
        m_YawRotationSpeed += delta.x/50f;
    }
}
