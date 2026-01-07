using UnityEngine;

namespace Game.Shared.Interfaces.Core {
    public interface ITrigger {
        int ID { get; }
        Transform transform { get; }
        void Init(int entityId);
        void Enable(bool enable = true);
    }
}
