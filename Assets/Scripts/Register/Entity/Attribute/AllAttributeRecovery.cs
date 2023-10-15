using System;
using RegisterSystem;

namespace InTime;

public class AllAttributeRecovery : RegisterManage<AttributeRecovery> {
    public static AttributeRecovery lifeRecovery;
    public static AttributeRecovery manaRecovery;
    public static AttributeRecovery tenacityRecovery;

    public override Type getBasicsRegisterManageType() => typeof(AllAttribute);

    public override void init() {
        base.init();

        lifeRecovery.attributeControl = AllAttributeControl.life;
        lifeRecovery.attributeOperationType = AttributeOperationType.add;
        lifeRecovery.recoveryTime = 1;

        manaRecovery.attributeControl = AllAttributeControl.mana;
        manaRecovery.attributeOperationType = AttributeOperationType.add;
        manaRecovery.recoveryTime = 1;

        tenacityRecovery.attributeControl = AllAttributeControl.tenacity;
        tenacityRecovery.attributeOperationType = AttributeOperationType.add;
        tenacityRecovery.recoveryTime = 1;
    }
}

public class AttributeRecovery : Attribute {
    protected internal AttributeControl attributeControl;
    protected internal AttributeOperationType attributeOperationType;
    protected internal float recoveryTime;
}