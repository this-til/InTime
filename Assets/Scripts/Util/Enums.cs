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
    [Obfuscation]
    lateUpdate,
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