using System;
using System.Collections.Generic;
using EventBus;
using RegisterSystem;

namespace InTime;

public class AllEntityState : RegisterManage<EntityState> {
    /// <summary>
    /// 在空中
    /// </summary>
    public static EntityState inAir;

    /// <summary>
    /// 在移动
    /// </summary>
    public static EntityState onMove;

    public static EntityState onMoveAutonomous;

    /// <summary>
    /// 在旋转
    /// </summary>
    public static EntityState onRotation;

    public static EntityState onRotationAutonomous;

    /// <summary>
    /// 失衡
    /// </summary>
    public static EntityState lossFocus;

    /// <summary>
    /// 有护盾
    /// </summary>
    public static EffectEntityTypeState hasShield;

    /// <summary>
    /// 处于时间缩放状态
    /// </summary>
    public static EffectEntityState hasTimeScale;

    /// <summary>
    /// 被禁锢
    /// </summary>
    public static EffectEntityTypeState beImprison;

    /// <summary>
    /// 被冰冻
    /// </summary>
    public static EffectEntityTypeState beFrozen;

    /// <summary>
    /// 被虚化
    /// </summary>
    public static EffectEntityTypeState beVoid;

    /// <summary>
    /// 被控制
    /// </summary>
    public static BeControlEntityState beControl;

    public override void init() {
        base.init();
        // TODO
    }
}

public class EntityState : RegisterBasics {
    protected virtual void onEvent_livingStartState(EntityLiving entityLiving, Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState @event) {
    }

    protected virtual void onEvent_livingEndState(EntityLiving entityLiving, Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState @event) {
        @event.entityState.onEvent_livingStartState(@event.entityLiving, @event);
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
        @event.entityState.onEvent_livingEndState(@event.entityLiving, @event);
    }
}

public class EffectEntityState : EntityState {
    protected internal EntityEffectBasics entityEffectBasics;

    /// <summary>
    /// 添加时间缩放状态
    /// </summary>
    [Event(priority = -300)]
    protected virtual void onEvent_addState(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
        if (@event.entityLiving.has(this)) {
            return;
        }
        if (!@event.effect.Equals(entityEffectBasics)) {
            return;
        }
        @event.entityLiving.set(this, true);
    }

    /// <summary>
    /// 清除时间缩放状态
    /// </summary>
    [Event(priority = -300)]
    protected virtual void onEvent_clearState(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect.EventLivingNatureClearEffect @event) {
        if (!@event.entityLiving.has(this)) {
            return;
        }
        if (!@event.effect.Equals(entityEffectBasics)) {
            return;
        }
        @event.entityLiving.set(this, false);
    }

    /// <summary>
    /// 结束状态时清除buff
    /// </summary>
    protected virtual void onEvent_endState(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
        if (!Equals(@event.entityState)) {
            return;
        }
        @event.entityLiving.clear(entityEffectBasics);
    }
}

public class EffectEntityTypeState : EntityState {
    protected internal Type effectType;

    /// <summary>
    /// 添加状态
    /// </summary>
    [Event(priority = -300)]
    protected virtual void onEvent_addState(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
        if (@event.entityLiving.has(this)) {
            return;
        }
        if (!effectType.IsInstanceOfType(@event.effect)) {
            return;
        }
        @event.entityLiving.set(this, true);
    }

    /// <summary>
    /// 清除状态
    /// </summary>
    [Event(priority = -300)]
    protected virtual void onEvent_clearState(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect.EventLivingNatureClearEffect @event) {
        if (!@event.entityLiving.has(this)) {
            return;
        }
        if (!effectType.IsInstanceOfType(@event.effect)) {
            return;
        }
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (!effectType.IsInstanceOfType(@event.effect)) {
                return;
            }
        }
        @event.entityLiving.set(this, false);
    }

    protected virtual void onEvent_clearStateOfEffect(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
        if (!Equals(@event.entityState)) {
            return;
        }
        foreach (var keyValuePair in @event.entityLiving.forEffect()) {
            if (!effectType.IsInstanceOfType(keyValuePair.Key)) {
                return;
            }
            @event.entityLiving.clear(keyValuePair.Key);
        }
    }
}

public class BeControlEntityState : EntityState {
    protected internal HashSet<EntityState> controlEntityStates;

    public override void awakeInit() {
        base.awakeInit();
        controlEntityStates = new HashSet<EntityState>() { AllEntityState.beFrozen, AllEntityState.beImprison, AllEntityState.beVoid };
    }

    [Event(priority = 300)]
    protected void onEvent_(Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState @event) {
        if (!controlEntityStates.Contains(@event.entityState)) {
            return;
        }
        @event.entityLiving.set(this, true);
    }

    [Event(priority = 300)]
    protected void onEvent_(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
        if (!controlEntityStates.Contains(@event.entityState)) {
            return;
        }
        foreach (var controlEntityState in controlEntityStates) {
            if (@event.entityLiving.has(controlEntityState)) {
                return;
            }
        }
        @event.entityLiving.set(this, false);
    }

    protected void onEvent__(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
        if (!@event.entityState.Equals(this)) {
            return;
        }
        foreach (var controlEntityState in controlEntityStates) {
            @event.entityLiving.set(controlEntityState, false);
        }
    }
}

public class SkillEntityState : EntityState {
    protected internal SkillBasics skillBasics;
    protected EffectType effectType;

    [Event(priority = -300)]
    protected void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet @event) {
        if (!@event.skill.Equals(skillBasics)) {
            return;
        }
        switch (effectType) {
            case EffectType.hasCd when @event.skillCell.cd <= 0:
            case EffectType.noCD when @event.skillCell.cd > 0:
                return;
        }
        @event.entityLiving.set(this, true);
    }

    [Event(priority = -300)]
    protected void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose @event) {
        if (!@event.skill.Equals(skillBasics)) {
            return;
        }
        @event.entityLiving.set(this, false);
    }

    [Event(priority = -300)]
    protected void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillSetNewCd @event) {
        if (!@event.skill.Equals(skillBasics)) {
            return;
        }
        switch (effectType) {
            case EffectType.hasCd:
                @event.entityLiving.set(this, true);
                break;

            case EffectType.noCD:
                @event.entityLiving.set(this, false);
                break;
        }
    }

    [Event(priority = -300)]
    protected void onEvent(Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillToNoCd @event) {
        if (!@event.skill.Equals(skillBasics)) {
            return;
        }
        switch (effectType) {
            case EffectType.noCD:
                @event.entityLiving.set(this, true);
                break;
            case EffectType.hasCd:
                @event.entityLiving.set(this, false);
                break;
        }
    }
}