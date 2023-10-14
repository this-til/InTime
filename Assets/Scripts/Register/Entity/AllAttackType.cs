using RegisterSystem;

namespace InTime; 

public class AllAttackType : RegisterManage<AttackType> {
    /// <summary>
    /// 主动攻击，由实体主动发起，用来过滤伤害
    /// </summary>
    public static AttackType main;

    /// <summary>
    /// 暴击伤害
    /// </summary>
    public static AttackType criticalHit;

    /// <summary>
    /// 免疫护盾
    /// </summary>
    public static AttackType_WearShieldAttack immuneShield;

    /// <summary>
    /// 免疫防御
    /// </summary>
    public static AttackType immuneDefense;

    /// <summary>
    /// 轻击
    /// </summary>
    public static AttackType_Tap tap;

    /// <summary>
    /// 重击
    /// </summary>
    public static AttackType_Thump thump;

    /// <summary>
    /// 技能
    /// </summary>
    public static AttackType skill;
}

public class AttackType : RegisterBasics {
    
}