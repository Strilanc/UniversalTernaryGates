using System;
using System.Collections.Generic;
using System.Linq;

namespace UniversalTernaryGate {
    class Program {
        private static void Main() {
            var gates = UniversalGates().ToArray();
            Console.WriteLine(string.Join(Environment.NewLine, gates));
            Console.WriteLine(gates.Length);
            Console.ReadLine();
        }
        
        private static TernaryGate Tand {
            get {
                Func<bool?, bool?> rotate = e => e == true ? false
                                               : e == false ? (bool?)null
                                               : true;


                return TernaryGate.FromTransitions((e1, e2) => e1 == e2 ? rotate(e1) : true);
            }
        }

        private static IEnumerable<TernaryGate> UniversalGates() {
            // hack: the hash code of a ternary gate is guaranteed to be under 2^18
            // take advantage of that to do known-result lookups faster
            var tryIsUniveralByHash = new bool?[1 << 18];
            
            // optimization: having a known universal gate significantly speeds things up
            // it allows the 'what can I reach?' to terminate as soon as the known gate is found
            // instead of after ALL gates are found
            tryIsUniveralByHash[Tand.GetHashCode()] = true;
            
            foreach (var candidate in TernaryGate.All) {
                if (tryIsUniveralByHash[candidate.GetHashCode()].HasValue) continue;
                
                // if it turns out that this candidate is not universal
                // then all of the gates it reached also can't be universal
                // so we track them to be marked in that case
                var weakGates = new List<TernaryGate> {candidate};

                var reachedCount = 0;
                foreach (var reachedGate in candidate.ReachableGates()) {
                    reachedCount += 1;

                    var tryIsUniversal = tryIsUniveralByHash[reachedGate.GetHashCode()];
                    if (tryIsUniversal == null) {
                        // if the candidate gate isn't universal, we learn this reached gate also isn't
                        weakGates.Add(reachedGate);
                    } else if (tryIsUniversal == true) {
                        // reaching a universal gate implies the candidate gate must be universal
                        reachedCount = TernaryGate.CountAll;
                        break;
                    }
                }

                if (reachedCount == TernaryGate.CountAll) {
                    yield return candidate;
                    tryIsUniveralByHash[candidate.GetHashCode()] = true;
                } else {
                    foreach (var gate in weakGates) {
                        tryIsUniveralByHash[gate.GetHashCode()] = false;
                    }
                }
            }
        }
    }
}