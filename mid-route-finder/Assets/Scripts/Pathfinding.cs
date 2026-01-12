using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding {
    public static bool FindShortestPath(
        City start,
        City goal,
        out List<City> cities,
        out int totalCost
    ) {
        cities = new List<City>();
        totalCost = 0;

        var distances = new Dictionary<City, int>();
        var previous = new Dictionary<City, City>();
        var unvisited = new HashSet<City>();

        // Initialize
        foreach (var city in GetAllConnectedCities(start)) {
            distances[city] = int.MaxValue;
            unvisited.Add(city);
        }

        distances[start] = 0;

        while (unvisited.Count > 0) {
            // Get city with smallest tentative distance
            City current = unvisited
                .OrderBy(c => distances[c])
                .First();

            if (current == goal)
                break;

            unvisited.Remove(current);

            foreach (var pathTo in current.Paths) {
                if (!pathTo.available)
                    continue;

                City neighbor = pathTo.to;

                if (!unvisited.Contains(neighbor))
                    continue;

                int alt = distances[current] + pathTo.cost;

                if (alt < distances[neighbor]) {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }

        // No path
        if (!previous.ContainsKey(goal) && start != goal)
            return false;

        // Reconstruct path
        City step = goal;
        while (step != null) {
            cities.Insert(0, step);
            previous.TryGetValue(step, out step);
        }

        totalCost = distances[goal];
        return true;
    }

    public static bool FindShortestPathEdges(
        City start,
        City goal,
        TeamColor color,
        out List<PathTo> paths

        // out int totalCost
    ) {
        paths = new List<PathTo>();

        // totalCost = 0;

        var distances = new Dictionary<City, int>();
        var previous = new Dictionary<City, (City from, PathTo edge)>();
        var visited = new HashSet<City>();
        var queue = new List<City>();

        distances[start] = 0;
        queue.Add(start);

        while (queue.Count > 0) {
            City current = null;
            int bestDistance = int.MaxValue;

            foreach (var city in queue) {
                if (distances[city] < bestDistance) {
                    bestDistance = distances[city];
                    current = city;
                }
            }

            queue.Remove(current);

            if (current == goal)
                break;

            visited.Add(current);

            foreach (var path in current.Paths) {
                bool unusableButSameTeam = !path.available && path.teamColor == color;
                bool usable = path.available || unusableButSameTeam;

                if (!usable)
                    continue;

                City neighbor = path.to;
                if (visited.Contains(neighbor))
                    continue;

                int newDist = distances[current] + path.cost;

                if (!distances.ContainsKey(neighbor) || newDist < distances[neighbor]) {
                    distances[neighbor] = newDist;
                    previous[neighbor] = (current, path);

                    if (!queue.Contains(neighbor))
                        queue.Add(neighbor);
                }
            }
        }

        if (!distances.ContainsKey(goal))
            return false;

        // Reconstruct edge path
        var reversed = new List<PathTo>();
        City currentCity = goal;

        while (currentCity != start) {
            if (!previous.TryGetValue(currentCity, out var data))
                return false;

            reversed.Add(data.edge);
            currentCity = data.from;
        }

        reversed.Reverse();
        paths = reversed;

        // totalCost = distances[goal];
        return true;
    }

    static HashSet<City> GetAllConnectedCities(City start) {
        var visited = new HashSet<City>();
        var stack = new Stack<City>();
        stack.Push(start);

        while (stack.Count > 0) {
            City current = stack.Pop();
            if (!visited.Add(current))
                continue;

            foreach (var path in current.Paths) {
                if (!path.available || path.to != null)
                    stack.Push(path.to);
            }
        }

        return visited;
    }
}
