using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class CityChecker : MonoBehaviour {
    [Header("Transform Parents")]
    [SerializeField]
    Transform _citiesT;

    Transform _cityVisualizerT;
    Transform _routesT;

    List<City> _cities;
    List<RouteData> _routesBetween;

    void Awake() {
        clearData();
    }

    void Start() {
        drawRouteBetween();
    }

    void OnDestroy() {
        clearData();
    }

    [Button]
    void clearData() {
        _cities = null;
        _routesBetween = null;

        if (Application.isPlaying) {
            if (_cityVisualizerT != null)
                Destroy(_cityVisualizerT.gameObject);

            if (_routesT != null)
                Destroy(_routesT.gameObject);
        }
        else {
            if (_cityVisualizerT != null)
                DestroyImmediate(_cityVisualizerT.gameObject);

            if (_routesT != null)
                DestroyImmediate(_routesT.gameObject);
        }
    }

    [Button]
    void copyCityJsonToClipboard() {
        clearData();
        tryLoadInCities();

        string json = string.Empty;
        foreach (City city in _cities) {
            json += $"{city}, \n";
        }

        Debug.Log("Cities Copied to clipboard");
        GUIUtility.systemCopyBuffer = json;
    }

    [Button]
    void drawRouteBetween() {
        clearData();
        tryLoadInCities();

        if (_routesT == null)
            _routesT = new GameObject("_routesT").transform;

        float smallestDistance = ResourceLibrary._.WagonPlaceholderPrefab.transform.localScale.z;

        foreach (RouteData routeData in _routesBetween) {
            var routeGo = Instantiate(ResourceLibrary._.RouteBetweenPrefab, Vector3.zero, Quaternion.identity,
                _routesT);
            var routeBetween = routeGo.GetComponent<RouteBetween>();
            var fromCity = _cities.Find(it => it.Name == routeData.FromCityName);

            // TODO: make them neighbors
            var toCity = _cities.Find(it => it.Name == routeData.ToCityName);

            // TODO: make them neighbors

            RouteSettings? routeSettings = tryGetRouteSettings(routeData.FromCityName, routeData.ToCityName);

            routeBetween.InitializeFromData(
                routeData.Distance,
                fromCity,
                toCity,
                routeSettings,
                smallestDistance
            );
        }
    }

    [Button]
    void drawCityVisualizer() {
        if (_cities == null)
            tryLoadInCities();

        if (_cities == null) {
            Debug.LogError("No cities found");
        }
    }

    void tryLoadInCities() {
        _cities = new();

        if (_cityVisualizerT == null)
            _cityVisualizerT = new GameObject("_cityVisualizerT").transform;

        var usedRoutes = new HashSet<(int, int)>();
        var allowedIds = new List<int>();
        _routesBetween = new();
        int id = 99;

        foreach (Transform childT in _citiesT) {
            var cityGo = Instantiate(ResourceLibrary._.CityVisualizerPrefab, Vector3.zero, Quaternion.identity,
                _cityVisualizerT);
            var city = cityGo.GetComponent<City>();
            city.Init(childT.gameObject);

            var cityRoutes = new List<RouteData>();

            foreach (Transform otherChildT in _citiesT) {
                if (childT.gameObject.name == otherChildT.gameObject.name) {
                    continue;
                }

                var otherCity = City.GetCityData(otherChildT.gameObject.name);
                id++;

                int a = Mathf.Min(city.ID, otherCity.id);
                int b = Mathf.Max(city.ID, otherCity.id);

                float dist = Vector3.Distance(childT.position, otherChildT.position);
                var route = new RouteData(id, dist, city.Name, otherCity.name);
                cityRoutes.Add(route);

                if (usedRoutes.Contains((a, b)))
                    continue;

                _routesBetween.Add(route);
                usedRoutes.Add((a, b));
            }

            var routesIds = cityRoutes
                .OrderBy(r => r.Distance)
                .Take(city.MaxConnections)
                .Select(r => r.ID)
                .ToArray();

            allowedIds.AddRange(routesIds);

            _cities.Add(city);
        }

        _routesBetween = _routesBetween
            .Where(r => allowedIds.Contains(r.ID))
            .Where(it => {
                bool isFromCity = RouteConstants.BANNED_ROUTES.ContainsKey(it.FromCityName);
                bool isToCity = isFromCity && RouteConstants.BANNED_ROUTES[it.FromCityName].Contains(it.ToCityName);
                return !(isFromCity && isToCity);
            })
            .ToList();

        foreach (var requiredRoute in RouteConstants.REQUIRED_ROUTES) {
            id++;
            var fromCity = _cities.Find(it => it.Name == requiredRoute.FromCityName);
            var toCity = _cities.Find(it => it.Name == requiredRoute.ToCityName);
            float dist = Vector3.Distance(fromCity.transform.position, toCity.transform.position);
            _routesBetween.Add(new RouteData(id, dist, requiredRoute.FromCityName, requiredRoute.ToCityName));
        }
    }

    RouteSettings? tryGetRouteSettings(string fromName, string toName) {
        bool hasSettings = RouteConstants.ROUTES_SETTINGS.ContainsKey(fromName);
        if (!hasSettings) return null;
        hasSettings = RouteConstants.ROUTES_SETTINGS[fromName].ContainsKey(toName);
        if (!hasSettings) return null;

        return RouteConstants.ROUTES_SETTINGS[fromName][toName];
    }
}
