using System;
using System.Reflection;
using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public class AllTaskComponent : RegisterManage<TaskComponentBasics> {
}

public class TaskComponentBasics : RegisterBasics {
}

public class TaskComponentCollect : TaskComponentBasics {
    public ItemBasic itemBasic;

    public void onEvent(Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackAddItem @event) {
        if (!@event.itemStack.getItem().Equals(itemBasic)) {
            return;
        }
        IEntityHasTask entityHasTask = @event.entityLiving as IEntityHasTask;
        if (entityHasTask is null) {
            return;
        }
        TaskStack taskStack = entityHasTask.getTaskStack();
        taskStack.addCurrent(@event.entityLiving, this, 1);
    }
}

public class TaskComponentKill : TaskComponentBasics {
    public Type entityType;

    public void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackSendDeath @event) {
        if (!entityType.IsInstanceOfType(@event.entityLiving)) {
            return;
        }
        IEntityHasTask entityHasTask = @event.entityLiving as IEntityHasTask;
        if (entityHasTask is null) {
            return;
        }
        TaskStack taskStack = entityHasTask.getTaskStack();
        taskStack.addCurrent(@event.entityLiving, this, 1);
    }
}

public class TaskComponentTime : TaskComponentBasics {
    public void onEvent(Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_1s @event) {
        IEntityHasTask entityHasTask = @event.entityLiving as IEntityHasTask;
        if (entityHasTask is null) {
            return;
        }
        TaskStack taskStack = entityHasTask.getTaskStack();
        taskStack.addCurrent(@event.entityLiving, this, 1);
    }
}

public class TaskComponentCell {
    /// <summary>
    /// 主控
    /// </summary>
    protected TaskComponentBasics taskComponent;

    /// <summary>
    /// 最大进度
    /// </summary>
    protected int max;

    /// <summary>
    /// 当前进度
    /// </summary>
    protected int current;

    /// <summary>
    /// 自定义数据
    /// </summary>
    protected JObject? customData;

    public TaskComponentCell() {
    }

    /// <summary>
    /// 获取进度类型
    /// </summary>
    public TaskComponentBasics getTaskComponentBasics() => taskComponent;

    /// <summary>
    /// 是完成的
    /// </summary>
    public bool isComplete() => current >= max;

    /// <summary>
    /// 获取当前进度
    /// </summary>
    public int getCurrent() => current;

    /// <summary>
    /// 获取最大进度
    /// </summary>
    public int getMaxCurrent() => max;

    /// <summary>
    /// 进度变动
    /// </summary>
    public void addCurrent(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, int add) {
        if (add == 0) {
            return;
        }
        int old = current;
        int _add = Math.Min(add + current, max);
        current += add;
        new Event.EventEntity.EventLiving.EventTask.EventTaskComponent.EventTaskComponentAdd(entityLiving, taskStack, taskCell, this, current, _add).onEvent();
        if (isComplete()) {
            new Event.EventEntity.EventLiving.EventTask.EventTaskComponent.EventTaskComponentEnd.EventTaskComponentComplete(entityLiving, taskStack, taskCell, this).onEvent();
        }
    }

    public JObject getCustomData() => customData ??= new JObject();
}