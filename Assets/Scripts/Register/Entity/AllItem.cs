using Newtonsoft.Json.Linq;
using RegisterSystem;

namespace InTime;

public class AllItem : CanConfigRegisterManage<ItemBasic> {
}

public class ItemBasic : RegisterBasics, IDefaultConfig {
	/// <summary>
	/// 用于UI渲染的图像
	/// </summary>
	//[SerializeField] protected Sprite sprite;
	public void defaultConfig() {
	}

	/// <summary>
	/// 创建一个空栈
	/// </summary>
	public virtual void initStack(ItemStack itemStack) {
	}

	/// <summary>
	/// 根据物品栈获取物品的图标
	/// </summary>
	//public virtual Sprite getItemSprite(ItemStack itemStack) => sprite;

	/// <summary>
	/// 使用物品
	/// </summary>
	public virtual void use(EntityLiving entityLiving, ItemStack itemStack, EquipmentType equipmentType) {
	}

	/// <summary>
	/// 当物品在背包中被更新
	/// </summary>
	public virtual void fixedUpdate(EntityLiving entityLiving, ItemStack itemStack, EquipmentType? equipmentType) {
	}

	protected virtual void onEventCatchAttribute(EntityLiving entityLiving, ItemStack itemStack,
		Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
	}

	protected virtual void onEventCatchAttribute_equipment(EntityLiving entityLiving, ItemStack itemStack, EquipmentType equipmentType,
		Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
	}

	protected virtual void onEventLivingCatchSkill(EntityLiving eventEntityLiving, ItemStack itemStack,
		Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
	}

	protected virtual void onEventLivingCatchSkill_equipment(EntityLiving eventEntityLiving, ItemStack itemStack, EquipmentType equipmentType,
		Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
	}

	public static void onEvent(Event.EventEntity.EventLiving.EventAttribute.EventCatchAttribute @event) {
		foreach (var itemStack in @event.entityLiving.forItem()) {
			itemStack.getItem().onEventCatchAttribute(@event.entityLiving, itemStack, @event);
		}
		foreach (var keyValuePair in @event.entityLiving.forEquipment()) {
			keyValuePair.Value.getItem().onEventCatchAttribute(@event.entityLiving, keyValuePair.Value, @event);
			keyValuePair.Value.getItem().onEventCatchAttribute_equipment(@event.entityLiving, keyValuePair.Value, keyValuePair.Key, @event);
		}
	}

	public static void onEvent(Event.EventEntity.EventLiving.EventSkill.EventLivingCatchSkill @event) {
		foreach (var itemStack in @event.entityLiving.forItem()) {
			itemStack.getItem().onEventLivingCatchSkill(@event.entityLiving, itemStack, @event);
		}
		foreach (var keyValuePair in @event.entityLiving.forEquipment()) {
			keyValuePair.Value.getItem().onEventLivingCatchSkill(@event.entityLiving, keyValuePair.Value, @event);
			keyValuePair.Value.getItem().onEventLivingCatchSkill_equipment(@event.entityLiving, keyValuePair.Value, keyValuePair.Key, @event);
		}
	}
}

public class ItemStack {
	protected ItemBasic item;
	protected JObject? customData;

	public ItemStack() {
	}

	public ItemStack(ItemStack itemStack) : this(itemStack.item, itemStack.customData) {
	}

	public ItemStack(ItemBasic item, JObject? customData = null) {
		this.item = item;
		this.customData = customData;
		item.initStack(this);
	}

	public ItemBasic getItem() => item;

	public JObject getCustomData() => customData ??= new JObject();
}
