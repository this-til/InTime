using System;
using EventBus;
using Godot;
using RegisterSystem;

namespace InTime;

public class AllKeyPack : RegisterManage<KeyPack> {
    public static KeyPack W;
    public static KeyPack S;
    public static KeyPack A;
    public static KeyPack D;

    public static KeyPack jump;

    public static KeyPack attackTap;
    public static KeyPack attackThump;

    public static KeyPack dodge;

    public override void init() {
        base.init();

        W.initSetDefaultKey(Key.W);
        S.initSetDefaultKey(Key.S);
        A.initSetDefaultKey(Key.A);
        D.initSetDefaultKey(Key.D);

        jump.initSetDefaultKey(Key.Space);

        attackTap.initSetDefaultKey(Key.Left);
        attackTap.initDoubleHitTime(0.2f);
        attackThump.initSetDefaultKey(Key.Right);
        attackThump.initDoubleHitTime(0.2f);
        dodge.initSetDefaultKey(Key.Shift);
        dodge.initDoubleHitTime(0.2f);
    }
}

public class KeyPack : RegisterBasics {
    /// <summary>
    /// 使用key来代表多个映射
    /// </summary>
    [FieldRegister] protected OverallConfig<Key[]> keys;

    protected Key[] defaultKeys = Array.Empty<Key>();

    /// <summary>
    /// 连击时间
    /// NaN
    /// </summary>
    protected float doubleHitTime;

    /// <summary>
    /// 帧时间的按下
    /// </summary>
    protected bool _timeDown;

    /// <summary>
    /// 按钮被真实摁下
    /// </summary>
    protected bool _isDown;

    /// <summary>
    /// 是连击状态
    /// </summary>
    protected bool _isDoubleHit;

    /// <summary>
    /// 被一直按着
    /// </summary>
    protected bool _isAlways;

    /// <summary>
    /// 间隔时间
    /// </summary>
    protected float downTime;

    /// <summary>
    /// 帧锁
    /// </summary>
    protected bool frameLock = true;

    public override void awakeInit() {
        base.awakeInit();
        keys.initSetDefaultDate(defaultKeys);
    }

    [Event(priority = 10)]
    protected void onEvent(Event.EventWorld.Update @event) {
        Key[] _keys = keys.getData();
        if (_keys.isEmpty()) {
            return;
        }
        foreach (var key in _keys) {
            _isDown = Input.IsKeyPressed(key);
            if (_isDown) {
                break;
            }
        }
        if (frameLock) {
            _timeDown = _timeDown || _isDown;
        }
        frameLock = true;
    }

    [Event(priority = 10)]
    protected void onEvent(Event.EventWorld.FixedUpdate @event) {
        frameLock = false;
        if (_isDown) {
            if (downTime < 0) {
                downTime = 0;
            }
            downTime += @event.unscaledFixedDeltaTime;
        }
        else {
            if (downTime > 0) {
                downTime = 0;
            }
            downTime -= @event.unscaledFixedDeltaTime;
        }
        if (doubleHitTime > 0) {
            if (_isDown) {
                if (downTime > doubleHitTime) {
                    _isDoubleHit = false;
                    _isAlways = true;
                }
                else {
                    _isDoubleHit = true;
                }
            }
            else {
                if (_isAlways) {
                    _isAlways = false;
                }
                if (downTime < -doubleHitTime) {
                    _isDoubleHit = false;
                }
            }
        }
    }

    public bool isDown(DownType downType) => downType switch {
        DownType.timeIsDown => _timeDown,
        DownType.isDown => _isDown,
        DownType.doubleHit => _isDoubleHit,
        DownType.always => _isAlways,
        _ => false
    };

    public void initSetDefaultKey(params Key[]? _defaultKeys) {
        initTest();
        if (_defaultKeys is null) {
            return;
        }
        defaultKeys = _defaultKeys;
    }

    public void initDoubleHitTime(float _doubleHitTime) {
        initTest();
        doubleHitTime = _doubleHitTime;
    }
}