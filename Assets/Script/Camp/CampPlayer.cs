using GameSetting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampPlayer : MonoBehaviour {
    public float F_MoveSpeed;
    CharacterController m_Controller;
    public CampInteract m_Interact { get; private set; }
    private void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
        transform.Find("InteractDetector").GetComponent<InteractDetector>().Init(OnInteractCheck);
    }
    private void Start()
    {
        CameraController.Attach(this.transform);
        CameraController.Instance.RotateCamera(new Vector2( 180f, 0));
    }
    private void OnEnable()
    {
        PCInputManager.Instance.AddMouseRotateDelta(OnRotateDelta);
        PCInputManager.Instance.AddMovementDelta(OnMovementDelta);
        PCInputManager.Instance.AddBinding<EntityCharacterPlayer>(enum_BindingsName.Fire, OnInteractDown);
    }
    private void OnDisable()
    {
        PCInputManager.Instance.DoBindingRemoval<CampPlayer>();
    }
    private void Update()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, CameraController.CameraXZRotation, GameConst.F_PlayerCameraSmoothParam);
    }
    void OnRotateDelta(Vector2 delta)
    {
        delta.y = 0;
        delta.x = (delta.x / Screen.width) * 180f;
        CameraController.Instance.RotateCamera(delta);
    }
    void OnMovementDelta(Vector2 delta)
    {
        Vector3 moveDirection = (transform.right * delta.x + transform.forward * delta.y).normalized;
        m_Controller.Move(moveDirection*Time.deltaTime* F_MoveSpeed);
    }
    void OnInteractDown(bool down)
    {
        if (m_Interact == null)
            return;

        m_Interact.TryInteract(null);
        m_Interact = null;
    }

    public void OnInteractCheck(InteractBase interactTarget, bool isEnter)
    {
        if (!interactTarget.B_Interactable)
            return;

        if (interactTarget.B_InteractOnTrigger)
        {
            interactTarget.TryInteract(null);
            return;
        }

        if (isEnter)
            m_Interact = interactTarget as CampInteract;
        else if (m_Interact == interactTarget)
            m_Interact = null;
    }
}
