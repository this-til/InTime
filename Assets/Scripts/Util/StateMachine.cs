using System;
using System.Collections.Generic;

namespace InTime;

/// <summary>
/// 有限状态机
/// </summary>
public class FiniteStateMachine<G> {
    /// <summary>
    /// 结束状态回调
    /// </summary>
    protected Action end;

    /// <summary>
    /// 开始状态的回调
    /// </summary>
    protected Action start;

    /// <summary>
    /// 能够切换到下一个状态
    /// </summary>
    protected Func<G, bool> canSwitch;

    /// <summary>
    /// 当前的状态
    /// </summary>
    protected G state;

    /// <summary>
    /// 状态锁，在改锁打开时将不会调用结束回调
    /// </summary>
    protected bool endLock;

    /// <summary>
    /// 状态锁，在改锁打开时将不会调用开始回调
    /// </summary>
    protected bool startLock;

    public void init(Action _end = null, Action _start = null, Func<G, bool> _canSwitch = null) {
        this.end = _end;
        this.start = _start;
        this.canSwitch = _canSwitch;
    }

    public void startState(G g) {
        if (state.Equals(g)) {
            return;
        }
        if (canSwitch is not null && !canSwitch(g)) {
            return;
        }
        if (!endLock) {
            endLock = true;
            runEnd();
            endLock = false;
        }
        state = g;
        if (!startLock) {
            startLock = true;
            runStart();
            startLock = false;
        }
    }

    public bool hasState(G g) => state.Equals(g);

    /// <summary>
    /// 强制设置某种状态
    /// 调用原状态结束回调
    /// 启用新状态的回调
    /// </summary>
    /// <param name="g"></param>
    public void setForceState(G g) {
        state = g;
    }

    public void runEnd() {
        if (end is null) {
            return;
        }
        end();
    }

    public void runStart() {
        if (start is null) {
            return;
        }
        start();
    }

    public G get() => state;
}

/// <summary>
/// 无限状态机
/// </summary>
public class InfiniteStateMachine<G> : List<G> {
    /// <summary>
    /// 结束状态回调
    /// </summary>
    protected Action<G> end;

    /// <summary>
    /// 开始状态的回调
    /// </summary>
    protected Action<G> start;

    public void init(Action<G> _end, Action<G> _start) {
        this.end = _end;
        this.start = _start;
    }

    public void startState(G g) {
        if (Contains(g)) {
            return;
        }
        Add(g);
        start(g);
    }

    public void endState(G g) {
        if (!Contains(g)) {
            return;
        }
        Remove(g);
        end(g);
    }

    public bool hasState(G g) => Contains(g);

    public virtual void setState(G g, bool b) {
        if (g == null) {
            return;
        }
        if (b) {
            startState(g);
        }
        else {
            endState(g);
        }
    }
}