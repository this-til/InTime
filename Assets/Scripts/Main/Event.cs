using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;
using Newtonsoft.Json.Linq;

namespace InTime;

public abstract class Event : EventBus.Event {
    /// <summary>
    /// 实体事件
    /// </summary>
    public abstract class EventEntity : Event {
        public readonly Entity entity;

        public EventEntity(Entity entity) {
            this.entity = entity;
        }

        /// <summary>
        /// 实体Awake阶段发布
        /// </summary>
        public class EventEntityAwake : EventEntity {
            public EventEntityAwake(Entity entity) : base(entity) {
            }
        }

        /// <summary>
        /// 当实体被摧毁
        /// </summary>
        public class EventEntityDestroy : EventEntity {
            public EventEntityDestroy(Entity entity) : base(entity) {
            }
        }

        /// <summary>
        /// 启用实体
        /// </summary>
        public class EventEntityOnUse : EventEntity {
            public EventEntityOnUse(Entity entity) : base(entity) {
            }
        }

        /// <summary>
        /// 实体获得新的时间缩放
        /// </summary>
        public class EventEntityNewTimeScale : EventEntity {
            public readonly float oldTimeScale;
            public readonly float newTimeScale;

            public EventEntityNewTimeScale(Entity entity, float oldTimeScale, float newTimeScale) : base(entity) {
                this.oldTimeScale = oldTimeScale;
                this.newTimeScale = newTimeScale;
            }

            /// <summary>
            /// 是时间加快
            /// </summary>
            public bool isTimeSeepUp() => newTimeScale > oldTimeScale;
        }

        /// <summary>
        /// 活体实体
        /// </summary>
        public abstract class EventLiving : EventEntity {
            public readonly EntityLiving entityLiving;

            public EventLiving(EntityLiving entityLiving) : base(entityLiving) {
                this.entityLiving = entityLiving;
            }

            /// <summary>
            /// 实体初始化时
            /// </summary>
            public abstract class EventLivingInit : EventLiving {
                public EventLivingInit(EntityLiving entityLiving) : base(entityLiving) {
                }

