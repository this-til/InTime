using System;
using RegisterSystem;

namespace InTime;

public class AllAttributeControl : RegisterManage<AttributeControl> {
    /// <summary>
    /// 生命
    /// </summary>
    public static AttributeControl life;

    /// <summary>
    /// 元素力
    /// </summary>
    public static AttributeControl mana;

    /// <summary>
    /// 韧性
    /// </summary>
    public static AttributeControl tenacity;

    public override Type getBasicsRegisterManageType() => typeof(AllAttribute);
}

public class AttributeControl : Attribute {
    [FieldRegister] protected AttributeLimit attributeLimit;

    public AttributeLimit getLimitAttribute() => attributeLimit;
}