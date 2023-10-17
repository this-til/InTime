using System;
using System.Reflection;
using EventBus;
using Godot;
using Godot.Collections;
using log4net;
using Newtonsoft.Json.Linq;

namespace InTime;

public partial class Entity : Node3D {
	/// <summary>
	/// 实体的生命周期状态
	/// </summary>
	protected EntityLifeState entityLifeState = EntityLifeState.@null;

	/// <summary>
	/// 该实体从生成出来所度过的时间
	/// </summary>
	protected float lifeTime;

	/// <summary>
	/// 实体的唯一id
	/// </summary>
	protected int entityId;

	/// <summary>
	/// 父实体
	/// </summary>
	protected Entity? basicsEntity;

	/// <summary>
	/// 作为子实体的名字
	/// </summary>
	protected string workSonEntityName = String.Empty;

	/// <summary>
	/// 运行时的子实体
	/// </summary>
	protected readonly Dictionary<string, Entity> runTimeSonEntity = new Dictionary<string, Entity>();

	/// <summary>
	/// 计时运行
	/// </summary>
	protected readonly TimeRun timeRun = new TimeRun();

	/// <summary>
	/// 属于实体的时间缩放
	/// </summary>
	protected float timeScale = 1;

	/// <summary>
	/// 在初始化时生成对应实例的事件发布器
	/// </summary>
	protected readonly IEventBus eventsRunPackGather = new EventBus.EventBus();

	/// <summary>
	/// 顶层节点
	/// </summary>
	protected Node? baseNode;

	/// <summary>
	/// 自定义数据
	/// </summary>
	[SaveField] protected JObject? customData = new JObject();

	protected readonly Random random = new Random();

	public sealed override void _EnterTree() {
		base._EnterTree();
		baseNode = GetParent();
		entityLifeState = EntityLifeState.enterTree;
		enterTreeNecessary();
		enterTreeInit();
		enterTreeReflex();
		enterTreeEnd();
		new Event.EventEntity.EventEntityAwake(this).onEvent();
	}

	protected void enterTreeNecessary() {
		entityId = World.getInstance().getEntityManage().nextEntityID();
		eventsRunPackGather.setLog(LogOut.getInstance());
		eventsRunPackGather.put(this);
	}

	protected virtual void enterTreeInit() {
	}

	protected virtual void enterTreeReflex() {
		World.getInstance().voluntarilyAssignment(this);
		World.getInstance().getRegisterSystem().voluntarilyAssignment(this);
		foreach (var fieldInfo in GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
			if (fieldInfo.GetCustomAttribute<ObsoleteAttribute>() is not null) {
				continue;
			}
			enterTreeInitField(fieldInfo, fieldInfo.GetValue(this));
		}
		foreach (var methodInfo in GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
			if (methodInfo.GetCustomAttribute<ObsoleteAttribute>() is not null) {
				continue;
			}
			enterTreeInitMethod(methodInfo);
		}
	}

	protected virtual void enterTreeEnd() {
	}

	/// <summary>
	/// 反射所有字段
	/// </summary>
	protected virtual void enterTreeInitField(FieldInfo fieldInfo, object? fieldObj) {
		switch (fieldObj) {
			case IFieldSupply<IEntityEventBusRegister> bus:
				foreach (var entityEventBusRegister in bus.supply(fieldInfo)) {
					eventsRunPackGather?.put(entityEventBusRegister);
				}
				break;
			case IFieldGetEntity fieldGetEntity:
				fieldGetEntity.operation(this);
				break;
			/*case IFieldSupply<TimeMeterCell> timeMeterCellFieldSupply:
				int id = 0;
				foreach (var timeMeterCell in timeMeterCellFieldSupply.supply(fieldInfo)) {
					timeMeter.addTimeMeterCell($"{fieldInfo.Name}.{id}", timeMeterCell);
					id++;
				}
				break;*/
			case IFieldSupply<TimerCell> timerCellFieldSupply:
				foreach (var timerCell in timerCellFieldSupply.supply(fieldInfo)) {
					timeRun.addTimerCell(timerCell);
				}
				break;
		}
	}

	/// <summary>
	/// 反射所有方法
	/// </summary>
	protected virtual void enterTreeInitMethod(MethodInfo methodInfo) {
		TimerCellCycleAttribute? timerCellCycleAttribute = methodInfo.GetCustomAttribute<TimerCellCycleAttribute>();
		if (timerCellCycleAttribute is not null) {
			Delegate @delegate = Delegate.CreateDelegate(typeof(Action), this, methodInfo);
			addCycleRun(
				timerCellCycleAttribute.timer,
				(Action)@delegate,
				true,
				timerCellCycleAttribute.priority,
				timerCellCycleAttribute.triggerType,
				timerCellCycleAttribute.timeType
			);
		}
	}

	public sealed override void _Ready() {
		base._Ready();
		entityLifeState = EntityLifeState.ready;
		init();
		new Event.EventEntity.EventEntityOnUse(this).onEvent();
		entityLifeState = EntityLifeState.readyEnd;
	}

	/// <summary>
	/// 该方法初始化时游戏物体还是睡眠状态
	/// </summary>
	protected virtual void init() {
	}

	/// <summary>
	///  在使用后第一个FixedUpdate调用
	/// </summary>
	protected virtual void initEnd() {
	}

	public override void _ExitTree() {
		base._ExitTree();
		entityLifeState = EntityLifeState.exitTree;
		new Event.EventEntity.EventEntityDestroy(this).onEvent();
		foreach (var entity in runTimeSonEntity.Values) {
			entity.unbound();
		}
	}

