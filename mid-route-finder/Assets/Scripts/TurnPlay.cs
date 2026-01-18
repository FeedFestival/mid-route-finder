using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using DG.Tweening;
using Game.Shared.Core.Store;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class TurnPlay : MonoBehaviour {
    [SerializeField] CardDeck _cardDeck;
    [SerializeField] PlayerWagons _playerWagons;
    [SerializeField] CityChecker _cityChecker;

    [Header("Debuging")] [SerializeField] bool _randomMissionPicking;
    [SerializeField] bool _randomPathPickingInMission;
    [SerializeField] bool _manualPlayByEnter;
    [SerializeField] float _turnTimeSpeed;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    const float DEFAULT_WAIT_TIME = 0.18f; // 0.0018 - is AWESOME

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    const string GAME_SCENE = "SampleScene";

    int _roundTurnIndex = -1;
    int _roundIndex = 0;
    int _turnIndex = -1;
    TeamColor? _lastRoundColor;
    bool _endGame;

    List<Mission> _missionPool;

    List<Player> _players;

    HashSet<TeamColor> _playerDidNothing = new();

    IEnumerator _waitBeforeNextTurnCo;
    IEnumerator _waitBeforeResetGame;

    void Start() {
        if (_manualPlayByEnter) return;

        _waitBeforeResetGame = waitBeforeResetGame(true);
        StartCoroutine(_waitBeforeResetGame);
    }

    void Update() {
        if (!_manualPlayByEnter) return;

        if (Input.GetKeyUp(KeyCode.S)) {
            resetGame();
        }

        if (Input.GetKeyUp(KeyCode.Return) ||
            Input.GetKeyUp(KeyCode.KeypadEnter)) {
            nextTurn();
        }
    }

    void resetGame() {
        var go = GameObject.Find("PlayerWagons");
        _playerWagons = go.GetComponent<PlayerWagons>();
        _playerWagons.SetTurnPlayDelegates(getPlayer);

        go = GameObject.Find("CityChecker");
        _cityChecker = go.GetComponent<CityChecker>();

        go = GameObject.Find("CardDeck");
        _cardDeck = go.GetComponent<CardDeck>();

        // -----------------

        _roundTurnIndex = -1;
        _roundIndex = 0;
        _turnIndex = -1;
        _lastRoundColor = null;
        _endGame = false;

        _players = new();
        _players.Add(new(TeamColor.Blue));
        _players.Add(new(TeamColor.Red));
        _players.Add(new(TeamColor.Yellow));
        _players.Add(new(TeamColor.Black));
        _players.Add(new(TeamColor.Green));

        _missionPool = createMissionPool();

        foreach (Player player in _players) {
            _cardDeck.DrawFromDeck(4, out var cards);
            player.IncrementCards(cards);
            assignRandomMissionsToPlayer(player);
        }

        if (_manualPlayByEnter) return;

        _waitBeforeNextTurnCo = waitBeforeNextTurn(true);
        StartCoroutine(_waitBeforeNextTurnCo);
    }

    void nextTurn() {
        if (_endGame) {
            Debug.Log(" =========  END GAME       ===== ------");
            StopCoroutine(_waitBeforeNextTurnCo);
            _waitBeforeNextTurnCo = null;

            _waitBeforeResetGame = waitBeforeResetGame();
            StartCoroutine(_waitBeforeResetGame);
            return;
        }

        _roundTurnIndex++;
        _turnIndex++;

        if (_roundTurnIndex == _players.Count) {
            _roundIndex++;
            _roundTurnIndex = 0;
        }

        var player = _players[_roundTurnIndex];
        doTurn(player);
    }

    void doTurn(Player player) {
        if (_lastRoundColor.HasValue) {
            doLastRoundTurn(player);

            if (_lastRoundColor == player.TeamColor)
                _endGame = true;

            return;
        }

        if (player.Missions.Count == 0) {
            // This means we are out of missions
            // We should get more missions this Round
            assignRandomMissionsToPlayer(player);
            return;
        }

        if (_cardDeck.IsDiscardedEmpty) {
            // We should try to get rid of our cards
            player.TurnContext = tryGetTurnContextFromAllRoutesBetween(player.Cards, player.WagonCount);

            if (player.TurnContext.HasValue) {
                _playerDidNothing.Remove(player.TeamColor);
                finishTurnByPlacingWagons(player);
            }

            if (_playerDidNothing.Contains(player.TeamColor)) {
                Debug.LogError(
                    "Game seems to be ending in a draw ... as one or more players were not able to do anything in the last two turns.");
            }

            _playerDidNothing.Add(player.TeamColor);
            Debug.LogWarning($"Player {player.TeamColor} can't do anything. He just activated DRAW MODE.");
            return;
        }

        if (_playerDidNothing.Contains(player.TeamColor))
            _playerDidNothing.Remove(player.TeamColor);

        var cardCount = player.Cards.Sum(kvp => kvp.Value);
        if (cardCount == 0) {
            finishTurnByPickingCards(player);
            return;
        }

        determineColorPriorityAndCreateTurnContext(ref player);

        if (!player.TurnContext.HasValue) {
            finishTurnByPickingCards(player);
            return;
        }

        finishTurnByPlacingWagons(player);
    }

    void determineColorPriorityAndCreateTurnContext(ref Player player) {
        var colorMaxCost = new Dictionary<CardColor, int>();

        for (int i = 0; i < player.Missions.Count; i++) {
            var mission = player.Missions[i];
            var hasPath = getMissionPath(mission, player.TeamColor, out List<PathTo> path);
            if (hasPath == false) {
                player.SetFailedMission(mission);
            }

            var routesBetween = getRouteBetweens(player, path, out int costToComplete);

            if (routesBetween.Count == 0) {
                player.SetCompletedMission(mission);
            }

            foreach (RouteBetween pathRouteBetween in routesBetween) {
                foreach (Route route in pathRouteBetween.Routes) {
                    if (route.InUse) // Is it really this easy?
                        continue;

                    if (route.Color != CardColor.Universal) {
                        if (!colorMaxCost.ContainsKey(route.Color)) {
                            colorMaxCost.Add(route.Color, pathRouteBetween.Distance);
                            continue;
                        }

                        colorMaxCost[route.Color] += colorMaxCost[route.Color] + pathRouteBetween.Distance;
                    }

                    if (player.TurnContext.HasValue) continue;

                    player.TurnContext = TurnPlay.tryCreateTurnContextFromRoute(
                        player.Cards,
                        route,
                        pathRouteBetween,
                        i,
                        routesBetween.Count
                    );
                }
            }
        }

        var cardPriority = new List<CardPriority>();
        foreach (var kvp in colorMaxCost) {
            cardPriority.Add(new(kvp.Key, kvp.Value));
        }

        player.SetCardPriority(cardPriority.OrderByDescending(card => card.priority).ToArray());
    }

    static TurnContext? tryCreateTurnContextFromRoute(
        Dictionary<CardColor, int> cards,
        Route route,
        RouteBetween routeBetween,
        int missionIndex = -1,
        int routesBetweenCount = -1
    ) {
        bool canBuildRoute;
        bool isRouteUniversal = route.Color == CardColor.Universal;
        int cost = routeBetween.Distance;
        if (isRouteUniversal) {
            var fittingCards = cards
                .Where(it => it.Value >= cost)
                .OrderBy(it => it.Value).ToArray();

            canBuildRoute = fittingCards.Length != 0;
            if (!canBuildRoute) return null;

            return new(
                fittingCards[0].Key, // TODO: maybe we can try to more clever about the color we use
                route.Color,
                routeBetween.ID,
                cost,
                missionIndex,
                routesBetweenCount
            );
        }

        bool playerHasRouteColors = cards.TryGetValue(route.Color, out int colorCardCount);

        if (!playerHasRouteColors) return null;

        canBuildRoute = colorCardCount >= routeBetween.Distance;
        if (canBuildRoute) {
            return new(
                route.Color,
                route.Color,
                routeBetween.ID,
                cost,
                missionIndex,
                routesBetweenCount
            );
        }

        return null;
    }

    void finishTurnByPickingCards(Player player) {
        var cardPriority = player.GetCardPriority();

        if (cardPriority == null || cardPriority.Length == 0) {
            bool canDraw = _cardDeck.DrawFromDeck(2, out var cards);
            if (!canDraw) return;

            player.IncrementCards(cards);

            return;
        }

        // Pick cards based on priority
        int pickedCardsCount = 0;
        foreach (var cp in cardPriority) {
            if (pickedCardsCount == 2) break;

            for (int i = 0; i < 2; i++) {
                if (pickedCardsCount == 2) break;

                if (_cardDeck.FaceUpCards.Contains(cp.color)) {
                    bool canTakeMore = _cardDeck.TakeFromFaceUp(cp.color);
                    if (!canTakeMore) return;

                    player.IncrementCards(cp.color);
                    pickedCardsCount++;
                }
            }
        }

        if (pickedCardsCount != 2) {
            for (int i = pickedCardsCount; i < 2; i++) {
                if (pickedCardsCount == 2) break;

                try {
                    var cardColor = _cardDeck.DrawFromDeck();
                    player.IncrementCards(cardColor);
                }
                catch (InvalidOperationException e) {
                    Debug.LogWarning($"We probably can't pick anymore cards as there aren't any: {e.Message}");
                }

                pickedCardsCount++;
            }
        }
    }

    void finishTurnByPlacingWagons(Player player) {
        if (!player.TurnContext.HasValue) {
            Debug.LogError(
                $"We try to finish turn by placing wagons but we have no TurnContext for player{player.TeamColor}. How did this happen?");
            return;
        }

        var turnContext = player.TurnContext.Value;

        if (turnContext.missionIndex >= 0) {
            var missionInProgress = player.Missions[turnContext.missionIndex];
            var isLastRoute = turnContext.routesBetweenCount == 1;
            if (isLastRoute) {
                player.SetCompletedMission(missionInProgress);
            }
        }

        player.WagonCount -= turnContext.cost;
        _cardDeck.AddToDiscardPile(turnContext.cardColor, turnContext.cost);

        if (player.WagonCount <= 2) {
            // TODO: What happens if we no longer have wagons to complete a route ?
            // I guess we need to think about that
            // One thing can be, up before we pick a routeBetween. We should check if we have enough to complete it

            if (!_lastRoundColor.HasValue) {
                _lastRoundColor = player.TeamColor;
                Debug.Log($"player({player.TeamColor}): ACTIVATED LAST ROUND: {player}");
            }
        }

        // AI needs to do this to simulate user click
        Store2.SetFocusedID(turnContext.routeId);

        player.SetCompletedRoutes(turnContext.routeId);

        _playerWagons.ClickPerformed();
    }

    void doLastRoundTurn(Player player) {
        // It's the last round, we should check if we can finish a mission
        for (int i = 0; i < player.Missions.Count; i++) {
            var mission = player.Missions[i];
            getMissionPath(mission, player.TeamColor, out List<PathTo> paths);
            var routesBetween = getRouteBetweens(player, paths, out int costToComplete);

            if (routesBetween.Count == 0)
                continue;

            if (costToComplete < player.WagonCount)
                continue;

            foreach (var routeBetween in routesBetween) {
                foreach (Route route in routeBetween.Routes) {
                    if (route.InUse) continue;

                    if (player.TurnContext.HasValue) break;

                    player.TurnContext = TurnPlay.tryCreateTurnContextFromRoute(
                        player.Cards,
                        route,
                        routeBetween,
                        i,
                        routesBetween.Count
                    );
                }
            }

            if (player.TurnContext.HasValue)
                break;
        }

        // Or if we can't finish a mission, we look to find a RouteBetween to populate it with our remaining wagons
        if (!player.TurnContext.HasValue) {
            player.TurnContext = tryGetTurnContextFromAllRoutesBetween(player.Cards, player.WagonCount);
        }

        if (!player.TurnContext.HasValue) {
            finishTurnByPickingCards(player);
            return;
        }

        finishTurnByPlacingWagons(player);
    }

    TurnContext? tryGetTurnContextFromAllRoutesBetween(Dictionary<CardColor, int> cards, int wagonCount) {
        var remainingRoutesBetween = new List<RouteBetween>();
        foreach (var kvp in _cityChecker.RoutesBetween) {
            if (kvp.Value.Routes.Count == 0) continue;

            var inUse = kvp.Value.Routes.Any(it => it.InUse);

            if (inUse) continue;

            if (kvp.Value.Distance > wagonCount) continue;

            remainingRoutesBetween.Add(kvp.Value);
        }

        remainingRoutesBetween = remainingRoutesBetween.OrderByDescending(it => it.Distance).ToList();

        foreach (var routeBetween in remainingRoutesBetween) {
            foreach (Route route in routeBetween.Routes) {
                if (route.InUse) // Is it really this easy?
                    continue;

                var context = TurnPlay.tryCreateTurnContextFromRoute(
                    cards,
                    route,
                    routeBetween
                );

                if (context.HasValue)
                    return context;
            }
        }

        return null;
    }

    List<RouteBetween> getRouteBetweens(Player player, List<PathTo> path, out int costToComplete) {
        var routesBetween = new List<RouteBetween>();
        costToComplete = 0;

        foreach (PathTo pathTo in path) {
            if (player.CompletedRouteIds.Contains(pathTo.routeBetweenID))
                continue;

            costToComplete += pathTo.cost;

            if (player.WagonCount < pathTo.cost) continue;

            routesBetween.Add(_cityChecker.RoutesBetween[pathTo.routeBetweenID]);
        }

        return routesBetween;
    }

    List<Mission> createMissionPool() {
        var missionPool = new List<Mission>();
        foreach (var kvp in MissionConstants.MISSION_CARD_COUNT) {
            var category = kvp.Key;
            int drawCount = kvp.Value;

            var availableMissions = new List<Mission>(MissionConstants.MISSION_BANK[category]);

            for (int i = 0; i < drawCount && availableMissions.Count > 0; i++) {
                missionPool.Add(MissionConstants.DrawWeighted(ref availableMissions));
            }
        }

        return missionPool;
    }

    void assignRandomMissionsToPlayer(Player player) {
        var missions = new List<Mission>();

        getRandomMissionFromPool(ref missions);
        getRandomMissionFromPool(ref missions);
        getRandomMissionFromPool(ref missions);

        player.SetMissions(missions);
    }

    void getRandomMissionFromPool(ref List<Mission> missions) {
        var index = Random.Range(0, _missionPool.Count);
        var randomMission = _missionPool[index];

        missions.Add(randomMission);
        _missionPool.RemoveAt(index);
    }

    Player getPlayer() {
        return _players[_roundTurnIndex];
    }

    TeamColor getPlayerTeamColor() {
        var player = _players[_roundTurnIndex];
        return player.TeamColor;
    }

    bool getMissionPath(Mission mission, TeamColor color, out List<PathTo> path) {
        var from = _cityChecker.Cities.Find(it => it.Name == mission.cityA);
        var to = _cityChecker.Cities.Find(it => it.Name == mission.cityB);
        return Pathfinding.FindShortestPathEdges(from, to, color, out path);
    }

    // ----

    IEnumerator waitBeforeResetGame(bool initial = false) {
        Scene scene = SceneManager.GetSceneByName(GAME_SCENE);
        if (initial && scene.isLoaded) {
            yield return new WaitForSeconds(DEFAULT_WAIT_TIME);

            resetGame();
            StopCoroutine(_waitBeforeResetGame);
            _waitBeforeResetGame = null;
        }
        else {
            yield return new WaitUntil(() => DOTween.TotalPlayingTweens() == 0);

            yield return new WaitForSeconds(2f);

            if (scene.isLoaded) {
                AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(GAME_SCENE);
                yield return unloadOp;
            }

            yield return new WaitForSeconds(DEFAULT_WAIT_TIME);

            AsyncOperation loadOp = SceneManager.LoadSceneAsync(GAME_SCENE, LoadSceneMode.Additive);
            yield return loadOp;


            Scene loadedScene = SceneManager.GetSceneByName(GAME_SCENE);
            SceneManager.SetActiveScene(loadedScene);

            yield return new WaitForSeconds(DEFAULT_WAIT_TIME);

            resetGame();
            StopCoroutine(_waitBeforeResetGame);
            _waitBeforeResetGame = null;
        }
    }

    IEnumerator waitBeforeNextTurn(bool initial = false) {
        if (initial)
            yield return new WaitForSeconds(DEFAULT_WAIT_TIME);

        nextTurn();

        yield return new WaitForSeconds(_turnTimeSpeed);

        _waitBeforeNextTurnCo = waitBeforeNextTurn();
        StartCoroutine(_waitBeforeNextTurnCo);
    }
}
