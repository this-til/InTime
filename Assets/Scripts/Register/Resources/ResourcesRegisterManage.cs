using Godot;
using RegisterSystem;

namespace InTime.Resources;

public abstract class ResourcesRegisterManage<T> : RegisterManage<ResourcesRegister<T>> where T : class {
}

public class ResourcesRegister<T> : RegisterBasics where T : class {
    protected string path;
    protected T resources;

    public override void awakeInit() {
        base.awakeInit();
        path = $"res://Assets/Resources/{getRegisterManage().getCompleteName()}/{getName()}";
        resources = GD.Load<T>(path);
    }

    public string getPath() => path;
    public T getResources() => resources;
}