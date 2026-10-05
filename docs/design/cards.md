# 现行卡表

<!-- 生成文件:scripts/export-card-table.py 从源码 ctor 与 zhs 本地化生成,勿手改。 -->
<!-- 过时校验:check.sh 调用 --check;再生成:python3 scripts/export-card-table.py -->

共 88 张（含衍生 token）。效果文本为当前运行文本;升级数值以源码与游戏内为准。
稀有度颜色对照：普通=白卡，罕见=蓝卡，稀有=金卡。
原案数值与设计过程见[技术历史](../history/design/README.md)。

## 初始卡（4）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 以守待攻 | 初始 | 1 | 攻击 | 造成{Damage:diff()}点伤害，获得{Block:diff()}点[gold]格挡[/gold]。 |
| 防御 | 初始 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。 |
| 打击 | 初始 | 1 | 攻击 | 造成{Damage:diff()}点伤害。 |
| 熔铁之拳 | 初始 | 0 | 攻击 | 造成{Damage:diff()}点伤害，给予{VulnerablePower:diff()}层[gold]易伤[/gold]。 |

## 攻击（31）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 借力打力 | 普通 | 2 | 攻击 | 造成{Damage:diff()}点伤害2次。若在下回合开始前你失去过生命，获得{BorrowedForceMarkerPower:diff()}点[gold]荆棘[/gold]。 |
| 养精蓄锐 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害，抽{Cards:diff()}张牌。 |
| 净焰剑护 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害，获得{DexterityPower:diff()}点[gold]敏捷[/gold]和{ThornsPower:diff()}点[gold]荆棘[/gold]。 |
| 拳甲打击 | 普通 | 1 | 攻击 | 造成{Damage:diff()}点伤害，获得{Block:diff()}点[gold]格挡[/gold]。 |
| 鎏金突击 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害，获得{DexterityPower:diff()}点[gold]敏捷[/gold]。 |
| 迎头猛击 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害{Hits:diff()}次，给予自身{VulnerablePower:diff()}层[gold]易伤[/gold]。 |
| 奉还痛楚 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害{Hits:diff()}次。 |
| 以逸待劳 | 普通 | 1 | 攻击 | 对所有敌人造成{Damage:diff()}点伤害，获得{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 明映万乘 | 普通 | 2 | 攻击 | 对所有敌人造成{Damage:diff()}点伤害，抽{Cards:diff()}张牌。 |
| 舍身打击 | 普通 | 1 | 攻击 | 失去{HpLoss:diff()}点生命，造成{Damage:diff()}点伤害。 |
| 炽热余烬 | 普通 | 0 | 攻击 | 造成{Damage:diff()}点伤害，丢弃1张手牌。若弃掉的不是0费牌，抽{Cards:diff()}张牌。 |
| 当头棒喝 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害，给予{WeakPower:diff()}层[gold]虚弱[/gold]。 |
| 血溅沙场 | 罕见 | 1 | 攻击 | 本回合你每失去过一次生命，便造成{Damage:diff()}点伤害一次。 |
| 驰骋荒漠 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害。 |
| 格挡猛击 | 罕见 | 3 | 攻击 | 获得{Block:diff()}点[gold]格挡[/gold]，造成{Damage:diff()}点伤害。若本回合你已打出的牌少于{CardsThreshold}张：失去{HpLoss:diff()}点生命，获得{Energy:energyIcons()}点能量。 |
| 重金悬赏 | 罕见 | 0 | 攻击 | 造成{CalculatedDamage:diff()}点伤害，每持有100金币，伤害额外+{ExtraDamage:diff()}。以此牌击败敌人时，战斗胜利后获得30金币。 |
| 铁血之剑 | 罕见 | 3 | 攻击 | 造成{Damage:diff()}点伤害，获得{RegenPower:diff()}层[gold]再生[/gold]。 |
| 斩铁断金 | 罕见 | 3 | 攻击 | 造成 {Damage:diff()} 点伤害。你打出的下一张攻击牌耗能 -2。 |
| 陷阵之志 | 罕见 | 3 | 攻击 | 造成{Damage:diff()}点伤害。
若此伤害击败敌人，对一名随机存活的敌人免费再打出这张牌（可以连续触发）。 |
| 狮牙之怒 | 罕见 | 2 | 攻击 | 造成{Damage:diff()}点伤害，抽{Cards:diff()}张牌。 |
| 蓄势打击 | 罕见 | 1 | 攻击 | 造成{Damage:diff()}点伤害。抽{Cards:diff()}张牌。
每次打出这张牌：费用+1、抽牌数+1（本场战斗）。 |
| 黄沙百战 | 罕见 | 2 | 攻击 | 造成{CalculatedDamage:diff()}点伤害。弃牌堆中每有一张费用不低于2的牌，伤害额外+{ExtraDamage:diff()}。 |
| 渐入佳境 | 稀有 | 0 | 攻击 | 造成{Damage:diff()}点伤害，获得{StrengthPower:diff()}点[gold]力量[/gold]。
每次打出这张牌：费用+1、伤害翻倍（本场战斗）。 |
| 赤焰流火 | 稀有 | 3 | 攻击 | 失去{HpLoss:diff()}点生命，造成{Damage:diff()}点伤害。 |
| 燎原野火 | 稀有 | 5 | 攻击 | 造成 {Damage:diff()} 点伤害。本回合内你每打出一张 0 费牌，这张牌的耗能 -1（可叠加；回合结束或打出后复原）。 |
| 夜尽天明 | 稀有 | 0 | 攻击 | 造成 {Damage:diff()} 点伤害并标记敌人：你每打出一张以该敌人为目标的 0 费攻击牌，该敌人失去 {DawnbreakMarkPower:diff()} 点生命。 |
| 终结之斩 | 稀有 | 3 | 攻击 | 造成{Damage:diff()}点伤害，获得{Energy:energyIcons()}。
若此伤害击败敌人，将这张牌的一个复制（保留升级状态）放入抽牌堆。 |
| 准备万全 | 稀有 | 0 | 攻击 | 移除你全部的[gold]覆甲[/gold]，对所有敌人造成{CalculatedDamage:diff()}点伤害。若没有[gold]覆甲[/gold]，改为获得{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 千锤百炼 | 稀有 | 3 | 攻击 | 造成{CalculatedDamage:diff()}点伤害。每有1层[gold]覆甲[/gold]，伤害额外+{ExtraDamage:diff()}。随后移除你全部的[gold]覆甲[/gold]，获得{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 剑斩群岳 | 稀有 | 4 | 攻击 | 造成 {CalculatedDamage:diff()} 点伤害；你每有 1 层 [gold]荆棘[/gold]，伤害 +{ExtraDamage:diff()}。 |
| 锤砺锋芒 | 稀有 | 2 | 攻击 | 造成 {Damage:diff()} 点伤害。本局游戏内你每打出一张耗能不低于 2 的牌，这张卡的伤害永久 +{Bonus:diff()}。 |

## 技能（31）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 预支定金 | 普通 | 0 | 技能 | 消耗{Gold}金币，抽{Cards:diff()}张牌。 |
| 黄沙蔽日 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]和{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 紧急止血 | 普通 | 0 | 技能 | 移除你的[gold]易伤[/gold]和[gold]虚弱[/gold]，获得{RegenPower:diff()}层[gold]再生[/gold]。 |
| 烈狮怒瞳 | 普通 | 0 | 技能 | 每有一名敌人持有攻击意图：获得{Block:diff()}点[gold]格挡[/gold]并抽1张牌。若没有敌人持有攻击意图：改为获得{RegenPower:diff()}层[gold]再生[/gold]，且此牌打出后消耗。 |
| 快速调整 | 普通 | 0 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。若本回合你尚未打出过费用不低于2的牌：抽{Cards}张牌；否则弃1张牌，获得{Energy:energyIcons()}点能量。 |
| 灼热之血 | 普通 | 1 | 技能 | 失去{HpLoss:diff()}生命。本回合获得{StrengthPower:diff()}点力量和{DexterityPower:diff()}点敏捷。 |
| 伤口处理 | 普通 | 1 | 技能 | 获得{RegenPower:diff()}层[gold]再生[/gold]，抽{Cards:diff()}张牌。 |
| 处变不惊 | 普通 | 1 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。在下个回合获得{Energy:energyIcons()}。 |
| 调整呼吸 | 罕见 | 1 | 技能 | 抽1张牌。若抽到的是0费牌，弃1张牌，再抽{Cards:diff()}张牌。 |
| 至痛至怒 | 罕见 | 0 | 技能 | 消耗你手牌、抽牌堆和弃牌堆中所有的状态牌和诅咒牌。每消耗1张，获得{PlatingPower:diff()}层[gold]覆甲[/gold]和{ThornsPower:diff()}点[gold]荆棘[/gold]。 |
| 卸甲强袭 | 罕见 | 1 | 技能 | 移除你全部的[gold]格挡[/gold]并获得{CalculatedStrength:diff()}点力量（每3点[gold]格挡[/gold]换1点力量）。本回合内你无法获得[gold]格挡[/gold]。 |
| 整装待发 | 罕见 | X | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]X次。下个回合获得X点[gold]力量[/gold]和{IfUpgraded:show:X\|X-1}点能量。 |
| 焚势掠地 | 罕见 | 3 | 技能 | 抽牌，直到抽到一张费用不低于2的牌。 |
| 负血奋进 | 罕见 | 1 | 技能 | 抽{Cards:diff()}张牌。若你的生命值高于50%，失去{HpLoss:diff()}生命并获得{StrengthPower:diff()}点力量；否则再抽1张牌。 |
| 热血沸腾 | 罕见 | 0 | 技能 | 抽{Cards:diff()}张牌，失去{HpLoss:diff()}生命。若你的生命值高于50%，再抽2张牌。 |
| 悬赏委托 | 罕见 | 2 | 技能 | 标记一名敌人。该敌人被击败时：获得1点能量，抽{Cards:diff()}张牌，并在战斗胜利后获得{BountyMarkPower:diff()}金币。 |
| 剑斗技巧 | 罕见 | 3 | 技能 | 下回合获得 {DelayedTemporaryStrengthPower:diff()} 点临时力量。本回合内你每打出一张攻击牌，这张牌的耗能 -1（回合结束或打出后复原）。 |
| 忘却极限 | 罕见 | 3 | 技能 | 抽牌直到手牌已满。本次抽到的每张非0费牌使你失去{HpLoss:diff()}点生命。 |
| 搏命之灵 | 罕见 | 1 | 技能 | 失去{HpLoss:diff()}生命，获得{StrengthPower:diff()}点力量。若你的生命值不高于50%，再获得{BonusStrength:diff()}点力量。 |
| 裂帛灼金 | 罕见 | 4 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]、{ThornsPower:diff()}点[gold]荆棘[/gold]和{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 血气方刚 | 罕见 | 0 | 技能 | 失去{HpLoss:diff()}生命，获得{Energy:energyIcons()}。若你的生命值不高于50%，再获得{BonusEnergy:energyIcons()}。 |
| 加固运输 | 罕见 | 1 | 技能 | 获得{CalculatedBlock:diff()}点[gold]格挡[/gold]。每持有100金币：消耗{GoldCost}金币，提供{CalculationExtra:diff()}点额外格挡。 |
| 步步为营 | 罕见 | 1 | 技能 | 获得{PlatingPower:diff()}层[gold]覆甲[/gold]。若到下回合开始时你未因攻击失去生命，获得{SteadyEncampmentMarkerPower:diff()}层[gold]再生[/gold]。 |
| 坚毅不倒 | 罕见 | 3 | 技能 | 获得{Block:diff()}点[gold]格挡[/gold]。在下个回合，获得{BlockNextTurnPower:diff()}点[gold]格挡[/gold]。 |
| 包扎伤口 | 稀有 | 2 | 技能 | 获得{RegenPower:diff()}层[gold]再生[/gold]。若在下回合结束前你失去生命：移除你全部的[gold]再生[/gold]，并对所有敌人造成{BandageWoundsMarkerPower:diff()}点伤害×被移除的层数。 |
| 碧血丹心 | 稀有 | 3 | 技能 | 失去{HpLoss:diff()}生命，获得{Energy:energyIcons()}，抽{Cards:diff()}张牌，获得{ThornsPower:diff()}点[gold]荆棘[/gold]。 |
| 炽鬃怒喝 | 稀有 | 0 | 技能 | 给予所有敌人{WeakPower:diff()}层[gold]虚弱[/gold]。 |
| 长夜明焰 | 稀有 | 2 | 技能 | 获得{RegenPower:diff()}层[gold]再生[/gold]，结束你的回合。 |
| 加码加价 | 稀有 | 1 | 技能 | 消耗抽牌堆顶部的{Cards}张牌，获得{StrengthPower:diff()}点[gold]力量[/gold]。本场战斗的奖励金币清零。 |
| 重整旗鼓 | 稀有 | 3 | 技能 | 移除你身上的[gold]易伤[/gold]、[gold]虚弱[/gold]与[gold]脆弱[/gold]，抽{Cards:diff()}张牌。 |
| 活力迸发 | 稀有 | 1 | 技能 | 获得{Energy:energyIcons()}点能量。本回合你无法抽牌，也无法获得能量。 |

## 能力（20）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 血之誓约 | 罕见 | 1 | 能力 | 每回合开始时，若你的生命值不高于50%，抽{BloodOathPower:diff()}张牌并获得{BloodOathPower:diff()}点能量。 |
| 燃血成灰 | 罕见 | 2 | 能力 | 每回合你第一次失去生命时，可以从抽牌堆选择1张牌消耗，并恢复{BloodToAshesPower:diff()}生命。 |
| 染血纷争 | 罕见 | 2 | 能力 | 你每打出一张费用2或以上的牌，恢复等同于其费用的生命。 |
| 烈狮血性 | 罕见 | 3 | 能力 | 每回合你打出的第一张费用大于等于3的牌：你获得{FierceLionSpiritPower:diff()}层[gold]易伤[/gold]，并获得{FierceLionSpiritPower:diff()}点能量。 |
| 铜墙铁壁 | 罕见 | 3 | 能力 | 获得{PlatingPower:diff()}层[gold]覆甲[/gold]。 |
| 两败俱伤 | 罕见 | 1 | 能力 | 施放时和每回合开始时，给予所有敌人{MutualHarmPower:diff()}层[gold]易伤[/gold]。每回合开始时，你获得{MutualHarmPower:diff()}层[gold]易伤[/gold]。 |
| 咆哮之剑 | 罕见 | 1 | 能力 | 每回合你打出的第一张费用大于等于2的牌：抽{RoaringBladePower:diff()}张牌。 |
| 纵横沙海 | 罕见 | 1 | 能力 | 回合结束时，若你的[gold]格挡[/gold]至少为{Threshold:diff()}点，对所有敌人造成{AoeDamage:diff()}点伤害，并获得{RegenPower:diff()}层[gold]再生[/gold]。 |
| 余焰不息 | 罕见 | 2 | 能力 | 每当你打出一张耗能不低于 2 的牌，将一张随机的 0 费牌置入弃牌堆。 |
| 炎啸狮咬 | 稀有 | 2 | 能力 | 每当你打出一张 0 费牌，获得 {BlazingLionBitePower:diff()} 点力量（由本能力追踪）。回合结束时，移除全部由其给予的力量，并对所有敌人造成等量伤害。 |
| 炽鬃狮血 | 稀有 | 1 | 能力 | 你每缺失 {BlazingLionBloodPower:diff()} 点生命，便获得 1 点力量；生命恢复时相应回落。 |
| 燃烧心火 | 稀有 | 3 | 能力 | 你的 0 费攻击牌造成的伤害 +{BurningHeartPower:diff()}。 |
| 灼目之鬃 | 稀有 | 2 | 能力 | 每当你因 [gold]荆棘[/gold] 对敌人造成伤害，再对该敌人造成一次等值伤害。 |
| 死战不屈 | 稀有 | 2 | 能力 | 失去生命的效果无法将你的生命值降至0。攻击伤害仍可击倒你。 |
| 殊死搏斗 | 稀有 | 1 | 能力 | 回合开始时，若上回合起你失去过生命，本回合获得{DesperateBrawlPower:diff()}点临时力量。 |
| 重剑无锋 | 稀有 | 2 | 能力 | 每当你打出一张耗能不低于 2 的牌，获得其耗能 ×{EdgelessGreatswordPower:diff()} 的 [gold]格挡[/gold]。 |
| 雇佣关系 | 稀有 | 1 | 能力 | 获得{Gold:diff()}金币。回合开始时，每持有100金币，消耗3金币并获得1点能量；每击败一名敌人，战斗胜利后获得{MercenaryContractPower:diff()}金币。 |
| 极速突击 | 稀有 | 2 | 能力 | 每当你打出1张费用为0的卡牌时，获得{RapidOnslaughtPower:diff()}点[gold]活力[/gold]。 |
| 灼热形态 | 稀有 | 3 | 能力 | 获得{PlatingPower:diff()}层[gold]覆甲[/gold]。每回合开始时：失去1点生命，获得1层[gold]荆棘[/gold]，对所有敌人造成{AoeDamage:diff()}点伤害。 |
| 焰灼夜幕 | 稀有 | 1 | 能力 | 回合内你每失去一次生命，获得{ScorchingNightveilPower:diff()}点[gold]荆棘[/gold]。 |

## 先古强化（2）

| 卡牌 | 稀有度 | 费用 | 类型 | 效果 |
|---|---|---|---|---|
| 铓辉灿漫 | 先古 | 2 | 能力 | 每回合开始时，恢复{BladeBrilliancePower:diff()}点生命。 |
| 怒势疾迅 | 先古 | 0 | 攻击 | 造成{Damage:diff()}点伤害，给予{VulnerablePower:diff()}层[gold]易伤[/gold]，抽{Cards:diff()}张牌。 |
