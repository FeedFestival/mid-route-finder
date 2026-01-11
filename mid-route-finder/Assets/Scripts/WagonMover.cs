using System;
using System.Collections.Generic;
using System.Linq;
using BezierSolution;
using DG.Tweening;
using UnityEngine;

public class WagonMover : MonoBehaviour {
    [SerializeField] int _initialPoolSize = 6;

    readonly Queue<BezierSpline> _availableSplines = new();

    void Awake() {
        for (int i = 0; i < _initialPoolSize; i++) {
            var spline = createSpline();
            returnSpline(spline);
        }
    }

    public void PlaceWagons(
        RouteBetween routeBetween,
        HashSet<Wagon> coloredWagons,
        Route route,
        RouteColor cardColor
    ) {
        for (int i = 0; i < routeBetween.Distance; i++) {
            var wagon = coloredWagons.FirstOrDefault();
            if (wagon == null) {
                Debug.LogError("Couldn't find any more wagons.");
                break;
            }

            SpatialData spatialData = route.GetPlaceholderSpatialDataAt(i);
            bool isLast = i == routeBetween.Distance - 1;

            move(
                wagon,
                spatialData,
                isLast
                    ? () => {
                        var areRoutesEmpty = routeBetween.RemoveRoute(cardColor);
                        if (areRoutesEmpty)
                            routeBetween.DisableInteractions();
                        else
                            routeBetween.EnableInteractions();

                        routeBetween = null;
                    }
                    : null
            );

            coloredWagons.Remove(wagon);
        }
    }

    void move(Wagon wagon, SpatialData spatialData, Action movementComplete = null) {
        BezierSpline spline = getSpline();

        var fromPos = wagon.transform.position;
        var toPos = spatialData.position;
        var fromRot = wagon.transform.rotation;
        var toRot = spatialData.rotation;

        spline[0].position = fromPos;

        var dir = toPos - fromPos;
        var middlePos = fromPos + (dir * 0.32f);
        middlePos.y = 32;

        spline[1].position = middlePos;
        spline[^1].position = toPos;

        float duration = 2f;

        DOVirtual.Float(0f, 1f, duration, t => {
                wagon.transform.position = spline.GetPoint(t);
                wagon.transform.rotation = Quaternion.Slerp(fromRot, toRot, t);
            })
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                returnSpline(spline);
                movementComplete?.Invoke();
            });
    }

    BezierSpline createSpline() {
        var spline = Instantiate(ResourceLibrary._.BezierSplinePrefab, transform);
        spline.gameObject.SetActive(false);
        return spline;
    }

    BezierSpline getSpline() {
        BezierSpline spline = _availableSplines.Count > 0
            ? _availableSplines.Dequeue()
            : createSpline();

        spline.gameObject.SetActive(true);
        return spline;
    }

    void returnSpline(BezierSpline spline) {
        spline.gameObject.SetActive(false);
        _availableSplines.Enqueue(spline);
    }
}
