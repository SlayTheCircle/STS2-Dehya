using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using DehyaMod.Content.Relics;

namespace DehyaMod.Content.Compat;

/// <summary>
/// 伤害加成遗物基线:0.111 的 AbstractModel.ModifyDamageAdditive 带 CardPlay 第六参,0.107.1 没有。
/// 差异吸收在本基线(与 DehyaPowerBase 同款做法,子类只覆写五参 Core),
/// 两个游戏目标共用同一份逻辑(现有子类不使用 cardPlay)。
/// </summary>
public abstract class DehyaDamageRelicBase : DehyaRelicBase
{
#if !MOD_GAME_0107_1
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => ModifyDamageAdditiveCore(target, amount, props, dealer, cardSource);
#else
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        => ModifyDamageAdditiveCore(target, amount, props, dealer, cardSource);
#endif

    protected virtual decimal ModifyDamageAdditiveCore(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
        => 0m;
}
