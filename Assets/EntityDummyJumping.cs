using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class EntityDummyJumping : EntityBase {
    Rigidbody rb_current;
    public override void Init(int id, SEntity entityInfo)
    {
        base.Init(id, entityInfo);
        rb_current = GetComponent<Rigidbody>();
        rb_current.velocity = Vector3.zero;
    }
    float jumpCheck;
    bool tik_tok = false;
    protected override void Update()
    {
        base.Update();
        if (m_Target != null && Time.time > jumpCheck)
        {
            jumpCheck = Time.time + .5f;
            tik_tok = !tik_tok;
            Vector3 jumpDirection;
            if (tik_tok)
                jumpDirection = m_Target.transform.position - transform.position + Vector3.up * 20;
            else
                jumpDirection = Vector3.down;
            jumpDirection.Normalize();
            rb_current.AddForce(jumpDirection*7, ForceMode.Impulse);
        }
    }
    
}
