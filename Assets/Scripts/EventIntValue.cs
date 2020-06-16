using System;

public class EventIntValue {
    public event Action<int, int> OnValueChanged;

    private int _Value;

    public int Value {
        get => _Value;
        set {
            var delta = value - _Value;
            _Value = value;
            OnValueChanged?.Invoke(delta, _Value);
        }
    }

    public EventIntValue(int value = 0) {
        _Value = 0;
    }
}