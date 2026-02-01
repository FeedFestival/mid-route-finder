using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Random = UnityEngine.Random;

public class CardDeck : MonoBehaviour {
    internal bool IsDiscardedEmpty = false;
    internal Stack<CardColor> DeckCards;
    internal List<CardColor> FaceUpCards;
    List<CardColor> _discardedCards;

    void Awake() {
        DeckCards = new();
        FaceUpCards = new();
    }

    void Start() {
        _discardedCards = new();
        foreach (var cardColor in CardConstants.CardColors) {
            for (int i = 0; i < 12; i++) {
                _discardedCards.Add(cardColor);
            }
        }

        shuffle();
        dealFaceUp();
    }

    public bool DrawFromDeck(int count, out CardColor[] cardColors) {
        CardColor[] drawCards = new CardColor[count];

        try {
            for (int i = 0; i < count; i++) {
                drawCards[i] = DrawFromDeck();
            }
        }
        catch (InvalidOperationException ex) {
            Debug.LogWarning($"We probably can't pick anymore cards as there aren't any: {ex.Message}");

            cardColors = null;
            return false;
        }

        cardColors = drawCards;
        return true;
    }

    public CardColor DrawFromDeck() {
        if (DeckCards.Count == 0) {
            shuffle();
        }

        CardColor card = DeckCards.Pop();
        return card;
    }

    public bool TakeFromFaceUp(CardColor card) {
        int index = FaceUpCards.FindIndex(it => it == card);

        FaceUpCards.RemoveAt(index);
        try {
            FaceUpCards.Add(DrawFromDeck());
        }
        catch (InvalidOperationException e) {
            return false;
        }

        return true;
    }

    public void AddToDiscardPile(CardColor cardColor, int count) {
        for (int i = 0; i < count; i++) {
            _discardedCards.Add(cardColor);
        }

        if (FaceUpCards.Count < 5) {
            // _discardedCards were empty and FaceUpCards were left incomplete

            int sum = FaceUpCards.Count + _discardedCards.Count;
            bool canCompleteFaceUp = sum >= 5;
            if (canCompleteFaceUp) {
                int diff = 5 - FaceUpCards.Count;
                for (int i = 0; i < diff; i++) {
                    FaceUpCards.Add(_discardedCards[i]);
                    _discardedCards.RemoveAt(i);
                }

                Assert.AreEqual(FaceUpCards.Count, 5);
                Assert.IsTrue(_discardedCards.Count >= 0);
            }
            else {
                Debug.LogWarning(
                    $"But we still can't complete FaceUpCards as the sum ({sum}) of discarded and FaceUp don't make up for 5");
            }
        }

        IsDiscardedEmpty = _discardedCards.Count == 0;
    }

    void dealFaceUp() {
        FaceUpCards.Clear();
        for (int i = 0; i < 5; i++) {
            FaceUpCards.Add(DrawFromDeck());
        }
    }

    void shuffle() {
        int discardedCardsCount = _discardedCards.Count;
        if (discardedCardsCount == 0) {
            IsDiscardedEmpty = true;
            return;
        }

        for (int i = discardedCardsCount - 1; i > 0; i--) {
            int j = Random.Range(0, i + 1);
            (_discardedCards[i], _discardedCards[j]) = (_discardedCards[j], _discardedCards[i]);
        }

        foreach (CardColor cardColor in _discardedCards) {
            DeckCards.Push(cardColor);
        }

        _discardedCards.Clear();

        Debug.Log(
            $"Shuffle (Fisher–Yates using UnityEngine.Random) from ({discardedCardsCount}) _discardedCards ({_discardedCards.Count}) -> {DeckCards.Count}");
    }
}
