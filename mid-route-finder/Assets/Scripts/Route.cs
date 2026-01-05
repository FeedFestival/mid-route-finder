using System;
using UnityEngine;

public class Route : MonoBehaviour {
    GameObject[] _placeholders;

    public void Init(
        int wagonsCount,
        Vector3 from,
        Vector3 to,
        float? enforcedPlaceholderSizeRatio
    ) {
        _placeholders = new GameObject[wagonsCount];

        float segmentLength = Route.getSegmentLength(wagonsCount, from, to, enforcedPlaceholderSizeRatio);

        for (int i = 0; i < wagonsCount; i++) {
            float t = (i + 1f) / (wagonsCount + 1f);
            Vector3 position = Vector3.Lerp(from, to, t);
            var placeholder = Instantiate(ResourceLibrary._.WagonPlaceholderPrefab, position, Quaternion.identity,
                transform);

            placeholder.gameObject.name = $"placeholder {i}";


            _placeholders[i] = placeholder;
        }

        for (int i = 0; i < _placeholders.Length; i++) {
            Vector3 targetPosition;

            if (i < _placeholders.Length - 1) {
                targetPosition = _placeholders[i + 1].transform.position;
            }
            else {
                // Last one looks at the destination city
                targetPosition = to;
            }

            Transform tr = _placeholders[i].transform;
            Vector3 direction = targetPosition - tr.position;

            if (direction != Vector3.zero) {
                tr.rotation = Quaternion.LookRotation(direction);
            }

            // Scale on local Z to match spacing
            Vector3 scale = tr.localScale;
            scale.z = segmentLength;
            tr.localScale = scale;
        }
    }

    static float getSegmentLength(int wagonsCount, Vector3 from, Vector3 to, float? enforcedPlaceholderSizeRatio) {
        if (!enforcedPlaceholderSizeRatio.HasValue) {
            float totalDistance = Vector3.Distance(from, to);
            return totalDistance / (wagonsCount + 1.5f);
        }

        return ResourceLibrary._.WagonPlaceholderPrefab.transform.localScale.z *
               enforcedPlaceholderSizeRatio.Value;
    }

    public void ApplySettings(RouteSettings routeSettings, RouteColor? routeColor = null) {
        if (!routeColor.HasValue) {
            routeColor = routeSettings.Color;
        }

        gameObject.name = $"Route {routeColor.ToString()}";

        if (_placeholders == null) {
            _placeholders = new GameObject[transform.childCount];
            int i = 0;
            foreach (Transform childT in transform) {
                _placeholders[i] = childT.gameObject;
                i++;
            }
        }

        // size is 80%

        if (routeColor != RouteColor.Gray) {
            foreach (var placeholder in _placeholders) {
                var renderer = placeholder.GetComponent<Renderer>();
                renderer.material = ResourceLibrary._.ColorMaterials[routeColor.Value];
            }
        }
    }
}
