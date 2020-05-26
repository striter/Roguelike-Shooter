using System.Collections;
using System.Collections.Generic;
using GameSetting;
using UnityEngine;

public class InteractPerkShrine : InteractBattleBase {
    public override enum_Interaction m_InteractType => enum_Interaction.PerkShrine;
    int m_TryCount;
    float m_BaseTradePrice;
    public int I_MuzzleSuccess;
    public InteractPerkShrine Play(float tradePrice)
    {
        base.Play();
        m_TryCount = 0;
        m_BaseTradePrice = tradePrice;
        SetTradePrice(tradePrice);
        return this;
    }

    protected override bool OnInteractedContinousCheck(EntityCharacterPlayer _interactor)
    {
        base.OnInteractedContinousCheck(_interactor);
        m_TryCount++;
        SetTradePrice(m_BaseTradePrice*GameExpression.GetPerkShrinePriceMultiply(m_TryCount));
        enum_Rarity rarity = TCommon.RandomPercentage(GameConst.D_PerkShrineRate, enum_Rarity.Invalid);
        if (rarity != enum_Rarity.Invalid)
        {
            _interactor.m_CharacterInfo.OnActionPerkAcquire(GameDataManager.RandomPlayerPerk(rarity, _interactor.m_CharacterInfo.m_ExpirePerks));
            GameObjectManager.PlayMuzzle(-1,_interactor.transform.position,Vector3.up, I_MuzzleSuccess);
        }
        return m_TryCount < GameConst.I_PerkShrineTryCountMax;
    }
}
