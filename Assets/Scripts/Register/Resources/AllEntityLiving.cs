using System;

namespace InTime;

public class AllEntityLiving : ResourcesRegisterManage<EntityLiving> {

    public static ResourcesRegister<EntityLiving> player;
    
    public override Type getBasicsRegisterManageType() {
        return typeof(AllEntity);
    }
}