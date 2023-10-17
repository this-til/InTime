using System;
using System.Collections.Generic;
using Godot;
using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public class AllEntityEffect : CanConfigRegisterManage<EntityEffectBasics> {
    public static EntityEffectTimeScale effectTimeScale;
}

public class EntityEffectBasics : RegisterBasics, IDefaultConfig {
    protected TimeType timeType = TimeType.part;
    protected Nature nature = Nature.neutral;

    /// <summary>
    /// 从注册对象获得当前buff的性质，用于玩些骚操作
    /// </summary>
    /// <param name="effectCell"></param>
    /// <returns></returns>
    public virtual Nature getNature(EntityEffectCell effectCell) => nature;

    /// <summary>
    /// 获取buff的流逝时间类型
    /// </summary>
    public virtual TimeType getEffectTimeType(EntityEffectCell effectCell) => timeType;

    public virtual void defaultConfig() {
    }

    public virtual EntityEffectCell fuse(EntityEffectCell old, EntityEffectCell @new) {
        double oldMagnitude = old.level * old.time;
        double newMagnitude = @new.level * @new.time;
        return oldMagnitude > newMagnitude ? old : @new;
    }

    protected virtual void onEventLivingAddEffect(EntityLiving entityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
    }

    protected virtual void onEventLivingAddEffect_effect(EntityLiving entityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCellMap) {
            kv.Value.create(@event.entityLiving, kv.Key);
        }*/
    }

    protected virtual void onEventLivingClearEffect(EntityLiving entityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
    }

    protected virtual void onEventLivingClearEffect_effect(EntityLiving entityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCellMap) {
            if (!kv.Value.isSonEntity()) {
                return;
            }
            @event.entityLiving.clearSonEntity(kv.Key);
        }*/
    }

    protected virtual void onEventCatchAttribute(EntityLiving entityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
    }

    protected virtual void onEventLivingCatchSkill(EntityLiving eventEntityLiving, EntityEffectCell effectCell,
        Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, Entity? attackerEntity, EntityEffectCell entityEffectCell,
        AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, EntityEffectCell entityEffectCell,
        AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntityLiving">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, EntityEffectCell entityEffectCell,
        AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, Entity? attackerEntity, EntityEffectCell entityEffectCell,
        AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, EntityEffectCell entityEffectCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntityLiving">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, EntityEffectCell entityEffectCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
        @event.effect.onEventLivingAddEffect_effect(@event.entityLiving, @event.effectCell, @event);
        @event.effect.onEventLivingAddEffect(@event.entityLiving, @event.effectCell, @event);
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
        @event.effect.onEventLivingClearEffect_effect(@event.entityLiving, @event.effectCell, @event);
        @event.effect.onEventLivingClearEffect(@event.entityLiving, @event.effectCell, @event);
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventCatchAttribute(@event.entityLiving, keyValuePair.Value, @event);
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventLivingCatchSkill(@event.entityLiving, keyValuePair.Value, @event);
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        Entity entity = @event.stack.getEntity<Entity>();
        EntityLiving attackerEntityLiving = entity as EntityLiving;
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (!keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
            if (attackerEntityLiving is not null) {
                keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
        if (attackerEntityLiving is not null) {
            foreach (var keyValuePair in attackerEntityLiving.forEffect()) {
                if (!keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventAttackEquipment_attack(attackerEntityLiving, @event.entityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
        Entity entity = @event.stack.getEntity<Entity>();
        EntityLiving attackerEntityLiving = entity as EntityLiving;
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (!keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
            if (attackerEntityLiving is not null) {
                keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
        if (attackerEntityLiving is not null) {
            foreach (var keyValuePair in attackerEntityLiving.forEffect()) {
                if (!keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventAttackEnd_attack(attackerEntityLiving, @event.entityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
    }
}

public class EntityEffectTimeScale : EntityEffectBasics {
    public override Nature getNature(EntityEffectCell effectCell) => Nature.neutral;
}

public class ShieldEntityEffect : EntityEffectBasics {
    public Color color = new Color(1, 1, 1, 1);

    public virtual double getShield(EntityEffectCell cell) => cell.getCustomData().gatAs<double>(StringPrefab.shield);

    public virtual void setShield(EntityEffectCell cell, double d) => cell.getCustomData().Add(StringPrefab.shield, d);

    public virtual void addMultipleShield(EntityEffectCell cell, double d) => setShield(cell, (d + 1) * getShield(cell));

    /// <summary>
    /// 能够防护
    /// </summary>
    protected virtual bool canShield(EntityLiving entityLiving, EntityEffectCell effectCell) => true;

    /// <summary>
    /// 护盾抵消时
    /// </summary>
    protected virtual void shieldOffset(EntityLiving entityLiving, EntityEffectCell effectCell, double offset) {
    }

    /// <summary>
    /// 护盾破碎时
    /// </summary>
    protected virtual void shieldFracture(EntityLiving entityLiving, EntityEffectCell effectCell) {
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipmentOfShield @event) {
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            ShieldEntityEffect shieldEntityEffect = keyValuePair.Key as ShieldEntityEffect;
            if (shieldEntityEffect is null) {
                return;
            }
            EntityEffectCell effectCell = keyValuePair.Value;
            if (effectCell.isEmpty()) {
                return;
            }
            if (!shieldEntityEffect.canShield(@event.entityLiving, effectCell)) {
                return;
            }
            if (!new Event.EventEntity.EventLiving.EventShield.EventShieldCan(@event.entityLiving, @event.stack).onEvent().isResistAttack()) {
                return;
            }
            double shieldValue = shieldEntityEffect.getShield(effectCell);
            double consume = Math.Min(@event.stack.getAttack(), shieldValue);
            shieldEntityEffect.shieldOffset(@event.entityLiving, effectCell, consume);
            new Event.EventEntity.EventLiving.EventShield.EventShieldResist(@event.entityLiving, consume, shieldEntityEffect, effectCell, @event.stack).onEvent();
            shieldEntityEffect.setShield(effectCell, shieldValue - consume);
            if (consume > @event.stack.getAttack()) {
                @event.cancellationAttack();
            }
            else {
                @event.stack.addAttack(-consume);
            }
            if (shieldEntityEffect.getShield(effectCell) < 0) {
                new Event.EventEntity.EventLiving.EventShield.EventShieldAttackFractureFracture(@event.entityLiving, shieldValue, shieldEntityEffect, effectCell,
                    @event.stack).onEvent();
                @event.entityLiving.clear(shieldEntityEffect);
            }
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventShield.EventShieldResist @event) {
        //TODO
        /*if (@event.effect.effectCellEventPack is null) {
            return;
        }
        foreach (var keyValuePair in @event.effect.effectCellEventPack.effectCellMap) {
            EffectShieldBasics shieldBasicsEffect = @event.entityLiving.getSonEntity(keyValuePair.Key) as EffectShieldBasics;
            if (shieldBasicsEffect is null) {
                continue;
            }
            shieldBasicsEffect.beBeaten(@event.attackStack, @event);
        }*/
    }

    /// <summary>
    /// 护盾破碎
    /// </summary>
    /// <param name="event"></param>
    public virtual void onEvent(Event.EventEntity.EventLiving.EventShield.EventShieldFracture @event) {
        @event.effect.shieldFracture(@event.entityLiving, @event.effectCell);
    }

    /// <summary>
    /// 护盾被清除时发布护盾破碎事件
    /// </summary>
    /// <param name="event"></param>
    public static void _onEvent(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect.EventLivingNatureClearEffect @event) {
        ShieldEntityEffect shieldEntityEffect = @event.effect as ShieldEntityEffect;
        if (shieldEntityEffect is null) {
            return;
        }
        new Event.EventEntity.EventLiving.EventShield.EventShieldFracture(@event.entityLiving, @event.effectCell.level, shieldEntityEffect, @event.effectCell).onEvent();
    }
}

public class EntityEffectCell {
    public static EntityEffectCell empty = new EntityEffectCell(0, 0);

    public double level;
    public float time;
    protected JObject? customData;

    public EntityEffectCell(double level, float time, JObject customData = null) {
        this.level = level;
        this.time = time;
        this.customData = customData;
    }

    public EntityEffectCell copy() {
        return new EntityEffectCell(level, time, (JObject)customData?.DeepClone());
    }

    public JObject getCustomData() => customData ??= new JObject();

    /// <summary>
    /// 判断效果是不是有效
    /// </summary>
    public bool isEmpty() => level <= 0 || time <= 0;
}