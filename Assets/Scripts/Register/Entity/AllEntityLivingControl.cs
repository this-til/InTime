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
        if (AllKeyPack.W.isDown(DownType.isDown)) {
            entityInput.addInputOption(InputOption.w);
        }
        if (AllKeyPack.S.isDown(DownType.isDown)) {
            entityInput.addInputOption(InputOption.s);
        }
        if (AllKeyPack.A.isDown(DownType.isDown)) {
            entityInput.addInputOption(InputOption.a);
        }
        if (AllKeyPack.D.isDown(DownType.isDown)) {
            entityInput.addInputOption(InputOption.d);
        }
        if (AllKeyPack.attackTap.isDown(DownType.doubleHit)) {
            entityInput.addInputOption(InputOption.tap);
        }
        if (AllKeyPack.attackTap.isDown(DownType.always)) {
            entityInput.addInputOption(InputOption.tapLong);
        }
        if (AllKeyPack.attackThump.isDown(DownType.doubleHit)) {
            entityInput.addInputOption(InputOption.thump);
        }
        if (AllKeyPack.attackThump.isDown(DownType.always)) {
            entityInput.addInputOption(InputOption.thumpLong);
        }
        if (AllKeyPack.dodge.isDown(DownType.doubleHit)) {
            entityInput.addInputOption(InputOption.dodge);
        }
        if (AllKeyPack.dodge.isDown(DownType.always)) {
            entityInput.addInputOption(InputOption.dodgeLong);
        }
        if (AllKeyPack.jump.isDown(DownType.doubleHit)) {
            entityInput.addInputOption(InputOption.jump);
        }
        if (AllKeyPack.jump.isDown(DownType.always)) {
            entityInput.addInputOption(InputOption.jumpLong);
        }
    }

    public override void balance(EntityLiving entityLiving, EntityLiving.EntityInput entityInput) {
        //TODO
        /*entityInput.targetRotation = Mathf.Atan2(entityInput.getMove().X, entityInput.getMove().Y) * Mathf.Rad2Deg +
                                     World.getWorld().getWorldRun().getCameraManage().getCamera().transform.eulerAngles.y;*/
    }
}