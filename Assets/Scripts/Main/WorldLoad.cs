using Godot;

namespace InTime;

public partial class WorldLoad : Node {
    public override void _Ready() {
        base._Ready();
        World.getInstance();
    }
}