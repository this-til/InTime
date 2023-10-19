using System;
using Godot;
using RegisterSystem;

namespace InTime;

public abstract class ResourcesRegisterManage<T> : RegisterManage<ResourcesRegister<T>> where T : class {
}

public class ResourcesRegister<T> : RegisterBasics where T : class {
    protected string path;
    protected T resources;

    /// <summary>
    /// 这是一个预制体
    /// 资源类型继承自Node
    /// 作为节点或者节点的脚本
    /// </summary>
    protected bool isPrefab;

    protected PackedScene prefab;

    protected ResourceFileManage resourceFileManage;

    public override void initBack() {
        base.initBack();
        if (typeof(Resource).IsAssignableFrom(typeof(T))) {
            if (!resourceFileManage.hasFileExtension(typeof(T))) {
                World.getInstance().getLog().Error($"无法获取{typeof(T)}类型资源后缀");
                return;
            }
            path = $"res://Assets/Resources/{getRegisterManage().getCompleteName()}/{getName()}{resourceFileManage.getFileExtension(typeof(T))}";
            resources = GD.Load<T>(path);
            return;
        }
        if (typeof(Node).IsAssignableFrom(typeof(T))) {
            path = $"res://Assets/Resources/{getRegisterManage().getCompleteName()}/{getName()}{resourceFileManage.getFileExtension(typeof(PackedScene))}";
            prefab = GD.Load<PackedScene>(path);
            isPrefab = true;
            return;
        }
        World.getInstance().getLog().Error($"加载资源:{path}时类型出错它不属于{typeof(Resource)}或者{typeof(Node)}");
    }

    public string getPath() => path;
    public T getResources() => resources;

    public PackedScene getPrefab() => prefab;
}