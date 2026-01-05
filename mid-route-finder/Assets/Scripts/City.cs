using TMPro;
using UnityEngine;

public class City : MonoBehaviour {
    public int ID;
    public string Name;
    public int MaxConnections;

    [SerializeField] TextMeshPro _cityNameTxt;

    internal void Init(GameObject cityGo) {
        var cityData = City.GetCityData(cityGo.name);
        ID = cityData.id;
        Name = cityData.name;
        MaxConnections = cityData.maxConnections;
        transform.position = cityGo.transform.position;

        gameObject.name = $"{ID} {Name} ({MaxConnections})";
        _cityNameTxt.text = CityConstants.NAME_TEXT[Name];

        if (RouteConstants.CITY_SETTINGS.ContainsKey(Name)) {
            var citySettings = RouteConstants.CITY_SETTINGS[Name];

            if (citySettings.TextAlignmentOptions != _cityNameTxt.alignment) {
                _cityNameTxt.alignment = citySettings.TextAlignmentOptions;
            }
        }
    }

    public static (int id, string name, int maxConnections) GetCityData(string cityGoName) {
        string[] parts = cityGoName.Split('_');
        int id = int.Parse(parts[0]);
        string name = parts[1];
        int maxConnections = parts.Length > 2 ? int.Parse(parts[2]) : 4;

        return (id: id, name: name, maxConnections: maxConnections);
    }
}
