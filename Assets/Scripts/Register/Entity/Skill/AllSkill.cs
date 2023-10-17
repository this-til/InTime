using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public class AllSkill : CanConfigRegisterManage<SkillBasics> {
}

public abstract class SkillBasics : RegisterBasics, IDefaultConfig {
    //protected Dictionary<string, SkillStateEffectCell>? effectCells;
    protected internal TimeType timeType = TimeType.part;

    public virtual void defaultConfig() {
    }

    /// <summary>
    /// 获取技能cd消耗类型
    /// </summary>
    public TimeType getTimeType() => timeType;

    protected virtual void onEventSkillGet(EntityLiving entityLiving, SkillCell skillCell, Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet @event) {
    }

    protected virtual void onEventSkillGet_effect(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        if (skillCell.level < 0) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCells) {
            if (kv.Value.effectType.Equals(EffectType.hasCd) && skillCell.cd <= 0) {
                return;
            }
            if (kv.Value.effectType.Equals(EffectType.noCD) && skillCell.cd > 0) {
                return;
            }
            kv.Value.create(entityLiving, kv.Key);
        }*/
    }

    protected virtual void onEventSkillLose(EntityLiving entityLiving, SkillCell skillCell, Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose @event) {
    }

    protected virtual void onEventSkillLose_effect(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCells) {
            if (!kv.Value.isSonEntity()) {
                return;
            }
            entityLiving.clearSonEntity(kv.Key);
        }*/
    }

    protected virtual void onEventSkillSetNewCd(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillSetNewCd @event) {
    }

    protected virtual void onEventSkillSetNewCd_effect(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillSetNewCd @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCells) {
            if (kv.Value.effectType.Equals(EffectType.noCD)) {
                if (!kv.Value.isSonEntity()) {
                    return;
                }
                @event.entityLiving.clearSonEntity(kv.Key);
            }
            if (kv.Value.effectType.Equals(EffectType.hasCd)) {
                kv.Value.create(entityLiving, kv.Key);
            }
        }*/
    }

    protected virtual void onEventSkillToNoCd(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillToNoCd @event) {
    }

    protected virtual void onEventSkillToNoCd_effect(EntityLiving entityLiving, SkillCell skillCell,
        Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillToNoCd @event) {
        //TODO
        /*if (effectCellEventPack is null) {
            return;
        }
        foreach (var kv in effectCellEventPack.effectCells) {
            if (kv.Value.effectType.Equals(EffectType.noCD)) {
                kv.Value.create(@event.entityLiving, kv.Key);
            }
            if (kv.Value.effectType.Equals(EffectType.hasCd)) {
                if (!kv.Value.isSonEntity()) {
                    return;
                }
                entityLiving.clearSonEntity(kv.Key);
            }
        }*/
    }

    protected virtual void onEventCatchAttribute(EntityLiving entityLiving, SkillCell skillCell, Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
    }

    protected virtual void onEventLivingCatchSkill(EntityLiving eventEntityLiving, SkillCell skillCell, Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, Entity? attackerEntity, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntityLiving">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEquipment_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, Entity? attackerEntity, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntity">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    /// <summary>
    /// 攻击事件拦截
    /// </summary>
    /// <param name="attackerEntityLiving">发动攻击的实体</param>
    /// <param name="entityLiving">被攻击的实体</param>
    protected virtual void onEventAttackEnd_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, SkillCell skillCell, AttackStack attackStack,
        Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
    }

    protected static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet @event) {
        @event.skill.onEventSkillGet_effect(@event.entityLiving, @event.skillCell, @event);
        @event.skill.onEventSkillGet(@event.entityLiving, @event.skillCell, @event);
    }

    protected static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose @event) {
        @event.skill.onEventSkillLose_effect(@event.entityLiving, @event.skillCell, @event);
        @event.skill.onEventSkillLose(@event.entityLiving, @event.skillCell, @event);
    }

    protected static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillSetNewCd @event) {
        @event.skill.onEventSkillSetNewCd_effect(@event.entityLiving, @event.skillCell, @event);
        @event.skill.onEventSkillSetNewCd(@event.entityLiving, @event.skillCell, @event);
    }

    protected static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillToNoCd @event) {
        @event.skill.onEventSkillToNoCd_effect(@event.entityLiving, @event.skillCell, @event);
        @event.skill.onEventSkillToNoCd(@event.entityLiving, @event.skillCell, @event);
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
        foreach (var keyValuePair in @event.entityLiving.forSkill()) {
            if (keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventCatchAttribute(@event.entityLiving, keyValuePair.Value, @event);
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
        foreach (var keyValuePair in @event.entityLiving.forSkill()) {
            if (keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventLivingCatchSkill(@event.entityLiving, keyValuePair.Value, @event);
        }
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        Entity entity = @event.stack.getEntity<Entity>();
        EntityLiving attackerEntityLiving = entity as EntityLiving;
        foreach (var keyValuePair in @event.entityLiving.forSkill()) {
            if (!keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
            if (attackerEntityLiving is not null) {
                keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
        if (attackerEntityLiving is not null) {
            foreach (var keyValuePair in attackerEntityLiving.forSkill()) {
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
        foreach (var keyValuePair in @event.entityLiving.forSkill()) {
            if (!keyValuePair.Value.isEmpty()) {
                continue;
            }
            keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
            if (attackerEntityLiving is not null) {
                keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
        if (attackerEntityLiving is not null) {
            foreach (var keyValuePair in attackerEntityLiving.forSkill()) {
                if (!keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventAttackEnd_attack(attackerEntityLiving, @event.entityLiving, keyValuePair.Value, @event.stack, @event);
            }
        }
    }
}

public class SkillCell {
    public static SkillCell empty = new SkillCell();

    /// <summary>
    /// 技能的原始等级
    /// </summary>
    public double originalLevel;

    public float cd;

    protected JObject? customData;

    /// <summary>
    /// 技能的等级
    /// </summary>
    [JsonIgnore] public double level;

    [JsonIgnore] public double oldLevel;
    [JsonIgnore] public bool isEventChange;
    [JsonIgnore] public bool isNewCd;
    [JsonIgnore] public bool isNoCd;
    [JsonIgnore] public bool isNew;

    public SkillCell() : this(0, 0) {
    }

    public SkillCell(double level, float cd, JObject customData = null) {
        this.level = level;
        this.originalLevel = level;
        this.cd = cd;
        this.customData = customData;
    }

    public void setCD(float _cd) {
        cd = _cd;
        isNewCd = true;
        if (cd <= 0) {
            cd = 0;
            isNoCd = true;
        }
    }

    /// <summary>
    /// 判空
    /// 如果为空将不存在意义
    /// 使用前请判断
    /// </summary>
    /// <returns></returns>
    public bool isEmpty() => level <= 0 || cd > 0;

    public JObject getCustomData() => customData ??= new JObject();
}