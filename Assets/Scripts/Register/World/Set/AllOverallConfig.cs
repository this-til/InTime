using System;
using RegisterSystem;

namespace InTime;

public class AllOverallConfig : CanConfigRegisterManage<OverallConfig> {
}

public abstract class OverallConfig : RegisterBasics, IDefaultConfig {
    /// <summary>
    /// 获取数据类型
    /// </summary>
    /// <returns></returns>
    public abstract Type getDataType();

    public abstract void defaultConfig();
}

public class OverallConfig<T> : OverallConfig {
    protected ConfigManage configManage;

    [ConfigField] protected T data;

    protected T defaultDate;

    public override Type getDataType() => typeof(T);

    public void setData(T t) {
        data = t;
        configManage.writeRegister(this);
    }

    public T getData() => data;

    public T getDefaultDate() => defaultDate;

    public override void defaultConfig() {
        data = defaultDate;
    }

    public void initSetDefaultDate(T t) {
        defaultDate = t;
        initTest();
    }
}