using System;

namespace InTime;

public class AllControlEntityEffect : CanConfigRegisterManage<ControlEntityEffect> {
    protected static FrozenEntityEffect frozenEntityEffect;
    protected static ImprisonEntityEffect imprisonEntityEffect;
    protected static VoidEntityEffect voidEntityEffect;

    
    
    public override Type getBasicsRegisterManageType() => typeof(AllEntityEffect);
}

public class ControlEntityEffect : EntityEffectBasics {
}

public class FrozenEntityEffect : ControlEntityEffect {
}

public class ImprisonEntityEffect : ControlEntityEffect {
    public override void awakeInit() {
        base.awakeInit();
        timeType = TimeType.world;
    }
}

public class VoidEntityEffect : ControlEntityEffect {
}