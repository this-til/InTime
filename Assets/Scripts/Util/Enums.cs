using System;
using System.Reflection;

namespace InTime;

public enum AttributeOperationType {
    add,
    addMultiple,
    reduce,
    reduceMultiple
}

public enum AttributeChangeType {
    set,
    add,
    reduce
}

/// <summary>
/// 触发类型，表明运行的时刻
/// </summary>
public enum TriggerType {
    update,
    [Obfuscation] lateUpdate,
    fixedUpdate,
}

/// <summary>
/// 时间流逝的类型
/// </summary>
public enum TimeType {
    /// <summary>
    /// 时间以实体为标准流逝
    /// </summary>
    part,

    /// <summary>
    /// 时间以世界时间流逝
    /// </summary>
    world
}

/// <summary>
/// 实体生命周期的状态
/// </summary>
public enum EntityLifeState {
    @null,
    enterTree,
    ready,
    readyEnd,
    run,
    exitTree,
}

public enum Nature {
    gain,
    neutral,
    bad
}

public enum SkillOperationTypeType {
    add,
    reduce,
}

public enum EffectType {
    all,
    hasCd,
    noCD
}

public enum InputOption {
    @null,
    w,
    s,
    a,
    d,
    tap,
    tapLong,
    thump,
    thumpLong,
    dodge,
    dodgeLong,
    jump,
    jumpLong
}

public enum AnimationPlayDetermineType {
    /// <summary>
    /// 严格的
    /// 动画限制等级必须大于原有限制等级才能播放
    /// </summary>
    strict,

    /// <summary>
    /// 重播的
    /// 动画限制等级必须大于等于原有限制等级才能播放
    /// </summary>
    replay,

    /// <summary>
    /// 重播后摇
    /// 只有在后摇状态才能进行重播判定
    /// </summary>
    replayBackSway,

    /// <summary>
    /// 跳过后摇
    /// </summary>
    skipBackSway
}

[Flags]
public enum AnimationType {
    /// <summary>
    /// 基础的，动画播放完成之后不会主动寻求下一段动画
    /// </summary>
    loop = 1 << 0,

    /// <summary>
    /// 普通的
    /// </summary>
    ordinary = 1 << 1,

    /// <summary>
    /// 移动动作
    /// </summary>
    move = 1 << 2,

    /// <summary>
    /// 攻击类型
    /// </summary>
    attack = 1 << 3,

    /// <summary>
    /// 闪避
    /// </summary>
    dodge = 1 << 4,

    /// <summary>
    /// 施法
    /// </summary>
    skill = 1 << 5,

    /// <summary>
    /// 在空中
    /// </summary>
    air = 1 << 6,

    /// <summary>
    /// 死亡动作 
    /// </summary>
    death = 1 << 7
}

/// <summary>
/// 限制类型
/// </summary>
public enum AnimationLimitType {
    /// <summary>
    /// 前摇
    /// </summary>
    forwardSway = 0,

    /// <summary>
    /// 正常
    /// </summary>
    normal = 1,

    /// <summary>
    /// 后摇
    /// </summary>
    backSway = 2,

    /// <summary>
    /// 结束
    /// </summary>
    end = 3,
}

public enum AnimationLimitLevel {
    /// <summary>
    /// 普通的
    /// </summary>
    ordinary = 0,

    /// <summary>
    /// 发动攻击
    /// </summary>
    attack = 100,

    /// <summary>
    /// 闪避
    /// </summary>
    dodge = 100, //200,

    /// <summary>
    /// 使用技能
    /// </summary>
    skill = 300,

    /// <summary>
    /// 受到控制
    /// </summary>
    controlled = 1000,

    /// <summary>
    /// 爆发动画（大招）
    /// </summary>
    outbreak = 1000,

    /// <summary>
    /// 死亡
    /// </summary>
    death = Int32.MaxValue,
}

/// <summary>
/// 攻击的进程
/// </summary>
public enum AttackProcess {
    /// <summary>
    /// 未进行攻击 new阶段
    /// </summary>
    noPerformed,

    /// <summary>
    /// 验证攻击
    /// </summary>
    verification,

    /// <summary>
    /// 调制攻击类型
    /// </summary>
    equipmentAttackType,

    /// <summary>
    /// 调制参数
    /// </summary>
    equipment,

    /// <summary>
    /// 进行攻击
    /// </summary>
    injured,

    /// <summary>
    /// 结束
    /// </summary>
    end,
}