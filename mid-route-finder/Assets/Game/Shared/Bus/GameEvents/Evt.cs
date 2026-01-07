namespace Game.Shared.Bus.GameEvents {
    public enum Evt {
        GAME_SCENE_LOADED,
        NEW_GAME,
        CONTINUE_GAME,
        //
        CLOSE_MENU,
        //
        PLAYER_INTERACTED,
        PLAYER_INTERACTED_WITH_UNIT,
        PLAYER_ATTACKED_WITH_UNIT,
        //
        PLAY_SFX,
        PLAY_AMBIENT,
    }
}
