using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Pathfinding {
    public static bool FindShortestPath(
        City start,
        City goal,
        out List<City> path,
        out int totalCost
    ) {
        path = new List<City>();
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
            path.Insert(0, step);
            previous.TryGetValue(step, out step);
        }

        totalCost = distances[goal];
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
