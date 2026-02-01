// ReSharper disable InconsistentNaming

namespace Game.Shared.Constants.Scene {

public static class SceneConst {
    public const string Main = "Main";
    public const string Player = "PLAYER";

    public const string SampleScene = "SampleScene";
    public const string TurnPlay = "TurnPlay";

    public static readonly string MainScenePath = $"Assets/Scenes/{Main}.unity";

    public static readonly string SampleSceneScenePath = $"Assets/Scenes/{SampleScene}.unity";
    public static readonly string PlayerScenePath = $"Assets/Game/Player/{Player}.unity";
    public static readonly string TurnPlayScenePath = $"Assets/Scenes/{TurnPlay}.unity";
}

}
