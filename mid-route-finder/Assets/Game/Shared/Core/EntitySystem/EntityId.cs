using System;
using System.Security.Cryptography;
using System.Text;
using Game.Shared.Interfaces.EntitySystem;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Shared.Core.EntitySystem {

public class EntityId : MonoBehaviour, IEntityId {
    [SerializeField, Tooltip("Deterministic seed used to generate the ID")]
    string _seed;

    ulong _id;
    public ulong ID => _id;

    void Awake() {
        if (_id == 0UL)
            generate();
    }

    [Button(ButtonSizes.Medium)]
    void generate() {
        if (string.IsNullOrEmpty(_seed)) {
            _seed = Guid.NewGuid().ToString( /*"N"  // People don't like hyphens, or some systems but I do*/);
        }

        _id = EntityId.generateDeterministicUlong(_seed);
    }

    static ulong generateDeterministicUlong(string seed) {
        if (string.IsNullOrEmpty(seed))
            return 0UL;

        byte[] input = Encoding.UTF8.GetBytes(seed);

        using (var sha = SHA256.Create()) {
            byte[] hash = sha.ComputeHash(input);
            return BitConverter.ToUInt64(hash, 0);
        }
    }
}

}
