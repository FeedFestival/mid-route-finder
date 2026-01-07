using Game.Shared.Constants.Store;
using Game.Shared.Core.Store;
using UnityEngine;

namespace Game.Player.Debug {

public class StoreDebugger : MonoBehaviour {
    [Header("Game State")] public GamePhase GamePhase;
    public Gameplay Gameplay;
    public UIScreen UIScreen;

    public int PreviousFocusedTriggerID;
    public int FocusedTriggerID;

    // Update is called once per frame
    void LateUpdate() {
        var state = Store2.State.Now();
        GamePhase = state.gamePhase;
        Gameplay = state.gameplay;
        UIScreen = state.uiScreen;
        PreviousFocusedTriggerID = state.previousFocusedTriggerID;
        FocusedTriggerID = state.focusedTriggerID;
    }
}

}
