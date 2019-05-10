using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSetting;
public class SFXBullet : SFXBase {
    protected float m_bulletDamage;
    Vector3 m_simulateGravity;
    Vector2 m_BulletSpeed;
    public void Play(int sourceID, float damage,Vector3 direction,Vector2 bulletSpeed)
    {
        m_bulletDamage = damage;
        m_simulateGravity = Vector3.zero;
        m_BulletSpeed = bulletSpeed;
        transform.rotation = Quaternion.LookRotation(direction);
        base.Play(sourceID,GameConst.I_BulletMaxLastTime);
    }
    private void FixedUpdate()
    {
        m_simulateGravity += Time.fixedDeltaTime * m_BulletSpeed.y * Vector3.down;
        transform.Translate((transform.forward * m_BulletSpeed.x + m_simulateGravity) * Time.fixedDeltaTime, Space.World);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == GameLayer.I_Entity)
        {
            HitCheckEntity target = other.GetComponent<HitCheckEntity>();
            if (target.I_AttacherID != I_SourceID)
            {
                target.TryHit(m_bulletDamage);
                OnPlayFinished();
            }
        }
        else if (other.gameObject.layer == GameLayer.I_Static)
        {
            OnPlayFinished();
        }
    }
}
