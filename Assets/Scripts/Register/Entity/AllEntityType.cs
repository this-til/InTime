using RegisterSystem;

namespace InTime;

/// <summary>
/// 实体类型（寿衣）
/// </summary>
public class AllEntityType : RegisterManage<EntityType> {
    /// <summary>
    /// 生物
    /// </summary>
    public static EntityType living;

    /// <summary>
    /// 机械
    /// </summary>
    public static EntityType machinery;

    /// <summary>
    /// 英雄
    /// </summary>
    public static EntityType hero;

    /// <summary>
    /// 灵能
    /// </summary>
    public static EntityType mana;

    /// <summary>
    /// 轻甲
    /// </summary>
    public static EntityType lightArmour;

    /// <summary>
    /// 重甲
    /// </summary>
    public static EntityType heavyArmour;
}

public class EntityType : RegisterBasics {
}