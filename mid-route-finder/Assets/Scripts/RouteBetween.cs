using Game.Shared.Interfaces;
using UnityEngine;

public class RouteBetween : MonoBehaviour {
    internal City FromCity;
    internal float Distance;
    internal City ToCity;

    [SerializeField] GameObject _plane;

    public void InitializeFromData(
        float distance,
        City fromCity,
        City toCity,
        routeSettings? routeSettings,
        float smallestDistance
    ) {
        Distance = distance;
        FromCity = fromCity;
        ToCity = toCity;

        int wagonCount = getWagonCount(routeSettings, smallestDistance);
        gameObject.name = $"Route ({wagonCount}) {FromCity.ID}. {FromCity.Name} -> {ToCity.Name}";

        Vector3 from = FromCity.transform.position;
        Vector3 to = ToCity.transform.position;
        Vector3 dir = (to - from).normalized;
        float xScale = 1;

        string colorName = routeSettings.HasValue ? routeSettings.Value.Color.ToString() : "default";
        var routeGo = new GameObject($"Route {colorName}");
        routeGo.transform.parent = transform;
        Vector3 routePos = Vector3.Lerp(from, to, 0.5f);

        routeGo.transform.position = routePos;
        routeGo.transform.rotation = Quaternion.LookRotation(dir);

        var route = routeGo.AddComponent<Route>();
        route.Init(wagonCount, from, to, routeSettings?.EnforcedPlaceholderSizeRatio);

        if (routeSettings.HasValue) {
            var settings = routeSettings.Value;
            route.ApplySettings(settings);

            if (settings.TwoWay) {
                var secondRouteGo = Instantiate(routeGo, transform);
                var secondRoute = secondRouteGo.GetComponent<Route>();
                secondRoute.ApplySettings(settings, settings.SecondColor);

                float sideOffset = 0.42f;
                Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;
                routeGo.transform.position += side * sideOffset;
                secondRouteGo.transform.position += side * (sideOffset * -1);

                xScale = 1.8f;
            }
        }

        positionPlane(ref _plane, from, to, xScale);
    }

    void positionPlane(ref GameObject go, Vector3 from, Vector3 to, float xScale) {
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        go.transform.position = from;
        go.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        Vector3 s = go.transform.localScale;
        s.z = dist;
        s.x = xScale;
        go.transform.localScale = s;

        var focusTrigger = GetComponent<IFocusTrigger>();
        (focusTrigger as RouteFocusTrigger)?.AlignAndScaleToFit(from, dir, go.transform.localScale.y, dist);
    }

    int getWagonCount(routeSettings? routeSettings, float smallestDistance) {
        int wagonCount = Mathf.Max(1, Mathf.FloorToInt(Distance / smallestDistance));
        if (routeSettings.HasValue && routeSettings.Value.EnforcedLength > 0) {
            wagonCount = routeSettings.Value.EnforcedLength;
        }

        return wagonCount;
    }

    public override string ToString() {
        return $@"{{
        ""distance"": {Distance},
        ""fromCity"": {{
            ""id"": {FromCity.ID},
            ""name"": ""{FromCity.Name}""
        }}
        ""toCity"": {{
            ""id"": {ToCity.ID},
            ""name"": ""{ToCity.Name}""
        }}
    }}";
    }
}

public struct routeData {
    public readonly int ID;
    public readonly string FromCityName;
    public readonly float Distance;
    public readonly string ToCityName;

    public routeData(int id, float distance, string fromCityName, string toCityName) {
        ID = id;
        Distance = distance;
        FromCityName = fromCityName;
        ToCityName = toCityName;
    }
}
