using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using Game.Shared.Constants.Store;
using Game.Shared.Core.Player_Input;
using Game.Shared.Core.Store;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Unity.Mathematics.Geometry;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

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

    [SerializeField] BezierSpline _bezierSpline;

    Dictionary<TeamColor, Transform> _wagonsTs;
    Dictionary<TeamColor, List<Wagon>> _wagons;
    static readonly Vector3 OutOfViewSpawnPosition = new Vector3(999, 999, 0);

    int _maxWagonsCount = 45;
    int _currentWagonIndex;
    int _readyTeams = 0;
    int _maxTeams = 5;

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
                PlayerInput.ExplorationActionMap.ConfirmSelect += clickPerformed;
            }
            else {
                PlayerInput.ExplorationActionMap.ConfirmSelect -= clickPerformed;
            }
        }).AddTo(ref d);

        d.RegisterTo(this.destroyCancellationToken);

        StartCoroutine(spawnWagonsCoroutine());
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

    void clickPerformed() {
        var entityId = Store2.State.FocusedID.CurrentValue;
        if (entityId == 0) return;

        Debug.Log($"clickPerformed -> entityId: {entityId}");

        var routesBetween = _cityChecker.GetRoutesBetween();
        var routeBetween = routesBetween[entityId];

        TeamColor randomColor =
            (TeamColor)UnityEngine.Random.Range(
                0,
                System.Enum.GetValues(typeof(TeamColor)).Length
            );

        var coloredWagons = _wagons[randomColor];

        if (coloredWagons == null || coloredWagons.Count == 0) return;

        for (int i = 0; i < routeBetween.RouteCost; i++) {
            var wagon = coloredWagons.FirstOrDefault();
            if (wagon == null) {
                Debug.LogError("Couldn't find any more wagons");
                break;
            }

            moveWagonToPlaceholderPosition(wagon, routeBetween.PlaceholderPositions[i]);

            coloredWagons.Remove(wagon);
        }

        routeBetween.DisableInteractions();
    }

    void onWagonReady(WagonRigidbody wagonRb) {
        var color = wagonRb.TeamColor;
        var wagon = createWagon(_wagonsTs[color], wagonRb);

        _wagons[color].Add(wagon);

        if (_wagons[color].Count == _maxWagonsCount) {
            _readyTeams++;

            if (_readyTeams == _maxTeams) {
                foreach (TeamColor teamColor in PlayerConstants.TEAM_COLORS) {
                    _wagons[teamColor] = _wagons[teamColor].OrderBy(w => w.transform.position.y).ForEach(w => w.Place())
                        .ToList();
                }
            }
        }
    }

    IEnumerator spawnWagonsCoroutine() {
        int indexWhenToDestroyInvisibleWall = Mathf.FloorToInt(_maxWagonsCount * 0.90f);
        while (_currentWagonIndex < _maxWagonsCount) {
            foreach (KeyValuePair<TeamColor, Transform> kvpWagon in _wagonsTs) {
                var t = kvpWagon.Value;

                createFallingWagon(t, kvpWagon.Key);
            }

            _currentWagonIndex++;
            if (_currentWagonIndex == indexWhenToDestroyInvisibleWall)
                PlayerWagons.Destroy(_invisibleWallGo);

            yield return new WaitForSeconds(0.18f);
        }
    }

    void createFallingWagon(Transform t, TeamColor color) {
        var go = PlayerWagons.Instantiate(ResourceLibrary._.WagonRigidbodyPrefab, Vector3.zero, Quaternion.identity, t);

        var pos = t.position;
        pos.y = t.position.y + Random.Range(32, 64);
        Vector3 offset = Random.insideUnitSphere * 4f;
        pos += offset;

        go.transform.position = pos;
        go.transform.rotation = Random.rotation;

        var wagon = go.GetComponent<WagonRigidbody>();
        wagon.Init(color, onWagonReady);
    }

    Wagon createWagon(Transform t, WagonRigidbody wagonRb) {
        var go = PlayerWagons.Instantiate(ResourceLibrary._.WagonPrefab, OutOfViewSpawnPosition,
            wagonRb.transform.rotation, t);

        var wagon = go.GetComponent<Wagon>();
        wagon.Init(wagonRb);
        return wagon;
    }

    void moveWagonToPlaceholderPosition(Wagon wagon, SpatialData placeholder) {
        Debug.Log($"_bezierSpline.Count: {_bezierSpline.Count}");

        float t = 0.1f;

        _bezierSpline.GetPoint(t);

        for (int i = 0; i < _bezierSpline.Count; i++) {
            Debug.Log($"_bezierSpline[i].gameObject: {_bezierSpline[i].gameObject}");
        }

        wagon.transform.position = placeholder.position;
        wagon.transform.rotation = placeholder.rotation;
    }
}
