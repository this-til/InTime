using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using EventBus;
using Godot;
using log4net.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public partial class EntityLiving : Entity {
	[Export] protected CharacterBody3D characterBody3D;
	[Export] protected CollisionObject3D collisionObject3D;
	[Export] protected MeshInstance3D meshInstance;
	[Export] protected AnimationPlayer animationPlayer;

	/// <summary>
	/// 实体的默认材质
	/// </summary>
	protected Material[] defaultMaterial;

	/// <summary>
	/// 材质更改列表
	/// </summary>
	protected readonly List<DataStruct<string, IEntityLivingMaterialChange>> materialChangeList = new List<DataStruct<string, IEntityLivingMaterialChange>>();

	/// <summary>
	/// 使用其他材质
	/// </summary>
	protected bool useOtherMaterial;

	/// <summary>
	/// 实体所携带的所有动画
	/// </summary>
	protected readonly Dictionary<string, AnimationPlayableBehaviour> animationPlayableBehaviourName = new Dictionary<string, AnimationPlayableBehaviour>();

	/// <summary>
	/// 当前动画的
	/// </summary>
	protected internal AnimationState animationState;

	/// <summary>
	/// 进行播放动画
	/// </summary>
	protected bool isPlayer = true;

	/// <summary>
	/// 东湖的循环计数
	/// </summary>
	protected int loopCount;

	/// <summary>
	/// 记录的触发器id
	/// </summary>
	protected int triggerId;

	/// <summary>
	/// 动画播放的速度
	/// 受时间缩放和基础速度影响
	/// </summary>
	protected float seep = 1;

	/// <summary>
	/// 正在播放的主动画
	/// </summary>
	protected AnimationPlayableBehaviour mainAnimationPlayableBehaviour;

	/// <summary>
	/// 准备过渡成的动画
	/// </summary>
	protected AnimationPlayableBehaviour? excessiveAnimationPlayableBehaviour;

	/// <summary>
	/// 选定要播放的动画，在过渡时做缓存
	/// </summary>
	protected AnimationPlayableBehaviour? selectedAnimationPlayableBehaviour;

	/// <summary>
	/// 站立动画借点
	/// </summary>
	[AnimationPlayableBehaviourData(AnimationType.loop, (int)AnimationLimitLevel.ordinary)]
	protected AnimationPlayableBehaviour standAnimationPlayableBehaviour;

	/// <summary>
	/// 移动动画 
	/// </summary>
	[AnimationPlayableBehaviourData(AnimationType.loop | AnimationType.move, (int)AnimationLimitLevel.ordinary)]
	protected AnimationPlayableBehaviour moveAnimationPlayableBehaviour;

	/// <summary>
	/// 滞空动画
	/// </summary>
	[AnimationPlayableBehaviourData(AnimationType.loop | AnimationType.air, (int)AnimationLimitLevel.ordinary)]
	protected AnimationPlayableBehaviour airAnimationPlayableBehaviour;

	/// <summary>
	/// 打断动画
	/// </summary>
	[AnimationPlayableBehaviourData(AnimationType.ordinary, (int)AnimationLimitLevel.controlled)]
	protected AnimationPlayableBehaviour interruptAnimationPlayableBehaviour;

	/// <summary>
	/// 击飞动画
	/// </summary>
	[Obsolete] [AnimationPlayableBehaviourData(AnimationType.loop | AnimationType.ordinary, (int)AnimationLimitLevel.controlled)]
	protected AnimationPlayableBehaviour blowUpAnimationPlayableBehaviour;

	/// <summary>
	/// 死亡动画节点
	/// </summary>
	[AnimationPlayableBehaviourData(AnimationType.death, (int)AnimationLimitLevel.death)]
	protected AnimationPlayableBehaviour deathAnimationPlayableBehaviour;

	[SaveField] protected AttributeStack attribute = new AttributeStack();

	/// <summary>
	/// 当前所有效果
	/// </summary>
	[SaveField] protected Dictionary<EntityEffectBasics, EntityEffectCell> effectStack = new Dictionary<EntityEffectBasics, EntityEffectCell>();

	/// <summary>
	/// 技能
	/// </summary> 
	//public SkillStack skill;
	[SaveField] protected Dictionary<SkillBasics, SkillCell> skill = new Dictionary<SkillBasics, SkillCell>();

	/// <summary>
	/// 实体状态
	/// </summary>
	protected readonly InfiniteStateMachine<EntityState> entityStateInfiniteStateMachine = new InfiniteStateMachine<EntityState>();

	/// <summary>
	/// 实体类型
	/// </summary>
	[SaveField] protected InfiniteStateMachine<EntityType> entityTypeInfiniteStateMachine = new InfiniteStateMachine<EntityType>();

	/// <summary>
	/// 实体阵营
	/// </summary>
	[SaveField] protected FiniteStateMachine<Camp> campFiniteStateMachine = new FiniteStateMachine<Camp>();

	/// <summary>
	/// 实体定位点
	/// </summary>
	protected Dictionary<EntityPos, Node3D> entityPos = new Dictionary<EntityPos, Node3D>();

	/// <summary>
	/// 装备
	/// </summary>
	[SaveField] protected Dictionary<EquipmentType, ItemStack?> equipment = new Dictionary<EquipmentType, ItemStack?>();

	/// <summary>
	/// 背包
	/// </summary>
	[SaveField] protected List<ItemStack> itemPack = new List<ItemStack>();

	protected GroundedDetection? groundedDetection = new GroundedDetection();

	protected BasicRigidBodyPush? basicRigidBodyPush = new BasicRigidBodyPush();

	/// <summary>
	/// 移动限制
	/// </summary>
	protected MoveLimit moveLimit = new MoveLimit();

	/// <summary>
	/// 实体的移动
	/// </summary>
	protected readonly Move move = new Move();

	/// <summary>
	/// 实体操作输入（AI）
	/// </summary>
	protected readonly EntityInput entityInput = new EntityInput();

	/// <summary>
	/// 实体默认数
	/// 在init之前设置用于从存档中读取的数据复写
	/// </summary>
	protected JObject? defaultData;

	/// <summary>
	/// 需不需要刷新属性
	/// </summary>
	protected bool needCatch;

	/// <summary>
	/// 需要更改材质
	/// </summary>
	protected bool needChangeMaterial;

	/// <summary>
	/// 更新被控制状态
	/// </summary>
	protected bool needUpdateBeControl;

	protected override void enterTreeInitField(FieldInfo fieldInfo, object? fieldObj) {
		base.enterTreeInitField(fieldInfo, fieldObj);
		switch (fieldObj) {
			case AnimationPlayableBehaviour animationPlayableBehaviour:
				animationPlayableBehaviour.name = fieldInfo.Name;
				if (animationPlayableBehaviourName.ContainsKey(animationPlayableBehaviour.name)) {
					throw new Exception();
				}
				animationPlayableBehaviourName.Add(animationPlayableBehaviour.name, animationPlayableBehaviour);
				break;
		}
	}

	protected override void enterTreeInit() {
		base.enterTreeInit();

		{
			/*character ??= gameObject.get<CharacterController>();
			animator ??= gameObject.get<Animator>();
			skinnedMeshRenderer ??= gameObject.get<SkinnedMeshRenderer>();*/

			animationPlayer.PlaybackProcessMode = AnimationPlayer.AnimationProcessCallback.Physics;
		}

		defaultMaterial = new Material[meshInstance.GetBlendShapeCount()];
		for (var i = 0; i < defaultMaterial.Length; i++) {
			defaultMaterial[i] = meshInstance.GetSurfaceOverrideMaterial(i);
		}
	}

	protected override void init() {
		base.init();
		if (defaultData != null) {
			World.getInstance().getJsonSerializer().deserialize(this, defaultData, info => info.GetCustomAttribute<SaveField>() is not null);
		}
		else {
			generateDefaultData();
		}
		//initState
		{
			entityStateInfiniteStateMachine.init(endState, startState);
		}
		//initAnimation
		{
			set(standAnimationPlayableBehaviour);
		}
		//initCamp
		{
			campFiniteStateMachine.init(endCamp, startCamp);
		}
		//initEntityType
		{
			entityTypeInfiniteStateMachine.init(endEntityType, startEntityType);
		}

		new Event.EventEntity.EventLiving.EventLivingInit.EventLivingInitProcess(this).onEvent();
	}

	protected override void initEnd() {
		base.initEnd();
		//initEntityType
		{
			var list = new List<Event.EventEntity.EventLiving.EventLivingType.EventLivingStartType.EventLivingInitType>();
			foreach (var entityType in entityTypeInfiniteStateMachine) {
				list.Add(new Event.EventEntity.EventLiving.EventLivingType.EventLivingStartType.EventLivingInitType(this, entityType));
			}
			addNextTimeRun(0, () => list.ForEach(e => e.onEvent()));
		}
		//initEntityCamp
		{
			addNextTimeRun(0, () => new Event.EventEntity.EventLiving.EventLivingCamp.EventLivingStartCamp.EventLivingInitCamp(this, getCamp()).onEvent());
		}
		//initEffect
		{
			var list = new List<Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect.EventLivingInitAddEffect>();
			foreach (var entityEffectStack in effectStack) {
				if (entityEffectStack.Value.isEmpty()) {
					continue;
				}
				list.Add(new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect.EventLivingInitAddEffect(this, entityEffectStack.Key, entityEffectStack.Value));
			}
			addNextTimeRun(0, () => {
				foreach (var eventLivingInitAddEffect in list) {
					eventLivingInitAddEffect.onEvent();
				}
			});
		}
		//initSkill
		{
			var skillEvent = new List<Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet.EventSkillInit>();
			foreach (var keyValuePair in skill) {
				if (keyValuePair.Value.originalLevel > 0) {
					keyValuePair.Value.level = keyValuePair.Value.originalLevel;
					skillEvent.Add(new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet.EventSkillInit(this, keyValuePair.Key, keyValuePair.Value));
				}
			}
			addNextTimeRun(0, () => skillEvent.ForEach(e => e.onEvent()));
		}

		//initEquipment
		{
			new Event.EventEntity.EventLiving.EventEntityPack.EventEntityPackInit.EventEntityPackInitEquipment(this).onEvent();
			var equipmentEvent = new List<Event.EventEntity.EventLiving.EventEntityPack.EventEquipment.EventEquipmentUse.EventEquipmentInitUse>();
			foreach (var keyValuePair in equipment) {
				if (keyValuePair.Value is null) {
					continue;
				}
				equipmentEvent.Add(
					new Event.EventEntity.EventLiving.EventEntityPack.EventEquipment.EventEquipmentUse.EventEquipmentInitUse(this, keyValuePair.Value, keyValuePair.Key));
			}
			addNextTimeRun(0, () => equipmentEvent.ForEach(e => e.onEvent()));
		}
		//initPack
		{
			new Event.EventEntity.EventLiving.EventEntityPack.EventEntityPackInit.EventEntityPackInitPack(this).onEvent();
			var itemEvent = new List<Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackAddItem.EventPackInitAddItem>();
			foreach (var itemStack in itemPack) {
				itemEvent.Add(new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackAddItem.EventPackInitAddItem(this, itemStack));
			}
			addNextTimeRun(0, () => itemEvent.ForEach(e => e.onEvent()));
		}
		catchData();
		new Event.EventEntity.EventLiving.EventLivingInit.EventLivingInitEnd(this).onEvent();
	}

	protected override void setTimeScale(float _timeScale) {
		base.setTimeScale(_timeScale);
		nextCatchData();
	}

	public virtual void startState(EntityState entityState) {
		new Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState(this, entityState).onEvent();
	}

	public virtual void endState(EntityState entityState) {
		new Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState(this, entityState).onEvent();
	}

	public void nextAnimation() {
		AnimationPlayableBehaviour? newAnimationPlayableBehaviour =
			new Event.EventEntity.EventLiving.EventLivingAnimator.EventSettlementNextAnimation(this, mainAnimationPlayableBehaviour).onEvent().getNextAnimation();
		if (newAnimationPlayableBehaviour is null) {
			return;
		}
		set(newAnimationPlayableBehaviour);
	}

	/// <summary>
	/// 谁在实体状态
	/// </summary>
	public void set(EntityState entityState, bool b) => entityStateInfiniteStateMachine.setState(entityState, b);

	/// <summary>
	/// 有没有状态
	/// </summary>
	public bool has(EntityState entityState) => entityStateInfiniteStateMachine.hasState(entityState);

	/// <summary>
	/// 遍历状态
	/// </summary>
	public IEnumerable<EntityState> forState() => entityStateInfiniteStateMachine;

	/// <summary>
	/// 设置状态
	/// </summary>
	public void set(EntityType __entityType, bool b) => entityTypeInfiniteStateMachine.setState(__entityType, b);

	/// <summary>
	/// 获取实体类型
	/// </summary>
	public bool has(EntityType __entityType) => entityTypeInfiniteStateMachine.hasState(__entityType);

	protected virtual void endEntityType(EntityType entityType) {
		new Event.EventEntity.EventLiving.EventLivingType.EventLivingEndType(this, entityType).onEvent();
	}

	protected virtual void startEntityType(EntityType entityType) {
		new Event.EventEntity.EventLiving.EventLivingType.EventLivingStartType(this, entityType).onEvent();
	}

	/// <summary>
	/// 遍历所有实体状态
	/// </summary>
	public IEnumerable<EntityType> forEntityType() => entityTypeInfiniteStateMachine;

	/// <summary>
	/// 返回该技能的数据
	/// </summary>
	/// <param name="_skill"></param>
	/// <returns></returns>
	public SkillCell get(SkillBasics _skill) {
		if (!skill.ContainsKey(_skill)) {
			return SkillCell.empty;
		}
		SkillCell skillCell = skill[_skill];
		if (skillCell is null || skillCell.isEmpty()) {
			return SkillCell.empty;
		}
		return skillCell;
	}

	public void set(SkillBasics _skill, double level) {
		if (level <= 0) {
			level = 0;
		}
		SkillCell skillCell;
		if (skill.ContainsKey(_skill)) {
			skillCell = skill[_skill];
			skillCell.level = level;
			if (level == 0) {
				new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose(this, _skill, skillCell).onEvent();
			}
			return;
		}

		if (level <= 0) {
			return;
		}
		skillCell = new SkillCell(level, 0);
		new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet(this, _skill, skillCell).onEvent();
		skill.put(_skill, skillCell);
	}

	/// <summary>
	/// 判断有没有技能
	/// </summary>
	public bool has(SkillBasics _skill) {
		SkillCell skillCell = get(_skill);
		if (skillCell is null) {
			return false;
		}
		if (skillCell.isEmpty()) {
			return false;
		}
		return true;
	}

	/// <summary>
	/// 遍历所有实体技能
	/// </summary>
	public IEnumerable<KeyValuePair<SkillBasics, SkillCell>> forSkill() => skill;

	/// <summary>
	/// 获取角色当前的属性
	/// </summary>
	public double get(Attribute _attribute) => attribute.get(_attribute);

	/// <summary>
	/// 设置属性
	/// </summary>
	public void set(Attribute _attribute, double v) {
		attribute.set(_attribute, v);
		nextCatchData();
	}

	/// <summary>
	/// 设置角色原有的属性
	/// </summary>
	public void set(AttributeLimit _attribute, double v) {
		if (v < 0) {
			return;
		}
		v = new Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit(this, _attribute, AttributeChangeType.set, get(_attribute), v).onEvent().getValue();
		attribute.set(_attribute, v);
		nextCatchData();
	}

	/// <summary>
	/// 添加某属性，并返回添加多少
	/// </summary>
	/// <param name="_attribute"></param>
	/// <param name="v"></param>
	public double add(AttributeLimit _attribute, double v) {
		if (v < 0) {
			return 0;
		}
		double _v = get(_attribute);
		double add = new Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit(this, _attribute, AttributeChangeType.add, _v, v).onEvent().getValue();
		attribute.add(_attribute, add);
		nextCatchData();
		return add;
	}

	/// <summary>
	/// 减少某属性，并返回减少多少
	/// </summary>
	/// <param name="_attribute"></param>
	/// <param name="v"></param>
	public double reduce(AttributeLimit _attribute, double v) {
		if (v < 0) {
			return 0;
		}
		double _v = get(_attribute);
		double reduce = new Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit(this, _attribute, AttributeChangeType.reduce, _v, v).onEvent().getValue();
		attribute.add(_attribute, -reduce);
		nextCatchData();
		return reduce;
	}

	/// <summary>
	/// 获取角色当前的属性
	/// </summary>
	public double get(AttributeLimit _attribute) => attribute.get(_attribute);

	/// <summary>
	/// 获取角色原有的属性
	/// </summary> 
	public double getThis(Attribute _attribute) => attribute.getThis(_attribute);

	/// <summary>
	/// 遍历现实值
	/// </summary>
	/// <returns></returns>
	public IEnumerable<KeyValuePair<Attribute, double>> forAttribute() => attribute.value;

	/// <summary>
	/// 遍历现实值
	/// </summary>
	/// <returns></returns>
	public IEnumerable<KeyValuePair<AttributeLimit, double>> forAttributeLimit() => attribute.limitValue;

	/// <summary>
	/// 遍历本身值
	/// </summary>
	/// <returns></returns>
	public IEnumerable<KeyValuePair<Attribute, double>> forThisAttribute() => attribute.thisValue;

	/// <summary>
	/// 获取buff 为空表示没有
	/// </summary>
	/// <param name="entityEffect"></param>
	/// <returns></returns>
	public EntityEffectCell get(EntityEffectBasics entityEffect) {
		if (!effectStack.ContainsKey(entityEffect)) {
			return EntityEffectCell.empty;
		}
		EntityEffectCell effectCell = effectStack[entityEffect];
		if (effectCell.isEmpty()) {
			return EntityEffectCell.empty;
		}
		return effectCell;
	}

	/// <summary>
	/// 添加一个buff
	/// </summary>
	public void add(EntityEffectBasics effect, EntityEffectCell effectCell) {
		if (effectCell.isEmpty()) {
			return;
		}
		EntityEffectCell oldEffectCell = get(effect);
		if (oldEffectCell.isEmpty()) {
			if (!new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect.EventLivingNatureAddEffect(this, effect, effectCell).onEvent().isCanAdd()) {
				return;
			}
			effectStack.put(effect, effectCell);
			nextCatchData();
			return;
		}
		if (!new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect.EventLivingFuseAddEffect(this, effect, effectCell).onEvent().isCanAdd()) {
			return;
		}
		new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect.EventLivingFuseClearEffect(this, effect, oldEffectCell).onEvent();
		EntityEffectCell inEffect = effect.fuse(oldEffectCell, effectCell);
		if (oldEffectCell.Equals(inEffect)) {
			return;
		}
		new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingFuseEffect(this, effect, inEffect).onEvent();
		effectStack.put(effect, inEffect);
		nextCatchData();
	}

	/// <summary>
	/// 清除某个buff
	/// </summary>
	/// <param name="effectCell"></param>
	public void clear(EntityEffectBasics e) {
		EntityEffectCell effectCell = get(e);
		addNextTimeRun(0, () => {
			effectStack.Remove(e);
			nextCatchData();
			new Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect.EventLivingNatureClearEffect(this, e, effectCell).onEvent();
		}, TriggerType.fixedUpdate, TimeType.world);
	}

	/// <summary>
	/// 判断有没有buff
	/// </summary>
	public bool has(EntityEffectBasics e) {
		return !get(e).isEmpty();
	}

	public IEnumerable<KeyValuePair<EntityEffectBasics, EntityEffectCell>> forEffect() => effectStack;

	/// <summary>
	/// 获取实体阵营
	/// </summary>
	public Camp getCamp() => campFiniteStateMachine.get();

	/// <summary>
	/// 对比两个实体阵营是不是相同的
	/// </summary>
	/// <param name="entityLiving"></param>
	/// <returns></returns>
	public bool isCamp(EntityLiving entityLiving) => entityLiving.campFiniteStateMachine.hasState(campFiniteStateMachine.get());

	/// <summary>
	/// 设置实体阵营
	/// </summary>
	public void set(Camp c) => campFiniteStateMachine.startState(c);

	protected virtual void endCamp() {
		new Event.EventEntity.EventLiving.EventLivingCamp.EventLivingEndCamp(this, getCamp()).onEvent();
	}

	protected virtual void startCamp() {
		new Event.EventEntity.EventLiving.EventLivingCamp.EventLivingStartCamp(this, getCamp()).onEvent();
	}

	/// <summary>
	/// 添加一个子实体
	/// </summary>
	public void addSonEntity(string entityName, Entity entity, EntityPos _entityPos) => addSonEntity(entityName, entity, get(_entityPos));

	/// <summary>
	/// 添加一个子实体
	/// 用实体id当做key
	/// </summary>
	public void addSonEntity(Entity entity, EntityPos _entityPos) => addSonEntity(entity.getEntityID().ToString(), entity, _entityPos);

	/// <summary>
	/// 通过实体坐标点返回对应Transform
	/// </summary>
	/// <param name="__entityPos"></param>
	/// <returns></returns>
	public Node3D get(EntityPos __entityPos) => entityPos.ContainsKey(__entityPos) ? entityPos[__entityPos] : this;

	/// <summary>
	/// 当实体受到伤害
	/// </summary>
	/// <param name="attackStack"></param>
	public virtual AttackStack underAttack(AttackStack attackStack) {
		if (attackStack.getAttack() <= 0) {
			return attackStack;
		}
		if (new Event.EventEntity.EventLiving.EventAttack.EventAttackVerification(this, attackStack).onEvent().isCancellationAttack() || !attackStack.isEffective()) {
			new Event.EventEntity.EventLiving.EventAttack.EventAttackDodge(this, attackStack).onEvent();
			return attackStack;
		}
		new Event.EventEntity.EventLiving.EventAttack.EventAttackEquipmentOfAttackType(this, attackStack).onEvent();
		if (!attackStack.isEffective()) {
			new Event.EventEntity.EventLiving.EventAttack.EventAttackDodge(this, attackStack).onEvent();
			return attackStack;
		}
		new Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment(this, attackStack).onEvent();
		if (!attackStack.isEffective()) {
			new Event.EventEntity.EventLiving.EventAttack.EventAttackDodge(this, attackStack).onEvent();
			return attackStack;
		}
		if (has(AllEntityState.hasShield)) {
			if (new Event.EventEntity.EventLiving.EventAttack.EventAttackEquipmentOfShield(this, attackStack).isCancellationAttack() || !attackStack.isEffective()) {
				return attackStack;
			}
		}
		new Event.EventEntity.EventLiving.EventAttack.EventAttackInjured(this, attackStack).onEvent();
		toughnessCalculation(attackStack);
		reduce(AllAttributeControl.life.getLimitAttribute(), attackStack.getAttack());
		new Event.EventEntity.EventLiving.EventAttack.EventAttackEnd(this, attackStack).onEvent();
		if (isDeath()) {
			new Event.EventEntity.EventLiving.EventAttack.EventAttackSendDeath(this, attackStack).onEvent();
		}
		return attackStack;
	}

	/// <summary>
	/// 计算韧性
	/// </summary>
	public virtual void toughnessCalculation(AttackStack attackStack) {
		if (!has(AllEntityState.lossFocus)) {
			reduce(AllAttributeControl.tenacity.getLimitAttribute(), attackStack.getReduceToughness());
		}
		if (!has(AllEntityState.lossFocus)) {
			return;
		}
		if (canSet(interruptAnimationPlayableBehaviour, AnimationPlayDetermineType.replay, true)) {
			set(interruptAnimationPlayableBehaviour, true);
		}
	}

	/// <summary>
	/// 尝试移动
	/// </summary>
	public void tryMove(Vector3 pos) {
		KinematicCollision3D kinematicCollision3D = getCharacterBody3D().MoveAndCollide(moveLimit.limit(pos, false));
		//TODO
	}

	/// <summary>
	/// 尝试旋转
	/// </summary>
	public void tryRotation(float rotation) {
		move.targetRotation = rotation;
	}

	/// <summary>
	///  尝试旋转
	/// </summary>
	/// <param name="isPointTo">如果为true，将认识输入向量是从自身触发指向的目标位置</param>
	public void tryRotation(Vector3 pos, bool isPointTo = false) =>
		tryRotation(QuaternionExtendMethod.LookRotation(isPointTo ? pos : pos - Position, QuaternionExtendMethod.up).GetEuler().Y);

	//LookRotation

	/// <summary>
	/// 获取背包的容量
	/// </summary>
	public virtual int getPackCapacity() => int.MaxValue;

	/// <summary>
	/// 添加物品
	/// 返回是否添加成功
	/// </summary>
	/// <returns></returns>
	public bool add(ItemStack? itemStack, bool isAdd) {
		if (itemStack is null) {
			return false;
		}
		if (!isHaveSpace()) {
			return false;
		}
		itemPack.Add(itemStack);
		if (isAdd) {
			new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackAddItem(this, itemStack).onEvent();
		}
		nextCatchData();
		return true;
	}

	/// <summary>
	/// 有没有物品
	/// </summary>
	/// <param name="itemBasic"></param>
	/// <returns></returns>
	public bool has(ItemBasic itemBasic) {
		foreach (var itemStack in itemPack) {
			if (itemStack.getItem().Equals(itemBasic)) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// 获取物品
	/// </summary>
	/// <param name="itemBasic"></param>
	/// <returns></returns>
	public virtual ItemStack? get(ItemBasic itemBasic) {
		foreach (var itemStack in itemPack) {
			if (itemStack.getItem().Equals(itemBasic)) {
				return itemStack;
			}
		}
		return null;
	}

	/// <summary>
	/// 获取所有相同的物品
	/// </summary>
	public virtual List<ItemStack> getAll(ItemBasic itemBasic) {
		List<ItemStack> list = new List<ItemStack>();
		foreach (var itemStack in itemPack) {
			if (itemStack.getItem().Equals(itemBasic)) {
				list.Add(itemStack);
			}
		}
		return list;
	}

	/// <summary>
	/// 删除物品
	/// 返回是否删除成功
	/// </summary>
	/// <param name="itemStack"></param>
	public virtual bool remove(ItemStack? itemStack, bool isDelete, bool isDiscard, bool isTransfer, bool isConsume) {
		if (itemStack is null) {
			return false;
		}
		if (!itemPack.Contains(itemStack)) {
			return false;
		}
		itemPack.Remove(itemStack);
		if (isDelete) {
			new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackLose.EventPackDelete(this, itemStack).onEvent();
		}
		if (isDiscard) {
			new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackLose.EventPackDiscard(this, itemStack).onEvent();
		}
		if (isTransfer) {
			new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackLose.EventPackTransfer(this, itemStack)
				.onEvent();
		}
		if (isConsume) {
			new Event.EventEntity.EventLiving.EventEntityPack.EventPack.EventPackLose.EventPackConsume(this, itemStack).onEvent();
		}
		nextCatchData();
		return true;
	}

	public IEnumerable<ItemStack> forItem() => itemPack;

	/// <summary>
	/// 有空间
	/// </summary>
	public virtual bool isHaveSpace() => itemPack.Count < getPackCapacity() - 1;

	/// <summary>
	/// 添加物品
	/// 返回是否添加成功
	/// </summary>
	public virtual bool addEquipment(EquipmentType? equipmentType, ItemStack? itemStack) {
		if (equipmentType is null) {
			return false;
		}
		if (itemStack is null) {
			return false;
		}
		if (get(equipmentType) != null) {
			return false;
		}
		equipment.put(equipmentType, itemStack);
		new Event.EventEntity.EventLiving.EventEntityPack.EventEquipment.EventEquipmentUse(this, itemStack, equipmentType).onEvent();
		nextCatchData();
		return true;
	}

	/// <summary>
	/// 删除装备
	/// </summary>
	/// <param name="itemStack"></param>
	/// <returns></returns>
	public virtual bool removeEquipment(ItemStack? itemStack) {
		if (itemStack is null) {
			return false;
		}
		EquipmentType? equipmentType = null;
		foreach (var keyValuePair in equipment) {
			if (Equals(keyValuePair.Value, itemStack)) {
				equipmentType = keyValuePair.Key;
			}
		}
		if (equipmentType == null) {
			return false;
		}
		return removeEquipment(equipmentType);
	}

	/// <summary>
	/// 删除装备
	/// </summary>
	/// <param name="itemStack"></param>
	/// <returns></returns>
	public virtual bool removeEquipment(EquipmentType equipmentType) {
		if (!equipment.ContainsKey(equipmentType)) {
			return false;
		}
		ItemStack? itemStack = equipment[equipmentType];
		if (itemStack is null) {
			return false;
		}
		equipment.Remove(equipmentType);
		new Event.EventEntity.EventLiving.EventEntityPack.EventEquipment.EventEquipmentEnd(this, itemStack, equipmentType).onEvent();
		nextCatchData();
		return true;
	}

	/// <summary>
	/// 获取装备物品
	/// </summary>
	public virtual ItemStack? get(EquipmentType equipmentType) {
		if (!equipment.ContainsKey(equipmentType)) {
			return null;
		}
		return equipment[equipmentType];
	}

	public IEnumerable<KeyValuePair<EquipmentType, ItemStack>> forEquipment() => equipment;

	/// <summary>
	/// 设置实体死亡
	/// </summary>
	public void setLivingDeath() {
		if (new Event.EventEntity.EventLiving.EventLivingDeath.EventLivingCanDeath(this).onEvent().isCanDeath()) {
			set(deathAnimationPlayableBehaviour);
			return;
		}
		set(AllAttributeControl.life.getLimitAttribute(), 1);
		new Event.EventEntity.EventLiving.EventLivingDeath.EventLivingNoDeath(this).onEvent();
	}

	/// <summary>
	/// 死亡的时候
	/// </summary>
	protected virtual void onLivingDeath() {
		addNextTimeRun(0, unbound);
	}

	/// <summary>
	/// 更新角色控制器的移动
	/// </summary>
	[TimerCellCycle(triggerType = TriggerType.update)]
	protected void updateCharacterControllerMove() {
		float _deltaTime = getDeltaTime(TriggerType.update, TimeType.part);
		Vector3 _move = move.fixedUpdateMove * _deltaTime;
		_move += move.move * _deltaTime;
		tryMove(_move);

		float rotation = move.targetRotation;
		Vector3 quaternionEuler = Quaternion.GetEuler();
		set(AllEntityState.onRotation, (quaternionEuler.Y - rotation).isEffective(0.01f) && !moveLimit.rotateLock);
		if (moveLimit.rotateLock) {
			return;
		}
		if (float.IsNaN(moveLimit.rotationSmoothTime)) {
			rotation = MathUtil.SmoothDampAngle(quaternionEuler.Y, move.targetRotation, ref move.rotationVelocity, moveLimit.rotationSmoothTime, float.PositiveInfinity,
				_deltaTime);
		}
		Quaternion = Quaternion.FromEuler(new Vector3(0.0f, rotation, 0.0f));
	}

	[TimerCellCycle(triggerType = TriggerType.update)]
	protected void updateAnimationExcessive() {
		if (selectedAnimationPlayableBehaviour is null) {
			return;
		}

		if (mainAnimationPlayableBehaviour!.animationType.HasFlag(AnimationType.death)) {
			new Event.EventEntity.EventLiving.EventLivingDeath.EventLivingDoDeath(this).onEvent();
			onLivingDeath();
			return;
		}
		triggerId = 0;
		loopCount = 0;
		seep = 1;
		new Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingEndAnimation(this, mainAnimationPlayableBehaviour).onEvent();
		excessiveAnimationPlayableBehaviour = selectedAnimationPlayableBehaviour;
		selectedAnimationPlayableBehaviour = null;
		animationPlayer.Play(excessiveAnimationPlayableBehaviour.name, excessiveAnimationPlayableBehaviour.blendTime);
	}

	/// <summary>
	/// 更新动作
	/// </summary>
	[TimerCellCycle(triggerType = TriggerType.update)]
	protected virtual void updateAnimation() {
		animationPlayer.SetProcess(isPlayer);
		if (!isPlayer) {
			return;
		}
		float _seep = timeScale;
		if (animationState != AnimationState.excessive) {
			_seep *= mainAnimationPlayableBehaviour.seepBasics;
			_seep *= seep;
		}
		animationPlayer.SpeedScale = _seep;
		float currentTime = (float)animationPlayer.CurrentAnimationPosition;
		switch (animationState) {
			case AnimationState.excessive:
				if (!animationPlayer.HasAnimation(animationPlayer.CurrentAnimation)) {
					mainAnimationPlayableBehaviour = excessiveAnimationPlayableBehaviour!;
					excessiveAnimationPlayableBehaviour = null;
					new Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingStartAnimation(this, mainAnimationPlayableBehaviour!).onEvent();
					animationState = float.IsNaN(mainAnimationPlayableBehaviour.forwardTime) ? AnimationState.normal : AnimationState.forwardSway;
				}
				break;
			case AnimationState.forwardSway:
				if (currentTime > mainAnimationPlayableBehaviour.forwardTime) {
					animationState = AnimationState.normal;
				}
				break;
			case AnimationState.normal:
			case AnimationState.backSway:
				if (animationState == AnimationState.normal && !float.IsNaN(mainAnimationPlayableBehaviour.backTime)) {
					if (currentTime > mainAnimationPlayableBehaviour.backTime) {
						animationState = AnimationState.backSway;
					}
				}
				if (currentTime > mainAnimationPlayableBehaviour.animationClip.Length) {
					if (mainAnimationPlayableBehaviour.animationType.HasFlag(AnimationType.loop)) {
						triggerId = 0;
						loopCount++;
						return;
					}
					animationState = AnimationState.end;
					nextAnimation();
				}
				break;
		}
	}

	/// <summary>
	/// 收集属性数据
	/// </summary>
	protected void catchAttribute() {
		Dictionary<Attribute, ValueUtil> map = new Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute(this).onEvent().map;
		foreach (var keyValuePair in map) {
			attribute.value.put(keyValuePair.Key, keyValuePair.Key.limit(keyValuePair.Value.getModificationValue()));
		}
	}

	/// <summary>
	/// 收集技能数据
	/// </summary>
	protected void catchSkill() {
		Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event = new Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill(this).onEvent();
		foreach (var keyValuePair in @event.endEvent()) {
			keyValuePair.Value.isEventChange = false;
			double old = keyValuePair.Value.oldLevel;
			switch (old) {
				case > 0 when keyValuePair.Value.level <= 0:
					new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillLose(this, keyValuePair.Key, keyValuePair.Value).onEvent();
					break;
				case <= 0 when keyValuePair.Value.level > 0:
					new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillGet(this, keyValuePair.Key, keyValuePair.Value).onEvent();
					break;
			}
			if (!keyValuePair.Value.isNew) {
				continue;
			}
			keyValuePair.Value.isNew = false;
			skill.put(keyValuePair);
		}
	}

	/// <summary>
	/// 收集数据
	/// </summary>
	public void catchData() {
		needCatch = false;
		catchSkill();
		catchAttribute();
	}

	public void nextCatchData() {
		needCatch = true;
	}

	public void nextNeedChangeMaterial() {
		needChangeMaterial = true;
	}

	public void nextNeedUpdateBeControl() {
		needUpdateBeControl = true;
	}

	/// <summary>
	/// 返回该实体的当前控制行为
	/// 实体ai由此扩展出
	/// </summary>
	protected virtual EntityLivingControl? getEntityLivingControl() => null;

	[TimerCellCycle(priority = 500)]
	protected void fixedUpdateInput() {
		entityInput.recovery();
		getEntityLivingControl()?.writeIn(this, entityInput);
		entityInput.balance();
		getEntityLivingControl()?.balance(this, entityInput);
	}

	/// <summary>
	/// 更新收集数据
	/// </summary>
	[TimerCellCycle(priority = 200, timer = 0.2f, timeType = TimeType.world)]
	protected virtual void fixedUpdateCatchData() {
		if (!needCatch) {
			return;
		}
		catchData();
	}

	[TimerCellCycle(priority = 200, timeType = TimeType.world)]
	protected virtual void fixedUpdateChangeMaterial() {
		if (!needChangeMaterial) {
			return;
		}
		changeMaterial();
	}

	[TimerCellCycle(priority = 200, timeType = TimeType.world)]
	protected virtual void fixedUpdateUpdateBeControl() {
		if (!needUpdateBeControl) {
			return;
		}
		updateBeControl();
	}

	/// <summary>
	/// 刷新buff
	/// </summary>
	[TimerCellCycle(priority = 150, timer = 0, timeType = TimeType.world)]
	protected virtual void fixedUpdateEffect() {
		if (effectStack.isEmpty()) {
			return;
		}
		foreach (var entityEffectStack in effectStack) {
			entityEffectStack.Value.time -= getDeltaTime(TriggerType.fixedUpdate, entityEffectStack.Key.getEffectTimeType(entityEffectStack.Value));
			if (entityEffectStack.Value.isEmpty()) {
				clear(entityEffectStack.Key);
			}
		}
	}

	/// <summary>
	/// 刷新技能
	/// </summary>
	[TimerCellCycle(priority = 150, timer = 0, timeType = TimeType.world)]
	protected virtual void fixedUpdateSkill() {
		if (skill.isEmpty()) {
			return;
		}
		foreach (var keyValuePair in skill) {
			SkillCell skillCell = keyValuePair.Value;
			if (skillCell.cd >= 0) {
				skillCell.cd -= getDeltaTime(TriggerType.fixedUpdate, keyValuePair.Key.getTimeType());
				if (skillCell.cd >= 0) {
					skillCell.isNoCd = true;
				}
			}
			if (skillCell.isNewCd) {
				new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillSetNewCd(this, keyValuePair.Key, skillCell).onEvent();
				skillCell.isNewCd = false;
			}
			if (skillCell.isNoCd) {
				new Event.EventEntity.EventLiving.EventSkill.EventSkillCell.EventSkillToNoCd(this, keyValuePair.Key, skillCell).onEvent();
				skillCell.isNoCd = false;
			}
		}
	}

	/// <summary>
	/// 更新物品
	/// </summary>
	[TimerCellCycle(priority = 150)]
	protected virtual void fixedUpdateItem() {
		if (!itemPack.isEmpty()) {
			foreach (var itemStack in itemPack) {
				itemStack.getItem().fixedUpdate(this, itemStack, null);
			}
		}

		if (!equipment.isEmpty()) {
			foreach (var keyValuePair in equipment) {
				if (keyValuePair.Value is null) {
					continue;
				}
				keyValuePair.Value.getItem().fixedUpdate(this, keyValuePair.Value, keyValuePair.Key);
			}
		}
	}

	/// <summary>
	/// 重置帧移动
	/// </summary>
	[TimerCellCycle(priority = 50)]
	protected virtual void fixedUpdateRecoveryMove() {
		move.fixedUpdateMove = Vector3.Zero;
	}

	[TimerCellCycle(priority = 30)]
	protected virtual void fixedUpdateMoveToward() {
		Vector2 move2d = entityInput.getMove();

		//主管移动并且更改状态
		bool isMove = move2d != Vector2.Zero;
		set(AllEntityState.onMoveAutonomous, isMove);

		//是有效移动
		bool isEffectiveMove = isMove;

		//当前动画状态不允许移动时更改状态
		if (getAnimation().limitLevel > 0 || has(AllEntityState.inAir)) {
			isEffectiveMove = false;
		}

		set(AllEntityState.onMove, isEffectiveMove);
	}

	[TimerCellCycle(priority = 20)]
	protected virtual void fixedUpdateRotationToward() {
		bool isRotation = (entityInput.targetRotation - move.rotationVelocity).isEffective(0.01f);
		set(AllEntityState.onRotationAutonomous, isRotation);
		if (isRotation && has(AllEntityState.onMove)) {
			tryRotation(entityInput.targetRotation);
		}
	}

	/// <summary>
	/// 更新力的移动
	/// </summary>
	[TimerCellCycle]
	protected virtual void fixedUpdateForce() {
		if (moveLimit.constantForce.tryGet(out var _constantForce)) {
			move.move += _constantForce * getDeltaTime(TriggerType.fixedUpdate, TimeType.part);
		}
		move.move = moveLimit.limit(move.move, true);
	}

	/// <summary>
	/// 站立状态判定
	/// </summary>
	[TimerCellCycle(priority = -60)]
	protected virtual void fixedUpdateStandState() {
		if (groundedDetection is null) {
			return;
		}

		if (move.move.Y > groundedDetection.groundedRadius) {
			set(AllEntityState.inAir, true);
			return;
		}

		//TODO

		/*PhysicsDirectSpaceState3D physicsDirectSpaceState3D = GetWorld3D().DirectSpaceState;
		Vector3 spherePosition = get(AllEntityPos.standPos).Position;
		spherePosition.Y += groundedDetection.groundedOffset;
		PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D();
		var result = physicsDirectSpaceState3D.IntersectShape(query);
		set(AllEntityState.inAir, !isGrounded);*/

		/*if (move.move.Y > groundedDetection.groundedRadius) {
			set(AllEntityState.inAir, true);
			return;
		}
		LayerMask layerMask = GameLayerUtil.asLayerMask(GameLayer.groundLayer);
		Vector3 spherePosition = get(AllEntityPos.standPos).position;
		spherePosition.y += groundedDetection.groundedOffset;
		bool isGrounded = Physics.CheckSphere(spherePosition, groundedDetection.groundedRadius, layerMask, QueryTriggerInteraction.Ignore);
		set(AllEntityState.inAir, !isGrounded);*/
	}

	/// <summary>
	/// 移动动作切换
	/// </summary>
	[TimerCellCycle(priority = -80)]
	protected virtual void fixedUpdateMoveAnimation() {
		if (has(AllEntityState.inAir)) {
			return;
		}
		if (has(AllEntityState.onMoveAutonomous)) {
			if (canSet(moveAnimationPlayableBehaviour)) {
				set(moveAnimationPlayableBehaviour);
			}
		}
		else {
			if (canSet(standAnimationPlayableBehaviour)) {
				set(standAnimationPlayableBehaviour);
			}
		}
	}

	/// <summary>
	/// 在空中的动作切换
	/// </summary>
	[TimerCellCycle(priority = -100)]
	public virtual void fixedUpdateAirAnimation() {
		if (!has(AllEntityState.inAir)) {
			return;
		}
		if (!canSet(airAnimationPlayableBehaviour)) {
			if (!getAnimation().animationType.HasFlag(AnimationType.air)) {
				return;
			}
		}
		set(airAnimationPlayableBehaviour);
	}

	/// <summary>
	/// 站立状态判断
	/// </summary>
	[TimerCellCycle(priority = -100)]
	[Obsolete]
	protected virtual void fixedUpdateStandAnimation() {
		if (!getAnimation().Equals(moveAnimationPlayableBehaviour)) {
			return;
		}
		if (has(AllEntityState.onMove)) {
			return;
		}
		if (!canSet(standAnimationPlayableBehaviour)) {
			return;
		}
		set(standAnimationPlayableBehaviour);
	}

	/// <summary>
	/// 更新动作
	/// </summary>
	[TimerCellCycle(priority = -80)]
	protected virtual void fixedUpdateAnimation() {
		if (mainAnimationPlayableBehaviour.posAction is null) {
			return;
		}
		DataStruct<float, Action> triggerPos = mainAnimationPlayableBehaviour.posAction[triggerId];
		if (animationPlayer.CurrentAnimationPosition > triggerPos.k + loopCount * mainAnimationPlayableBehaviour.animationClip.Length) {
			triggerPos.v();
			if (mainAnimationPlayableBehaviour.posAction.Count != triggerId) {
				triggerId++;
			}
		}
	}

	[TimerCellCycle(timer = 1, priority = 100)]
	protected virtual void fixedUpdateCycle_1s() {
		new Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_1s(this).onEvent();
	}

	[TimerCellCycle(timer = 3, priority = 100)]
	protected virtual void fixedUpdateCycle_3s() {
		new Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_3s(this).onEvent();
	}

	[TimerCellCycle(timer = 5, priority = 100)]
	protected virtual void fixedUpdateCycle_5s() {
		new Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_5s(this).onEvent();
	}

	[TimerCellCycle(timer = 20, priority = 100)]
	protected virtual void fixedUpdateCycle_20s() {
		nextCatchData();
		new Event.EventEntity.EventLiving.EventLivingCycle.EventCycle_20s(this).onEvent();
	}

	[TimerCellCycle(priority = 30)]
	protected virtual void fixedUpdateGravity() {
		if (move.move.Y > 0) {
			return;
		}
		if (has(AllEntityState.inAir)) {
			return;
		}
		move.move.Y = -2;
	}

	/// <summary>
	/// 从实体当前的状态保存数据
	/// </summary>
	public virtual string saveData() {
		new Event.EventEntity.EventLiving.EventLivingSaveData(this).onEvent();
		return World.getInstance().getJsonSerializer().serialize(this, info => info.GetCustomAttribute<SaveField>() is not null);
	}

	/// <summary>
	/// 生成默认数据
	/// 在defaultData为null的时候生成数据信息
	/// </summary>
	protected void generateDefaultData() {
		attribute.thisValue.Add(AllAttributeControl.life, 1000);
		attribute.thisValue.Add(AllAttributeControl.mana, 100);
		attribute.thisValue.Add(AllAttributeControl.tenacity, 100);
		attribute.limitValue.Add(AllAttributeControl.life.getLimitAttribute(), 1000);
		attribute.limitValue.Add(AllAttributeControl.mana.getLimitAttribute(), 100);
		attribute.limitValue.Add(AllAttributeControl.tenacity.getLimitAttribute(), 100);

		attribute.thisValue.Add(AllAttributeRecovery.lifeRecovery, 5);
		attribute.thisValue.Add(AllAttributeRecovery.manaRecovery, 5);
		attribute.thisValue.Add(AllAttributeRecovery.tenacityRecovery, 20);

		attribute.thisValue.Add(AllAttribute.attackSpeed, 1);
		attribute.thisValue.Add(AllAttribute.spellSpeed, 1);

		attribute.thisValue.Add(AllAttribute.penetrate, 20);

		attribute.thisValue.Add(AllAttribute.criticalHitProbability, 0.05);
		attribute.thisValue.Add(AllAttribute.criticalHitMultiple, 0.5);

		attribute.thisValue.Add(AllAttribute.attack, 50);
		attribute.thisValue.Add(AllAttribute.defense, 200);

		attribute.thisValue.Add(AllAttribute.speed, 1);

		campFiniteStateMachine.setForceState(AllCamp.@void);
	}

	/// <summary>
	/// 获取动画切片
	/// </summary>
	public AnimationPlayableBehaviour getAnimation(string _name) => animationPlayableBehaviourName.get(_name);

	/// <summary>
	/// 遍历说有动画
	/// </summary>
	public IEnumerable<KeyValuePair<string, AnimationPlayableBehaviour>> forAllAnimation() => animationPlayableBehaviourName;

	/// <summary>
	///  设置动画
	/// </summary>
	public void set(AnimationPlayableBehaviour _animation, bool replay = false) {
		if (!animationPlayableBehaviourName.ContainsValue(_animation)) {
			throw new Exception("意外的动画节点");
		}
		if (mainAnimationPlayableBehaviour is null) {
			mainAnimationPlayableBehaviour = _animation;
			excessiveAnimationPlayableBehaviour = null;
			animationPlayer.Play(mainAnimationPlayableBehaviour.name);
			return;
		}
		if (mainAnimationPlayableBehaviour.Equals(_animation)) {
			if (replay) {
				//mainAnimationPlayableBehaviour.replay();
				animationPlayer.PlaybackDefaultBlendTime = 0;
			}
			return;
		}
		if (Equals(excessiveAnimationPlayableBehaviour, _animation)) {
			return;
		}
		selectedAnimationPlayableBehaviour = _animation;
	}

	/// <summary>
	/// 判断能不能切换
	/// </summary>
	/// <param name="_animation"></param>
	public bool canSet(
		AnimationPlayableBehaviour? _animation,
		AnimationPlayDetermineType animationPlayDetermineType = AnimationPlayDetermineType.skipBackSway,
		bool replay = false
	) {
		if (deathAnimationPlayableBehaviour.Equals(_animation)) {
			return true;
		}
		if (!canPlay(_animation, animationPlayDetermineType, replay)) {
			return false;
		}
		if (has(AllEntityState.beImprison) || has(AllEntityState.beFrozen)) {
			return false;
		}
		if (has(AllEntityState.beVoid)) {
			if (_animation is null) {
				return true;
			}
			if (
				_animation.animationType.HasFlag(AnimationType.attack)
				|| _animation.animationType.HasFlag(AnimationType.skill)
				|| _animation.animationType.HasFlag(AnimationType.dodge)
			) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 获取当前的动画
	/// </summary>
	public AnimationPlayableBehaviour getAnimation() => animationState == AnimationState.excessive ? excessiveAnimationPlayableBehaviour : mainAnimationPlayableBehaviour;

	/// <summary>
	/// 判断实体是不是死亡
	/// </summary> 
	public bool isDeath() => getAnimation().animationType.HasFlag(AnimationType.death);

	public virtual JObject? getDefaultData() => defaultData;

	public virtual void set(JObject jObject) {
		if (entityLifeState != EntityLifeState.enterTree) {
			World.getInstance().getLog().Error("实体的初始数据仅在启用之前更改");
			return;
		}
		defaultData = jObject;
	}

	public virtual bool adopt(Entity entity) {
		if (!getCamp().getEntityScreen().adopt(entity)) {
			return false;
		}
		return true;
	}

	/// <summary>
	/// 恢复成默认材质
	/// </summary>
	public void recoverDefaultMaterial() {
		if (!useOtherMaterial) {
			return;
		}
		useOtherMaterial = false;
		for (var i = 0; i < defaultMaterial.Length; i++) {
			meshInstance.SetSurfaceOverrideMaterial(i, defaultMaterial[i]);
		}
	}

	/// <summary>
	/// 设置实体的材质
	/// </summary>
	public void setMaterial(Material[] materials) {
		useOtherMaterial = true;
		for (var i = 0; i < defaultMaterial.Length; i++) {
			meshInstance.SetSurfaceOverrideMaterial(i, i < materials.Length ? materials[i] : defaultMaterial[i]);
		}
	}

	/// <summary>
	/// 设置实体的材质
	/// </summary>
	public void setMaterial(Material material) {
		Material[] materials = new Material[defaultMaterial.Length];
		for (var i = 0; i < materials.Length; i++) {
			materials[i] = material;
		}
		setMaterial(materials);
	}

	/// <summary>
	/// 获取默认材质
	/// </summary>
	public Material[] getDefaultMaterial() {
		Material[] materials = new Material[defaultMaterial.Length];
		Array.Copy(defaultMaterial, new Material[defaultMaterial.Length], defaultMaterial.Length);
		return materials;
	}

	/// <summary>
	/// 添加材质更改
	/// </summary>
	public void addMaterialChange(string materialChangeName, IEntityLivingMaterialChange material) {
		foreach (var keyValuePair in materialChangeList) {
			if (keyValuePair.k.Contains(materialChangeName)) {
				keyValuePair.v = material;
			}
		}
		materialChangeList.Add(new DataStruct<string, IEntityLivingMaterialChange>(materialChangeName, material));
		nextNeedChangeMaterial();
	}

	/// <summary>
	/// 删除材质更改
	/// </summary>
	public void clearMaterialChange(string materialChangeName) {
		for (var index = 0; index < materialChangeList.Count; index++) {
			var dataStruct = materialChangeList[index];
			if (dataStruct.k.Equals(materialChangeName)) {
				materialChangeList.RemoveAt(index);
				nextNeedChangeMaterial();
				return;
			}
		}
	}

	/// <summary>
	/// 通过materialChangeList的状态更改材质
	/// </summary>
	protected void changeMaterial() {
		needChangeMaterial = false;
		if (materialChangeList.isEmpty()) {
			recoverDefaultMaterial();
			return;
		}
		setMaterial(materialChangeList[0].v.materialChange(this));
	}

	/// <summary>
	/// 更新控制状态
	/// </summary>
	protected virtual void updateBeControl() {
		needUpdateBeControl = false;
		if (has(AllEntityState.beImprison) || has(AllEntityState.beFrozen)) {
			isPlayer = false;
			setLockMove(true, true, true, true);
			return;
		}
		if (!has(AllEntityState.beImprison) && !has(AllEntityState.beFrozen)) {
			isPlayer = true;
			setLockMove(false, false, false, false);
		}
	}

	/// <summary>
	/// 添加一个瞬间力
	/// </summary>
	public virtual void addMomentForce(Vector3 _move, bool needRotate = true) => move.fixedUpdateMove += needRotate ? this.Rotation * _move : _move;

	/// <summary>
	/// 添加一个力
	/// </summary>
	public virtual void addForce(Vector3 _move, bool needRotate = true) => move.move += needRotate ? this.Rotation * _move : _move;

	/// <summary>
	/// 设置力
	/// </summary>s
	public virtual void setForce(Vector3 _move, bool xValid, bool yValid, bool zValid) {
		if (xValid) {
			move.move.X = _move.X;
		}
		if (yValid) {
			move.move.Y = _move.Y;
		}
		if (zValid) {
			move.move.Z = _move.Z;
		}
	}

	/// <summary>
	/// 锁定移动
	/// </summary>
	public virtual void setLockMove(bool xValid, bool xLock, bool yValid, bool yLock, bool zValid, bool zLock, bool rotateValid, bool rotateLock) {
		if (xValid) {
			moveLimit.x.moveLock = xLock;
		}
		if (yValid) {
			moveLimit.y.moveLock = yLock;
		}
		if (zValid) {
			moveLimit.z.moveLock = zLock;
		}
		if (rotateValid) {
			moveLimit.rotateLock = rotateLock;
		}
	}

	public virtual void setLockMove(bool xLock, bool yLock, bool zLock, bool rotateLock) {
		moveLimit.x.moveLock = xLock;
		moveLimit.y.moveLock = yLock;
		moveLimit.z.moveLock = zLock;
		moveLimit.rotateLock = rotateLock;
	}

	//TODO
	/*protected void OnAnimatorMove() {
		if (!isPlayer) {
			return;
		}
		//transform.position += moveLimit.limit(getAnimator().deltaPosition, false);
		tryMove(getAnimator().deltaPosition);
		if (!moveLimit.rotateLock) {
			transform.rotation *= getAnimator().deltaRotation;
		}
	}*/

	/// <summary>
	/// 模拟角色碰撞器对其他物体释加力的效果
	/// </summary>
	//TODO
	/*protected virtual void OnControllerColliderHit(ControllerColliderHit hit) {
		if (basicRigidBodyPush is null) {
			return;
		}
		Rigidbody body = hit.collider.attachedRigidbody;
		if (body is null) {
			return;
		}
		if (body.isKinematic) {
			return;
		}
		int bodyLayerMask = 1 << body.gameObject.layer;
		if ((bodyLayerMask & _basicRigidBodyPush.pushLayers.value) == 0) {
			return;
		}
		if (hit.moveDirection.y < -0.3f) {
			return;
		}
		Vector3 pushDir = new Vector3(hit.moveDirection.x, 0.0f, hit.moveDirection.z);
		body.AddForce(pushDir * _basicRigidBodyPush.strength, ForceMode.Impulse);
	}*/

	//TODO
	/*protected virtual void OnDrawGizmosSelected() {
		if (!EditorApplication.isPlaying) {
			return;
		}
		if (!groundedDetection.tryGet(out var _groundedDetection)) {
			return;
		}
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);
		Gizmos.color = has(AllEntityState.inAir) ? transparentRed : transparentGreen;
		Vector3 pos = get(AllEntityPos.standPos).position;
		pos.y += _groundedDetection.groundedOffset;
		Gizmos.DrawSphere(pos, _groundedDetection.groundedRadius);
	}*/
	public CharacterBody3D getCharacterBody3D() => characterBody3D;

	public CollisionObject3D getCollisionObject3D() => collisionObject3D;

	/// <summary>
	/// 提供站立动作作为最后的动作
	/// </summary>
	[Event(priority = -1000)]
	protected virtual void onEvent_defaultNextAnimation(Event.EventEntity.EventLiving.EventLivingAnimator.EventSettlementNextAnimation @event) {
		if (animationState == AnimationState.end) {
			return;
		}
		@event.setNextAnimationPlayableBehaviour(has(AllEntityState.inAir) ? airAnimationPlayableBehaviour : standAnimationPlayableBehaviour);
	}

	/// <summary>
	/// 切换攻击速度
	/// </summary>
	protected virtual void onEvent_switchAttackSpeed(Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingStartAnimation @event) {
		if (!@event.animationPlayableBehaviour.animationType.HasFlag(AnimationType.attack)) {
			return;
		}
		seep = (float)get(AllAttribute.attackSpeed);
	}

	/// <summary>
	/// 切换技能速度
	/// </summary>
	protected virtual void onEvent_switchCastSpeed(Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingStartAnimation @event) {
		if (!@event.animationPlayableBehaviour.animationType.HasFlag(AnimationType.skill)) {
			return;
		}
		seep = (float)get(AllAttribute.spellSpeed);
	}

	/// <summary>
	/// 根据移动速度切换移动动画播放速度
	/// </summary>
	protected virtual void onEvent_switchMoveSpeed(Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingStartAnimation @event) {
		if (!@event.animationPlayableBehaviour.animationType.HasFlag(AnimationType.move)) {
			return;
		}
		seep = (float)get(AllAttribute.speed);
	}

	/// <summary>
	/// 数据搜集时更新移动速度
	/// 移动速度可能会发生变化时更新动画速度
	/// </summary>
	[Event(priority = -1000)]
	protected virtual void onEvent_switchMoveSpeed(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
		if (!getAnimation().animationType.HasFlag(AnimationType.move)) {
			return;
		}
		float newSeep = (float)@event.map.get(AllAttribute.speed).getModificationValue();
		if (Math.Abs(newSeep - get(AllAttribute.speed)) < 0.01) {
			return;
		}
		seep = newSeep;
	}

	/// <summary>
	/// 失衡判定
	/// </summary>
	[Event(priority = -200)]
	protected virtual void onEvent_unbalanceJudge(Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit @event) {
		if (!@event.attributeChangeType.Equals(AttributeChangeType.reduce)) {
			return;
		}
		if (!@event.attributeLimit.Equals(AllAttributeControl.tenacity.getLimitAttribute())) {
			return;
		}
		if (has(AllEntityState.lossFocus)) {
			return;
		}
		if (@event.simulationGet() > 0) {
			return;
		}
		set(AllEntityState.lossFocus, true);
	}

	/// <summary>
	/// 失衡恢复的判定
	/// </summary>
	[Event(priority = -100)]
	protected virtual void onEvent_balanceJudge(Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit @event) {
		if (!@event.attributeChangeType.Equals(AttributeChangeType.add)) {
			return;
		}
		if (!@event.attributeLimit.Equals(AllAttributeControl.tenacity.getLimitAttribute())) {
			return;
		}
		if (!has(AllEntityState.lossFocus)) {
			return;
		}
		if (get(AllAttributeControl.tenacity) > @event.simulationGet()) {
			return;
		}
		set(AllEntityState.lossFocus, false);
	}

	/// <summary>
	/// 死亡判定
	/// </summary>
	[Event(priority = -200)]
	protected virtual void onEvent_deathJudge(Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit @event) {
		if (!@event.attributeChangeType.Equals(AttributeChangeType.reduce)) {
			return;
		}
		if (!@event.attributeLimit.Equals(AllAttributeControl.life.getLimitAttribute())) {
			return;
		}
		if (@event.simulationGet() > 0) {
			return;
		}
		if (@event.entityLiving.isDeath()) {
			return;
		}
		@event.entityLiving.addNextTimeRun(0, @event.entityLiving.setLivingDeath);
	}

	/// <summary>
	/// 获得时间缩放的buff时更改实体对应的时间缩放
	/// </summary>
	[Event(priority = 300)]
	protected virtual void onEvent_addTimeEffect(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
		if (!@event.effect.Equals(AllEntityEffect.effectTimeScale)) {
			return;
		}
		if (timeScale > @event.effectCell.level) {
			return;
		}
		setTimeScale((float)@event.effectCell.level);
	}

	/// <summary>
	/// 清除时间buff时判断减少
	/// </summary>
	[Event(priority = 300)]
	protected virtual void onEvent_clearTimeEffect(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
		if (!@event.effect.Equals(AllEntityEffect.effectTimeScale)) {
			return;
		}
		setTimeScale(1);
	}

	/// <summary>
	/// 添加背包物品的属性加成
	/// </summary>
	/*[Event(priority = 300)]
	protected virtual void onEvent_catchItemAttribute(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
		if (@event.entityLiving.equipment.isEmpty()) {
			return;
		}
		foreach (var keyValuePair in @event.entityLiving.equipment) {
			ItemStack itemStack = keyValuePair.Value;
			if (itemStack.attribute is null) {
				continue;
			}
			if (itemStack.attribute.isEmpty()) {
				continue;
			}
			foreach (var valuePair in itemStack.attribute) {
				@event.add(valuePair.Key, valuePair.Value);
			}
		}
	}*/
	protected virtual void onEvent(Event.EventEntity.EventLiving.EventLivingAnimator.EventLivingStartAnimation @event) {
		if (!@event.animationPlayableBehaviour.Equals(deathAnimationPlayableBehaviour)) {
			return;
		}
		set(AllEntityState.beControl, false);
	}

	/// <summary>
	/// 禁锢开始
	/// </summary>
	protected void onEvent_ImprisonStartState(Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState @event) {
		if (!@event.entityState.Equals(AllEntityState.beImprison)) {
			return;
		}
		setTimeScale(0);
		set(AllEntityState.hasTimeScale, false);
		nextNeedUpdateBeControl();
	}

	/// <summary>
	/// 禁锢结束
	/// </summary>
	protected void onEvent_ImprisonStartEnd(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
		if (!@event.entityState.Equals(AllEntityState.beImprison)) {
			return;
		}
		setTimeScale(1);
		nextNeedUpdateBeControl();
	}

	/// <summary>
	/// 冰冻状态开始
	/// </summary>
	protected void onEvent_FrozenStartState(Event.EventEntity.EventLiving.EventLivingState.EventLivingStartState @event) {
		if (!@event.entityState.Equals(AllEntityState.beFrozen)) {
			return;
		}
		nextNeedUpdateBeControl();
	}

	/// <summary>
	/// 冻结结束
	/// </summary>
	protected void onEvent_FrozenStartEnd(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
		if (!@event.entityState.Equals(AllEntityState.beFrozen)) {
			return;
		}
		nextNeedUpdateBeControl();
	}

	/// <summary>
	/// 当不在空中时设置动画站立
	/// </summary>
	protected virtual void onEvent_endInAir(Event.EventEntity.EventLiving.EventLivingState.EventLivingEndState @event) {
		if (!@event.entityState.Equals(AllEntityState.inAir)) {
			return;
		}
		if (!canSet(standAnimationPlayableBehaviour)) {
			return;
		}
		set(standAnimationPlayableBehaviour);
	}

	public virtual bool canPlay(AnimationPlayableBehaviour? animationPlayableBehaviour, AnimationPlayDetermineType animationPlayDetermineType, bool replay) {
		if (!replay && animationPlayableBehaviour is not null) {
			if (Equals(animationPlayableBehaviour, mainAnimationPlayableBehaviour)) {
				return false;
			}
			if (Equals(animationPlayableBehaviour, excessiveAnimationPlayableBehaviour)) {
				return false;
			}
			if (Equals(animationPlayableBehaviour, selectedAnimationPlayableBehaviour)) {
				return false;
			}
		}
		if (excessiveAnimationPlayableBehaviour is not null) {
			if (!canPlayStatic(excessiveAnimationPlayableBehaviour, animationPlayableBehaviour, animationPlayDetermineType)) {
				return false;
			}
		}
		if (selectedAnimationPlayableBehaviour is not null) {
			if (!canPlayStatic(selectedAnimationPlayableBehaviour, animationPlayableBehaviour, animationPlayDetermineType)) {
				return false;
			}
		}
		else {
			if (!canPlayStatic(mainAnimationPlayableBehaviour, animationPlayableBehaviour, animationPlayDetermineType)) {
				return false;
			}
		}
		return true;
	}

	public static bool canPlayStatic(AnimationPlayableBehaviour old, AnimationPlayableBehaviour? @new, AnimationPlayDetermineType animationPlayDetermineType) {
		int newL = @new?.limitLevel ?? 0;
		int oldL = old.limitLevel;
		if (newL == 0 && oldL == 0) {
			return true;
		}
		bool isBackSway = old.limitLevel >= (int)AnimationState.backSway;
		switch (animationPlayDetermineType) {
			case AnimationPlayDetermineType.strict:
				return newL > oldL;
			case AnimationPlayDetermineType.replay:
				return newL >= oldL;
			case AnimationPlayDetermineType.replayBackSway:
				return newL > oldL || (isBackSway && newL >= oldL);
			case AnimationPlayDetermineType.skipBackSway:
				return newL > oldL || isBackSway;
			default:
				throw new ArgumentOutOfRangeException(nameof(animationPlayDetermineType), animationPlayDetermineType, null);
		}
	}

	public class BasicRigidBodyPush {
		public int pushLayers = LayerPrefab.entity | LayerPrefab.ground;
		public float strength = 1.0f;
	}

	public class GroundedDetection {
		/// <summary>
		/// 地面检测偏移
		/// </summary>
		public float groundedOffset = 0;

		/// <summary>
		/// 地面检测范围
		/// </summary>
		public float groundedRadius = 0.27f;
	}

	public class MoveLimit {
		public MoveCell x = new MoveCell();

		public MoveCell y = new MoveCell() {
			max = -52
		};

		public MoveCell z = new MoveCell();

		/// <summary>
		/// 恒定力（重力）
		/// </summary>
		public ControlledField<Vector3> constantForce = new Vector3(0, -18, 0);

		/// <summary>
		/// 旋转平滑时间
		/// NaN
		/// </summary>
		public float rotationSmoothTime = 0.12f;

		/// <summary>
		/// 旋转锁
		/// </summary>
		public bool rotateLock;

		/// <summary>
		/// 移动平滑阈值
		/// </summary>
		public MoveSmooth? moveSmooth = new MoveSmooth();

		/// <summary>
		/// 速度平滑
		/// </summary>
		public class MoveSmooth {
			public float speedOffset = 0.333f;
			public float speedChangeRate = 0.1f;
		}

		/// <summary>
		/// 速度乘积
		/// </summary>
		public ControlledField<float> seepMultiple = 1;

		public Vector3 limit(Vector3 move, bool useGain) {
			return new Vector3(x.limit(move.X, useGain), y.limit(move.Y, useGain), z.limit(move.Z, useGain));
		}

		public class MoveCell {
			/// <summary>
			/// 最大
			/// </summary>
			public float max = Single.MaxValue;

			/// <summary>
			/// 最小
			/// </summary>
			public float min;

			/// <summary>
			/// 增益(速度*gain)小于1大于0
			/// </summary>
			public float gain = 0.97f;

			/// <summary>
			/// 移动的锁
			/// </summary>
			public bool moveLock;

			public float getMax() => max;

			public float getMin() => min;

			public float limit(float t, bool useGain) {
				if (moveLock) {
					return 0;
				}
				if (useGain && !float.IsNaN(gain)) {
					t *= gain;
				}
				float _t = Mathf.Clamp(Mathf.Abs(t), getMin(), getMax());
				return _t * Mathf.Sign(t);
			}
		}
	}

	public class Move {
		/// <summary>
		/// 固定帧移动，在帧中做插值运算，在updateMove中更新
		/// </summary>
		public Vector3 fixedUpdateMove;

		/// <summary>
		/// 对于重力等一些其他的因素力，这里的值将不会被清除而是主动更改
		/// </summary>
		public Vector3 move;

		/// <summary>
		/// 上一帧移动的速度
		/// </summary>
		public float fixedUpdateMoveSeep;

		/// <summary>
		/// 角色的旋转
		/// </summary>
		public float targetRotation;

		/// <summary>
		/// 旋转速度
		/// </summary>
		public float rotationVelocity;

		public Vector3 fixedMove(float deltaTime) {
			Vector3 _move = fixedUpdateMove;
			_move += move;
			return _move * deltaTime;
		}

		public void setFixedUpdateMove(Vector3 _move) {
			fixedUpdateMove = _move;
			fixedUpdateMoveSeep = _move.Length(); //move.magnitude;
		}
	}

	public class EntityInput {
		public float targetRotation;
		protected Vector2 move;

		protected HashSet<InputOption> inputOptionList = new HashSet<InputOption>();

		public Vector2 getMove() => move;

		public void addInputOption(InputOption inputOption) {
			inputOptionList.Add(inputOption);
		}

		public bool hasInputOption(InputOption inputOption) {
			return inputOptionList.Contains(inputOption);
		}

		public bool hasInputOption(params InputOption[] inputOption) {
			foreach (var option in inputOption) {
				if (hasInputOption(option)) {
					return true;
				}
			}
			return false;
		}

		public void recovery() {
			move = Vector2.Zero;
			targetRotation = 0;
			inputOptionList.Clear();
		}

		public void balance() {
			move.X = 0;
			move.Y = 0;
			if (hasInputOption(InputOption.w)) {
				move.Y++;
			}
			if (hasInputOption(InputOption.s)) {
				move.Y--;
			}

			if (hasInputOption(InputOption.d)) {
				move.X++;
			}
			if (hasInputOption(InputOption.a)) {
				move.X--;
			}
		}
	}
}

/// <summary>
/// 单一动画节点
/// </summary>
public class AnimationPlayableBehaviour {
	/// <summary>
	/// 动画对象
	/// </summary>
	protected internal Animation animationClip;

	/// <summary>
	/// 动画的名称，是字段名称
	/// </summary>
	protected internal string name = String.Empty;

	/// <summary>
	/// 基础速度
	/// </summary>
	protected internal float seepBasics = 1;

	/// <summary>
	/// 混合时间
	/// NaN
	/// </summary>
	protected internal float blendTime = 0.1f;

	/// <summary>
	/// 重播时间
	/// Nan
	/// </summary>
	protected internal float replayTime = Single.NaN;

	/// <summary>
	/// 前摇结束时间
	/// NaN
	/// </summary>
	protected internal float forwardTime = Single.NaN;

	/// <summary>
	/// 后摇开始时间
	/// NaN
	/// </summary>
	protected internal float backTime = Single.NaN;

	/// <summary>
	/// 事件点
	/// </summary>
	protected internal List<DataStruct<float, Action>>? posAction;

	/// <summary>
	/// 动作的类型
	/// </summary>
	protected internal AnimationType animationType;

	/// <summary>
	/// 限制等级
	/// </summary>
	protected internal int limitLevel;

	public AnimationPlayableBehaviour addPosAction(float pos, Action action) {
		posAction ??= new List<DataStruct<float, Action>>();
		bool needInsert = true;
		for (var index = 0; index < posAction.Count; index++) {
			DataStruct<float, Action> _timerCell = posAction[index];
			if (_timerCell.k <= pos) {
				continue;
			}
			posAction.Insert(index, new DataStruct<float, Action>(pos, action));
			needInsert = false;
			break;
		}
		if (needInsert) {
			posAction.Add(new DataStruct<float, Action>(pos, action));
		}
		return this;
	}
}

public class AnimationPlayableBehaviourData : System.Attribute {
	public readonly AnimationType animationType;
	public readonly int limitLevel;

	public AnimationPlayableBehaviourData(AnimationType animationType, int limitLevel) {
		this.animationType = animationType;
		this.limitLevel = limitLevel;
	}
}
