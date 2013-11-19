using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Strilanc.LinqToCollections;

[DebuggerDisplay("{ToString()}")]
struct TernaryGate {
    public static int CountAll { get { return (int)Math.Pow(3, 9); } }
    
    public static TernaryGate First { get { return FromTransitions((e1, e2) => e1); } }
    public static TernaryGate Second { get { return FromTransitions((e1, e2) => e2); } }

    private readonly PackedTritVector _outputs;
    private TernaryGate(PackedTritVector outputs) {
        this._outputs = outputs;
    }

    public static IEnumerable<TernaryGate> All {
        get {
            return from t1 in Trit.All
                   from t2 in Trit.All
                   from t3 in Trit.All
                   from t4 in Trit.All
                   from t5 in Trit.All
                   from t6 in Trit.All
                   from t7 in Trit.All
                   from t8 in Trit.All
                   from t9 in Trit.All
                   select new TernaryGate(new PackedTritVector(new[] {t1, t2, t3, t4, t5, t6, t7, t8, t9}));
        }
    }

    public IEnumerable<TernaryGate> ReachableGates() {
        var seen = new bool[1 << 18];
        var reached = new List<TernaryGate>(new[] { First, Second });
        yield return First;
        yield return Second;
        seen[First._outputs.PackedValue] = true;
        seen[Second._outputs.PackedValue] = true;

        for (var i = 0; i < reached.Count; i++) {
            var in1 = reached[i];
            var n = reached.Count;
            for (var j = 0; j < n && reached.Count < CountAll; j++) {
                var in2 = reached[j];
                var out1 = ApplyToOutputsOf(in2, in1);
                var out2 = ApplyToOutputsOf(in1, in2);
                if (!seen[out1._outputs.PackedValue]) {
                    seen[out1._outputs.PackedValue] = true;
                    yield return out1;
                    reached.Add(out1);
                }
                if (!seen[out2._outputs.PackedValue]) {
                    seen[out2._outputs.PackedValue] = true;
                    yield return out2;
                    reached.Add(out2);
                }
            }
        }
    }
    public bool? IsUniversalGateTryQuickCheck() {
        if (Evaluate(Trit.Off, Trit.Off) == Trit.Off) return false;
        if (Evaluate(Trit.Neutral, Trit.Neutral) == Trit.Neutral) return false;
        if (Evaluate(Trit.On, Trit.On) == Trit.On) return false;
        return null;
    }
    public bool IsUniversalGate() {
        var r = IsUniversalGateTryQuickCheck();
        if (r.HasValue) return r.Value;
        return ReachableGates().Count() == CountAll;
    }

    public static TernaryGate FromTransitions(Func<Trit, Trit, Trit> transitions) {
        return new TernaryGate(new PackedTritVector((from v2 in Trit.All
                                               from v1 in Trit.All
                                               select transitions(v1, v2)).ToArray()));
    }

    public Trit Evaluate(Trit input1, Trit input2) {
        return _outputs[input1.Value*3 + input2.Value];
    }

    public TernaryGate ApplyToOutputsOf(TernaryGate g1, TernaryGate g2) {
        var r = new Trit[9];
        for (var i = 0; i < 9; i++) {
            r[i] = Evaluate(g1._outputs[i], g2._outputs[i]);
        }
        return new TernaryGate(new PackedTritVector(r));
    }
    public override bool Equals(object obj) {
        if (!(obj is TernaryGate)) return false;
        return _outputs.PackedValue == ((TernaryGate)obj)._outputs.PackedValue;
    }
    public override int GetHashCode() {
        return _outputs.PackedValue;
    }
    public override string ToString() {
        var s = this;
        return String.Join(
            "",
            9.Range()
             .Select(e => s._outputs[e])
             .Select(e => e == null ? "0" : e == false ? "-" : "+"));
    }
}
