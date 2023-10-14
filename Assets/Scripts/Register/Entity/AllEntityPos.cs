using RegisterSystem;

namespace InTime;

public class AllEntityPos : RegisterManage<EntityPos> {
    public static EntityPos standPos;
    public static EntityPos headPos;
    public static EntityPos centerPos;
    public static EntityPos leftHand;
    public static EntityPos rightHand;
}

public class EntityPos : RegisterBasics {
}