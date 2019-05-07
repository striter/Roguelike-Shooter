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
        transform.Translate((transform.forward *GameConst.I_BulletSpeedForward + Vector3.down * GameConst.I_BulletSpeedDownward) * Time.deltaTime, Space.World);
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
