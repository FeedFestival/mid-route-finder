using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using UnityEngine;

public enum Category {
    Local = 0, // PROXIMITY_1
    Regional = 1, // PROXIMITY_2
    InterRegional = 2, // PROXIMITY_3
    Long = 3, // PROXIMITY_4
    Epic = 4 // PROXIMITY_5
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public readonly struct Mission : IEquatable<Mission> {
    internal readonly string cityA;
    internal readonly string cityB;
    internal readonly int cost;
    internal readonly int importancePoints;
    internal readonly Category category;

    public Mission(City cityA, City cityB, int cost, int importancePoints) {
        // Enforce deterministic order
        if (cityA.ID < cityB.ID) {
            this.cityA = cityA.Name;
            this.cityB = cityB.Name;
        }
        else {
            this.cityA = cityB.Name;
            this.cityB = cityA.Name;
        }

        this.cost = cost;
        this.importancePoints = importancePoints;
        category = MissionConstants.GetCategory(cost);
    }

    public Mission(string cityA, string cityB, int cost, int importancePoints) {
        this.cityA = cityA;
        this.cityB = cityB;
        this.cost = cost;
        this.importancePoints = importancePoints;
        category = MissionConstants.GetCategory(cost);
    }

    public bool Equals(Mission other) {
        return Equals(cityA, other.cityA) && Equals(cityB, other.cityB) && cost == other.cost &&
               importancePoints == other.importancePoints;
    }

    public override bool Equals(object obj) {
        return obj is Mission other && Equals(other);
    }

    public override int GetHashCode() {
        return HashCode.Combine(cityA, cityB, cost, importancePoints);
    }

    public override string ToString() {
        return "{{" +
               $"\"cityA\":\"{cityA}\"," +
               $"\"cityB\":\"{cityB}\"," +
               $"\"cost\":{cost}," +
               $"\"importancePoints\":{importancePoints}," +
               $"\"category\":\"{category}\"" +
               "}}";
    }
}

public static class MissionConstants {
    public static Category GetCategory(int cost) {
        if (cost < 8) return Category.Local;
        if (cost < 12) return Category.Regional;
        if (cost < 16) return Category.InterRegional;
        if (cost < 20) return Category.Long;

        return Category.Epic;
    }

    public static Mission[] GetCategoryMissions(Mission[] cards, Category category) {
        return cards.Where(c => c.category == category)
            .OrderByDescending(it => it.importancePoints)
            .ThenByDescending(it => it.cost)
            .ToArray();
    }

    public static readonly Dictionary<Category, int> MISSION_CARD_COUNT = new() {
        { Category.Local, 4 },
        { Category.Regional, 8 },
        { Category.InterRegional, 11 },
        { Category.Long, 5 },
        { Category.Epic, 5 },
    };

    public static Mission DrawWeighted(ref List<Mission> missions) {
        int totalWeight = 0;

        for (int i = 0; i < missions.Count; i++)
            totalWeight += Mathf.Max(1, missions[i].importancePoints);

        int roll = UnityEngine.Random.Range(0, totalWeight);

        int cumulative = 0;
        for (int i = 0; i < missions.Count; i++) {
            cumulative += Mathf.Max(1, missions[i].importancePoints);
            if (roll < cumulative) {
                Mission chosen = missions[i];
                missions.RemoveAt(i); // remove → no duplicates
                return chosen;
            }
        }

        // Fallback (should never happen)
        var last = missions[^1];
        missions.RemoveAt(missions.Count - 1);
        return last;
    }

