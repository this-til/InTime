using System;

namespace InTime;

[AttributeUsage(AttributeTargets.Field)]
public class ConfigField : System.Attribute {
}

[AttributeUsage(AttributeTargets.Method)]
public class TimerCellCycleAttribute : System.Attribute {
    public TriggerType triggerType = TriggerType.fixedUpdate;
    public TimeType timeType = TimeType.part;

    /// <summary>
    /// 每个周期需要的时间
    /// </summary>
    public float timer;

    /// <summary>
    /// 优先级
    /// </summary>
    public int priority;
}