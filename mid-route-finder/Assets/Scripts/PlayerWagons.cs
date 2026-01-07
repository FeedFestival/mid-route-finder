using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerWagons : MonoBehaviour {
    [SerializeField] Transform _blueWagonT;
    [SerializeField] Transform _redWagonT;
    [SerializeField] Transform _yellowWagonT;
    [SerializeField] Transform _blackWagonT;
    [SerializeField] Transform _greenWagonT;

    Dictionary<TeamColor, Transform> _wagonsTs;
    Dictionary<TeamColor, List<Wagon>> _wagons;

    int _maxWagonsCount = 45;
    int _currentWagonIndex;

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

        StartCoroutine(spawnWagonsCoroutine());
    }

    void onWagonReady(Wagon wagon) {
        _wagons[wagon.TeamColor].Add(wagon);

        Debug.Log($"[{wagon.TeamColor}].Count: {_wagons[wagon.TeamColor].Count}");

        if (_wagons[wagon.TeamColor].Count == _maxWagonsCount) {
            teamReady(wagon.TeamColor);
        }
    }

    IEnumerator spawnWagonsCoroutine() {
        while (_currentWagonIndex < _maxWagonsCount) {
            foreach (KeyValuePair<TeamColor, Transform> kvpWagon in _wagonsTs) {
                var t = kvpWagon.Value;

                createWagon(t, kvpWagon.Key);
            }

            _currentWagonIndex++;

            yield return new WaitForSeconds(0.18f);
        }
    }

    void createWagon(Transform t, TeamColor color) {
        var go = PlayerWagons.Instantiate(ResourceLibrary._.WagonPrefab, Vector3.zero, Quaternion.identity, t);

        var pos = t.position;
        pos.y = t.position.y + Random.Range(32, 64);
        Vector3 offset = Random.insideUnitSphere * 4f;
        pos += offset;

        go.transform.position = pos;
        go.transform.rotation = Random.rotation;

        var wagon = go.GetComponent<WagonRigidbody>();
        wagon.Init(color, onWagonReady);
    }

    void teamReady(TeamColor teamColor) {
        _wagons[teamColor] = _wagons[teamColor].OrderBy(w => w.transform.position.y).ToList();
        foreach (Wagon wagon in _wagons[teamColor]) {
            wagon.RemoveRigidbody();
        }
    }
}