                public class EventLivingInitProcess : EventLivingInit {
                    public EventLivingInitProcess(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化结束方法时触发
                /// </summary>
                public class EventLivingInitEnd : EventLivingInit {
                    public EventLivingInitEnd(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 实体初始化动作
                /// </summary>
                [Obsolete]
                public class EventLivingInitAnimation : EventLivingInit {
                    public EventLivingInitAnimation(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 实体初始化元素
                /// </summary>
                [Obsolete]
                public class EventLivingInitElement : EventLivingInit {
                    public EventLivingInitElement(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化buff
                /// </summary>
                [Obsolete]
                public class EventLivingInitEffect : EventLivingInit {
                    public EventLivingInitEffect(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化技能
                /// </summary>
                [Obsolete]
                public class EventLivingInitSkill : EventLivingInit {
                    public EventLivingInitSkill(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化属性
                /// </summary>
                [Obsolete]
                public class EventLivingInitAttribute : EventLivingInit {
                    public EventLivingInitAttribute(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化实体类型
                /// </summary>
                [Obsolete]
                public class EventLivingInitType : EventLivingInit {
                    public EventLivingInitType(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化阵营
                /// </summary>
                [Obsolete]
                public class EventLivingInitCamp : EventLivingInit {
                    public EventLivingInitCamp(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化NBT
                /// </summary>
                [Obsolete]
                public class EventLivingInitNBT : EventLivingInit {
                    public EventLivingInitNBT(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 初始化计时器
                /// </summary>
                [Obsolete]
                public class EventLivingInitTime : EventLivingInit {
                    public EventLivingInitTime(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }
            }

            /// <summary>
            /// 当实体保存数据时
            /// </summary>
            public class EventLivingSaveData : EventLiving {
                public EventLivingSaveData(EntityLiving entityLiving) : base(entityLiving) {
                }
            }

            /// <summary>
            /// 活体实体刷新
            /// </summary>
            [Obsolete("为了效率决定不发布帧事件")]
            public class EventLivingUpdate : EventLiving {
                public EventLivingUpdate(EntityLiving entityLiving) : base(entityLiving) {
                }
            }

            /// <summary>
            /// 和实体效果相关
            /// </summary>
            public abstract class EventLivingEffect : EventLiving {
                public readonly EntityEffectBasics effect;
                public readonly EntityEffectCell effectCell;

                public EventLivingEffect(EntityLiving entity, EntityEffectBasics effect,
                    EntityEffectCell effectCell) : base(entity) {
                    this.effectCell = effectCell;
                    this.effect = effect;
                }

                public EventLivingEffect(EntityLiving entity, EntityEffectBasics effect) : this(entity, effect,
                    entity.get(effect)) {
                }

                /// <summary>
                /// 实体添加效果
                /// </summary>
                public abstract class EventLivingAddEffect : EventLivingEffect {
                    public EventLivingAddEffect(EntityLiving entity, EntityEffectBasics effect,
                        EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                    }

                    public EventLivingAddEffect(EntityLiving entity, EntityEffectBasics effect) : base(entity,
                        effect) {
                    }

                    public override bool isContinue() => !effectCell.isEmpty();

                    /// <summary>
                    /// 初始化时添加buff
                    /// </summary>
                    public class EventLivingInitAddEffect : EventLivingAddEffect {
                        public EventLivingInitAddEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                        }

                        public EventLivingInitAddEffect(EntityLiving entity, EntityEffectBasics effect) : base(entity, effect) {
                        }
                    }

                    /// <summary>
                    /// 实体buff融合时添加
                    /// </summary>
                    public class EventLivingFuseAddEffect : EventLivingAddEffect {
                        /// <summary>
                        /// 可以添加
                        /// </summary>
                        protected bool canAdd = true;

                        public EventLivingFuseAddEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(
                            entity,
                            effect, effectCell) {
                        }

                        public void setNoCanAdd() => canAdd = false;
                        public bool isCanAdd() => canAdd;
                        public override bool isContinue() => isCanAdd();
                    }

                    /// <summary>
                    /// 自然添加
                    /// </summary>
                    public class EventLivingNatureAddEffect : EventLivingAddEffect {
                        /// <summary>
                        /// 可以添加
                        /// </summary>
                        protected bool canAdd = true;

                        public EventLivingNatureAddEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                        }

                        public EventLivingNatureAddEffect(EntityLiving entity, EntityEffectBasics effect) : base(entity, effect) {
                        }

                        public void setNoCanAdd() => canAdd = false;
                        public bool isCanAdd() => canAdd;
                        public override bool isContinue() => isCanAdd();
                    }
                }

                /// <summary>
                /// 实体buff融合添加
                /// </summary>
                public class EventLivingFuseEffect : EventLivingAddEffect {
                    public EventLivingFuseEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                    }

                    public EventLivingFuseEffect(EntityLiving entity, EntityEffectBasics effect) : base(entity, effect) {
                    }
                }

                /// <summary>
                /// 实体清除效果
                /// </summary>
                public abstract class EventLivingClearEffect : EventLivingEffect {
                    public EventLivingClearEffect(EntityLiving entity, EntityEffectBasics effect,
                        EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                    }

                    /// <summary>
                    /// 实体buff融合时清除
                    /// </summary>
                    public class EventLivingFuseClearEffect : EventLivingAddEffect {
                        public EventLivingFuseClearEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                        }

                        public EventLivingFuseClearEffect(EntityLiving entity, EntityEffectBasics effect) : base(entity, effect) {
                        }
                    }

                    /// <summary>
                    /// 自然清除
                    /// </summary>
                    public class EventLivingNatureClearEffect : EventLivingClearEffect {
                        public EventLivingNatureClearEffect(EntityLiving entity, EntityEffectBasics effect, EntityEffectCell effectCell) : base(entity, effect, effectCell) {
                        }
                    }
                }
            }

            /// <summary>
            /// 实体属性
            /// </summary>
            public abstract class EventAttribute : EventLiving {
                public EventAttribute(EntityLiving entityLiving) : base(entityLiving) {
                }

                public class EventAttributeLimit : EventAttribute {
                    public readonly AttributeLimit attributeLimit;
                    public readonly AttributeChangeType attributeChangeType;
                    public readonly double old;
                    protected readonly ValueUtil valueUtil;
                    public double value;
                    public bool hasValue;

                    public EventAttributeLimit(EntityLiving entity, AttributeLimit attributeLimit, AttributeChangeType attributeChangeType, double old,
                        double operationValue) : base(entity) {
                        this.attributeChangeType = attributeChangeType;
                        this.attributeLimit = attributeLimit;
                        this.old = old;
                        valueUtil = new ValueUtil(operationValue);
                    }

                    public void add(double a) => valueUtil.add(a);

                    public void addMultiply(Multiple multiple, double a) => valueUtil.addMultiple(multiple, a);

                    /// <summary>
                    /// 用于计算最终值
                    /// </summary>
                    /// <returns></returns>
                    public double getValue() => valueUtil.getModificationValue();

                    /// <summary>
                    /// 模拟值更改后的数量
                    /// </summary>
                    public double simulationGet() =>
                        attributeChangeType switch {
                            AttributeChangeType.set => getValue(),
                            AttributeChangeType.add => old + getValue(),
                            AttributeChangeType.reduce => old - getValue(),
                            _ => old
                        };
                }

                /// <summary>
                /// 收集属性时
                /// </summary>
                public class EventCatchAttribute : EventAttribute {
                    public readonly Dictionary<Attribute, ValueUtil> map;
                    //public readonly Map<Attribute, double> valueBasics;

                    public EventCatchAttribute(EntityLiving entityLiving) : base(entityLiving) {
                        map = new Dictionary<Attribute, ValueUtil>(8);
                        //valueBasics = new Map<Attribute, double>(8);
                        foreach (var keyValuePair in entityLiving.forThisAttribute()) {
                            map.put(keyValuePair.Key, new ValueUtil(keyValuePair.Value));
                            //valueBasics.put(keyValuePair.Key, keyValuePair.Value);
                        }
                    }

                    //public double get(Attribute attribute) => valueBasics.get(attribute);

                    public void add(Attribute attribute, double d) => map.get(attribute, () => new ValueUtil(0)).add(d);

                    public void addMultiple(Attribute attribute, Multiple multiple, double d) =>
                        map.get(attribute, () => new ValueUtil(0)).addMultiple(multiple, d);
                }
            }

            /// <summary>
            /// 和实体攻击相关
            /// </summary>
            public abstract class EventAttack : EventLiving {
                public readonly AttackStack stack;

                public EventAttack(EntityLiving entityLiving, AttackStack stack) : base(entityLiving) {
                    this.stack = stack;
                }

                public override bool isContinue() => stack.isEffective();

                /// <summary>
                /// 验证攻击
                /// cancel false 可取消攻击
                /// </summary>
                public class EventAttackVerification : EventAttack {
                    /// <summary>
                    /// 取消攻击
                    /// </summary>
                    protected bool _cancellationAttack;

                    public EventAttackVerification(EntityLiving entityLiving, AttackStack stack) : base(
                        entityLiving, stack) {
                    }

                    public void cancellationAttack() {
                        _cancellationAttack = true;
                    }

                    public bool isCancellationAttack() => _cancellationAttack;
                    public override bool isContinue() => !isCancellationAttack();
                }

                /// <summary>
                /// 在此更改攻击类型
                /// </summary>
                public class EventAttackEquipmentOfAttackType : EventAttack {
                    public EventAttackEquipmentOfAttackType(EntityLiving entityLiving, AttackStack stack) : base(
                        entityLiving, stack) {
                    }
                }

                /// <summary> 
                /// 当实体被攻击时
                /// 在此为伤害区间
                /// </summary>
                public class EventAttackEquipment : EventAttack {
                    public EventAttackEquipment(EntityLiving entityLiving, AttackStack stack) : base(entityLiving, stack) {
                    }
                }

                /// <summary> 
                /// 当实体被攻击时
                ///     允许在这一阶段更改攻击类型和初始伤害(为护盾操作)
                ///     cancellationAttack 可取消攻击
                /// </summary>
                public class EventAttackEquipmentOfShield : EventAttack {
                    /// <summary>
                    /// 取消攻击
                    /// </summary>
                    protected bool _cancellationAttack;

                    public EventAttackEquipmentOfShield(EntityLiving entityLiving, AttackStack stack) : base(
                        entityLiving, stack) {
                    }

                    public void cancellationAttack() {
                        _cancellationAttack = true;
                    }

                    public bool isCancellationAttack() => _cancellationAttack;
                    public override bool isContinue() => !isCancellationAttack();
                }

                /// <summary>
                /// 当活体实体受到伤害时
                ///     通过更改stack.attack更改受到的伤害
                ///     一般由攻击类型监听
                ///     但不可以取消伤害
                /// </summary>
                public class EventAttackInjured : EventAttack {
                    public EventAttackInjured(EntityLiving entityLiving, AttackStack stack) : base(entityLiving, stack) {
                    }
                }

                /// <summary>
                /// 攻击结束
                /// </summary>
                public class EventAttackEnd : EventAttack {
                    public EventAttackEnd(EntityLiving entityLiving, AttackStack stack) : base(entityLiving, stack) {
                    }
                }

                /// <summary>
                /// 触发一个由攻击者发起的事件
                /// </summary>
                [Obfuscation]
                public class EventAttackDo : EventAttack {
                    /// <summary>
                    /// 受击着
                    /// </summary>
                    public EntityLiving hit;

                    /// <summary>
                    /// 
                    /// </summary>
                    /// <param name="entityLiving">
                    ///    攻击者
                    /// </param>
                    /// <param name="stack"></param>
                    /// <param name="hit">
                    ///    收击着
                    /// </param>
                    public EventAttackDo(EntityLiving entityLiving, AttackStack stack, EntityLiving hit) : base(
                        entityLiving, stack) {
                        this.hit = hit;
                    }
                }

                /// <summary>
                /// 规避伤害触发
                /// </summary>
                public class EventAttackDodge : EventAttack {
                    public EventAttackDodge(EntityLiving entityLiving, AttackStack stack) : base(entityLiving,
                        stack) {
                    }
                }

                /// <summary>
                /// 攻击使对象死亡
                /// </summary>
                public class EventAttackSendDeath : EventAttack {
                    public EventAttackSendDeath(EntityLiving entityLiving, AttackStack stack) : base(entityLiving,
                        stack) {
                    }
                }
            }

            /// <summary>
            /// 和护盾相关的
            /// </summary>
            public abstract class EventShield : EventLiving {
                /// <summary>
                /// 抵挡伤害
                /// </summary>
                public readonly double resist;

                public readonly ShieldEntityEffect effect;

                /// <summary>
                /// 抵挡护盾的效果
                /// </summary>
                public readonly EntityEffectCell effectCell;

                public readonly AttackStack attackStack;

                public EventShield(EntityLiving entityLiving, double resist,
                    ShieldEntityEffect effect, EntityEffectCell effectCell,
                    AttackStack attackStack) : base(
                    entityLiving) {
                    this.resist = resist;
                    this.effectCell = effectCell;
                    this.attackStack = attackStack;
                    this.effect = effect;
                }

                /// <summary>
                /// 能不能用护盾抵挡伤害
                /// 仅从伤害类型判断
                /// </summary>
                public class EventShieldCan : EventShield {
                    /// <summary>
                    /// 可以抵抗伤害
                    /// </summary>
                    protected bool canResistAttack = true;

                    public EventShieldCan(EntityLiving entityLiving, AttackStack attackStack) : base(entityLiving,
                        0, null!, null!, attackStack) {
                    }

                    /// <summary>
                    /// 设置成不能抵抗
                    /// </summary>
                    public void setNotResistAttack() {
                        canResistAttack = false;
                    }

                    public bool isResistAttack() => canResistAttack;
                    public override bool isContinue() => isResistAttack();
                }

                /// <summary>
                /// 抵抗
                /// </summary>
                public class EventShieldResist : EventShield {
                    public EventShieldResist(EntityLiving entityLiving, double resist,
                        ShieldEntityEffect effect, EntityEffectCell effectCell,
                        AttackStack attackStack) : base(
                        entityLiving, resist, effect, effectCell, attackStack) {
                    }
                }

                /// <summary>
                /// 当护盾破碎的时候（攻击导致）
                /// </summary>
                public class EventShieldAttackFractureFracture : EventShield {
                    public EventShieldAttackFractureFracture(EntityLiving entityLiving, double resist,
                        ShieldEntityEffect effect,
                        EntityEffectCell effectCell,
                        AttackStack attackStack) : base(entityLiving, resist, effect, effectCell, attackStack) {
                    }
                }

                /// <summary>
                /// 当护盾破碎时
                /// 注攻击类型为空
                /// </summary>
                public class EventShieldFracture : EventShield {
                    public EventShieldFracture(EntityLiving entityLiving, double resist,
                        ShieldEntityEffect effect,
                        EntityEffectCell effectCell) : base(
                        entityLiving, resist, effect, effectCell, null) {
                    }
                }
            }

            /// <summary>
            /// 和实体死亡相关的
            /// </summary>
            public abstract class EventLivingDeath : EventLiving {
                public EventLivingDeath(EntityLiving entityLiving) : base(entityLiving) {
                }

                /// <summary>
                /// 实体能不能死亡
                /// </summary>
                public class EventLivingCanDeath : EventLivingDeath {
                    /// <summary>
                    /// 能够死亡
                    /// </summary>
                    protected bool canDeath = true;

                    public EventLivingCanDeath(EntityLiving entityLiving) : base(entityLiving) {
                    }

                    /// <summary>
                    /// 设置成不能死亡
                    /// </summary>
                    public void setCannotDeath() => canDeath = false;

                    public bool isCanDeath() => canDeath;
                    public override bool isContinue() => isCanDeath();
                }

                /// <summary>
                /// 实体进行死亡
                /// </summary>
                public class EventLivingDoDeath : EventLivingDeath {
                    public EventLivingDoDeath(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }

                /// <summary>
                /// 实体免疫了死亡
                /// </summary>
                public class EventLivingNoDeath : EventLivingDeath {
                    public EventLivingNoDeath(EntityLiving entityLiving) : base(entityLiving) {
                    }
                }
            }

            /// <summary>
            /// 和实体重生相关的
            /// _cancel为true表示已经有东西去重生了
            /// </summary>
            public class EventBackToLife : EventLiving {
                /*public EventBackToLife(EntityLiving entity) : base(entity) { }

                /// <summary>
                /// 实体将要重生
                /// </summary>
                public class EventWillBackToLife : EventBackToLife {
                    public EventWillBackToLife(EntityLiving entity) : base(entity) { }
                }

                /// <summary>
                /// 实体产生结束
                /// </summary>
                public class EventBackToLifeEnd : EventBackToLife {
                    public EventBackToLifeEnd(EntityLiving entity) : base(entity) { }
                }*/

                public EventBackToLife(EntityLiving entityLiving) : base(entityLiving) {
                }
            }

            /// <summary>
            /// 关于玩家任务
            /// </summary>
            public abstract class EventTask : EventLiving {
                public readonly TaskStack taskStack;
                public readonly TaskCell taskCell;

                public EventTask(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell) : base(entityLiving) {
                    this.taskStack = taskStack;
                    this.taskCell = taskCell;
                }

                /// <summary>
                /// 任务开始
                /// </summary>
                public class EventTaskStart : EventTask {
                    public EventTaskStart(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell) : base(entityLiving, taskStack, taskCell) {
                    }
                }

                /// <summary>
                /// 任务结束
                /// </summary>
                public class EventTaskEnd : EventTask {
                    public EventTaskEnd(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell) : base(entityLiving, taskStack, taskCell) {
                    }

                    /// <summary>
                    /// 任务完成
                    /// </summary>
                    public class EventTaskComplete : EventTaskEnd {
                        public EventTaskComplete(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell) : base(entityLiving, taskStack, taskCell) {
                        }
                    }

                    /// <summary>
                    /// 任务失败
                    /// </summary>
                    public class EventTaskFail : EventTaskEnd {
                        public EventTaskFail(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell) : base(entityLiving, taskStack, taskCell) {
                        }
                    }
                }

                /// <summary>
                /// 任务中组件
                /// </summary>
                public class EventTaskComponent : EventTask {
                    public readonly TaskComponentCell taskComponentCell;

                    public EventTaskComponent(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell) : base(entityLiving,
                        taskStack, taskCell) {
                        this.taskComponentCell = taskComponentCell;
                    }

                    /// <summary>
                    /// 获得新任务组件
                    /// </summary>
                    public class EventTaskComponentStart : EventTaskComponent {
                        public EventTaskComponentStart(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell) : base(
                            entityLiving, taskStack, taskCell, taskComponentCell) {
                        }
                    }

                    /// <summary>
                    /// 进度增加
                    /// </summary>
                    public class EventTaskComponentAdd : EventTaskComponent {
                        /// <summary>
                        /// 增加进度
                        /// </summary>
                        public readonly int addProgress;

                        /// <summary>
                        /// 之前进度
                        /// </summary>
                        public readonly int oldProgress;

                        public EventTaskComponentAdd(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell, int addProgress,
                            int oldProgress) : base(entityLiving, taskStack, taskCell, taskComponentCell) {
                            this.addProgress = addProgress;
                            this.oldProgress = oldProgress;
                        }
                    }

                    /// <summary>
                    /// 进度结束
                    /// </summary>
                    public class EventTaskComponentEnd : EventTaskComponent {
                        public EventTaskComponentEnd(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell) : base(
                            entityLiving, taskStack, taskCell, taskComponentCell) {
                        }

                        /// <summary>
                        /// 进度失败
                        /// </summary>
                        public class EventTaskComponentFail : EventTaskComponentEnd {
                            public EventTaskComponentFail(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell) : base(
                                entityLiving, taskStack, taskCell, taskComponentCell) {
                            }
                        }

                        /// <summary>
                        /// 完成的
                        /// </summary>
                        public class EventTaskComponentComplete : EventTaskComponentEnd {
                            public EventTaskComponentComplete(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskComponentCell taskComponentCell) : base(
                                entityLiving, taskStack, taskCell, taskComponentCell) {
                            }
                        }
                    }
                }

                /// <summary>
                /// 任务重失败的条件
                /// </summary>
                public class EventTaskFailCell : EventTask {
                    public readonly TaskFailCell taskFailCell;

                    public EventTaskFailCell(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskFailCell taskFailCell) : base(entityLiving, taskStack,
                        taskCell) {
                        this.taskFailCell = taskFailCell;
                    }

                    /// <summary>
                    /// 获得新任务组件
                    /// </summary>
                    public class EventTaskFailStart : EventTaskFailCell {
                        public EventTaskFailStart(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskFailCell taskFailCell) : base(entityLiving, taskStack,
                            taskCell, taskFailCell) {
                        }
                    }

                    /// <summary>
                    /// 进度增加
                    /// </summary>
                    public class EventTaskFailAdd : EventTaskFailCell {
                        /// <summary>
                        /// 增加进度
                        /// </summary>
                        public readonly int addProgress;

                        /// <summary>
                        /// 之前进度
                        /// </summary>
                        public readonly int oldProgress;

                        public EventTaskFailAdd(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskFailCell taskFailCell, int addProgress, int oldProgress)
                            : base(entityLiving, taskStack, taskCell, taskFailCell) {
                            this.addProgress = addProgress;
                            this.oldProgress = oldProgress;
                        }
                    }

                    /// <summary>
                    /// 触发任务失败
                    /// </summary>
                    public class EventTaskFailEnd : EventTaskFailCell {
                        public EventTaskFailEnd(EntityLiving entityLiving, TaskStack taskStack, TaskCell taskCell, TaskFailCell taskFailCell) : base(entityLiving, taskStack,
                            taskCell, taskFailCell) {
                        }
                    }
                }
            }

            /// <summary>
            /// 有背包的物品
            /// </summary>
            public abstract class EventEntityPack : EventLiving {
                public EventEntityPack(EntityLiving entityLiving) : base(entityLiving) {
                }

                /// <summary>
                /// 背包的初始化
                /// </summary>
                public abstract class EventEntityPackInit : EventEntityPack {
                    public EventEntityPackInit(EntityLiving entityLiving) : base(entityLiving) {
                    }

                    /// <summary>
                    /// 初始化背包
                    /// </summary>
                    public class EventEntityPackInitPack : EventEntityPackInit {
                        public EventEntityPackInitPack(EntityLiving entityLiving) : base(entityLiving) {
                        }
                    }

                    /// <summary>
                    /// 初始化装备
                    /// </summary>
                    public class EventEntityPackInitEquipment : EventEntityPackInit {
                        public EventEntityPackInitEquipment(EntityLiving entityLiving) : base(entityLiving) {
                        }
                    }
                }

                /// <summary>
                /// 背包
                /// </summary>
                public abstract class EventPack : EventEntityPack {
                    public readonly ItemStack itemStack;

                    public EventPack(EntityLiving entityLiving, ItemStack itemStack) : base(entityLiving) {
                        this.itemStack = itemStack;
                    }

                    /// <summary>
                    /// 当获得物品
                    /// </summary>
                    public class EventPackAddItem : EventPack {
                        public EventPackAddItem(EntityLiving entityLiving, ItemStack itemStack) : base(
                            entityLiving, itemStack) {
                        }

                        /// <summary>
                        /// 在初始化时添加物品
                        /// </summary>
                        public class EventPackInitAddItem : EventPackAddItem {
                            public EventPackInitAddItem(EntityLiving entityLiving, ItemStack itemStack) : base(entityLiving, itemStack) {
                            }
                        }
                    }

                    /// <summary>
                    /// 失去某东西时
                    /// </summary>
                    public abstract class EventPackLose : EventPack {
                        public EventPackLose(EntityLiving entityLiving, ItemStack itemStack) : base(entityLiving, itemStack) {
                        }

                        /// <summary>
                        /// 当删除物品
                        /// </summary>
                        public class EventPackDelete : EventPackLose {
                            public EventPackDelete(EntityLiving entityLiving, ItemStack itemStack) : base(
                                entityLiving, itemStack) {
                            }
                        }

                        /// <summary>
                        /// 丢弃物品
                        /// </summary>
                        public class EventPackDiscard : EventPackLose {
                            public EventPackDiscard(EntityLiving entityLiving, ItemStack itemStack) : base(
                                entityLiving, itemStack) {
                            }
                        }

                        /// <summary>
                        /// 转移物品
                        /// 当物品转移到使用背包中时
                        /// </summary>
                        public class EventPackTransfer : EventPackLose {
                            public EventPackTransfer(EntityLiving entityLiving, ItemStack itemStack) : base(
                                entityLiving, itemStack) {
                            }
                        }

                        /// <summary>
                        /// 物品被消耗
                        /// </summary>
                        public class EventPackConsume : EventPackLose {
                            public EventPackConsume(EntityLiving entityLiving, ItemStack itemStack) : base(
                                entityLiving, itemStack) {
                            }
                        }
                    }
                }

                /// <summary>
                /// 装备背包
                /// </summary>
                public class EventEquipment : EventEntityPack {
                    public readonly ItemStack itemStack;
                    public readonly EquipmentType equipmentType;

                    public EventEquipment(EntityLiving entityLiving, ItemStack itemStack, EquipmentType equipmentType) : base(entityLiving) {
                        this.itemStack = itemStack;
                        this.equipmentType = equipmentType;
                    }

                    /// <summary>
                    /// 物品启用
                    /// </summary>
                    public class EventEquipmentUse : EventEquipment {
                        public EventEquipmentUse(EntityLiving entityLiving, ItemStack itemStack, EquipmentType equipmentType) : base(entityLiving, itemStack,
                            equipmentType) {
                        }

                        /// <summary>
                        /// 初始化时启用
                        /// </summary>
                        public class EventEquipmentInitUse : EventEquipmentUse {
                            public EventEquipmentInitUse(EntityLiving entityLiving, ItemStack itemStack, EquipmentType equipmentType) : base(entityLiving, itemStack,
                                equipmentType) {
                            }
                        }
                    }

                    /// <summary>
                    /// 物品停用
                    /// 返回主物品栏
                    /// </summary>
                    /// 
                    public class EventEquipmentEnd : EventEquipment {
                        public EventEquipmentEnd(EntityLiving entityLiving, ItemStack itemStack,
                            EquipmentType equipmentType) : base(entityLiving, itemStack,
                            equipmentType) {
                        }
                    }
                }
            }

            /// <summary>
            /// 有关技能的
            /// </summary>
            public abstract class EventSkill : EventLiving {
                public EventSkill(EntityLiving entityLiving) : base(entityLiving) {
                }

                /// <summary>
                /// 技能元素
                /// </summary>
                public abstract class EventSkillCell : EventSkill {
                    public readonly SkillBasics skill;
                    public readonly SkillCell skillCell;

                    public EventSkillCell(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) : base(
                        entityLiving) {
                        this.skillCell = skillCell;
                        this.skill = skill;
                    }

                    /// <summary>
                    /// 获得技能时
                    /// </summary>
                    public class EventSkillGet : EventSkillCell {
                        public EventSkillGet(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) : base(
                            entityLiving, skill, skillCell) {
                        }

                        /// <summary>
                        /// 技能在初始化时发布
                        /// </summary>
                        public class EventSkillInit : EventSkillGet {
                            public EventSkillInit(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) : base(entityLiving, skill, skillCell) {
                            }
                        }
                    }

                    /// <summary>
                    /// 失去技能时
                    /// </summary>
                    public class EventSkillLose : EventSkillCell {
                        public EventSkillLose(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) : base(
                            entityLiving, skill, skillCell) {
                        }
                    }

                    /// <summary>
                    /// 当使用技能的时候
                    /// </summary>
                    public class EventSkillUse : EventSkillCell {
                        public EventSkillUse(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) : base(
                            entityLiving, skill, skillCell) {
                        }
                    }

                    /// <summary>
                    /// 设置新CD
                    /// </summary>
                    public class EventSkillSetNewCd : EventSkillCell {
                        public EventSkillSetNewCd(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) :
                            base(entityLiving, skill, skillCell) {
                        }
                    }

                    /// <summary>
                    /// 没有CD时
                    /// </summary>
                    public class EventSkillToNoCd : EventSkillCell {
                        public EventSkillToNoCd(EntityLiving entityLiving, SkillBasics skill, SkillCell skillCell) :
                            base(entityLiving, skill, skillCell) {
                        }
                    }
                }

                /// <summary>
                /// 收集技能属性
                /// </summary>
                public class EventLivingCatchSkill : EventLiving {
                    protected readonly Dictionary<SkillBasics, SkillCell> newSkill;

                    public EventLivingCatchSkill(EntityLiving entity) : base(entity) {
                        newSkill = new Dictionary<SkillBasics, SkillCell>();
                        foreach (var keyValuePair in entity.forSkill()) {
                            SkillCell skillCell = keyValuePair.Value;
                            skillCell.oldLevel = skillCell.level;
                            skillCell.level = keyValuePair.Value.originalLevel;
                            skillCell.isEventChange = true;
                            newSkill.Add(keyValuePair.Key, keyValuePair.Value);
                        }
                    }

                    public void addSkill(SkillBasics _skill, double i) => newSkill.get(_skill, () => {
                        SkillCell skillCell = new SkillCell {
                            isNew = true
                        };
                        return skillCell;
                    }).level += i;

                    public IEnumerable<KeyValuePair<SkillBasics, SkillCell>> endEvent() => newSkill;
                }
            }

            /// <summary>
            /// 实体周期刷新
            /// </summary>
            public abstract class EventLivingCycle : EventLiving {
                public readonly float time;

                public EventLivingCycle(EntityLiving entityLiving, float time) : base(entityLiving) {
                    this.time = time;
                }

                /// <summary>
                /// 间隔1秒
                /// </summary>
                public class EventCycle_1s : EventLivingCycle {
                    public EventCycle_1s(EntityLiving entityLiving) : base(entityLiving, 1) {
                    }
                }

                /// <summary>
                /// 间隔3秒
                /// </summary>
                public class EventCycle_3s : EventLivingCycle {
                    public EventCycle_3s(EntityLiving entityLiving) : base(entityLiving, 3) {
                    }
                }

                /// <summary>
                /// 间隔20秒
                /// </summary>
                public class EventCycle_5s : EventLivingCycle {
                    public EventCycle_5s(EntityLiving entityLiving) : base(entityLiving, 20) {
                    }
                }

                /// <summary>
                /// 间隔30秒
                /// </summary>
                public class EventCycle_20s : EventLivingCycle {
                    public EventCycle_20s(EntityLiving entityLiving) : base(entityLiving, 30) {
                    }
                }
            }

            /// <summary>
            /// 实体动画
            /// </summary>
            public abstract class EventLivingAnimator : EventLiving {
                /// <summary>
                /// 动画
                /// </summary>
                public readonly AnimationPlayableBehaviour animationPlayableBehaviour;

                public EventLivingAnimator(EntityLiving entityLiving, AnimationPlayableBehaviour animationPlayableBehaviour) : base(entityLiving) {
                    this.animationPlayableBehaviour = animationPlayableBehaviour;
                }

                /// <summary>
                /// 开始一个动画
                /// </summary>
                public class EventLivingStartAnimation : EventLivingAnimator {
                    public EventLivingStartAnimation(EntityLiving entityLiving, AnimationPlayableBehaviour animationPlayableBehaviour) : base(entityLiving,
                        animationPlayableBehaviour) {
                    }
                }

                /// <summary>
                /// 失去某动画
                /// </summary>
                public class EventLivingEndAnimation : EventLivingAnimator {
                    public EventLivingEndAnimation(EntityLiving entityLiving, AnimationPlayableBehaviour animationPlayableBehaviour) : base(entityLiving,
                        animationPlayableBehaviour) {
                    }
                }

                /// <summary>
                /// 选择下一个动画
                /// </summary>
                public class EventSettlementNextAnimation : EventLivingAnimator {
                    /// <summary>
                    /// 下一段动画 
                    /// </summary>
                    protected AnimationPlayableBehaviour? next;

                    public EventSettlementNextAnimation(EntityLiving entityLiving, AnimationPlayableBehaviour animationPlayableBehaviour) : base(entityLiving,
                        animationPlayableBehaviour) {
                    }

                    public override bool isContinue() => next is null;

                    public AnimationPlayableBehaviour? getNextAnimation() => next;

                    public void setNextAnimationPlayableBehaviour(AnimationPlayableBehaviour _animationPlayableBehaviour) => next = _animationPlayableBehaviour;
                }
            }

            /// <summary>
            /// 实体状态
            /// </summary>
            public abstract class EventLivingState : EventLiving {
                public readonly EntityState entityState;

                public EventLivingState(EntityLiving entityLiving, EntityState entityState) : base(entityLiving) {
                    this.entityState = entityState;
                }

                /// <summary>
                /// 开始一个状态
                /// </summary>
                public class EventLivingStartState : EventLivingState {
                    public EventLivingStartState(EntityLiving entityLiving, EntityState entityState) : base(entityLiving, entityState) {
                    }
                }

                /// <summary>
                /// 失去某状态
                /// </summary>
                public class EventLivingEndState : EventLivingState {
                    public EventLivingEndState(EntityLiving entityLiving, EntityState entityState) : base(entityLiving, entityState) {
                    }
                }
            }

            /// <summary>
            /// 玩家阵营发生变动
            /// </summary>
            public abstract class EventLivingCamp : EventLiving {
                public readonly Camp camp;

                public EventLivingCamp(EntityLiving entityLiving, Camp camp) : base(entityLiving) {
                    this.camp = camp;
                }

                /// <summary>
                /// 进入某阵营
                /// </summary>
                public class EventLivingStartCamp : EventLivingCamp {
                    public EventLivingStartCamp(EntityLiving entityLiving, Camp camp) : base(entityLiving, camp) {
                    }

                    /// <summary>
                    /// 初始化阵营
                    /// </summary>
                    public class EventLivingInitCamp : EventLivingCamp {
                        public EventLivingInitCamp(EntityLiving entityLiving, Camp camp) : base(entityLiving, camp) {
                        }
                    }
                }

                /// <summary>
                /// 不再是某阵营
                /// </summary>
                public class EventLivingEndCamp : EventLivingCamp {
                    public EventLivingEndCamp(EntityLiving entityLiving, Camp camp) : base(entityLiving, camp) {
                    }
                }
            }

            /// <summary>
            /// 实体类型变换
            /// </summary>
            public abstract class EventLivingType : EventLiving {
                public readonly EntityType entityType;

                public EventLivingType(EntityLiving entityLiving, EntityType entityType) : base(entityLiving) {
                    this.entityType = entityType;
                }

                /// <summary>
                /// 获得一个类型
                /// </summary>
                public class EventLivingStartType : EventLivingType {
                    public EventLivingStartType(EntityLiving entityLiving, EntityType entityType) : base(entityLiving, entityType) {
                    }

                    /// <summary>
                    /// 初始化类型
                    /// </summary>
                    public class EventLivingInitType : EventLivingType {
                        public EventLivingInitType(EntityLiving entityLiving, EntityType entityType) : base(entityLiving, entityType) {
                        }
                    }
                }

                /// <summary>
                /// 失去某类型
                /// </summary>
                public class EventLivingEndType : EventLivingType {
                    public EventLivingEndType(EntityLiving entityLiving, EntityType entityType) : base(entityLiving, entityType) {
                    }
                }
            }
        }
    }

    /// <summary>
    /// 世界
    /// </summary>
    public abstract class EventWorld : Event {
        public abstract class EventWorldInit : EventWorld {
            /// <summary>
            /// 初始化开始
            /// </summary>
            public class EventWorldInitStart : EventWorldInit {
            }

            /// <summary>
            /// 初始化结束
            /// </summary>
            public class EventWorldInitEnd : EventWorldInit {
            }

            /// <summary>
            /// 世界组件的初始化
            /// </summary>
            public class EventComponentInitBasics<T> : EventWorldInit where T : IWorldComponent {
                public readonly T component;

                public EventComponentInitBasics(T component) {
                    this.component = component;
                }

                /// <summary>
                /// 组件的初始化
                /// </summary>
                public class EventComponentInit : EventComponentInitBasics<T> {
                    public EventComponentInit(T component) : base(component) {
                    }
                }

                /// <summary>
                /// 第一次
                /// </summary>
                public class EventComponentInitBack : EventComponentInitBasics<T> {
                    public EventComponentInitBack(T component) : base(component) {
                    }
                }

                /// <summary>
                /// 第二次
                /// </summary>
                public class EventComponentInitBackToBack : EventComponentInitBasics<T> {
                    public EventComponentInitBackToBack(T component) : base(component) {
                    }
                }
            }
        }

        /// <summary>
        /// 世界更新
        /// </summary>
        public class FixedUpdate : EventWorld {
            public readonly float fixedDeltaTime;
            public readonly float unscaledFixedDeltaTime;

            public FixedUpdate(float fixedDeltaTime) {
                this.fixedDeltaTime = fixedDeltaTime;
                this.unscaledFixedDeltaTime = fixedDeltaTime / (float)Engine.TimeScale;
            }
        }

        /// <summary>
        /// 世界更新，每一帧都在进行
        /// </summary>
        public class Update : EventWorld {
            public readonly float deltaTime;
            public readonly float unscaledDdeltaDeltaTime;

            public Update(float deltaTime) {
                this.deltaTime = deltaTime;
                this.unscaledDdeltaDeltaTime = deltaTime / (float)Engine.TimeScale;
            }
        }
    }
}