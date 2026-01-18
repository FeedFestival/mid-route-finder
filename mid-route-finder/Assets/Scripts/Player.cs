using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;
using UnityEngine;

[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Player {
    public TeamColor TeamColor { get; private set; }
    public int WagonCount;
    public List<Mission> Missions { get; private set; }
    public Dictionary<CardColor, int> Cards { get; private set; }
    public List<ulong> CompletedRouteIds { get; private set; }
    public List<Mission> CompletedMissions { get; private set; }
    public List<Mission> FailedMission { get; private set; }

    CardPriority[] _colorPriority;
    public TurnContext? TurnContext;

    public Player(TeamColor teamColor) {
        TeamColor = teamColor;
        WagonCount = 45;
        Missions = new();
        CompletedRouteIds = new();
        Cards = new();
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

    public CardPriority[] GetCardPriority() {
        return _colorPriority;
    }

    public void SetCardPriority(CardPriority[] colorPriority) {
        _colorPriority = colorPriority;
    }

    public void IncrementCards(CardColor[] cardColors) {
        foreach (CardColor cardColor in cardColors) {
            IncrementCards(cardColor);
        }
    }

    public void IncrementCards(CardColor cardColor) {
        Cards.TryAdd(cardColor, 0);
        Cards[cardColor]++;
    }

    public override string ToString() {
        string MissionsToJson(IEnumerable<Mission> missions)
            => missions == null
                ? "[]"
                : $"[{string.Join(", ", missions)}]";

        string CardsToJson(Dictionary<CardColor, int> cards) {
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
  ""missions"": {MissionsToJson(Missions)},
  ""FailedMission"": {MissionsToJson(FailedMission)},
  ""completedMissions"": {MissionsToJson(CompletedMissions)},
  ""completedRouteIds"": {RouteIdsToJson(CompletedRouteIds)},
  ""cards"": {CardsToJson(Cards)}
}}";
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct TurnContext {
    public readonly CardColor cardColor;
    public readonly CardColor routeColor;

    public readonly ulong routeId;
    public readonly int cost;
    public readonly int missionIndex;
    public readonly int routesBetweenCount;

    public TurnContext(
        CardColor cardColor,
        CardColor routeColor,
        ulong routeId,
        int cost,
        int missionIndex = -1,
        int routesBetweenCount = -1
    ) {
        this.cardColor = cardColor;
        this.routeColor = routeColor;
        this.routeId = routeId;
        this.cost = cost;
        this.missionIndex = missionIndex;
        this.routesBetweenCount = routesBetweenCount;
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct CardPriority {
    public readonly CardColor color;
    public readonly int priority;

    public CardPriority(CardColor color, int priority) {
        this.color = color;
        this.priority = priority;
    }

    public override string ToString() {
        return
            $@"{{
  ""color"": ""{color}"",
  ""priority"": ""{priority}"",
}}";
    }
}
