using Game.Shared.Constants.ActionOrchestrator;
using UnityEngine;

namespace Game.Shared.Interfaces.Core {

public interface ICommandable {
    Command GetDefaultCommand();
    bool IsValidCommand(Command command);
    // void DoCommand(Command command, IUnit unit);

    /**
     * This is manly used for when the Player tries to interact with either an Item or Interactable
     * - and we need to know if we move there or not
     * - it's applicable both to INPCUnit, IInteractable, IItem
     */
    // bool IsInInteractionRange(IUnit unit);
    Vector3 GetInteractionMidPoint(Vector3 transformPosition);
}

}
