
using UnityEngine;
using UnityEngine.Events;

public class InteractBase : MonoBehaviour
{
    public float F_InteractCoolDown=.5f;
    public bool TriggerOnce=false;
    public UnityEvent OnInteractTrigger;
    bool b_Interactable;
    float f_interactCheck;
    protected virtual void Awake()
    {
        b_Interactable = true;
        if (F_InteractCoolDown == 0)
            Debug.LogError("Set Interact Cool Down Please!");
    }

    protected bool TryInteract()
    {
        if (b_Interactable&&Time.time > f_interactCheck)
        {
            if (TriggerOnce)
                b_Interactable = false;
            f_interactCheck = Time.time + F_InteractCoolDown;
            OnStartInteract();
            return true;
        }
        return false;
    }
    protected virtual void OnStartInteract()
    {
        OnInteractTrigger?.Invoke();
    }
}
