/// <summary>
/// Efficiently stores several trits, giving them each 2 bits of a single 32-bit integer.
/// Assumes the caller knows how many trits should be in the vector.
/// </summary>
struct PackedTritVector {
    public readonly int PackedValue;
    public PackedTritVector(Trit trit1, Trit trit2) {
        this.PackedValue = (trit2.Value << 2) | trit1.Value;
    }
    public PackedTritVector(params Trit[] trits) {
        var n = 0;
        for (var i = trits.Length - 1; i >= 0; i--) {
            n <<= 2;
            n |= trits[i].Value;
        }
        this.PackedValue = n;
    }
    public Trit Trit1 { get { return new Trit(PackedValue & 3); } }
    public Trit Trit2 { get { return new Trit((PackedValue>>2) & 3); } }
    public Trit this[int index] { get { return new Trit((PackedValue >> (index << 1)) & 3); }}
}