    public static readonly Dictionary<Category, Mission[]> MISSION_BANK = new() {
        {
            Category.Local, new Mission[18] {
                new("Bucuresti", "Craiova", 7, 76),
                new("Ploiesti", "Craiova", 5, 74),
                new("Craiova", "Brasov", 7, 65),
                new("Ploiesti", "Sibiu", 6, 62),
                new("Craiova", "Buzau", 7, 58),
                new("Galati", "Targoviste", 7, 57),
                new("Ploiesti", "Targu Jiu", 6, 53),
                new("Pitesti", "Focsani", 6, 50),
                new("Buzau", "Slatina", 6, 45),
                new("Pitesti", "Miercurea Ciuc", 7, 44),
                new("Targoviste", "Alba Iulia", 7, 43),
                new("Pitesti", "Braila", 7, 42),
                new("Buzau", "Ramnicu Valcea", 5, 42),
                new("Pitesti", "Slobozia", 7, 40),
                new("Ramnicu Valcea", "Focsani", 7, 35),
                new("Targoviste", "Drobeta Turnu Severin", 7, 33),
                new("Ramnicu Valcea", "Giurgiu", 7, 26),
                new("Slatina", "Sfantu Gheorghe", 7, 24),
            }
        }, {
            Category.Regional, new Mission[108] {
                new("Iasi", "Ploiesti", 11, 81),
                new("Ploiesti", "Cluj Napoca", 11, 79),
                new("Bucuresti", "Targu Mures", 11, 72),
                new("Iasi", "Brasov", 10, 72),
                new("Cluj Napoca", "Pitesti", 9, 72),
                new("Constanta", "Pitesti", 11, 71),
                new("Timisoara", "Pitesti", 11, 70),
                new("Ploiesti", "Targu Mures", 9, 70),
                new("Bucuresti", "Piatra Neamt", 11, 69),
                new("Bacau", "Pitesti", 10, 68),
                new("Cluj Napoca", "Targoviste", 10, 67),
                new("Ploiesti", "Piatra Neamt", 9, 67),
                new("Suceava", "Brasov", 9, 67),
                new("Constanta", "Targoviste", 10, 66),
                new("Bucuresti", "Sibiu", 8, 64),
                new("Craiova", "Galati", 11, 63),
                new("Bacau", "Targoviste", 9, 63),
                new("Pitesti", "Galati", 8, 62),
                new("Cluj Napoca", "Slatina", 11, 60),
                new("Suceava", "Buzau", 11, 60),
                new("Ploiesti", "Vaslui", 10, 60),
                new("Pitesti", "Piatra Neamt", 10, 60),
                new("Brasov", "Galati", 9, 60),
                new("Ploiesti", "Deva", 10, 59),
                new("Timisoara", "Slatina", 11, 58),
                new("Targu Mures", "Targoviste", 8, 58),
                new("Bucuresti", "Alba Iulia", 10, 57),
                new("Targoviste", "Piatra Neamt", 9, 55),
                new("Bucuresti", "Targu Jiu", 8, 55),
                new("Ploiesti", "Alba Iulia", 8, 55),
                new("Targu Mures", "Buzau", 11, 54),
                new("Oradea", "Sibiu", 10, 54),
                new("Brasov", "Botosani", 10, 54),
                new("Bacau", "Ramnicu Valcea", 11, 53),
                new("Bucuresti", "Miercurea Ciuc", 8, 53),
                new("Craiova", "Focsani", 9, 51),
                new("Brasov", "Vaslui", 9, 51),
                new("Targu Mures", "Slatina", 9, 51),
                new("Galati", "Slatina", 10, 50),
                new("Iasi", "Slobozia", 11, 48),
                new("Targoviste", "Vaslui", 11, 48),
                new("Bucuresti", "Drobeta Turnu Severin", 10, 47),
                new("Galati", "Ramnicu Valcea", 9, 47),
                new("Targoviste", "Deva", 9, 47),
                new("Ploiesti", "Resita", 10, 46),
                new("Buzau", "Sibiu", 8, 46),
                new("Piatra Neamt", "Ramnicu Valcea", 11, 45),
                new("Bacau", "Calarasi", 10, 45),
                new("Craiova", "Miercurea Ciuc", 10, 45),
                new("Pitesti", "Bistrita", 10, 45),
                new("Baia Mare", "Deva", 10, 45),
                new("Ploiesti", "Drobeta Turnu Severin", 8, 45),
                new("Cluj Napoca", "Drobeta Turnu Severin", 11, 44),
                new("Craiova", "Calarasi", 11, 44),
                new("Craiova", "Braila", 10, 43),
                new("Galati", "Alexandria", 10, 43),
                new("Craiova", "Slobozia", 10, 41),
                new("Brasov", "Calarasi", 9, 41),
                new("Targoviste", "Bistrita", 11, 40),
                new("Buzau", "Alba Iulia", 10, 39),
                new("Sibiu", "Focsani", 10, 39),
                new("Sibiu", "Satu Mare", 10, 38),
                new("Slatina", "Focsani", 8, 38),
                new("Pitesti", "Zalau", 11, 37),
                new("Galati", "Giurgiu", 9, 37),
                new("Craiova", "Sfantu Gheorghe", 8, 37),
                new("Buzau", "Targu Jiu", 8, 37),
                new("Sibiu", "Alexandria", 11, 36),
                new("Slatina", "Alba Iulia", 8, 36),
                new("Pitesti", "Tulcea", 9, 35),
                new("Buzau", "Miercurea Ciuc", 8, 35),
                new("Piatra Neamt", "Slobozia", 11, 34),
                new("Targoviste", "Resita", 9, 34),
                new("Brasov", "Tulcea", 10, 33),
                new("Slatina", "Miercurea Ciuc", 9, 32),
                new("Ramnicu Valcea", "Alexandria", 8, 32),
                new("Sibiu", "Braila", 11, 31),
                new("Slatina", "Calarasi", 10, 31),
                new("Sibiu", "Giurgiu", 10, 30),
                new("Vaslui", "Calarasi", 10, 30),
                new("Focsani", "Targu Jiu", 10, 30),
                new("Slatina", "Braila", 9, 30),
                new("Targoviste", "Tulcea", 8, 30),
                new("Piatra Neamt", "Tulcea", 11, 29),
                new("Sibiu", "Slobozia", 11, 29),
                new("Buzau", "Drobeta Turnu Severin", 10, 29),
                new("Slatina", "Slobozia", 9, 28),
                new("Ramnicu Valcea", "Calarasi", 9, 28),
                new("Ramnicu Valcea", "Braila", 8, 27),
                new("Alexandria", "Miercurea Ciuc", 11, 25),
                new("Ramnicu Valcea", "Slobozia", 8, 25),
                new("Slatina", "Tulcea", 11, 23),
                new("Targu Jiu", "Braila", 11, 22),
                new("Deva", "Sfantu Gheorghe", 10, 22),
                new("Ramnicu Valcea", "Zalau", 10, 22),
                new("Targu Jiu", "Slobozia", 11, 20),
                new("Ramnicu Valcea", "Tulcea", 10, 20),
                new("Miercurea Ciuc", "Giurgiu", 10, 19),
                new("Miercurea Ciuc", "Slobozia", 11, 18),
                new("Targu Jiu", "Zalau", 11, 17),
                new("Alexandria", "Sfantu Gheorghe", 9, 17),
                new("Alexandria", "Tulcea", 11, 16),
                new("Calarasi", "Sfantu Gheorghe", 10, 13),
                new("Giurgiu", "Sfantu Gheorghe", 8, 11),
                new("Giurgiu", "Tulcea", 10, 10),
                new("Slobozia", "Sfantu Gheorghe", 9, 10),
                new("Drobeta Turnu Severin", "Sfantu Gheorghe", 11, 8),
                new("Zalau", "Sfantu Gheorghe", 11, 7),
            }
        }, {
            Category.InterRegional, new Mission[180] {
                new("Bucuresti", "Iasi", 13, 83),
                new("Bucuresti", "Cluj Napoca", 13, 81),
                new("Iasi", "Cluj Napoca", 14, 80),
                new("Bucuresti", "Timisoara", 15, 79),
                new("Iasi", "Constanta", 14, 79),
                new("Bucuresti", "Suceava", 14, 78),
                new("Ploiesti", "Timisoara", 13, 77),
                new("Ploiesti", "Suceava", 12, 76),
                new("Iasi", "Pitesti", 13, 74),
                new("Constanta", "Bacau", 13, 73),
                new("Cluj Napoca", "Craiova", 12, 73),
                new("Constanta", "Craiova", 14, 72),
                new("Suceava", "Pitesti", 13, 69),
                new("Bacau", "Craiova", 13, 69),
                new("Iasi", "Targoviste", 12, 69),
                new("Constanta", "Brasov", 12, 69),
                new("Timisoara", "Brasov", 14, 68),
                new("Iasi", "Baia Mare", 15, 67),
                new("Bucuresti", "Botosani", 15, 65),
                new("Ploiesti", "Arad", 15, 65),
                new("Constanta", "Piatra Neamt", 15, 65),
                new("Pitesti", "Oradea", 14, 65),
                new("Timisoara", "Targoviste", 12, 65),
                new("Suceava", "Galati", 12, 65),
                new("Suceava", "Targoviste", 12, 64),
                new("Oradea", "Brasov", 15, 63),
                new("Iasi", "Sibiu", 14, 63),
                new("Ploiesti", "Botosani", 13, 63),
                new("Cluj Napoca", "Buzau", 13, 63),
                new("Timisoara", "Baia Mare", 13, 63),
                new("Iasi", "Slatina", 15, 62),
                new("Bucuresti", "Vaslui", 12, 62),
                new("Timisoara", "Buzau", 15, 61),
                new("Craiova", "Piatra Neamt", 13, 61),
                new("Bucuresti", "Deva", 12, 61),
                new("Constanta", "Sibiu", 15, 60),
                new("Oradea", "Targoviste", 15, 60),
                new("Iasi", "Ramnicu Valcea", 14, 59),
                new("Pitesti", "Baia Mare", 14, 59),
                new("Targu Mures", "Galati", 14, 59),
                new("Cluj Napoca", "Vaslui", 13, 59),
                new("Constanta", "Slatina", 13, 59),
                new("Pitesti", "Arad", 13, 58),
                new("Suceava", "Slatina", 15, 57),
                new("Brasov", "Baia Mare", 13, 57),
                new("Iasi", "Alba Iulia", 15, 56),
                new("Cluj Napoca", "Focsani", 14, 56),
                new("Pitesti", "Botosani", 14, 56),
                new("Brasov", "Arad", 14, 56),
                new("Constanta", "Ramnicu Valcea", 12, 56),
                new("Bacau", "Slatina", 12, 56),
                new("Oradea", "Botosani", 15, 55),
                new("Suceava", "Deva", 14, 55),
                new("Craiova", "Vaslui", 15, 54),
                new("Targoviste", "Baia Mare", 15, 54),
                new("Bucuresti", "Bistrita", 14, 54),
                new("Suceava", "Ramnicu Valcea", 14, 54),
                new("Bacau", "Deva", 14, 54),
                new("Oradea", "Slatina", 14, 53),
                new("Targoviste", "Arad", 14, 53),
                new("Pitesti", "Vaslui", 12, 53),
                new("Ploiesti", "Bistrita", 12, 52),
                new("Constanta", "Targu Jiu", 15, 51),
                new("Timisoara", "Alexandria", 15, 51),
                new("Bacau", "Satu Mare", 14, 51),
                new("Targoviste", "Botosani", 13, 51),
                new("Iasi", "Calarasi", 12, 51),
                new("Galati", "Sibiu", 12, 51),
                new("Iasi", "Giurgiu", 15, 49),
                new("Constanta", "Miercurea Ciuc", 15, 49),
                new("Pitesti", "Satu Mare", 14, 49),
                new("Timisoara", "Bistrita", 13, 49),
                new("Bacau", "Alexandria", 13, 49),
                new("Timisoara", "Miercurea Ciuc", 15, 48),
                new("Bacau", "Targu Jiu", 14, 48),
                new("Bucuresti", "Resita", 12, 48),
                new("Piatra Neamt", "Slatina", 12, 48),
                new("Cluj Napoca", "Giurgiu", 15, 47),
                new("Brasov", "Satu Mare", 15, 47),
                new("Buzau", "Botosani", 12, 47),
                new("Bucuresti", "Zalau", 15, 46),
                new("Suceava", "Calarasi", 15, 46),
                new("Baia Mare", "Vaslui", 14, 46),
                new("Craiova", "Bistrita", 13, 46),
                new("Piatra Neamt", "Deva", 13, 46),
                new("Arad", "Slatina", 13, 46),
                new("Cluj Napoca", "Resita", 13, 45),
                new("Botosani", "Sibiu", 13, 45),
                new("Suceava", "Braila", 12, 45),
                new("Targoviste", "Satu Mare", 15, 44),
                new("Targu Mures", "Alexandria", 14, 44),
                new("Galati", "Alba Iulia", 14, 44),
                new("Ploiesti", "Zalau", 13, 44),
                new("Baia Mare", "Ramnicu Valcea", 13, 44),
                new("Suceava", "Slobozia", 14, 43),
                new("Bacau", "Giurgiu", 12, 43),
                new("Oradea", "Miercurea Ciuc", 12, 43),
                new("Buzau", "Deva", 12, 43),
                new("Botosani", "Deva", 15, 42),
                new("Sibiu", "Vaslui", 13, 42),
                new("Galati", "Targu Jiu", 12, 42),
                new("Galati", "Bistrita", 15, 41),
                new("Botosani", "Ramnicu Valcea", 15, 41),
                new("Piatra Neamt", "Alexandria", 14, 41),
                new("Slatina", "Vaslui", 14, 41),
                new("Constanta", "Sfantu Gheorghe", 13, 41),
                new("Timisoara", "Sfantu Gheorghe", 15, 40),
                new("Targu Mures", "Calarasi", 15, 40),
                new("Piatra Neamt", "Targu Jiu", 14, 40),
                new("Targu Mures", "Braila", 14, 39),
                new("Baia Mare", "Targu Jiu", 14, 39),
                new("Botosani", "Satu Mare", 13, 39),
                new("Bacau", "Zalau", 13, 39),
                new("Suceava", "Tulcea", 14, 38),
                new("Craiova", "Zalau", 14, 38),
                new("Targu Mures", "Giurgiu", 13, 38),
                new("Botosani", "Alba Iulia", 13, 38),
                new("Vaslui", "Ramnicu Valcea", 13, 38),
                new("Targu Mures", "Slobozia", 14, 37),
                new("Arad", "Bistrita", 13, 37),
                new("Piatra Neamt", "Calarasi", 12, 37),
                new("Arad", "Miercurea Ciuc", 15, 36),
                new("Buzau", "Bistrita", 14, 36),
                new("Deva", "Focsani", 14, 36),
                new("Craiova", "Tulcea", 12, 36),
                new("Oradea", "Sfantu Gheorghe", 14, 35),
                new("Vaslui", "Alba Iulia", 14, 35),
                new("Piatra Neamt", "Giurgiu", 13, 35),
                new("Brasov", "Zalau", 12, 35),
                new("Vaslui", "Alexandria", 15, 34),
                new("Galati", "Drobeta Turnu Severin", 14, 34),
                new("Ramnicu Valcea", "Satu Mare", 13, 34),
                new("Slatina", "Bistrita", 12, 33),
                new("Targoviste", "Zalau", 12, 32),
                new("Botosani", "Braila", 12, 32),
                new("Sibiu", "Calarasi", 12, 32),
                new("Focsani", "Alba Iulia", 12, 32),
                new("Botosani", "Slobozia", 15, 30),
                new("Buzau", "Resita", 12, 30),
                new("Satu Mare", "Targu Jiu", 14, 29),
                new("Alba Iulia", "Alexandria", 13, 29),
                new("Arad", "Sfantu Gheorghe", 15, 28),
                new("Buzau", "Zalau", 15, 28),
                new("Deva", "Braila", 15, 28),
                new("Vaslui", "Giurgiu", 14, 28),
                new("Deva", "Giurgiu", 13, 27),
                new("Satu Mare", "Miercurea Ciuc", 12, 27),
                new("Deva", "Slobozia", 15, 26),
                new("Botosani", "Tulcea", 14, 25),
                new("Alba Iulia", "Calarasi", 14, 25),
                new("Slatina", "Zalau", 13, 25),
                new("Vaslui", "Zalau", 15, 24),
                new("Sibiu", "Tulcea", 13, 24),
                new("Alba Iulia", "Braila", 13, 24),
                new("Focsani", "Resita", 14, 23),
                new("Alba Iulia", "Giurgiu", 12, 23),
                new("Targu Jiu", "Calarasi", 12, 23),
                new("Satu Mare", "Resita", 14, 22),
                new("Alba Iulia", "Slobozia", 13, 22),
                new("Focsani", "Drobeta Turnu Severin", 12, 22),
                new("Bistrita", "Braila", 15, 21),
                new("Miercurea Ciuc", "Calarasi", 12, 21),
                new("Satu Mare", "Sfantu Gheorghe", 14, 19),
                new("Bistrita", "Resita", 14, 18),
                new("Alba Iulia", "Tulcea", 15, 17),
                new("Miercurea Ciuc", "Resita", 15, 17),
                new("Bistrita", "Drobeta Turnu Severin", 12, 17),
                new("Miercurea Ciuc", "Drobeta Turnu Severin", 13, 16),
                new("Braila", "Resita", 15, 15),
                new("Calarasi", "Drobeta Turnu Severin", 14, 15),
                new("Targu Jiu", "Tulcea", 13, 15),
                new("Braila", "Drobeta Turnu Severin", 13, 14),
                new("Giurgiu", "Resita", 13, 14),
                new("Slobozia", "Resita", 15, 13),
                new("Miercurea Ciuc", "Tulcea", 12, 13),
                new("Slobozia", "Drobeta Turnu Severin", 13, 12),
                new("Resita", "Zalau", 13, 10),
                new("Resita", "Sfantu Gheorghe", 13, 9),
                new("Drobeta Turnu Severin", "Zalau", 13, 9),
                new("Drobeta Turnu Severin", "Tulcea", 15, 7),
            }
        }, {
            Category.Long, new Mission[102] {
                new("Iasi", "Craiova", 16, 75),
                new("Bucuresti", "Oradea", 18, 74),
                new("Constanta", "Suceava", 18, 74),
                new("Iasi", "Oradea", 19, 73),
                new("Timisoara", "Suceava", 19, 73),
                new("Timisoara", "Bacau", 19, 72),
                new("Ploiesti", "Oradea", 16, 72),
                new("Suceava", "Craiova", 16, 70),
                new("Bucuresti", "Baia Mare", 18, 68),
                new("Constanta", "Targu Mures", 18, 68),
                new("Cluj Napoca", "Galati", 17, 68),
                new("Bucuresti", "Arad", 17, 67),
                new("Bacau", "Oradea", 16, 67),
                new("Timisoara", "Galati", 19, 66),
                new("Ploiesti", "Baia Mare", 16, 66),
                new("Timisoara", "Piatra Neamt", 18, 64),
                new("Suceava", "Arad", 19, 61),
                new("Constanta", "Botosani", 18, 61),
                new("Bacau", "Arad", 19, 60),
                new("Iasi", "Deva", 17, 60),
                new("Craiova", "Baia Mare", 17, 60),
                new("Bucuresti", "Satu Mare", 18, 58),
                new("Constanta", "Deva", 19, 57),
                new("Iasi", "Satu Mare", 17, 57),
                new("Craiova", "Botosani", 17, 57),
                new("Oradea", "Buzau", 18, 56),
                new("Ploiesti", "Satu Mare", 16, 56),
                new("Galati", "Baia Mare", 19, 55),
                new("Iasi", "Alexandria", 16, 55),
                new("Iasi", "Targu Jiu", 17, 54),
                new("Timisoara", "Focsani", 17, 54),
                new("Constanta", "Alba Iulia", 17, 53),
                new("Cluj Napoca", "Alexandria", 16, 53),
                new("Oradea", "Vaslui", 18, 52),
                new("Piatra Neamt", "Arad", 18, 52),
                new("Baia Mare", "Buzau", 18, 50),
                new("Suceava", "Alexandria", 17, 50),
                new("Craiova", "Satu Mare", 17, 50),
                new("Oradea", "Focsani", 19, 49),
                new("Cluj Napoca", "Calarasi", 17, 49),
                new("Arad", "Buzau", 17, 49),
                new("Suceava", "Targu Jiu", 16, 49),
                new("Cluj Napoca", "Braila", 16, 48),
                new("Galati", "Deva", 16, 48),
                new("Timisoara", "Calarasi", 19, 47),
                new("Baia Mare", "Slatina", 16, 47),
                new("Iasi", "Drobeta Turnu Severin", 19, 46),
                new("Timisoara", "Braila", 18, 46),
                new("Oradea", "Alexandria", 18, 46),
                new("Cluj Napoca", "Slobozia", 16, 46),
                new("Iasi", "Zalau", 16, 45),
                new("Timisoara", "Giurgiu", 16, 45),
                new("Constanta", "Resita", 19, 44),
                new("Timisoara", "Slobozia", 18, 44),
                new("Suceava", "Giurgiu", 16, 44),
                new("Botosani", "Slatina", 16, 44),
                new("Constanta", "Drobeta Turnu Severin", 17, 43),
                new("Baia Mare", "Focsani", 16, 43),
                new("Arad", "Focsani", 19, 42),
                new("Cluj Napoca", "Tulcea", 18, 41),
                new("Suceava", "Drobeta Turnu Severin", 18, 41),
                new("Bacau", "Resita", 18, 41),
                new("Oradea", "Giurgiu", 19, 40),
                new("Buzau", "Satu Mare", 18, 40),
                new("Bacau", "Drobeta Turnu Severin", 16, 40),
                new("Arad", "Alexandria", 17, 39),
                new("Vaslui", "Deva", 16, 39),
                new("Botosani", "Alexandria", 18, 37),
                new("Slatina", "Satu Mare", 16, 37),
                new("Botosani", "Targu Jiu", 17, 36),
                new("Vaslui", "Satu Mare", 16, 36),
                new("Baia Mare", "Braila", 19, 35),
                new("Galati", "Resita", 16, 35),
                new("Galati", "Zalau", 19, 33),
                new("Piatra Neamt", "Resita", 18, 33),
                new("Arad", "Giurgiu", 18, 33),
                new("Focsani", "Satu Mare", 18, 33),
                new("Botosani", "Calarasi", 16, 33),
                new("Vaslui", "Targu Jiu", 16, 33),
                new("Targu Mures", "Tulcea", 16, 32),
                new("Piatra Neamt", "Drobeta Turnu Severin", 16, 32),
                new("Baia Mare", "Resita", 16, 32),
                new("Botosani", "Giurgiu", 17, 31),
                new("Baia Mare", "Drobeta Turnu Severin", 16, 31),
                new("Deva", "Calarasi", 16, 29),
                new("Botosani", "Drobeta Turnu Severin", 19, 28),
                new("Alexandria", "Bistrita", 17, 26),
                new("Vaslui", "Drobeta Turnu Severin", 18, 25),
                new("Bistrita", "Calarasi", 18, 22),
                new("Deva", "Tulcea", 17, 21),
                new("Focsani", "Zalau", 16, 21),
                new("Satu Mare", "Drobeta Turnu Severin", 16, 21),
                new("Bistrita", "Giurgiu", 16, 20),
                new("Bistrita", "Slobozia", 17, 19),
                new("Alexandria", "Zalau", 18, 18),
                new("Calarasi", "Resita", 16, 16),
                new("Calarasi", "Zalau", 19, 14),
                new("Bistrita", "Tulcea", 17, 14),
                new("Braila", "Zalau", 18, 13),
                new("Giurgiu", "Zalau", 17, 12),
                new("Slobozia", "Zalau", 18, 11),
                new("Resita", "Tulcea", 17, 8),
            }
        }, {
            Category.Epic, new Mission[42] {
                new("Iasi", "Timisoara", 22, 78),
                new("Cluj Napoca", "Constanta", 20, 77),
                new("Constanta", "Timisoara", 22, 75),
                new("Constanta", "Oradea", 25, 70),
                new("Iasi", "Arad", 22, 66),
                new("Constanta", "Baia Mare", 25, 64),
                new("Constanta", "Arad", 24, 63),
                new("Oradea", "Galati", 22, 61),
                new("Timisoara", "Botosani", 20, 60),
                new("Timisoara", "Vaslui", 21, 57),
                new("Constanta", "Satu Mare", 25, 54),
                new("Galati", "Arad", 21, 54),
                new("Constanta", "Bistrita", 21, 50),
                new("Arad", "Botosani", 20, 48),
                new("Iasi", "Resita", 21, 47),
                new("Galati", "Satu Mare", 21, 45),
                new("Arad", "Vaslui", 21, 45),
                new("Constanta", "Zalau", 22, 42),
                new("Oradea", "Calarasi", 22, 42),
                new("Suceava", "Resita", 20, 42),
                new("Oradea", "Braila", 21, 41),
                new("Baia Mare", "Alexandria", 21, 40),
                new("Oradea", "Slobozia", 21, 39),
                new("Timisoara", "Tulcea", 20, 39),
                new("Baia Mare", "Calarasi", 22, 36),
                new("Arad", "Calarasi", 21, 35),
                new("Oradea", "Tulcea", 23, 34),
                new("Baia Mare", "Giurgiu", 20, 34),
                new("Arad", "Braila", 20, 34),
                new("Baia Mare", "Slobozia", 21, 33),
                new("Arad", "Slobozia", 20, 32),
                new("Satu Mare", "Alexandria", 21, 30),
                new("Botosani", "Resita", 21, 29),
                new("Baia Mare", "Tulcea", 21, 28),
                new("Arad", "Tulcea", 22, 27),
                new("Satu Mare", "Calarasi", 22, 26),
                new("Vaslui", "Resita", 20, 26),
                new("Satu Mare", "Braila", 21, 25),
                new("Satu Mare", "Giurgiu", 20, 24),
                new("Satu Mare", "Slobozia", 21, 23),
                new("Satu Mare", "Tulcea", 23, 18),
                new("Zalau", "Tulcea", 20, 6),
            }
        },
    };
}
