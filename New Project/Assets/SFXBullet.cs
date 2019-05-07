using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBullet : SFXBase {
    float m_bulletDamage;
    public void Play(float damage,Vector3 direction)
    {
        m_bulletDamage = damage;

        base.Play(GameConst.I_BulletMaxLastTime);
    }
    private void FixedUpdate()
    {
        transform.Translate((transform.forward * 50f + Vector3.down * 1.5f) * Time.deltaTime, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameLayer.I_Entity)
        {
            other.GetComponent<HitCheckBase>().TryHit(m_bulletDamage);
            OnPlayFinished();
        }
    }
}
