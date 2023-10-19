using Godot;
using RegisterSystem;

namespace InTime;

public class AllKeyAxis : RegisterManage<KeyAxis> {
    public static KeyAxis AD;
    public static KeyAxis WS;

    public override void init() {
        base.init();
        AD.initSetKeyPack(AllKeyPack.A, AllKeyPack.D);
        WS.initSetKeyPack(AllKeyPack.W, AllKeyPack.S);
    }
}

public class KeyAxis : RegisterBasics {
    protected float addSpeed = 10;
    protected KeyPack min;
    protected KeyPack max;

    protected float cursor;

    public float getCursor() => cursor;

    protected void onEvent(Event.EventWorld.FixedUpdate @event) {
        bool _max = max.isDown(DownType.isDown);
        bool _min = min.isDown(DownType.isDown);

        if (_min && _max) {
            cursor = Mathf.Lerp(cursor, Mathf.Lerp(0, 1, 0.5f), @event.unscaledFixedDeltaTime * addSpeed).threshold(0.05f);
        }
        else if (_min || _max) {
            if (_min) {
                cursor = Mathf.Clamp(cursor - addSpeed * @event.unscaledFixedDeltaTime, 0, 1).threshold(0.05f);
            }
            if (_max) {
                cursor = Mathf.Clamp(cursor + addSpeed * @event.unscaledFixedDeltaTime, 0, 1).threshold(0.05f);
            }
        }
        else {
            cursor = Mathf.Lerp(cursor, Mathf.Lerp(0, 1, 0.5f), @event.unscaledFixedDeltaTime * addSpeed).threshold(0.05f);
        }
    }

    public void initSetKeyPack(KeyPack _min, KeyPack _max) {
        initTest();
        min = _min;
        max = _max;
    }

    public void initSetAddSpeed(float _addSpeed) {
        initTest();
        addSpeed = _addSpeed;
    }
}