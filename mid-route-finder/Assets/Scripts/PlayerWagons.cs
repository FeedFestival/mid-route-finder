using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Player_Input;
using Game.Shared.Core.Store;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerWagons : MonoBehaviour {
    [SerializeField] Transform _blueWagonT;
    [SerializeField] Transform _redWagonT;
    [SerializeField] Transform _yellowWagonT;
    [SerializeField] Transform _blackWagonT;
    [SerializeField] Transform _greenWagonT;

    [SerializeField] GameObject _invisibleWallGo;

    [Header("City Checker ?")]
    [SerializeField]
    CityChecker _cityChecker;

    [SerializeField] WagonMover _wagonMover;

    [SerializeField] bool _initializeWithFallingWagons = false;

    Dictionary<TeamColor, Transform> _wagonsTs;
    Dictionary<TeamColor, List<Wagon>> _wagons;
    static readonly Vector3 OutOfViewSpawnPosition = new(999, 999, 0);

    int _maxWagonsCount = 45;
    int _currentWagonIndex;
    int _readyTeams;
    int _maxTeams = 5;
    Func<RouteColor> _getPlayerPreparedCardsColor;
    Func<TeamColor> _getPlayerTeamColor;

    void Awake() {
        _wagonsTs = new();
        _wagonsTs.Add(TeamColor.Blue, _blueWagonT);
        _wagonsTs.Add(TeamColor.Red, _redWagonT);
        _wagonsTs.Add(TeamColor.Yellow, _yellowWagonT);
        _wagonsTs.Add(TeamColor.Black, _blackWagonT);
        _wagonsTs.Add(TeamColor.Green, _greenWagonT);
    }

    void Start() {
        _wagons = new();
        _wagons.Add(TeamColor.Blue, new());
        _wagons.Add(TeamColor.Red, new());
        _wagons.Add(TeamColor.Yellow, new());
        _wagons.Add(TeamColor.Black, new());
        _wagons.Add(TeamColor.Green, new());

        var d = Disposable.CreateBuilder();

        Store2.State.Gameplay.Subscribe((gameplay) => {
            if (!gameObject.activeSelf) return;

            if (gameplay == Gameplay.Exploration) {
                PlayerInput.ExplorationActionMap.ConfirmSelect += ClickPerformed;
            }
            else {
                PlayerInput.ExplorationActionMap.ConfirmSelect -= ClickPerformed;
            }
        }).AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);

        if (_initializeWithFallingWagons) {
            StartCoroutine(spawnWagonsCoroutine());
        }
        else {
            // This can be used to resume a session or for debuging reasons, so we don't wait for cubes to fall

            foreach (TeamColor color in PlayerConstants.TEAM_COLORS) {
                var spatialDatas = PlayerConstants.WAGON_PLACEMENT[color];
                for (int i = 0; i < spatialDatas.Length; i++) {
                    var fakeFallingWagon = new fakeFallingWagon(spatialDatas[i], color);
                    onWagonReady(fakeFallingWagon);
                }
            }

            PlayerWagons.Destroy(_invisibleWallGo);
        }
    }

#if UNITY_EDITOR
    [BoxGroup("Editor Debug Functions")]
    [Button]
    void captureWagonPlacement() {
        if (_wagons == null) return;

        string s = string.Empty;
        foreach (var color in PlayerConstants.TEAM_COLORS) {
            s += $@"{{
    TeamColor.{color}, new SpatialData[{_wagons[color].Count}] {{
";
            foreach (Wagon wagon in _wagons[color]) {
                var p = wagon.transform.position;
                var q = wagon.transform.rotation;
                s += $@"
        new(new Vector3({p.x}f, {p.y}f, {p.z}f), new Quaternion({q.x}f, {q.y}f, {q.z}f, {q.w}f)),";
            }

            s += $@"
    }}
}},";
        }

        // Copy to clipboard
        EditorGUIUtility.systemCopyBuffer = s;

        Debug.Log("Wagon placement copied to clipboard! Should paste it in PlayerConstants WAGON_PLACEMENT Dictionary");
    }
