using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXShell : SFXBase
{
    public float F_PlayDelay = .5f;
    Rigidbody rb_current;
    float f_startTime;
    protected override void Awake()
    {
        rb_current = GetComponent<Rigidbody>();
        base.Awake();
        f_duration = GameSettings.CI_ShellLifeTime;
    }
    public override void Play()
    {
        base.Play();
        transform.localScale = Vector3.zero;
        f_startTime = Time.time;
        rb_current.isKinematic = true;
    }
    protected override void OnTickDelta(float delta)
    {
        base.OnTickDelta(delta);
        if (rb_current.isKinematic&&Time.time- f_startTime > F_PlayDelay)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            rb_current.isKinematic = false;
            rb_current.AddForce(transform.right * 100f);
            transform.localScale = Vector3.one;
            transform.SetParent(null);
        }
    }
}
