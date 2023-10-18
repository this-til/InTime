using Godot;
using RegisterSystem;

namespace InTime;

public class AllKeyValue : RegisterManage<KeyValue> {

}

public class KeyValue : RegisterBasics {
    protected internal KeyPack keyPack;
    protected internal float addSpeed = 10;

    protected float cursor;

    public float getCursor() => cursor;

    protected void onEvent(Event.EventWorld.FixedUpdate @event) {
        cursor = keyPack.isDown(DownType.isDown)
            ? Mathf.Clamp(cursor + addSpeed * @event.unscaledFixedDeltaTime, 0, 1)
            : Mathf.Clamp(cursor - addSpeed * @event.unscaledFixedDeltaTime, 0, 1).threshold(0.05f);
    }
}