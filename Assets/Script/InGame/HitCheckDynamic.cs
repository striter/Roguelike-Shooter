using System;
using GameSetting;

public class HitCheckDynamic : HitCheckBase {
    public override enum_HitCheck m_HitCheckType => enum_HitCheck.Dynamic;
    public new void Attach(int attacherID,Func<DamageInfo,bool> OnTryDamage)
    {
        base.Attach(attacherID,OnTryDamage);
    }
}
