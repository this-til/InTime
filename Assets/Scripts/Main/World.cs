using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using EventBus;
using log4net;
using RegisterSystem;
using log4net.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InTime;

public class World : SingletonPatternClass<World> {
    /// <summary>
    /// 世界组件  
    /// </summary>
    protected readonly Dictionary<Type, IWorldComponent> worldComponents = new Dictionary<Type, IWorldComponent>();

    protected readonly ILog log = LogManager.GetLogger(typeof(World));

    protected Version version = Assembly.GetEntryAssembly()!.GetName().Version;

    protected GraftEventBus eventBus;
    protected GraftRegisterSystem registerSystem;
    protected EntityManage entityManage;
    protected GraftJsonSerializer jsonSerializer;

    public World() {
        //加载所有组件
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            if (type.IsAbstract) {
                continue;
            }
            if (type.IsPrimitive) {
                continue;
            }
            if (type.IsValueType) {
                continue;
            }
            if (type.IsEnum) {
                continue;
            }
            if (type.IsGenericType) {
                continue;
            }
            if (type.GetCustomAttribute<ObsoleteAttribute>() is not null) {
                continue;
            }
            if (!typeof(IWorldComponent).IsAssignableFrom(type)) {
                continue;
            }
            worldComponents.Add(type, (IWorldComponent)Activator.CreateInstance(type));
        }

        voluntarilyAssignment(this);
        foreach (var worldComponentsValue in worldComponents.Values) {
            voluntarilyAssignment(worldComponentsValue);
        }

        eventBus.put(this);
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes()) {
            eventBus.put(type);
        }
        eventBus.onEvent(new Event.EventWorld.EventWorldInit.EventWorldInitStart());
        IWorldComponent[] executionOrderList = worldComponents.Values.OrderByDescending(c => c.getExecutionOrderList()).ToArray();
        foreach (var worldComponent in executionOrderList) {
            worldComponent.init();
        }
        foreach (var worldComponent in executionOrderList) {
            worldComponent.initBack();
        }
        foreach (var worldComponent in executionOrderList) {
            worldComponent.initBackToBack();
        }
        eventBus.onEvent(new Event.EventWorld.EventWorldInit.EventWorldInitEnd());
    }

    public IWorldComponent? getWorldComponent(Type type) {
        if (worldComponents.ContainsKey(type)) {
            return worldComponents[type];
        }
        return null;
    }

    public T? getWorldComponent<T>() where T : class, IWorldComponent => getWorldComponent(typeof(T)) as T;

    public void voluntarilyAssignment(object obj) {
        BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic;
        bindingFlags |= obj is Type ? BindingFlags.Static : BindingFlags.Instance;
        foreach (var fieldInfo in obj.GetType().GetFields(bindingFlags)) {
            if (fieldInfo.GetCustomAttribute<ObsoleteAttribute>() is not null) {
                continue;
            }
            if (typeof(IWorldComponent).IsAssignableFrom(fieldInfo.FieldType)) {
                fieldInfo.SetValue(fieldInfo.IsStatic ? null : obj, getWorldComponent(fieldInfo.FieldType));
            }
        }
    }

    public Version getVersion() => version;

    public IEventBus getEventBus() => eventBus;

    public RegisterSystem.RegisterSystem getRegisterSystem() => registerSystem;

    public ILog getLog() => log;
    public EntityManage getEntityManage() => entityManage;

    public GraftJsonSerializer getJsonSerializer() => jsonSerializer;
}

public class GraftEventBus : EventBus.EventBus, IWorldComponent {
    public int getExecutionOrderList() => Int32.MaxValue;
}

public class GraftRegisterSystem : RegisterSystem.RegisterSystem, IWorldComponent {
    protected GraftEventBus graftEventBus;
    protected ConfigManage configManage;

    protected void onEvent(Event.EventWorld.EventWorldInit.EventComponentInitBasics<GraftRegisterSystem>.EventComponentInit @event) {
        initAddAllManagedAssembly(GetType().Assembly);
        initAddAllManagedAssembly(typeof(World).Assembly);
        initAddRegisterManageAwakeInitEvent(graftEventBus.put);
        initAddRegisterBasicsAwakeInitEvent(graftEventBus.put);
        initAddRegisterBasicsPutEvent(World.getInstance().voluntarilyAssignment);
        initAddRegisterBasicsPutEvent(r => {
            if (r is IDefaultConfig config) {
                configManage.configRegister(config);
            }
        });
        initRegisterSystem();
    }

    public int getExecutionOrderList() => 10000;
}

public class GraftJsonSerializer : JsonSerializer, IWorldComponent {
    protected void onEvent(Event.EventWorld.EventWorldInit.EventComponentInitBasics<GraftJsonSerializer>.EventComponentInit @event) {
        Converters.Add(new RegisterItemJsonConverter());
        Converters.Add(new FiniteStateMachineJsonConverter());
    }

    public void deserialize(object obj, JObject jObject, Func<FieldInfo, bool> use) {
        foreach (var fieldInfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            if (!use(fieldInfo)) {
                continue;
            }
            if (!jObject.ContainsKey(fieldInfo.Name)) {
                continue;
            }
            JToken jToken = jObject[fieldInfo.Name];
            var v = jToken is null ? Activator.CreateInstance(fieldInfo.FieldType) : Deserialize(new JTokenReader(jToken), fieldInfo.FieldType);
            fieldInfo.SetValue(obj, v);
        }
    }

