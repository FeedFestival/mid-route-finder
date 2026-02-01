public struct fakeFallingWagon : IFallingWagon {
    public TeamColor TeamColor { get; }
    public SpatialData SpatialData { get; }

    public fakeFallingWagon(SpatialData spatialData, TeamColor teamColor) {
        SpatialData = spatialData;
        TeamColor = teamColor;
    }
}
