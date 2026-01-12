using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Player {
    public TeamColor TeamColor { get; private set; }
    public int WagonCount;
    public List<Mission> Missions { get; private set; }
    public Dictionary<RouteColor, int> Cards { get; private set; }
    public RouteColor PreparedCardsColor;
    public List<ulong> CompletedRouteIds { get; private set; }
    public List<Mission> CompletedMissions { get; private set; }
    public List<Mission> FailedMission { get; private set; }

    public Player(TeamColor teamColor) {
        TeamColor = teamColor;
        WagonCount = 45;
        Missions = new();
        CompletedRouteIds = new();
    }

    public void SetMissions(List<Mission> missions) {
        Missions.AddRange(missions);
    }

    public void SetCompletedRoutes(ulong routeBetweenID) {
        CompletedRouteIds.Add(routeBetweenID);
    }

    public void SetCompletedMission(Mission mission) {
        if (CompletedMissions == null)
            CompletedMissions = new();

        CompletedMissions.Add(mission);
        Missions.Remove(mission);
    }

    public void SetFailedMission(Mission mission) {
        if (FailedMission == null)
            FailedMission = new();

        FailedMission.Add(mission);
        Missions.Remove(mission);
    }

    public override string ToString() {
        string MissionsToJson(IEnumerable<Mission> missions)
            => missions == null
                ? "[]"
                : $"[{string.Join(", ", missions)}]";

        string CardsToJson(Dictionary<RouteColor, int> cards) {
            if (cards == null || cards.Count == 0)
                return "{}";

            var entries = new List<string>();
            foreach (var kvp in cards)
                entries.Add($"\"{kvp.Key}\": {kvp.Value}");

            return $"{{{string.Join(", ", entries)}}}";
        }

        string RouteIdsToJson(IEnumerable<ulong> ids)
            => ids == null
                ? "[]"
                : $"[{string.Join(", ", ids)}]";

        return
            $@"{{
  ""teamColor"": ""{TeamColor}"",
  ""WagonCount"": ""{WagonCount}"",
  ""preparedCardsColor"": ""{PreparedCardsColor}"",
  ""missions"": {MissionsToJson(Missions)},
  ""FailedMission"": {MissionsToJson(FailedMission)},
  ""completedMissions"": {MissionsToJson(CompletedMissions)},
  ""completedRouteIds"": {RouteIdsToJson(CompletedRouteIds)},
  ""cards"": {CardsToJson(Cards)}
}}";
    }
}
