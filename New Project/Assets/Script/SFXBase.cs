
using UnityEngine;

public class SFXBase : MonoBehaviour,ISingleCoroutine
{
    public enum_WeaponSFX E_Current { get; private set; }
    protected float f_duration;
    protected float f_durationCheck;
    public void Init(enum_WeaponSFX type)
    {
        E_Current = type;
    }
    protected virtual void Awake()
    {
        Reset();
    }
    public virtual void Reset()
    {
    }
    public virtual void OnDisable()
    {
        f_durationCheck += f_duration;
    }
    public virtual void Play()      
    {
        f_durationCheck = 0f;
        this.StartSingleCoroutine(1,TIEnumerators.TickDelta(OnTickDelta));
    }
    protected virtual void OnTickDelta(float delta)  //Use Coroutine To Make Sure SFX Still OnTicking While Parent Aren't Enabled
    {
        f_durationCheck += delta;
        if (f_durationCheck > f_duration)
        {
            Reset();
            EntityManager.RecycleWeaponSFX(E_Current, this);
            this.StopSingleCoroutine(1);
        }
    }
}
