using System.Collections.Generic;
using BezierSolution;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ResourceLibrary", menuName = "Singletons/ResourceLibrary")]
public class ResourceLibrary : ScriptableObject {
    private static ResourceLibrary instance;

    public static ResourceLibrary _ {
        get {
            if (instance == null) {
                instance = Resources.Load<ResourceLibrary>("ResourceLibrary");
            }

            return instance;
        }
    }

    [Header("Prefabs")] [SerializeField] internal LineRenderer LineRendererPrefab;
    [SerializeField] internal GameObject RouteBetweenPrefab;
    [SerializeField] internal GameObject WagonPlaceholderPrefab;
    [SerializeField] internal GameObject CityVisualizerPrefab;
    [SerializeField] internal GameObject PlanePrefab;
    [SerializeField] internal GameObject WagonRigidbodyPrefab;
    [SerializeField] internal GameObject WagonPrefab;
    [SerializeField] internal BezierSpline BezierSplinePrefab;

    [Header("Materials")] public Material defaultMaterial;
    public Material Red;
    public Material Blue;
    public Material Green;
    public Material Yellow;
    public Material Orange;
    public Material Pink;
    public Material Black;
    public Material White;

    [Header("Materials - Wagon")] public Material BlueWagon;
    public Material RedWagon;
    public Material YellowWagon;
    public Material BlackWagon;
    public Material GreenWagon;

    Dictionary<RouteColor, Material> _colorMaterials;
    Dictionary<TeamColor, Material> _wagonMaterials;

    public IReadOnlyDictionary<RouteColor, Material> ColorMaterials {
        get {
            if (_colorMaterials == null)
                initializeColors();
            return _colorMaterials;
        }
    }

    public IReadOnlyDictionary<TeamColor, Material> WagonMaterials {
        get {
            if (_wagonMaterials == null)
                initializeWagonMaterials();
            return _wagonMaterials;
        }
    }

    void initializeColors() {
        _colorMaterials = new Dictionary<RouteColor, Material> {
            { RouteColor.Red, Red },
            { RouteColor.Blue, Blue },
            { RouteColor.Green, Green },
            { RouteColor.Yellow, Yellow },
            { RouteColor.Orange, Orange },
            { RouteColor.Pink, Pink },
            { RouteColor.Black, Black },
            { RouteColor.White, White }
        };
    }

    void initializeWagonMaterials() {
        _wagonMaterials = new Dictionary<TeamColor, Material> {
            { TeamColor.Blue, BlueWagon },
            { TeamColor.Red, RedWagon },
            { TeamColor.Yellow, YellowWagon },
            { TeamColor.Black, BlackWagon },
            { TeamColor.Green, GreenWagon },
        };
    }

#if UNITY_EDITOR
    private void OnValidate() {
        _colorMaterials = null;
    }
#endif
}
