using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public class AllTaskFailBasics : RegisterManage<TaskFailBasics> {
    public static TaskFailTime taskFailTime;
}

public abstract class TaskFailBasics : RegisterBasics {
}

public class TaskFailTime : TaskFailBasics {
    public void onEvent(Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_1s @event) {
        IEntityHasTask entityHasTask = @event.entityLiving as IEntityHasTask;
        if (entityHasTask is null) {
            return;
        }
        TaskStack taskStack = entityHasTask.getTaskStack();
        taskStack.addCurrent(@event.entityLiving, this, 1);
    }
}

public class TaskFailCell {
    /// <summary>
    /// 失败条件
    /// </summary>
    protected TaskFailBasics taskFailBasics;

    /// <summary>
    /// 最大进度
    /// </summary>
    protected int max;

    /// <summary>
    /// 自定义数据
    /// </summary>
    protected JObject? customData;

    /// <summary>
    /// 当前进度
    /// </summary>
    protected int current;

    public bool isFail() => current >= max;

    public TaskFailBasics getTaskFailBasics() => taskFailBasics;

    /// <summary>
    /// 进度变动
    /// </summary>
    public void addCurrent(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, int add) {
        if (add == 0) {
            return;
        }
        new Event.EventEntity.EventLiving.EventTask.EventTaskFailCell.EventTaskFailAdd(entityLiving, taskStack, taskCell, this, current, add).onEvent();
        if (isFail()) {
            new Event.EventEntity.EventLiving.EventTask.EventTaskFailCell.EventTaskFailEnd(entityLiving, taskStack, taskCell, this).onEvent();
        }
    }

    public JObject getCustomData() => customData ??= new JObject();
}