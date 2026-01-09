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

    public static void SetFocusedID(ulong focusedID) {
        State.__focusedID.OnNext(focusedID);
    }

    public static void SetFocusedInstanceID(int instanceID) {
        State.__focusedInstanceID.OnNext(instanceID);
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
    public readonly ulong focusedID;
    public readonly int focusedInstanceID;

    public StateNow(
        GamePhase gamePhase,
        Gameplay gameplay,
        UIScreen uiScreen,
        ulong focusedID,
        int focusedInstanceID
    ) {
        this.gamePhase = gamePhase;
        this.gameplay = gameplay;
        this.uiScreen = uiScreen;
        this.focusedID = focusedID;
        this.focusedInstanceID = focusedInstanceID;
    }
}

public class StoreState2 {
    public ReadOnlyReactiveProperty<GamePhase> GamePhase => __gamePhase;
    internal readonly ReactiveProperty<GamePhase> __gamePhase;
    public ReadOnlyReactiveProperty<Gameplay> Gameplay => __gameplay;
    internal ReactiveProperty<Gameplay> __gameplay { get; }
    public ReadOnlyReactiveProperty<UIScreen> UIScreen => __uIScreen;
    internal ReactiveProperty<UIScreen> __uIScreen { get; }

    public ReadOnlyReactiveProperty<ulong> FocusedID => __focusedID;
    internal ReactiveProperty<ulong> __focusedID { get; }

    public ReadOnlyReactiveProperty<int> FocusedInstanceID => __focusedInstanceID;
    internal ReactiveProperty<int> __focusedInstanceID { get; }

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

        __focusedID = new(0);
        __focusedInstanceID = new(-1);
    }

    public StateNow Now() {
        return new(
            GamePhase.CurrentValue,
            Gameplay.CurrentValue,
            UIScreen.CurrentValue,
            FocusedID.CurrentValue,
            FocusedInstanceID.CurrentValue
        );
    }

    public void Cleanup() { }
}

}
