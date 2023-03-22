using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace WebRadio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RadioStationsController : ControllerBase
    {
        private readonly IRadioStationProvider _radioStationProvider;

        public RadioStationsController(IRadioStationProvider radioStationProvider)
        {
            _radioStationProvider = radioStationProvider;
        }

        [HttpGet]
        public IEnumerable<RadioStation> Get()
        {
            return _radioStationProvider.GetAllRadioStations();
        }

        [HttpPost]
        public ActionResult<RadioStation> AddRadioStation(RadioStation radioStation)
        {
            _radioStationProvider.AddRadioStation(radioStation);
            return CreatedAtAction(nameof(Get), new { name = radioStation.Name }, radioStation);
        }

        [HttpDelete("{name}")]
        public IActionResult DeleteRadioStation(string name)
        {
            _radioStationProvider.RemoveRadioStation(name);
            return NoContent();
        }
    }
}
