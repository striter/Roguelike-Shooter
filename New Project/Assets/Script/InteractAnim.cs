using UnityEngine;
using UnityEngine.Events;

public class InteractAnim : InteractBase, ISingleCoroutine
{
    [System.Serializable]
    public class OnSwitch : UnityEvent<bool> { }
    public OnSwitch OnInteractSwitchOn;
    HitCheckBase m_hitCheck;
    protected Transform tf_ButtonMain;
    public enum enum_AnimType
    {
        Invalid = -1,
        InAndOut,
        InOrOut,
    }
    public enum_AnimType E_AnimType = enum_AnimType.Invalid;
    public bool b_SwitchOn { get; private set; } = false;
    public bool B_TriggerOnAwake = false;
    Transform tf_AnimPositions;
    Transform tf_AnimStart, tf_AnimEnd;
    protected override void Awake()
    {
        base.Awake();
        tf_ButtonMain = transform.Find("ButtonMain");
        m_hitCheck = tf_ButtonMain.GetComponent<HitCheckStatic>();

        m_hitCheck.Attach((float value,enum_DamageType type,LivingBase target) => {
            return value == -1&& type== enum_DamageType.Interact ? TryInteract() : false;
        });
        

        tf_AnimPositions = transform.Find("AnimPositions");
        tf_AnimStart = tf_AnimPositions.transform.Find("AnimStart");
        tf_AnimEnd = tf_AnimPositions.transform.Find("AnimEnd");

        tf_ButtonMain.localPosition = E_AnimType== enum_AnimType.InOrOut&&b_SwitchOn ? tf_AnimEnd.localPosition : tf_AnimStart.localPosition;
        tf_ButtonMain.localRotation = E_AnimType == enum_AnimType.InOrOut && b_SwitchOn ? tf_AnimEnd.localRotation : tf_AnimStart.localRotation;

        if(B_TriggerOnAwake)
        OnInteractSwitchOn?.Invoke(b_SwitchOn);

    }
    protected virtual void OnDestroy()
    {
        this.StopSingleCoroutine(0);
    }
    protected override void OnStartInteract()
    {
        base.OnStartInteract();
        b_SwitchOn = !b_SwitchOn;
        OnInteractSwitchOn?.Invoke(b_SwitchOn);
        switch (E_AnimType)
        {
            case enum_AnimType.InOrOut:
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                    tf_ButtonMain.localPosition = Vector3.Lerp(tf_AnimStart.localPosition, tf_AnimEnd.localPosition, value);
                    tf_ButtonMain.localRotation = Quaternion.Lerp(tf_AnimStart.localRotation, tf_AnimEnd.localRotation, value);
                }, b_SwitchOn ? 0 : 1, b_SwitchOn ? 1 : 0, F_InteractCoolDown));
                break;
            case enum_AnimType.InAndOut:
                this.StartSingleCoroutine(0, TIEnumerators.ChangeValueTo((float value) => {
                    float timeParam = 1-Mathf.Abs(value - .5f) * 2;
                    tf_ButtonMain.localPosition = Vector3.Lerp(tf_AnimStart.localPosition, tf_AnimEnd.localPosition,  timeParam);
                    tf_ButtonMain.localRotation = Quaternion.Lerp(tf_AnimStart.localRotation, tf_AnimEnd.localRotation, timeParam);
                }, 0, 1, F_InteractCoolDown));
                break;
        }
    }
}
