using System;
using System.Text.Json.Serialization;
using RegisterSystem;

namespace InTime;

public class AllAttributeChangeLimit : RegisterManage<AttributeChangeLimit> {
    /// <summary>
    /// 治疗加成
    /// </summary>
    public static AttributeChangeLimit lifeRecoveryAdd;

    /// <summary>
    /// 攻击减免
    /// </summary>
    public static AttributeChangeLimit attackReduce;

    /// <summary>
    /// 韧性减免
    /// </summary>
    public static AttributeChangeLimit tenacityReduce;

    public override Type getBasicsRegisterManageType() => typeof(AllAttribute);

    public override void init() {
        base.init();
        lifeRecoveryAdd.attributeLimit = AllAttributeControl.life.getLimitAttribute();
        lifeRecoveryAdd.attributeChangeType = AttributeChangeType.add;
        lifeRecoveryAdd.isAttenuation = false;

        attackReduce.attributeLimit = AllAttributeControl.life.getLimitAttribute();
        attackReduce.attributeChangeType = AttributeChangeType.reduce;
        attackReduce.isAttenuation = true;

        tenacityReduce.attributeLimit = AllAttributeControl.tenacity.getLimitAttribute();
        tenacityReduce.attributeChangeType = AttributeChangeType.reduce;
        tenacityReduce.isAttenuation = true;
    }
}

public class AttributeChangeLimit : Attribute {
    protected internal AttributeLimit attributeLimit;
    protected internal AttributeChangeType attributeChangeType;

    /// <summary>
    /// 选择对值操作的时候是增益还是衰减
    /// </summary>
    protected internal bool isAttenuation;

    public virtual void onEvent_eventAttributeLimit(EntityLiving entityLiving, double value, Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit @event) {
        if (!@event.attributeChangeType.Equals(attributeChangeType)) {
            return;
        }
        if (!@event.attributeLimit.Equals(attributeLimit)) {
            return;
        }
        @event.addMultiply(AllMultiple.attribute, isAttenuation ? -value : value);
    }

    public static void onEvent(Event.EventEntity.EventLiving.EventAttribute.EventAttributeLimit @event) {
        foreach (var keyValuePair in @event.entityLiving.forAttribute()) {
            if (keyValuePair.Key is not AttributeChangeLimit attributeChangeLimit) {
                continue;
            }
            double value = @event.entityLiving.get(attributeChangeLimit);
            if (value <= 0) {
                return;
            }
            attributeChangeLimit.onEvent_eventAttributeLimit(@event.entityLiving, value, @event);
        }
    }
}
