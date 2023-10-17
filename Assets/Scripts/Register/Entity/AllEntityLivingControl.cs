using Godot;
using RegisterSystem;

namespace InTime;

public class AllEntityLivingControl : RegisterManage<EntityLivingControl> {
	public static PlayerEntityLivingControl playerEntityLivingControl;
}

public abstract class EntityLivingControl : RegisterBasics {
	public abstract void writeIn(EntityLiving entityLiving, EntityLiving.EntityInput entityInput);
	public abstract void balance(EntityLiving entityLiving, EntityLiving.EntityInput entityInput);
}

public class PlayerEntityLivingControl : EntityLivingControl {
	public override void writeIn(EntityLiving entityLiving, EntityLiving.EntityInput entityInput) {
		/*if (AllKeyPack.W.isDown(DownType.isDown)) {
			entityInput.addInputOption(EntityLiving.InputOption.w);
		}
		if (AllKeyPack.S.isDown(DownType.isDown)) {
			entityInput.addInputOption(EntityLiving.InputOption.s);
		}
		if (AllKeyPack.A.isDown(DownType.isDown)) {
			entityInput.addInputOption(EntityLiving.InputOption.a);
		}
		if (AllKeyPack.D.isDown(DownType.isDown)) {
			entityInput.addInputOption(EntityLiving.InputOption.d);
		}
		if (AllKeyPack.attack_1.isDown(DownType.doubleHit)) {
			entityInput.addInputOption(EntityLiving.InputOption.tap);
		}
		if (AllKeyPack.attack_1.isDown(DownType.always)) {
			entityInput.addInputOption(EntityLiving.InputOption.tapLong);
		}
		if (AllKeyPack.attack_2.isDown(DownType.doubleHit)) {
			entityInput.addInputOption(EntityLiving.InputOption.thump);
		}
		if (AllKeyPack.attack_2.isDown(DownType.always)) {
			entityInput.addInputOption(EntityLiving.InputOption.thumpLong);
		}
		if (AllKeyPack.dodge.isDown(DownType.doubleHit)) {
			entityInput.addInputOption(EntityLiving.InputOption.dodge);
		}
		if (AllKeyPack.dodge.isDown(DownType.always)) {
			entityInput.addInputOption(EntityLiving.InputOption.dodgeLong);
		}
		if (AllKeyPack.jump.isDown(DownType.doubleHit)) {
			entityInput.addInputOption(EntityLiving.InputOption.jump);
		}
		if (AllKeyPack.jump.isDown(DownType.always)) {
			entityInput.addInputOption(EntityLiving.InputOption.jumpLong);
		}*/
	}

	public override void balance(EntityLiving entityLiving, EntityLiving.EntityInput entityInput) {
		//TODO
		/*entityInput.targetRotation = Mathf.Atan2(entityInput.getMove().X, entityInput.getMove().Y) * Mathf.Rad2Deg +
									 World.getWorld().getWorldRun().getCameraManage().getCamera().transform.eulerAngles.y;*/
	}
}
