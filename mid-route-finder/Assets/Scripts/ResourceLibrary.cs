using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ResourceLibrary", menuName = "Singletons/ResourceLibrary")]
public class ResourceLibrary : ScriptableObject {
    private static ResourceLibrary _instance;

    public static ResourceLibrary _ {
        get {
            if (_instance == null) {
                _instance = Resources.Load<ResourceLibrary>("ResourceLibrary");
            }

            return _instance;
        }
    }

    [Header("Prefabs")] [SerializeField] internal LineRenderer LineRendererPrefab;
    [SerializeField] internal GameObject RouteBetweenPrefab;
    [SerializeField] internal GameObject WagonPlaceholderPrefab;
    [SerializeField] internal GameObject CityVisualizerPrefab;
    [SerializeField] internal GameObject PlanePrefab;

    [Header("Materials")] public Material defaultMaterial;
    public Material Red;
    public Material Blue;
    public Material Green;
    public Material Yellow;
    public Material Orange;
    public Material Pink;
    public Material Black;
    public Material White;

    private Dictionary<RouteColor, Material> _colorMaterials;

    public IReadOnlyDictionary<RouteColor, Material> ColorMaterials {
        get {
            if (_colorMaterials == null)
                Initialize();
            return _colorMaterials;
        }
    }

    void Initialize() {
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

#if UNITY_EDITOR
    private void OnValidate() {
        _colorMaterials = null;
    }
#endif
}

// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class ResourceLibrary : MonoBehaviour {
//     public static ResourceLibrary _ { get; private set; }
//
//     [Header("Prefabs")] [SerializeField] internal LineRenderer _lineRendererPrefab;
//     [SerializeField] internal GameObject _routeBetweenPrefab;
//     [SerializeField] internal GameObject WagonPlaceholderPrefab;
//     [SerializeField] internal GameObject _cityVisualizerPrefab;
//
//     [Header("Materials")] public Material defaultMaterial;
//     public Material Red;
//     public Material Blue;
//     public Material Green;
//     public Material Yellow;
//     public Material Orange;
//     public Material Pink;
//     public Material Black;
//     public Material White;
//
//     internal Dictionary<RouteColor, Material> ColorMaterials = new();
//
//     void Awake() {
//         if (_ != null && _ != this) {
//             Destroy(gameObject);
//             return;
//         }
//
//         _ = this;
//         DontDestroyOnLoad(gameObject);
//
//         ColorMaterials.Add(RouteColor.Red, Red);
//         ColorMaterials.Add(RouteColor.Blue, Blue);
//         ColorMaterials.Add(RouteColor.Green, Green);
//         ColorMaterials.Add(RouteColor.Yellow, Yellow);
//         ColorMaterials.Add(RouteColor.Orange, Orange);
//         ColorMaterials.Add(RouteColor.Pink, Pink);
//         ColorMaterials.Add(RouteColor.Black, Black);
//         ColorMaterials.Add(RouteColor.White, White);
//     }
//
//     void Start() { }
// }
