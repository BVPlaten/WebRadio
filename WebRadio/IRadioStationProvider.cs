using System.Collections.Generic;

namespace WebRadio
{
    public interface IRadioStationProvider
    {
        List<RadioStation> GetAllRadioStations();
        void AddRadioStation(RadioStation radioStation);
        void RemoveRadioStation(string name);
    }
}
