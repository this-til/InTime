using System.Collections.Generic;
using Newtonsoft.Json;

namespace InTime;

/// <summary>
/// 任务集
/// </summary>
public class TaskStack {
    /// <summary>
    /// 正在实行的任务
    /// </summary>
    [JsonProperty] protected List<TaskCell> implementTask;

    /// <summary>
    /// 失败的任务
    /// </summary>
    [JsonProperty] protected List<TaskCell> failTask;

    /// <summary>
    /// 完成的任务
    /// </summary>
    [JsonProperty] protected List<TaskCell> completeTask;

    public IEnumerable<TaskCell> forTaskCell() {
        foreach (var taskCell in implementTask) {
            yield return taskCell;
        }
        foreach (var taskCell in failTask) {
            yield return taskCell;
        }
        foreach (var taskCell in completeTask) {
            yield return taskCell;
        }
    }

    public IEnumerable<TaskCell> forImplementTask() => implementTask;

    public IEnumerable<TaskCell> forFailTask() => failTask;

    public IEnumerable<TaskCell> forCompleteTask() => completeTask;

    public void addCurrent(EntityLiving entityLiving, TaskComponentBasics taskComponentBasics, int current) {
        foreach (var taskCell in forImplementTask()) {
            taskCell.addCurrent(entityLiving, this, taskComponentBasics, current);
        }
        updateCompleteState();
    }

    public void addCurrent(EntityLiving entityLiving, TaskFailBasics taskFailBasics, int current) {
        foreach (var taskCell in forImplementTask()) {
            taskCell.addCurrent(entityLiving, this, taskFailBasics, current);
        }
        updateCompleteState();
    }

    /// <summary>
    /// 更新完成状态
    /// </summary>
    public void updateCompleteState() {
        for (int i = 0; i < implementTask.Count; i++) {
            TaskCell taskCell = implementTask[i];
            if (taskCell.isComplete()) {
                completeTask.Add(taskCell);
                implementTask.RemoveAt(i);
                i--;
            }
            if (taskCell.isFail()) {
                failTask.Add(taskCell);
                implementTask.RemoveAt(i);
                i--;
            }
        }
    }
}

public class TaskCell {
 
    /// <summary>
    /// 任务组件
    /// </summary>
     protected List<TaskComponentCell> componentCells;

    /// <summary>
    /// 失败条件
    /// </summary>
     protected List<TaskFailCell> failCells;

    public IEnumerable<TaskComponentCell> forTaskComponentCell() => componentCells;

    public IEnumerable<TaskFailCell> forTaskFailCell() => failCells;

    /// <summary>
    /// 是放弃的
    /// </summary>
    public bool isFail() {
        foreach (var taskFailCell in forTaskFailCell()) {
            if (taskFailCell.isFail()) {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 是完成的
    /// </summary>
    public bool isComplete() {
        bool _isComplete = true;
        foreach (var taskComponentCell in forTaskComponentCell()) {
            _isComplete = _isComplete && taskComponentCell.isComplete();
        }
        return _isComplete;
    }

    public void addCurrent(EntityLiving entityLiving, TaskStack taskStack, TaskComponentBasics taskComponentBasics, int add) {
        foreach (var taskComponentCell in forTaskComponentCell()) {
            if (taskComponentCell.isComplete()) {
                continue;
            }
            if (!taskComponentCell.getTaskComponentBasics().Equals(taskComponentBasics)) {
                continue;
            }
            taskComponentCell.addCurrent(entityLiving, taskStack, this, add);
        }
    }

    public void addCurrent(EntityLiving entityLiving, TaskStack taskStack, TaskFailBasics taskFailBasics, int add) {
        foreach (var taskFailCell in forTaskFailCell()) {
            if (taskFailCell.isFail()) {
                continue;
            }
            if (!taskFailCell.getTaskFailBasics().Equals(taskFailBasics)) {
                continue;
            }
            taskFailCell.addCurrent(entityLiving, taskStack, this, add);
        }
    }
}