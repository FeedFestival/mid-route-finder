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
    [SerializeField] PlayerWagons _playerWagons;
    [SerializeField] CityChecker _cityChecker;

    [Header("Debuging")] [SerializeField] bool _randomMissionPicking;
    [SerializeField] bool _randomPathPickingInMission;
    [SerializeField] bool _manualPlayByEnter;
    [SerializeField] float _turnTimeSpeed;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    const float DEFAULT_WAIT_TIME = 0.18f;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    const string GAME_SCENE = "SampleScene";

    int _roundTurnIndex = -1;
    int _roundIndex = 0;
    int _turnIndex = -1;
    TeamColor? _lastRoundColor;
    bool _endGame;

    List<Mission> _missionPool;

    List<Player> _players;

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
        _playerWagons.SetTurnPlayDelegates(getPlayerPreparedCardsColor, getPlayerTeamColor);

        go = GameObject.Find("CityChecker");
        _cityChecker = go.GetComponent<CityChecker>();

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
        Debug.Log($"Mission Pool Created: {_missionPool.Count}");
        foreach (Player player in _players) {
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

        int index = player.Missions.Count - 1;
        if (_randomMissionPicking)
            index = Random.Range(0, player.Missions.Count);

        var mission = player.Missions[index];
        var hasPath = getMissionPath(mission, player.TeamColor, out List<PathTo> path);
        if (hasPath == false) {
            // Debug.Log($"------------------- hasPath: {hasPath}");
            // Debug.Log($"player: {player}");

            player.SetFailedMission(mission);
            doTurn(player);
            return;
        }

        var routesBetween = getRouteBetweens(player, path, out int costToComplete);

        if (routesBetween.Count == 0) {
            // Debug.Log("---===---");
            // Debug.Log($"How did this happen? mission: {mission}");
            // var s = "";
            // foreach (PathTo to in path) {
            //     s += $"{to}, ";
            // }
            //
            // Debug.Log($"path: {s}");

            // apparently we finished with this Mission somehow
            player.SetCompletedMission(mission);
            doTurn(player);
            return;
        }

        var isLastRoute = routesBetween.Count == 1;
        if (isLastRoute) {
            // Debug.Log($"isLastRoute -> player({player.TeamColor}) ======= : {routesBetween[0].ID}");
            index = 0;
            player.SetCompletedMission(mission);
        }
        else {
            if (_randomPathPickingInMission)
                index = Random.Range(0, routesBetween.Count);
            else
                index = Math.Max(0, routesBetween.Count - 1);
        }

        RouteBetween routeBetween = routesBetween[index];

        // RouteBetween routeBetween = null;
        // try {
        //     routeBetween = routesBetween[index];
        // }
        // catch (Exception e) {
        //     Debug.LogError($"---------------- \n Route between {index} out of range. {e}");
        //     Debug.Log($"isLastRoute: {isLastRoute}");
        //     Debug.Log($"player: {player}");
        //     Debug.Break();
        // }
        //
        // Debug.Log($"player({player.TeamColor}) routeBetween: {routeBetween.ID}");

        // ------------------------------ This is FOW NOW ----------------------------------------------------------
        // TODO: We should get the route by looking for cardColor, right now we define it down below
        Route testRoute = routeBetween.Routes.FirstOrDefault(it => !it.InUse);

        // TODO: This is the card that should be considered the User has used from his hand
        var cardColor = testRoute.Color;

        // ------------------------------ end FOR NOW --------------------------------------------------------------

        finishTurnWithMovingWagons(player, routeBetween, cardColor);
    }

    void finishTurnWithMovingWagons(Player player, RouteBetween routeBetween, RouteColor cardColor) {
        player.WagonCount -= routeBetween.Distance;
        if (player.WagonCount <= 2) {
            // TODO: What happens if we no longer have wagons to complete a route ?
            // I guess we need to think about that
            // One thing can be, up before we pick a routeBetween. We should check if we have enough to complete it

            if (_lastRoundColor.HasValue) {
                // Someone else already activated the LAST ROUND, the first player to activate it counts
                // Debug.Log(
                //     $"player({player.TeamColor}): also reached a WagonCount of 2 or less, to bad he wasn't the first: {player}");
            }
            else {
                _lastRoundColor = player.TeamColor;
                Debug.Log($"player({player.TeamColor}): ACTIVATED LAST ROUND: {player}");
            }
        }

        // AI needs to do this to simulate user click
        Store2.SetFocusedID(routeBetween.ID);

        player.SetCompletedRoutes(routeBetween.ID);
        player.PreparedCardsColor = cardColor;

        _playerWagons.ClickPerformed();
    }

    void doLastRoundTurn(Player player) {
        // It's the last round, we should check if we can finish a mission
        RouteBetween routeBetween = null;
        foreach (var mission in player.Missions) {
            getMissionPath(mission, player.TeamColor, out List<PathTo> paths);
            var routeBetweens = getRouteBetweens(player, paths, out int costToComplete);

            if (costToComplete < player.WagonCount)
                continue;

            if (routeBetweens.Count == 0)
                continue;

            foreach (var rb in routeBetweens) {
                if (rb.Distance == player.WagonCount) {
                    routeBetween = rb;
                    break;
                }
            }

            if (routeBetween != null)
                break;
        }

        // Or if we can't finish a mission, we look to find a RouteBetween to populate it with our remaining wagons
        if (!routeBetween) {
            var remainingRouteBetween = new List<RouteBetween>();
            foreach (var kvp in _cityChecker.RoutesBetween) {
                var inUse = kvp.Value.Routes.Any(it => it.InUse);

                if (inUse) continue;

                if (kvp.Value.Distance > player.WagonCount) continue;

                remainingRouteBetween.Add(kvp.Value);
            }

            remainingRouteBetween = remainingRouteBetween.OrderByDescending(it => it.Distance).ToList();
            routeBetween = remainingRouteBetween.FirstOrDefault();

            // Debug.Break();
        }

        if (!routeBetween) {
            Debug.Log($"Player({player.TeamColor}) has no RouteBetween left to do right now. Game is ending?");
            return;
        }

        Debug.Log($"Player({player.TeamColor}) found a last routeBetween: {routeBetween}");

        // ------------------------------ This is FOW NOW ----------------------------------------------------------
        // TODO: We should get the route by looking for cardColor, right now we define it down below
        Route testRoute = routeBetween.Routes.FirstOrDefault(it => !it.InUse);

        // TODO: This is the card that should be considered the User has used from his hand
        var cardColor = testRoute.Color;

        // ------------------------------ end FOR NOW --------------------------------------------------------------

        finishTurnWithMovingWagons(player, routeBetween, cardColor);
    }

    List<RouteBetween> getRouteBetweens(Player player, List<PathTo> path, out int costToComplete) {
        var routesBetween = new List<RouteBetween>();
        costToComplete = 0;

        foreach (PathTo pathTo in path) {
            if (player.CompletedRouteIds.Contains(pathTo.routeBetweenID))
                continue;

            costToComplete += pathTo.cost;

            if (player.WagonCount < pathTo.cost) {
                // Debug.Log($"player{player.TeamColor} can't do this {pathTo}");
                // Debug.Log($"player.WagonCount: {player.WagonCount} < {pathTo.cost}");
                continue;
            }

            routesBetween.Add(_cityChecker.RoutesBetween[pathTo.routeBetweenID]);
        }

        return routesBetween;
    }

    List<Mission> createMissionPool() {
        var missionPool = new List<Mission>();
        foreach (var kvp in CardConstants.MISSION_CARD_COUNT) {
            var category = kvp.Key;
            int drawCount = kvp.Value;

            var availableMissions = new List<Mission>(CardConstants.MISSION_BANK[category]);

            for (int i = 0; i < drawCount && availableMissions.Count > 0; i++) {
                missionPool.Add(CardConstants.DrawWeighted(ref availableMissions));
            }
        }

        return missionPool;
    }

    void assignRandomMissionsToPlayer(Player player) {
        var missions = new List<Mission>();

        getRandomMissionFromPool(ref missions);
        getRandomMissionFromPool(ref missions);
        getRandomMissionFromPool(ref missions);

        Debug.Log($"Player picked 3 more missions. Mission pool count is {_missionPool.Count}");

        player.SetMissions(missions);
    }

    void getRandomMissionFromPool(ref List<Mission> missions) {
        var index = Random.Range(0, _missionPool.Count);
        var randomMission = _missionPool[index];

        // Debug.Log($"randomMission was picked: {randomMission}");
        missions.Add(randomMission);
        _missionPool.RemoveAt(index);
    }

    RouteColor getPlayerPreparedCardsColor() {
        var player = _players[_roundTurnIndex];
        return player.PreparedCardsColor;
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
