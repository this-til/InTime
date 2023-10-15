using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using EventBus;
using Godot;
using RegisterSystem;

namespace InTime;

public class AllAttackType : RegisterManage<AttackType> {
    /// <summary>
    /// 主动攻击，由实体主动发起，用来过滤伤害
    /// </summary>
    public static AttackType main;

    /// <summary>
    /// 暴击伤害
    /// </summary>
    public static AttackType criticalHit;

    /// <summary>
    /// 免疫护盾
    /// </summary>
    public static AttackType_WearShieldAttack immuneShield;

    /// <summary>
    /// 免疫防御
    /// </summary>
    public static AttackType immuneDefense;

    /// <summary>
    /// 轻击
    /// </summary>
    public static AttackType_Tap tap;

    /// <summary>
    /// 重击
    /// </summary>
    public static AttackType_Thump thump;

    /// <summary>
    /// 技能
    /// </summary>
    public static AttackType skill;
}

public class AttackType : RegisterBasics {
}

public class AttackType_Tap : AttackType, IDefaultConfig {
    protected double attackMultiple = 0.25f;

    protected void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
        if (entityLiving is null) {
            return;
        }
        if (!entityLiving.has(AllEntityType.lightArmour)) {
            return;
        }
        @event.stack.addMultiple(AllMultiple.fix, attackMultiple);
    }

    public void defaultConfig() {
        attackMultiple = 0.25f;
    }
}

public class AttackType_Thump : AttackType, IDefaultConfig {
    protected double attackMultiple = 0.25f;

    protected void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        EntityLiving entityLiving = @event.stack.getEntity<EntityLiving>();
        if (entityLiving is null) {
            return;
        }
        if (!entityLiving.has(AllEntityType.heavyArmour)) {
            return;
        }
        @event.stack.addMultiple(AllMultiple.fix, attackMultiple);
    }

    public void defaultConfig() {
        attackMultiple = 0.25f;
    }
}

public class AttackType_WearShieldAttack : AttackType {
    public void onEvent(Event.EventEntity.EventLiving.EventShield.EventShieldCan @event) {
        if (@event.attackStack.hasAttackStack(this)) {
            @event.setNotResistAttack();
        }
    }
}

public class AttackStack {
    /// <summary>
    /// 发动攻击的实体
    /// </summary>
    protected Entity? entity;

    /// <summary>
    /// 攻击类型
    /// </summary>
    protected HashSet<AttackType> attackList;

    /// <summary>
    /// 当前值
    /// </summary>
    protected ValueUtil attack;

    /// <summary>
    /// 受击方向
    /// </summary>
    protected Vector3 hitDirection;

    /// <summary>
    /// 减少韧性
    /// </summary>
    protected double reduceToughness;

    protected AttackProcess attackProcess;

    public AttackStack(double attack,  Entity? entity, double reduceToughness = 0, Vector3 hitDirection = new Vector3()) {
        this.entity = entity;
        this.attackList = new HashSet<AttackType>();
        this.attack = new ValueUtil(attack);
        this.reduceToughness = reduceToughness;
        this.hitDirection = hitDirection;
        this.attackProcess = AttackProcess.noPerformed;
    }

    public bool hasAttackStack(AttackType attackType) => attackList.Contains(attackType);

    public AttackStack addAttackType(AttackType attackType) {
        if (attackType is null) {
            return this;
        }
        switch (attackProcess) {
            case AttackProcess.verification:
            case AttackProcess.equipment:
            case AttackProcess.injured:
            case AttackProcess.end:
                World.getInstance().getLog().Error("AttackStack:在不正确的时间段更改攻击类型");
                return this;
        }

        attackList.Add(attackType);
        return this;
    }

    /// <summary>
    /// 获取当前值
    /// </summary>
    public double getAttack() => attack.getValue();

    /// <summary>
    /// 获取原始值 
    /// </summary>
    public double getOldAttack() => attack.getOldValue();

    /// <summary>
    /// 增加攻击值
    /// </summary>
    public void addAttack(double add) {
        switch (attackProcess) {
            case AttackProcess.noPerformed:
            case AttackProcess.verification:
            case AttackProcess.equipmentAttackType:
            case AttackProcess.end:
                World.getInstance().getLog().Error("AttackStack:在不正确的时间段更改值");
                return;
        }
        attack.add(add);
    }

    /// <summary>
    /// 增加攻击乘区
    /// </summary>
    public void addMultiple(Multiple multiple, double addMultiple) {
        switch (attackProcess) {
            case AttackProcess.noPerformed:
            case AttackProcess.verification:
            case AttackProcess.equipmentAttackType:
            case AttackProcess.end:
                World.getInstance().getLog().Error("AttackStack:在不正确的时间段更改值");
                return;
        }
        attack.addMultiple(multiple, addMultiple);
    }

    public void addReduceToughness(double addReduceToughness) {
        switch (attackProcess) {
            case AttackProcess.noPerformed:
            case AttackProcess.verification:
            case AttackProcess.equipmentAttackType:
            case AttackProcess.end:
                World.getInstance().getLog().Error("AttackStack:在不正确的时间段更改值");
                return;
        }
        reduceToughness += addReduceToughness;
    }

    public double getReduceToughness() => reduceToughness;
    public bool isEffective() => getAttack() > 0;

    public E? getEntity<E>() where E : class => entity as E;

    public IEnumerable<AttackType> forAttackType() => attackList;

    protected void nextProcess() {
        switch (attackProcess) {
            case AttackProcess.noPerformed:
                attackProcess = AttackProcess.verification;
                break;
            case AttackProcess.verification:
                attackProcess = AttackProcess.equipmentAttackType;
                break;
            case AttackProcess.equipmentAttackType:
                attackProcess = AttackProcess.equipment;
                break;
            case AttackProcess.equipment:
                attackProcess = AttackProcess.injured;
                attack.cover();
                break;
            case AttackProcess.injured:
                attackProcess = AttackProcess.end;
                break;
            case AttackProcess.end:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    [Event(priority = 1 << 10)]
    protected static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackVerification @event) {
        @event.stack.nextProcess();
    }

    [Event(priority = 1 << 10)]
    protected static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipmentOfAttackType @event) {
        @event.stack.nextProcess();
    }

    [Event(priority = 1 << 10)]
    protected static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        @event.stack.nextProcess();
    }

    [Event(priority = 1 << 10)]
    protected static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackInjured @event) {
        @event.stack.nextProcess();
    }

    [Event(priority = 1 << 10)]
    protected static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
        @event.stack.nextProcess();
    }
}