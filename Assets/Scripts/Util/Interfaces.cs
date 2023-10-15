using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using RegisterSystem;

namespace InTime;

public interface IDefaultConfig {
    void defaultConfig();
    RegisterBasics getAsRegisterBasics() => this as RegisterBasics ?? throw new Exception();
}

public interface IRegisterManageDefaultConfig {
    void defaultConfig(RegisterBasics registerBasics);
    RegisterManage getAsRegisterManage() => this as RegisterManage ?? throw new Exception();
}

public interface IEntityScreen {
    public bool adopt(Entity entity);

    public class EntityScreenTypeOf<E> : IEntityScreen where E : class {
        public static EntityScreenTypeOf<E> typeOf = new EntityScreenTypeOf<E>();

        protected EntityScreenTypeOf() {
        }

        public bool adopt(Entity entity) => entity is E;
    }

    public class EntityScreenDifferentCamp : IEntityScreen {
        public readonly Camp camp;

        public EntityScreenDifferentCamp(Camp camp) {
            this.camp = camp;
        }

        public bool adopt(Entity entity) {
            if (!(entity is EntityLiving entityHasCamp)) {
                return true;
            }
            return !entityHasCamp.getCamp().Equals(camp);
        }
    }

    public class EntityScreenExcludeEntity : IEntityScreen {
        public readonly Entity entity;
        public bool identical;

        public EntityScreenExcludeEntity(Entity entity, bool identical = false) {
            this.entity = entity;
            this.identical = identical;
        }

        public bool adopt(Entity _entity) => identical ? entity.Equals(_entity) : !entity.Equals(_entity);
    }
}

public interface IWorldComponent {
    /// <summary>
    /// 初始化
    /// </summary>
    public void init() {
        Type type = typeof(Event.EventWorld.EventWorldInit.EventComponentInitBasics<>.EventComponentInit).MakeGenericType(GetType());
        World.getInstance().getEventBus().onEvent((Event)Activator.CreateInstance(type, this)!);
    }

    /// <summary>
    /// 在第一次初始化后再一次的回调
    /// </summary>
    public void initBack() {
        Type type = typeof(Event.EventWorld.EventWorldInit.EventComponentInitBasics<>.EventComponentInitBack).MakeGenericType(GetType());
        World.getInstance().getEventBus().onEvent((Event)Activator.CreateInstance(type, this)!);
    }

    /// <summary>
    /// 第二次
    /// </summary>
    public void initBackToBack() {
        Type type = typeof(Event.EventWorld.EventWorldInit.EventComponentInitBasics<>.EventComponentInitBackToBack).MakeGenericType(GetType());
        World.getInstance().getEventBus().onEvent((Event)Activator.CreateInstance(type, this)!);
    }

    /// <summary>
    /// 获取执行顺序
    /// </summary>
    public virtual int getExecutionOrderList() => 0;
}

public interface IFieldSupply<out E> {
    IEnumerable<E> supply(FieldInfo fieldInfo);
}

public interface IFieldGetEntity {
    void operation(Entity entity);
}

/// <summary>
/// 实体注册，表明可以注册进实体内部的事件系统
/// </summary>
public interface IEntityEventBusRegister {
}

public interface IDataStruct<T> {
    T get();
    void set(T v);
}

public interface IEntityHasTask {
    TaskStack getTaskStack();
}

public interface IEntityLivingMaterialChange {
    Material[] materialChange(EntityLiving entityLiving);
}