using RegisterSystem;

namespace InTime; 

public class AllCamp : RegisterManage<Camp> {
    public static Camp player;
    public static Camp @void;
}

public class Camp : RegisterBasics {
    protected IEntityScreen entityScreen;

    public override void init() {
        base.init();
        entityScreen = new IEntityScreen.EntityScreenDifferentCamp(this);
    }
    
    public IEntityScreen getEntityScreen() => entityScreen;
}