using System;
using System.Collections.Generic;

namespace InTime;

public class TimeRun {
    /// <summary>
    /// 按周期运行，一般在初始化时注册，运行时不做更改 
    /// </summary>
    protected readonly Dictionary<TriggerType, List<TimerCell>> timerCells = new Dictionary<TriggerType, List<TimerCell>>();

    /// <summary>
    /// 待加入的
    /// </summary>
    protected readonly List<TimerCell> beAdded = new List<TimerCell>();

    /// <summary>
    /// 在运行
    /// </summary>
    protected bool isRun;

    protected void upBeAdded() {
        foreach (var timerCell in beAdded) {
            bool needInsert = true;
            List<TimerCell> timerCellList;
            if (timerCells.ContainsKey(timerCell.triggerType)) {
                timerCellList = timerCells[timerCell.triggerType];
            }
            else {
                timerCellList = new List<TimerCell>();
                timerCells.Add(timerCell.triggerType, timerCellList);
            }
            for (var index = 0; index < timerCellList.Count; index++) {
                TimerCell _timerCell = timerCellList[index];
                if (_timerCell.priority > timerCell.priority) {
                    continue;
                }
                timerCellList.Insert(index, timerCell);
                needInsert = false;
                break;
            }
            if (needInsert) {
                timerCellList.Add(timerCell);
            }
        }
        beAdded.Clear();
    }

    /// <summary>
    /// 刷新，每次被调用代表时间加一，一般在FixedUpdate中调用
    /// </summary>
    public void up(Entity entity, TriggerType triggerType) {
        isRun = true;
        if (beAdded.Count != 0) {
            upBeAdded();
        }
        if (timerCells.Count != 0) {
            return;
        }
        List<TimerCell> timerCellList;
        if (timerCells.ContainsKey(triggerType)) {
            timerCellList = timerCells[triggerType];
        }
        else {
            timerCellList = new List<TimerCell>();
            timerCells.Add(triggerType, timerCellList);
        }
        for (var index = 0; index < timerCellList.Count; index++) {
            TimerCell timerCell = timerCellList[index];
            if (timerCell.triggerType != triggerType) {
                continue;
            }
            timerCell.up(entity.getDeltaTime(triggerType, timerCell.timeType));
            if (!isRun) {
                return;
            }
            if (!timerCell.isValid()) {
                timerCellList.RemoveAt(index);
                index--;
            }
        }
    }

    public TimerCell addTimerCell(TimerCell timerCell) {
        if (!timerCell.isValid()) {
            return timerCell;
        }
        beAdded.Add(timerCell);
        return timerCell;
    }

    public virtual void recovery() {
        isRun = false;
        foreach (var keyValuePair in timerCells) {
            List<TimerCell> timerCellList = keyValuePair.Value;
            for (var index = 0; index < timerCellList.Count; index++) {
                TimerCell _timerCell = timerCellList[index];
                if (_timerCell.permanent) {
                    continue;
                }
                timerCellList.RemoveAt(index);
                index--;
            }
        }
    }
}

public class TimerCell {
    /// <summary>
    /// 计时结束的回调
    /// </summary>
    protected Action run;

    /// <summary>
    /// 触发的类型
    /// </summary>
    public readonly TriggerType triggerType;

    /// <summary>
    /// 时间类型
    /// </summary>
    public readonly TimeType timeType;

    /// <summary>
    /// 每个周期需要的时间
    /// </summary>
    public readonly float timer;

    /// <summary>
    /// 是不是周期
    /// </summary>
    public readonly bool cycle;

    /// <summary>
    /// 永久的 
    /// </summary>
    public readonly bool permanent;

    /// <summary>
    /// 当前的计时
    /// </summary>
    protected float time;

    /// <summary>
    /// 是启用的
    /// </summary>
    protected bool _use = true;

    /// <summary>
    /// 是不是有效值
    /// </summary>
    protected bool valid = true;

    /// <summary>
    /// 优先级
    /// </summary>
    public readonly int priority;

    public TimerCell(Action run, TriggerType triggerType, TimeType timeType, float timer, bool cycle, bool permanent, int priority) {
        this.run = run;
        this.triggerType = triggerType;
        this.timeType = timeType;
        this.timer = timer;
        this.cycle = cycle;
        this.priority = priority;
        this.permanent = permanent;
    }

    public void up(float _time) {
        if (!_use) {
            return;
        }
        time += _time;
        if (time >= timer) {
            time = 0;
            run();
            if (!cycle) {
                valid = false;
            }
        }
    }

    public void use(bool nowStart = false) {
        _use = true;
        time = 0;
        if (nowStart) {
            time = timer;
        }
    }

    public void end() {
        _use = false;
    }

    public void setFail() => valid = false;
    public bool isValid() => valid;
}