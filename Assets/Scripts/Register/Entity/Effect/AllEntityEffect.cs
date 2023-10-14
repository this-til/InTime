using RegisterSystem;

namespace InTime.Effect; 

public class AllEntityEffect : CanConfigRegisterManage<EntityEffectBasics> {
    
}

public class EntityEffectBasics : RegisterBasics, IDefaultConfig {
        [SerializeField] protected ArrayList<PosEffectCell> effects;
        [CanBeNull] protected EffectCellEventPack effectCellEventPack;

        public override void init(ReflexAssetsManage reflexAssetsManage) {
            base.init(reflexAssetsManage);
            if (effects.isEmpty()) {
                return;
            }
            effectCellEventPack = new EffectCellEventPack(this);
        }

        /// <summary>
        /// 从注册对象获得当前buff的性质，用于玩些骚操作
        /// </summary>
        /// <param name="effectCell"></param>
        /// <returns></returns>
        public virtual Nature getNature(EntityEffectCell effectCell) {
            return Nature.neutral;
        }

        /// <summary>
        /// 获取buff的流逝时间类型
        /// </summary>
        public virtual ScaleManage.TimeType getEffectTimeType() => ScaleManage.TimeType.part;

        public virtual EntityEffectCell fuse(EntityEffectCell old, EntityEffectCell @new) {
            double oldMagnitude = old.level * old.time;
            double newMagnitude = @new.level * @new.time;
            return oldMagnitude > newMagnitude ? old : @new;
        }

        protected virtual void onEventLivingAddEffect(EntityLiving entityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
        }

        protected virtual void onEventLivingAddEffect_effect(EntityLiving entityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
            if (AllSetBool.不显示buff特效.isOpen()) {
                return;
            }
            if (effectCellEventPack is null) {
                return;
            }
            foreach (var kv in effectCellEventPack.effectCellMap) {
                kv.Value.create(@event.entityLiving, kv.Key);
            }
        }

        protected virtual void onEventLivingClearEffect(EntityLiving entityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
        }

        protected virtual void onEventLivingClearEffect_effect(EntityLiving entityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
            if (effectCellEventPack is null) {
                return;
            }
            foreach (var kv in effectCellEventPack.effectCellMap) {
                if (!kv.Value.isSonEntity()) {
                    return;
                }
                @event.entityLiving.clearSonEntity(kv.Key);
            }
        }

        protected virtual void onEventCatchAttribute(EntityLiving entityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
        }

        protected virtual void onEventLivingCatchSkill(EntityLiving eventEntityLiving, EntityEffectCell effectCell,
            Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntity">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, [CanBeNull] Entity attackerEntity, EntityEffectCell entityEffectCell,
            AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntity">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEquipment_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, EntityEffectCell entityEffectCell,
            AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntityLiving">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEquipment_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, EntityEffectCell entityEffectCell,
            AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntity">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, [CanBeNull] Entity attackerEntity, EntityEffectCell entityEffectCell,
            AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntity">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEnd_underAttack(EntityLiving entityLiving, EntityLiving attackerEntity, EntityEffectCell entityEffectCell, AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
        }

        /// <summary>
        /// 攻击事件拦截
        /// </summary>
        /// <param name="attackerEntityLiving">发动攻击的实体</param>
        /// <param name="entityLiving">被攻击的实体</param>
        protected virtual void onEventAttackEnd_attack(EntityLiving attackerEntityLiving, EntityLiving entityLiving, EntityEffectCell entityEffectCell, AttackStack attackStack,
            Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingAddEffect @event) {
            @event.effect.onEventLivingAddEffect_effect(@event.entityLiving, @event.effectCell, @event);
            @event.effect.onEventLivingAddEffect(@event.entityLiving, @event.effectCell, @event);
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventLivingEffect.EventLivingClearEffect @event) {
            @event.effect.onEventLivingClearEffect_effect(@event.entityLiving, @event.effectCell, @event);
            @event.effect.onEventLivingClearEffect(@event.entityLiving, @event.effectCell, @event);
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
            foreach (var keyValuePair in @event.entityLiving.forEffect()) {
                if (keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventCatchAttribute(@event.entityLiving, keyValuePair.Value, @event);
            }
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
            foreach (var keyValuePair in @event.entityLiving.forEffect()) {
                if (keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventLivingCatchSkill(@event.entityLiving, keyValuePair.Value, @event);
            }
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEquipment @event) {
            Entity entity = @event.stack.getEntity<Entity>();
            EntityLiving attackerEntityLiving = entity as EntityLiving;
            foreach (var keyValuePair in @event.entityLiving.forEffect()) {
                if (!keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
                if (attackerEntityLiving is not null) {
                    keyValuePair.Key.onEventAttackEquipment_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
                }
            }
            if (attackerEntityLiving is not null) {
                foreach (var keyValuePair in attackerEntityLiving.forEffect()) {
                    if (!keyValuePair.Value.isEmpty()) {
                        continue;
                    }
                    keyValuePair.Key.onEventAttackEquipment_attack(attackerEntityLiving, @event.entityLiving, keyValuePair.Value, @event.stack, @event);
                }
            }
        }

        public static void onEvent(Event.EventEntity.EventLiving.EventAttack.EventAttackEnd @event) {
            Entity entity = @event.stack.getEntity<Entity>();
            EntityLiving attackerEntityLiving = entity as EntityLiving;
            foreach (var keyValuePair in @event.entityLiving.forEffect()) {
                if (!keyValuePair.Value.isEmpty()) {
                    continue;
                }
                keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, entity, keyValuePair.Value, @event.stack, @event);
                if (attackerEntityLiving is not null) {
                    keyValuePair.Key.onEventAttackEnd_underAttack(@event.entityLiving, attackerEntityLiving, keyValuePair.Value, @event.stack, @event);
                }
            }
            if (attackerEntityLiving is not null) {
                foreach (var keyValuePair in attackerEntityLiving.forEffect()) {
                    if (!keyValuePair.Value.isEmpty()) {
                        continue;
                    }
                    keyValuePair.Key.onEventAttackEnd_attack(attackerEntityLiving, @event.entityLiving, keyValuePair.Value, @event.stack, @event);
                }
            }
        }

        public class EffectCellEventPack {
            public readonly EntityEffectBasics entityEffectBasics;
            public readonly Map<string, PosEffectCell> effectCellMap;

            public EffectCellEventPack(EntityEffectBasics entityEffectBasics) {
                this.entityEffectBasics = entityEffectBasics;
                effectCellMap = new Map<string, PosEffectCell>(entityEffectBasics.effects.Count);
                for (var i = 0; i < entityEffectBasics.effects.Count; i++) {
                    effectCellMap.Add($"effect.{entityEffectBasics.getName()}.{i}", entityEffectBasics.effects[i]);
                }
            }
        }
    }