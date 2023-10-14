using System;

namespace InTime.Effect;

public class AllControlEntityEffect : CanConfigRegisterManage<ControlEntityEffect> {
    public override Type getBasicsRegisterManageType() => typeof(AllEntityEffect);
}

public class ControlEntityEffect : EntityEffectBasics {
}