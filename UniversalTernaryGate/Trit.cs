using System.Diagnostics;

[DebuggerDisplay("{ToString()}")]
public struct Trit {
    public static readonly Trit Off = new Trit(0);
    public static readonly Trit Neutral = new Trit(1);
    public static readonly Trit On = new Trit(2);

    public static readonly Trit[] All = new[] {Off, Neutral, On};

    public readonly int Value;
    public Trit(int value) {
        this.Value = value;
    }

    public static implicit operator bool?(Trit value) {
        if (value == On) return true;
        if (value == Off) return false;
        return null;
    }
    public static bool operator ==(Trit value1, Trit value2) {
        return value1.Value == value2.Value;
    }
    public static bool operator !=(Trit value1, Trit value2) {
        return value1.Value != value2.Value;
    }
    public static implicit operator Trit(bool? value) {
        if (value == true) return On;
        if (value == false) return Off;
        return Neutral;
    }
    public bool Equal(Trit other) {
        return this.Value == other.Value;
    }
    public override bool Equals(object obj) {
        return obj is Trit && Equals((Trit)obj);
    }
    public override int GetHashCode() {
        return Value;
    }
    public override string ToString() {
        if (this == On) return "On";
        if (this == Off) return "Off";
        return "Neutral";
    }
}