	/// <summary>
	/// 父级停用时调用，同时也会被带生命限制的实体在生命终点调用
	/// </summary>
	public virtual void unbound() {
		QueueFree();
	}

	protected virtual void setTimeScale(float _timeScale) {
		if (_timeScale < 0) {
			throw new Exception();
		}
		float old = getTimeScale();
		timeScale = _timeScale;
		new Event.EventEntity.EventEntityNewTimeScale(this, old, _timeScale).onEvent();
	}

	/// <summary>
	/// 添加一个子实体
	/// 添加前请用has(_name)检查
	/// </summary>
	public virtual void addSonEntity(string entityName, Entity entity, Node3D? node = null, bool worldPositionStays = false) {
		node ??= this;
		entity.Reparent(node, worldPositionStays);
		runTimeSonEntity.Add(entityName, entity);
		entity.setRunBasicsEntity(this, entityName);
	}

	/// <summary>
	/// 添加一个子实体
	/// 用实体id当做key
	/// </summary>
	public void addSonEntity(Entity entity, Node3D? _transform = null, bool worldPositionStays = false) =>
		addSonEntity(entity.getEntityID().ToString(), entity, _transform, worldPositionStays);

	/// <summary>
	/// 获取一个子实体
	/// </summary>
	public Entity? getSonEntity(string entityName) {
		if (!runTimeSonEntity.ContainsKey(entityName)) {
			return null;
		}
		Entity entity = runTimeSonEntity[entityName];
		if (entity is null) {
			return null;
		}
		return entity;
	}

	/// <summary>
	/// 拥有实体
	/// </summary>
	public bool hasSonEntity(string entityName) => getSonEntity(entityName) is not null;

	/// <summary>
	/// 拥有实体
	/// 用实体id当做key
	/// </summary>
	public bool hasSonEntity(Entity entity) => getSonEntity(entity.getEntityID().ToString()) is not null;

	/// <summary>
	/// 清除一个子实体
	/// </summary>
	public virtual Entity? clearSonEntity(string entityName, bool end = true) {
		Entity? entity = getSonEntity(entityName);
		if (entity is null) {
			return null;
		}
		runTimeSonEntity.Remove(entityName);
		entity.setRunBasicsEntity(null, String.Empty);
		if (end) {
			entity.unbound();
		}
		entity.Reparent(entity.baseNode);
		return entity;
	}

	/// <summary>
	/// 清除一个子实体
	/// 用实体id当做key
	/// </summary>
	public void clearSonEntity(Entity entity) => clearSonEntity(entity.getEntityID().ToString());

	public virtual float getDeltaTime(TriggerType timeType, TimeType type) {
		float time = 0;
		switch (timeType) {
			case TriggerType.update:
			case TriggerType.lateUpdate:
				time = deltaTime;
				break;
			case TriggerType.fixedUpdate:
				time = fixedDeltaTime;
				break;
		}
		switch (type) {
			case TimeType.part:
				time *= timeScale;
				break;
		}
		return time;
	}

	protected float deltaTime;
	protected float fixedDeltaTime;

	public sealed override void _PhysicsProcess(double delta) {
		base._PhysicsProcess(delta);
		deltaTime = (float)delta;
		if (entityLifeState == EntityLifeState.readyEnd) {
			initEnd();
			entityLifeState = EntityLifeState.run;
		}
		timeRun.up(this, TriggerType.fixedUpdate);
	}

	public sealed override void _Process(double delta) {
		base._Process(delta);
		fixedDeltaTime = (float)delta;
		lifeTime += getDeltaTime(TriggerType.update, TimeType.part);
		timeRun.up(this, TriggerType.update);
	}

	/// <summary>
	/// 添加在某一段时间后运行的回调
	/// </summary>
	/// <param name="i"></param>
	/// <param name="action"></param>
	public TimerCell addNextTimeRun(float i, Action action, TriggerType triggerType = TriggerType.fixedUpdate, TimeType timeType = TimeType.part) =>
		timeRun.addTimerCell(new TimerCell(action, triggerType, timeType, i, false, false, 1000));

	/// <summary>
	/// 添加一个在重复周期运行的委托
	/// </summary>
	/// <param name="i"></param>
	/// <param name="action"></param>
	public TimerCell addCycleRun(float i, Action action, bool permanent = false, int priority = 0, TriggerType triggerType = TriggerType.fixedUpdate,
		TimeType timeType = TimeType.part) =>
		timeRun.addTimerCell(new TimerCell(action, triggerType, timeType, i, true, permanent, priority));

	protected virtual void setRunBasicsEntity(Entity? entity, string sonEntityName) {
		if (entity is null) {
			basicsEntity = null;
			workSonEntityName = String.Empty;
			return;
		}
		if (basicsEntity is not null) {
			World.getInstance().getLog().Error("实体已经有父实体了");
		}
		basicsEntity = entity;
		workSonEntityName = sonEntityName;
	}

	public float getTimeScale() => timeScale;

	public float getLifeTime() => lifeTime;

	public Entity? getBasicsEntity() => basicsEntity;

	public IEventBus getEntityEventBus() => eventsRunPackGather;

	public JObject? getCustomData() => customData;

	public Random getRandom() => random;

	public int getEntityID() => entityId;

	/// <summary>
	/// 发布内部事件 
	/// </summary>
	protected static void onEvent_toInside(Event.EventEntity @event) {
		@event.entity.eventsRunPackGather.onEvent(@event);
	}
}
