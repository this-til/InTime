using System;
using System.Collections.Generic;
using RegisterSystem;

namespace InTime;

public abstract class CanConfigRegisterManage<T> : RegisterManage<T>, IRegisterManageDefaultConfig where T : RegisterBasics, IDefaultConfig {
    protected Dictionary<T, Action<T>> defaultConfigMap = new Dictionary<T, Action<T>>();

    public void defaultConfig(RegisterBasics registerBasics) {
        T t = (registerBasics as T)!;
        if (defaultConfigMap.ContainsKey(t)) {
            defaultConfigMap[t](t);
        }
    }

    protected void addDefaultConfig(T registerBasics, Action<T> action) {
        defaultConfigMap.Add(registerBasics, action);
    }

    protected void addDefaultConfig(Action<T> action, params T[] registerBasics) {
        if (registerBasics is null) {
            return;
        }
        foreach (var registerBasic in registerBasics) {
            defaultConfigMap.Add(registerBasic, action);
        }
    }
}