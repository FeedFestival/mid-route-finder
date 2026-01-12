using System.Collections.Generic;
using System.Linq;
using Game.Shared.Interfaces.EntitySystem;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

public class CityChecker : MonoBehaviour {
    [Header("Transform Parents")]
    [SerializeField]
    Transform _citiesT;

    Transform _cityVisualizerT;
    Transform _routesT;

    internal List<City> Cities;
    internal Dictionary<ulong, RouteBetween> RoutesBetween { get; private set; }

    public void DisableCityPath(int fromCityID, int toCityID, TeamColor color) {
        var fromCity = Cities.Find(it => it.ID == fromCityID);
        var toCity = Cities.Find(it => it.ID == toCityID);

        int index = fromCity.Paths.FindIndex(it => it.to.ID == toCity.ID);
        var path = fromCity.Paths[index];
        var modifiedPath = new PathTo(path, color, false);
        fromCity.Paths.RemoveAt(index);
        fromCity.Paths.Add(modifiedPath);

        index = toCity.Paths.FindIndex(it => it.to.ID == fromCity.ID);
        path = toCity.Paths[index];
        modifiedPath = new PathTo(path, color, false);
        toCity.Paths.RemoveAt(index);
        toCity.Paths.Add(modifiedPath);
    }

    void Awake() {
        var routesData = loadCitiesAndCreateRoutesData();
        drawRouteBetween(routesData);
    }

    void drawRouteBetween(List<RouteData> routesData) {
        if (_routesT == null)
            _routesT = new GameObject("_routesT").transform;

        RoutesBetween = new();
        float smallestDistance = ResourceLibrary._.WagonPlaceholderPrefab.transform.localScale.z;

        foreach (RouteData routeData in routesData) {
            var routeGo = Instantiate(ResourceLibrary._.RouteBetweenPrefab, Vector3.zero, Quaternion.identity,
                _routesT);
            var routeBetween = routeGo.GetComponent<RouteBetween>();
            var fromCity = Cities.Find(it => it.Name == routeData.fromCityName);
            var toCity = Cities.Find(it => it.Name == routeData.toCityName);
            routeSettings? routeSettings = tryGetRouteSettings(routeData.fromCityName, routeData.toCityName);

            routeBetween.InitializeFromData(
                routeData.distance,
                fromCity,
                toCity,
                routeSettings,
                smallestDistance
            );

            if (fromCity.Paths == null)
                fromCity.Paths = new();
            fromCity.Paths.Add(new(toCity, routeBetween.ID, routeBetween.Distance));

            if (toCity.Paths == null)
                toCity.Paths = new();
            toCity.Paths.Add(new(fromCity, routeBetween.ID, routeBetween.Distance));

            RoutesBetween.Add(routeBetween.GetComponent<IEntityId>().ID, routeBetween);
        }
    }

    List<RouteData> loadCitiesAndCreateRoutesData() {
        Cities = new();

        if (_cityVisualizerT == null)
            _cityVisualizerT = new GameObject("_cityVisualizerT").transform;

        var usedRoutes = new HashSet<(int, int)>();
        var allowedIds = new List<int>();
        List<RouteData> routesData = new();
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

                routesData.Add(route);
                usedRoutes.Add((a, b));
            }

            var routesIds = cityRoutes
                .OrderBy(r => r.distance)
                .Take(city.MaxConnections)
                .Select(r => r.id)
                .ToArray();

            allowedIds.AddRange(routesIds);

