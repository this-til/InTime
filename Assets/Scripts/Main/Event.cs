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
        /// 实体获得新的事件缩放
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
    }
}