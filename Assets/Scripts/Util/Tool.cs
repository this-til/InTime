using System;
using System.Collections.Generic;
using Godot;

namespace InTime;

public class ControlledField<E> {
    protected bool k;
    protected E v;

    public ControlledField() {
        this.k = false;
        v = default;
    }

    public ControlledField(bool k, E v) {
        this.k = k;
        this.v = v;
    }

    public ControlledField(ControlledField<E> controlled) {
        this.k = controlled.k;
        this.v = controlled.v;
    }

    public bool tryGet(out E e) {
        e = k ? v : default;
        return k;
    }

    public E forceGet() => v;

    public bool isAdopt() => k;

    public static implicit operator ControlledField<E>(E v) {
        return new ControlledField<E>(true, v);
    }
}

public abstract class SingletonPatternClass<T> where T : SingletonPatternClass<T>, new() {
    protected static T? instance;

    public static T getInstance() {
        if (instance is null) {
            instance = new T();
            instance.init();
        }
        return instance;
    }

    protected SingletonPatternClass() {
    }

    protected virtual void init() {
    }
}

/// <summary>
/// 用来控制值的乘区
/// </summary>
public class ValueUtil {
    /// <summary>
    /// 在被创建时传入的数值
    /// </summary>
    protected double old;

    /// <summary>
    /// 当前的数值
    /// </summary>
    protected double v;

    /// <summary>
    /// 乘区
    /// </summary>
    protected double[] m;

    public ValueUtil(double v) {
        this.old = v;
        this.v = v;
        m = Array.Empty<double>();
    }

    public double getValue() => v;
    public double getOldValue() => old;

    /// <summary>
    /// 对当前值做加法
    /// </summary>
    /// <param name="d">加上多少</param>
    public void add(double d) => v += d;

    /// <summary>
    /// 添加一个乘区
    /// </summary>
    /// <param name="multiple">乘区类型</param>
    /// <param name="d">值</param>
    public void addMultiple(Multiple multiple, double d) {
        int length = multiple.getIndex() + 1;
        if (m.Length < length) {
            Array.Resize(ref m, length);
        }
        m[multiple.getIndex()] += d;
    }

    /// <summary>
    /// 计算乘区后的值 
    /// </summary>
    /// <returns><最终值/returns>
    public double getModificationValue() {
        double d = v;
        for (var index = 0; index < m.Length; index++) {
            double d1 = m[index];
            if (d1 == 0) {
                continue;
            }
            Multiple? multiple = World.getInstance().getRegisterSystem().getRegisterManageOfManageType<AllMultiple>()?.getByIndex(index);
            if (multiple is not null) {
                d1 = multiple.limit(d1);
                d *= d1 + 1;
            }
        }
        return d;
    }

    /// <summary>
    /// 更新其中的值，乘区的值覆盖成基础值
    /// </summary>
    public void cover() {
        v = getModificationValue();
        m = Array.Empty<double>();
    }
}

[Serializable]
public class DataStruct<K> : IDataStruct<K> {
    public static DataStruct<K> empty = new DataStruct<K>(default);

    public K a;

    public DataStruct() {
    }

    public DataStruct(K a) {
        this.a = a;
    }

    public K get() => a;
    public void set(K v) => a = v;

    public static implicit operator DataStruct<K>(K v) {
        return new DataStruct<K>(v);
    }

    public static implicit operator K(DataStruct<K> v) => v.a;
}

public class DataStruct<K, V> {
    public K k;
    public V v;

    public DataStruct(K k, V v) {
        this.k = k;
        this.v = v;
    }

    public static implicit operator KeyValuePair<K, V>(DataStruct<K, V> dataStruct) => new KeyValuePair<K, V>(dataStruct.k, dataStruct.v);

    public static implicit operator DataStruct<K, V>(KeyValuePair<K, V> keyValuePair) => new DataStruct<K, V>(keyValuePair.Key, keyValuePair.Value);
}

