using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class TurnPlay : MonoBehaviour {
    [SerializeField] PlayerWagons _playerWagons;
    [SerializeField] CityChecker _cityChecker;

    int _roundTurnIndex = -1;
    int _roundIndex = 0;
    int _turnIndex = -1;

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    struct Player {
        public TeamColor teamColor;

        public Player(TeamColor teamColor) {
            this.teamColor = teamColor;
        }
    }

    List<Player> _players;

    void Start() {
        _players = new();
        _players.Add(new(TeamColor.Blue));
        _players.Add(new(TeamColor.Red));
        _players.Add(new(TeamColor.Yellow));
        _players.Add(new(TeamColor.Black));
        _players.Add(new(TeamColor.Green));

        nextTurn();
    }

    void nextTurn() {
        _roundTurnIndex++;
        _turnIndex++;

        if (_roundTurnIndex == _players.Count - 1) {
            _roundIndex++;
            _roundTurnIndex = 0;
        }

        var player = _players[_roundTurnIndex];



        // _cityChecker.RoutesBetween

        _playerWagons.ClickPerformed();
    }
}
