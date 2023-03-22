namespace WebRadio
{
    using System.Collections.Generic;
    using System.IO;
    using Newtonsoft.Json;

    public class JsonRadioStationProvider : IRadioStationProvider
    {
        private readonly string _jsonFilePath;
        private readonly List<RadioStation> _radioStations;

        public JsonRadioStationProvider(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;
            _radioStations = JsonConvert.DeserializeObject<List<RadioStation>>(File.ReadAllText(jsonFilePath));
        }

        public List<RadioStation> GetAllRadioStations()
        {
            return _radioStations;
        }

        public void AddRadioStation(RadioStation radioStation)
        {
            _radioStations.Add(radioStation);
            SaveRadioStations();
        }

        public void RemoveRadioStation(string name)
        {
            var radioStation = _radioStations.FirstOrDefault(rs => rs.Name == name);
            if (radioStation != null)
            {
                _radioStations.Remove(radioStation);
                SaveRadioStations();
            }
        }

        private void SaveRadioStations()
        {
            File.WriteAllText(_jsonFilePath, JsonConvert.SerializeObject(_radioStations, Formatting.Indented));
        }
    }
}