public class DataStruct<A, B, C> {
    public A a;
    public B b;
    public C c;

    public DataStruct(A a, B b, C c) {
        this.a = a;
        this.b = b;
        this.c = c;
    }
}

public class DataStruct<A, B, C, D> {
    public A a;
    public B b;
    public C c;
    public D d;

    public DataStruct(A a, B b, C c, D d) {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
    }
}

public class DataStruct<A, B, C, D, E> {
    public A a;
    public B b;
    public C c;
    public D d;
    public E e;

    public DataStruct(A a, B b, C c, D d, E e) {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
    }
}

public class DataStruct<A, B, C, D, E, F> {
    public A a;
    public B b;
    public C c;
    public D d;
    public E e;
    public F f;

    public DataStruct(A a, B b, C c, D d, E e, F f) {
        this.a = a;
        this.b = b;
        this.c = c;
        this.d = d;
        this.e = e;
        this.f = f;
    }
}

public class LogOut : SingletonPatternClass<LogOut>, ILogOut, EventBus.ILogOut, RegisterSystem.ILogOut {
    void ILogOut.Info(object message) {
        GD.Print(message);
    }

    void EventBus.ILogOut.Info(object message) {
        GD.Print(message);
    }

    void RegisterSystem.ILogOut.Info(object message) {
        GD.Print(message);
    }

    void ILogOut.Warn(object message) {
        GD.PrintRaw(message);
    }

    void EventBus.ILogOut.Warn(object message) {
        GD.PrintRaw(message);
    }

    void RegisterSystem.ILogOut.Warn(object message) {
        GD.PrintRaw(message);
    }

    void ILogOut.Error(object message) {
        GD.PrintErr(message);
    }

    void EventBus.ILogOut.Error(object message) {
        GD.PrintErr(message);
    }

    void RegisterSystem.ILogOut.Error(object message) {
        GD.PrintErr(message);
    }
}

public static class MathUtil {
    public static float SmoothDampAngle(
        float current,
        float target,
        ref float currentVelocity,
        float smoothTime,
        float maxSpeed,
        float deltaTime) {
        target = current + DeltaAngle(current, target);
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    /// <summary>
    ///   <para>Calculates the shortest difference between two given angles given in degrees.</para>
    /// </summary>
    /// <param name="current"></param>
    /// <param name="target"></param>
    public static float DeltaAngle(float current, float target) {
        float num = Repeat(target - current, 360f);
        if (num > 180.0)
            num -= 360f;
        return num;
    }

    /// <summary>
    ///   <para>Loops the value t, so that it is never larger than length and never smaller than 0.</para>
    /// </summary>
    /// <param name="t"></param>
    /// <param name="length"></param>
    public static float Repeat(float t, float length) => Mathf.Clamp(t - Mathf.Floor(t / length) * length, 0.0f, length);

    public static float SmoothDamp(
        float current,
        float target,
        ref float currentVelocity,
        float smoothTime,
        float maxSpeed,
        float deltaTime) {
        smoothTime = Mathf.Max(0.0001f, smoothTime);
        float num1 = 2f / smoothTime;
        float num2 = num1 * deltaTime;
        float num3 = (float)(1.0 / (1.0 + (double)num2 + 0.479999989271164 * (double)num2 * (double)num2 + 0.234999999403954 * (double)num2 * (double)num2 * (double)num2));
        float num4 = current - target;
        float num5 = target;
        float max = maxSpeed * smoothTime;
        float num6 = Mathf.Clamp(num4, -max, max);
        target = current - num6;
        float num7 = (currentVelocity + num1 * num6) * deltaTime;
        currentVelocity = (currentVelocity - num1 * num7) * num3;
        float num8 = target + (num6 + num7) * num3;
        if ((double)num5 - (double)current > 0.0 == (double)num8 > (double)num5) {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }
        return num8;
    }
}