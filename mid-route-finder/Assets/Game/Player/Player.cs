using Game.Shared.Bus;
using Game.Shared.Core.Store;
using UnityEngine;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Player_Input;
using Game.Shared.Interfaces.Core;
using Game.Shared.Interfaces.Player;

namespace Game.Player {

public class Player : MonoBehaviour, IPlayer {
    public void SetInputMouse(IInputMouse inputMouse) {
        PlayerInput.SetInputMouse(inputMouse);
    }

    void Awake() {
        __.SetPlayer(this);
    }

    void Start() {
        Store2.SetGameplay(Gameplay.Exploration);
        Store2.SetUIScreen(UIScreen.HUD);
        Store2.SetGamePhase(GamePhase.InGame);
    }

    void OnApplicationQuit() {
        Debug.Log("ApplicationQuit - Reseting Static Properties");
        Store2.Cleanup();
    }

    void openMainMenu() {
        Store2.SetGameplay(Gameplay.MainMenu);
        Store2.SetUIScreen(UIScreen.MainMenu);
    }
}

}
