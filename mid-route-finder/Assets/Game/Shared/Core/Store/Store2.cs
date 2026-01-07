using Game.Shared.Constants.Store;
using R3;
using UnityEngine;

namespace Game.Shared.Core.Store {

public static class Store2 {
    public static StoreState2 State { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    static void OnRuntimeMethodLoad() {
        State = new(GamePhase.Starting, Gameplay.Loading, UIScreen.Loading);
    }

    public static void SetGamePhase(GamePhase gamePhase) {
        State.__gamePhase.OnNext(gamePhase);
    }

    public static void SetGameplay(Gameplay gameplay) {
        State.__previousGameplay = State.__gameplay.CurrentValue;
        State.__gameplay.OnNext(gameplay);
    }

    public static void SetUIScreen(UIScreen uiScreen) {
        State.__uIScreen.OnNext(uiScreen);
    }

    public static void SetFocusedTriggerID(int focusedTriggerID) {
        State.setPreviousFocusedTriggerID(State.FocusedTriggerID.CurrentValue);
        State.__focusedTriggerID.OnNext(focusedTriggerID);
    }

    public static void Cleanup() {
        State.Cleanup();
        State = null;
    }
}

public class StateNow {
    public readonly GamePhase gamePhase;
    public readonly Gameplay gameplay;
    public readonly UIScreen uiScreen;
    public readonly int previousFocusedTriggerID;
    public readonly int focusedTriggerID;

    public StateNow(
        GamePhase gamePhase,
        Gameplay gameplay,
        UIScreen uiScreen,
        int previousFocusedTriggerID,
        int focusedTriggerID
    ) {
        this.gamePhase = gamePhase;
        this.gameplay = gameplay;
        this.uiScreen = uiScreen;
        this.previousFocusedTriggerID = previousFocusedTriggerID;
        this.focusedTriggerID = focusedTriggerID;
    }
}

public class StoreState2 {
    public ReadOnlyReactiveProperty<GamePhase> GamePhase => __gamePhase;
    internal readonly ReactiveProperty<GamePhase> __gamePhase;
    public ReadOnlyReactiveProperty<Gameplay> Gameplay => __gameplay;
    internal ReactiveProperty<Gameplay> __gameplay { get; }
    public ReadOnlyReactiveProperty<UIScreen> UIScreen => __uIScreen;
    internal ReactiveProperty<UIScreen> __uIScreen { get; }

    public int PreviousFocusedTriggerID { get; private set; }
    public ReadOnlyReactiveProperty<int> FocusedTriggerID => __focusedTriggerID;
    internal ReactiveProperty<int> __focusedTriggerID { get; }

    public Gameplay PreviousGameplay => __previousGameplay;
    internal Gameplay __previousGameplay = Constants.Store.Gameplay.Loading;

    internal StoreState2(
        GamePhase _gamePhase,
        Gameplay _gameplay,
        UIScreen _uiScreen
    ) {
        __gamePhase = new(_gamePhase);
        __gameplay = new(_gameplay);
        __uIScreen = new(_uiScreen);
        PreviousFocusedTriggerID = -1;
        __focusedTriggerID = new(-1);
    }

    internal void setPreviousFocusedTriggerID(int previousFocusedTriggerID) {
        PreviousFocusedTriggerID = previousFocusedTriggerID;
    }

    public StateNow Now() {
        return new(
            GamePhase.CurrentValue,
            Gameplay.CurrentValue,
            UIScreen.CurrentValue,
            PreviousFocusedTriggerID,
            FocusedTriggerID.CurrentValue
        );
    }

    public void Cleanup() { }
}

}