            Cities.Add(city);
        }

        routesData = routesData
            .Where(r => allowedIds.Contains(r.id))
            .Where(it => {
                bool isFromCity = RouteConstants.BANNED_ROUTES.ContainsKey(it.fromCityName);
                bool isToCity = isFromCity && RouteConstants.BANNED_ROUTES[it.fromCityName].Contains(it.toCityName);
                return !(isFromCity && isToCity);
            })
            .ToList();

        foreach (var requiredRoute in RouteConstants.REQUIRED_ROUTES) {
            id++;
            var fromCity = Cities.Find(it => it.Name == requiredRoute.FromCityName);
            var toCity = Cities.Find(it => it.Name == requiredRoute.ToCityName);
            float dist = Vector3.Distance(fromCity.transform.position, toCity.transform.position);
            routesData.Add(new RouteData(id, dist, requiredRoute.FromCityName, requiredRoute.ToCityName));
        }

        return routesData;
    }

    routeSettings? tryGetRouteSettings(string fromName, string toName) {
        bool hasSettings = RouteConstants.ROUTES_SETTINGS.ContainsKey(fromName);
        if (!hasSettings) return null;
        hasSettings = RouteConstants.ROUTES_SETTINGS[fromName].ContainsKey(toName);
        if (!hasSettings) return null;

        return RouteConstants.ROUTES_SETTINGS[fromName][toName];
    }

#if UNITY_EDITOR
    [Title("Play Mode Debug", "Method that can run in PlayMode")]
    [Button]
    void copyCityJsonToClipboard() {
        if (!Application.isPlaying) return;

        string json = string.Empty;
        foreach (City city in Cities) {
            json += $"{city}, \n";
        }

        Debug.Log("Cities Copied to clipboard");
        GUIUtility.systemCopyBuffer = json;
    }

    [ButtonGroup]
    void findShortestPathTest() {
        if (!Application.isPlaying) return;

        var fromCity = Cities.Find(it => it.Name == "Arad");
        var toCity = Cities.Find(it => it.Name == "Constanta");

        var hasPath = Pathfinding.FindShortestPath(fromCity, toCity, out List<City> shortestPath, out int totalCost);

        if (!hasPath)
            Debug.Log("No path found");
    }

    [ButtonGroup]
    void calculateAllPossibleMissions() {
        if (!Application.isPlaying) return;

        HashSet<Mission> missions = new();

        int minCost = 4;
        int maxImportancePoints = (Cities.Count * 2) + 2;
        foreach (City cityA in Cities) {
            foreach (City cityB in Cities) {
                if (cityA.ID == cityB.ID) continue;

                Pathfinding.FindShortestPath(cityA, cityB, out List<City> cities, out int cost);

                int pathCount = 0;
                foreach (City city in cities) {
                    if (city.ID != cityA.ID)
                        pathCount++;
                }

                if (pathCount < 4) continue;

                // Debug.Log($"pathCount: {pathCount}");

                if (cost < minCost) continue;

                missions.Add(new(cityA, cityB, cost, maxImportancePoints - cityA.ID - cityB.ID));
            }
        }

        var sortedMissions = missions.OrderByDescending(it => it.cost).ToArray();
        var missionBank = new Dictionary<Category, Mission[]>() {
            {
                Category.Local, CardConstants.GetCategoryMissions(sortedMissions, Category.Local)
            }, {
                Category.Regional, CardConstants.GetCategoryMissions(sortedMissions, Category.Regional)
            }, {
                Category.InterRegional, CardConstants.GetCategoryMissions(sortedMissions, Category.InterRegional)
            }, {
                Category.Long, CardConstants.GetCategoryMissions(sortedMissions, Category.Long)
            }, {
                Category.Epic, CardConstants.GetCategoryMissions(sortedMissions, Category.Epic)
            }
        };

        int missionCount = 0;
        string s = string.Empty;
        foreach (var kvp in missionBank) {
            var category = kvp.Key;
            var categoryMissions = kvp.Value;
            s += $@"{{
    Category.{category.ToString()}, new Mission[{categoryMissions.Length}] {{";
            foreach (Mission mission in categoryMissions) {
                missionCount++;
                s += $@"
        new(""{mission.cityA}"", ""{mission.cityB}"", {mission.cost}, {mission.importancePoints}),";
            }

            s += $@"
    }}
}},";
        }

        // Copy to clipboard
        EditorGUIUtility.systemCopyBuffer = s;

        Debug.Log($"{missionCount} possible mission were copied to clipboard! Should paste it in CardConstants MISSION_BANK Dictionary");
    }
#endif
}
