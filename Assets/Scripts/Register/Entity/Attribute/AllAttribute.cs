using System;
using System.Collections.Generic;
using EventBus;
using Newtonsoft.Json;
using RegisterSystem;

namespace InTime;

public class AllAttribute : RegisterManage<Attribute> {
	/// <summary>
	/// 攻击力
	/// </summary>
	public static Attribute attack;

	/// <summary>
	/// 防御力
	/// </summary>
	public static AttributeDefense defense;

	/// <summary>
	/// 穿透
	/// </summary>
	public static Attribute penetrate;

	/// <summary>
	/// 穿透百分比
	/// </summary>
	public static Attribute penetratePercentage;

	/// <summary>
	/// 暴击率
	/// </summary>
	public static AttributeCriticalProbability criticalHitProbability;

	/// <summary>
	/// 暴击伤害
	/// </summary>
	public static AttributeCriticalHit criticalHitMultiple;

	/// <summary>
	/// 吸血
	/// </summary>
	public static AttributeSuck suckLife;

	/// <summary>
	/// 移速
	/// </summary>
	public static Attribute speed;

	/// <summary>
	/// 攻击速度
	/// </summary>
	public static Attribute attackSpeed;

	/// <summary>
	/// 施法速度
	/// </summary>
	public static Attribute spellSpeed;

	public override void init() {
		base.init();
		penetratePercentage.max = 1;
		criticalHitProbability.max = 1;
		suckLife.max = 1;
		speed.max = 3;
		attackSpeed.max = 3;
		spellSpeed.max = 3;
	}
}

public class Attribute : RegisterBasics {
	protected internal double max = Double.MaxValue;
	protected internal double min = 0;

	public double getMax() => max;
	public double getMin() => min;

	public double limit(double t) => Math.Clamp(t, getMax(), getMin());
}

/// <summary>
/// 暴击伤害
/// </summary>
public class AttributeCriticalHit : Attribute {
	public void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
		EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
		if (entityLiving is null) {
			return;
		}
		if (!@event.stack.hasAttackStack(AllAttackType.criticalHit)) {
			return;
		}
		@event.stack.addMultiple(AllMultiple.criticalHit, entityLiving.get(AllAttribute.criticalHitMultiple));
	}
}

/// <summary>
/// 暴击率
/// </summary>
public class AttributeCriticalProbability : AttributeCriticalHit {
	[Event(priority = 100)]
	public void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipmentOfAttackType @event) {
		EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
		if (entityLiving is null) {
			return;
		}
		if (@event.stack.hasAttackStack(AllAttackType.criticalHit)) {
			return;
		}
		if (entityLiving.getRandom().NextDouble() < entityLiving.get(AllAttribute.criticalHitProbability)) {
			@event.stack.addAttackType(AllAttackType.criticalHit);
		}
	}
}

/// <summary>
/// 防御值减免伤害
/// </summary>
public class AttributeDefense : Attribute {
	public double intermediateValue = 175;

	[Event(priority = -100)]
	public void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
		if (@event.stack.hasAttackStack(AllAttackType.immuneDefense)) {
			return;
		}
		EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
		double d = @event.entityLiving.get(AllAttribute.defense);
		if (entityLiving is not null) {
			d -= entityLiving.get(AllAttribute.penetrate);
			d *= 1 - entityLiving.get(AllAttribute.penetratePercentage);
		}
		if (d < 0) {
			return;
		}
		@event.stack.addMultiple(AllMultiple.defense, -d / (d + intermediateValue));
	}
}

public class AttributeSuck : Attribute {
	public AttributeLimit attributeLimit;

	[Event(priority = -100)]
	public void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
		if (!@event.stack.hasAttackStack(AllAttackType.main)) {
			return;
		}
		EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
		if (entityLiving is null) {
			return;
		}
		double d = entityLiving.get(this);
		if (d <= 0) {
			return;
		}
		entityLiving.addNextTimeRun(0, () => entityLiving.add(attributeLimit, @event.stack.getAttack() * d));
	}
}

public class AttributeStack {
	public Dictionary<Attribute, double> thisValue = new Dictionary<Attribute, double>();
	public Dictionary<AttributeLimit, double> limitValue = new Dictionary<AttributeLimit, double>();
	[JsonIgnore] public Dictionary<Attribute, double> value = new Dictionary<Attribute, double>();

	/// <summary>
	/// 返回真实值
	/// </summary>
	public double get(Attribute attribute) => value.get(attribute);

	/// <summary>
	/// 返回未被修饰的值（实体自身的值）
	/// </summary>
	public double getThis(Attribute attribute) => thisValue.get(attribute);

	/// <summary>
	/// 设置值（设置本身）
	/// </summary>
	public double set(Attribute attribute, double d) => thisValue.put(attribute, attribute.limit(d));

	/// <summary>
	/// 在原来值的基础上做加法
	/// </summary>
	public double add(Attribute attribute, double d) => thisValue.put(attribute, attribute.limit(thisValue.get(attribute) + d));

	/// <summary>
	/// 返回真实值
	/// </summary>
	public double get(AttributeLimit attribute) => limitValue.get(attribute);

	/// <summary>
	/// 设置本身值
	/// </summary>
	public double set(AttributeLimit attribute, double d) => limitValue.put(attribute, math.clamp(d, 0, get(attribute.getAttributeControl())));

	/// <summary>
	/// 在本身值上做更改
	/// </summary>
	public double add(AttributeLimit attribute, double d) => limitValue.put(attribute, math.clamp(get(attribute) + d, 0, get(attribute.getAttributeControl())));
}
