using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBullet : SFXBase {
    float m_bulletDamage;
    Vector3 m_simulateGravity;
    Vector2 m_BulletSpeed;
    public void Play(float damage,Vector3 direction,Vector2 bulletSpeed)
    {
        m_bulletDamage = damage;
        m_simulateGravity = Vector3.zero;
        m_BulletSpeed = bulletSpeed;
        transform.rotation = Quaternion.LookRotation(direction);
        base.Play(GameConst.I_BulletMaxLastTime);
    }
    private void FixedUpdate()
    {
        m_simulateGravity += Time.fixedDeltaTime * m_BulletSpeed.y * Vector3.down;
        transform.Translate((transform.forward * m_BulletSpeed.x + m_simulateGravity) * Time.fixedDeltaTime, Space.World);
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
