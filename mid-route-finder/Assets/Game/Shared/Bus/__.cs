using Game.Shared.Bus.GameEvents;
using Game.Shared.Interfaces.Player;

namespace Game.Shared.Bus {

public static class __ {
    public static IPlayer Player { get; private set; }

    public static GameBus Game {
        get {
            if (_game == null) {
                _game = new GameBus();
            }

            return _game;
        }
    }

    static GameBus _game;

    public static void SetPlayer(IPlayer player) {
        Player = player;
    }

    public static void ClearAll() {
        _game = null;
    }
}

}
