using System;
using System.Collections.Generic;
using RegisterSystem;

namespace InTime;

public class AllMultiple : RegisterManage<Multiple> {
    /// <summary>
    /// 爆伤
    /// </summary>
    public static Multiple criticalHit;

    /// <summary>
    /// 用于buff乘区
    /// </summary>
    public static Multiple effect;

    /// <summary>
    /// 用于特定技能乘区
    /// </summary>
    public static Multiple skill;

    /// <summary>
    /// 星命的独立负乘区
    /// </summary>
    public static Multiple skillStarFate;

    /// <summary>
    /// 属性乘区
    /// </summary>
    public static Multiple attribute;

    /// <summary>
    /// 特殊 一般用于机制
    /// </summary>
    public static Multiple fix;

    /// <summary>
    /// 对于防御力起减伤作用
    /// </summary>
    public static Multiple defense;

    protected List<Multiple> index = new List<Multiple>();

    public override void put(RegisterBasics register, bool fromSon) {
        base.put(register, fromSon);
        Multiple t = (Multiple)register;
        t.setIndex(index.Count);
        index.Add(t);
    }

    public Multiple getByIndex(int id) => index[id];
}

public class Multiple : RegisterBasics {
    protected double max = Double.MaxValue;
    protected double min = 0;

    protected int index;

    public double getMax() => max;

    public double getMin() => min;

    public int getIndex() => index;

    public void setIndex(int _index) => index = _index;

    public double limit(double i) => Math.Clamp(i, getMin(), getMax());
}