#endif

    internal void SetTurnPlayDelegates(
        Func<RouteColor> getPlayerPreparedCardsColor,
        Func<TeamColor> getPlayerTeamColor
    ) {
        _getPlayerPreparedCardsColor = getPlayerPreparedCardsColor;
        _getPlayerTeamColor = getPlayerTeamColor;
    }

    internal void ClickPerformed() {
        var entityId = Store2.State.FocusedID.CurrentValue;
        if (entityId == 0) return;

        var cardColor = _getPlayerPreparedCardsColor();
        var routeBetween = _cityChecker.RoutesBetween[entityId];

        Route route = routeBetween.Routes.Find(it => it.Color == cardColor && !it.InUse);
        if (!route) {
            Debug.LogError(
                $"You selected a RouteBetween {routeBetween.gameObject.name} that has no more Routes. How did this happen?");
            return;
        }

        route.InUse = true;
        var userColor = _getPlayerTeamColor();
        var isRouteBlocked = routeBetween.Routes.Where(it => !it.InUse).Count() == 0;
        if (isRouteBlocked) {
            Store2.SetFocusedID(0);
            Store2.SetFocusedInstanceID(-1);
            _cityChecker.DisableCityPath(routeBetween.FromCity.ID, routeBetween.ToCity.ID, userColor);
            routeBetween.DisableInteractions();
        }
        else {
            // TODO: modify _cityChecker -> City Paths -> To know what colors are on them
        }

        var userWagons = _wagons[userColor];
        if (userWagons == null || userWagons.Count == 0) {
            Debug.LogError("Couldn't find any more wagons. I guess you won.");
            return;
        }

        HashSet<Wagon> coloredWagons = new();
        for (int i = 0; i < routeBetween.Distance; i++) {
            var wagon = userWagons.FirstOrDefault();
            coloredWagons.Add(wagon);
            userWagons.Remove(wagon);
        }

        _wagonMover.PlaceWagons(routeBetween, coloredWagons, route, cardColor);
    }

    void onWagonReady(IFallingWagon fallingWagon) {
        var color = fallingWagon.TeamColor;
        var wagon = createWagon(_wagonsTs[color], fallingWagon);

        _wagons[color].Add(wagon);

        if (_wagons[color].Count == _maxWagonsCount) {
            _readyTeams++;

            tryStartGame();
        }
    }

    IEnumerator spawnWagonsCoroutine() {
        while (_currentWagonIndex < _maxWagonsCount) {
            foreach (KeyValuePair<TeamColor, Transform> kvpWagon in _wagonsTs) {
                var t = kvpWagon.Value;

                createFallingWagon(t, kvpWagon.Key);
            }

            _currentWagonIndex++;

            yield return new WaitForSeconds(0.18f);
        }

        PlayerWagons.Destroy(_invisibleWallGo);
    }

    void createFallingWagon(Transform t, TeamColor color) {
        var go = PlayerWagons.Instantiate(ResourceLibrary._.WagonRigidbodyPrefab, Vector3.zero, Quaternion.identity, t);

        var pos = t.position;
        pos.y = t.position.y + Random.Range(32, 64);
        Vector3 offset = Random.insideUnitSphere * 4f;
        pos += offset;

        go.transform.position = pos;
        go.transform.rotation = Random.rotation;

        var wagon = go.GetComponent<FallingWagon>();
        wagon.Init(color, onWagonReady);
    }

    Wagon createWagon(Transform t, IFallingWagon fallingWagon) {
        var go = PlayerWagons.Instantiate(
            ResourceLibrary._.WagonPrefab, OutOfViewSpawnPosition, Quaternion.identity, t
        );

        var wagon = go.GetComponent<Wagon>();
        wagon.Init(fallingWagon);
        return wagon;
    }

    void tryStartGame() {
        if (_readyTeams == _maxTeams) {
            foreach (TeamColor teamColor in PlayerConstants.TEAM_COLORS) {
                _wagons[teamColor] = _wagons[teamColor].OrderBy(w => w.transform.position.y).ForEach(w => w.Place())
                    .ToList();
            }
        }
    }
}
