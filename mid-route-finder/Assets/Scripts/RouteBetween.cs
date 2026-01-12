using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Game.Shared.Interfaces;
using Game.Shared.Interfaces.EntitySystem;
using UnityEngine;

public class RouteBetween : MonoBehaviour {
    internal ulong ID { get; private set; }
    internal int Distance;
    internal City FromCity;
    internal City ToCity;
    internal List<Route> Routes;

    [SerializeField] GameObject _plane;

    public void InitializeFromData(
        float physicalDistance,
        City fromCity,
        City toCity,
        routeSettings? routeSettings,
        float smallestDistance
    ) {
        ID = gameObject.GetComponent<IEntityId>().ID;
        FromCity = fromCity;
        ToCity = toCity;

        Distance = getWagonCount(routeSettings, physicalDistance, smallestDistance);
        Routes = new();
        gameObject.name = $"Route ({Distance}) {FromCity.ID}. {FromCity.Name} -> {ToCity.Name}";

        Vector3 from = FromCity.transform.position;
        Vector3 to = ToCity.transform.position;
        Vector3 dir = (to - from).normalized;
        float xScale = 1;

        var color = routeSettings?.Color ?? RouteColor.Gray;
        string colorName = color == RouteColor.Gray ? "default" : color.ToString();
        var routeGo = new GameObject($"Route {colorName}");
        routeGo.transform.parent = transform;
        Vector3 routePos = Vector3.Lerp(from, to, 0.5f);

        routeGo.transform.position = routePos;
        routeGo.transform.rotation = Quaternion.LookRotation(dir);

        var route = routeGo.AddComponent<Route>();
        route.Init(Distance, from, to, routeSettings?.EnforcedPlaceholderSizeRatio);
        Routes.Add(route);

        if (routeSettings.HasValue) {
            var settings = routeSettings.Value;
            route.ApplySettings(settings);

            if (settings.TwoWay) {
                var secondRouteGo = Instantiate(routeGo, transform);
                var secondRoute = secondRouteGo.GetComponent<Route>();
                secondRoute.ApplySettings(settings, settings.SecondColor);
                Routes.Add(secondRoute);

                float sideOffset = 0.42f;
                Vector3 side = Vector3.Cross(Vector3.up, dir).normalized;
                routeGo.transform.position += side * sideOffset;
                secondRouteGo.transform.position += side * (sideOffset * -1);

                xScale = 1.8f;
            }
        }

        positionPlane(ref _plane, from, to, xScale);
    }

    public void DisableInteractions() {
        var focusTrigger = gameObject.GetComponent<IFocusTrigger>();
        focusTrigger?.Disable();
    }

    public void EnableInteractions() {
        var focusTrigger = gameObject.GetComponent<IFocusTrigger>();
        focusTrigger?.Enable();
    }

    public bool RemoveRoute(RouteColor color) {
        Route route = Routes.Find(it => it.Color == color);
        Routes.Remove(route);
        Destroy(route.gameObject);

        return Routes.Count == 0;
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

    int getWagonCount(routeSettings? routeSettings, float physicalDistance, float smallestDistance) {
        int wagonCount = Mathf.Max(1, Mathf.FloorToInt(physicalDistance / smallestDistance));
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

[SuppressMessage("ReSharper", "InconsistentNaming")]
public struct RouteData {
    public readonly int id;
    public readonly string fromCityName;
    public readonly float distance;
    public readonly string toCityName;

    public RouteData(int id, float distance, string fromCityName, string toCityName) {
        this.id = id;
        this.distance = distance;
        this.fromCityName = fromCityName;
        this.toCityName = toCityName;
    }
}
