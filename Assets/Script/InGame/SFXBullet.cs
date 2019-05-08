using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBullet : SFXBase {
    float m_bulletDamage;
    Vector3 m_simulateGravity;
    public void Play(float damage,Vector3 direction)
    {
        m_bulletDamage = damage;
        m_simulateGravity = Vector3.zero;
        base.Play(GameConst.I_BulletMaxLastTime);
    }
    private void FixedUpdate()
    {
        m_simulateGravity += Time.fixedDeltaTime * GameConst.I_BulletSpeedDownward * Vector3.down;
        transform.Translate((transform.forward *GameConst.I_BulletSpeedForward + m_simulateGravity) * Time.fixedDeltaTime, Space.World);
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
