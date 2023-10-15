using System;

namespace InTime;

public class AllControlEntityEffect : CanConfigRegisterManage<ControlEntityEffect> {
    public override Type getBasicsRegisterManageType() => typeof(AllEntityEffect);
}

public class ControlEntityEffect : EntityEffectBasics {
}