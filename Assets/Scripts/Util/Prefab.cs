namespace InTime;

public class StringPrefab {

    public const string gameName = "InTime";
    public const string overallConfigRoute = "InTime/ConfigRoute";
    
    public const string shield = "shield";
}

public class LayerPrefab {
    public const int air = 1 << 0;
    public const int ui = 1 << 1;
    public const int effect = 1 << 2;
    public const int entity = 1 << 3;
    public const int ground = 1 << 4;
}