    /// <summary>
    /// 序列化
    /// </summary>
    public string serialize(object obj, Func<FieldInfo, bool> use) {
        StringWriter stringWriter = new StringWriter(new StringBuilder(256), CultureInfo.InvariantCulture);
        JsonTextWriter jsonTextWriter = new JsonTextWriter(stringWriter);
        jsonTextWriter.Formatting = Formatting;
        jsonTextWriter.WriteStartObject();
        foreach (var fieldInfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
            if (!use(fieldInfo)) {
                continue;
            }
            object o = fieldInfo.GetValue(obj);
            if (o == null) {
                continue;
            }
            jsonTextWriter.WritePropertyName(fieldInfo.Name);
            Serialize(jsonTextWriter, o, o.GetType());
        }
        jsonTextWriter.WriteEndObject();
        return stringWriter.ToString();
    }

    public class RegisterItemJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) => typeof(RegisterBasics).IsAssignableFrom(objectType);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            writer.WriteValue(((RegisterBasics)value).getName());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return World.getInstance().getRegisterSystem().getRegisterManageOfRegisterType(objectType)?.get_erase(reader.Value as string ?? "");
        }
    }

    public class FiniteStateMachineJsonConverter : JsonConverter {
        public override bool CanConvert(Type objectType) {
            if (objectType == null) {
                return false;
            }
            if (!objectType.IsGenericType) {
                return false;
            }
            return objectType.GetGenericTypeDefinition() == typeof(FiniteStateMachine<>);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) =>
            serializer.Serialize(writer, value.GetType().GetField("state", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.GetValue(value),
                value.GetType().GetGenericArguments()[0]);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (existingValue == null) {
                existingValue = Activator.CreateInstance(objectType);
            }
            existingValue?.GetType().GetField("state", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
                .SetValue(existingValue, serializer.Deserialize(reader, objectType.GetGenericArguments()[0]));
            return existingValue;
        }
    }
}

public class ConfigManage : IWorldComponent {
    protected GraftJsonSerializer graftJsonSerializer;

    protected readonly Dictionary<IDefaultConfig, FileInfo> needWrite = new Dictionary<IDefaultConfig, FileInfo>();

    protected readonly DirectoryInfo folder = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/config");

    public FileInfo mackFile(IDefaultConfig registerBasics) {
        FileInfo fileInfo =
            new FileInfo(Path.Combine(
                $"{folder.FullName}/{World.getInstance().getVersion().ToString()}/{registerBasics.getAsRegisterBasics().getRegisterManage().getCompleteName()}",
                $"{registerBasics.getAsRegisterBasics().getName()}.json"));
        return fileInfo;
    }

    public void configRegister(IDefaultConfig registerBasics) {
        FileInfo file = mackFile(registerBasics);
        if (!file.Exists) {
            needWrite.Add(registerBasics, file);
            registerBasics.defaultConfig();
            if (registerBasics.getAsRegisterBasics().getRegisterManage() is IRegisterManageDefaultConfig registerManageDefaultConfig) {
                registerManageDefaultConfig.defaultConfig(registerBasics.getAsRegisterBasics());
            }
            return;
        }

        String s = readFile(file);

        if (string.IsNullOrEmpty(s)) {
            return;
        }

        graftJsonSerializer.deserialize(
            registerBasics,
            graftJsonSerializer.Deserialize<JObject>(new JsonTextReader(new StreamReader(s))),
            info => info.GetCustomAttribute<ConfigField>() is not null);
    }

    protected void onEvent(Event.EventWorld.EventWorldInit.EventComponentInitBasics<ConfigManage>.EventComponentInitBack @event) {
        foreach (var keyValuePair in needWrite) {
            string s = graftJsonSerializer.serialize(keyValuePair.Key, info => info.GetCustomAttribute<ConfigField>() is not null);
            writeFile(keyValuePair.Value, s);
        }
    }

    public static void writeFile(FileInfo fileInfo, string text) {
        StreamWriter streamWriter = null;
        try {
            streamWriter = fileInfo.CreateText();
            streamWriter.Write(text);
            streamWriter.Flush();
        }
        catch (Exception e) {
            World.getInstance().getLog().Error(e);
        }
        finally {
            streamWriter?.Close();
        }
    }

    public static string readFile(FileInfo fileInfo) {
        StreamReader streamReader = null;
        try {
            streamReader = fileInfo.OpenText();
            return streamReader.ReadToEnd();
        }
        catch (Exception e) {
            World.getInstance().getLog().Error(e);
            return String.Empty;
        }
        finally {
            streamReader?.Close();
        }
    }
}

public abstract class EntityManageBasics<E> : IWorldComponent where E : Entity {
    protected readonly HashSet<E> entity = new HashSet<E>();

    public IEnumerable<E> forEntity() => entity;

    [Event(priority = -100)]
    protected void onEvent(Event.EventEntity.EventEntityOnUse @event) {
        if (@event.entity is not E eventEntity) {
            return;
        }
        entity.Add(eventEntity);
    }

    protected void pnEvent(Event.EventEntity.EventEntityDestroy @event) {
        if (@event.entity is not E eventEntity) {
            return;
        }
        entity.Remove(eventEntity);
    }
}

public class EntityManage : EntityManageBasics<Entity> {
    protected int useEntityID;

    /// <summary>
    /// 获取下一个自然id
    /// </summary>
    /// <returns></returns>
    public int nextEntityID() {
        useEntityID++;
        return useEntityID;
    }
    
    /// <summary>
    /// 通过id获取实体
    /// </summary>
    public Entity? getEntity(int id) {
        Entity _entity = null;
        foreach (var entity1 in entity) {
            if (entity1.getEntityID().Equals(id)) {
                _entity = entity1;
            }
        }
        return _entity;
    }
    
}