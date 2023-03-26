using LibVLCSharp.Shared;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebRadio.Controllers
{
    // url to the webstream with the audio data
    public class StreamUrl
    {
        public string Url { get; set; }
    }

    // helper to adjust the equalizer of the vlc player
    public class EqualizerSettings
    {
        public uint Preset { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    /*
     * RadioController allows to play a stream, stop the stream, increase the volume, decrease the volume and mute the volume.
     * 
     * */
    public class RadioController : ControllerBase
    {
        private readonly MediaPlayerService _mediaPlayer;
        private readonly Equalizer _equalizer;
        private int? _previousVolume; // Variable stores the volume before muting
        public RadioController(MediaPlayerService mediaPlayerService)
        {
            Core.Initialize();
            _mediaPlayer = mediaPlayerService;
            _equalizer = new Equalizer();   // TODO that does not work 
        }

        [HttpPost("play")]
        public IActionResult Play([FromBody] StreamUrl streamUrl)
        {
            if (string.IsNullOrWhiteSpace(streamUrl.Url))
            {
                return BadRequest("URL cannot be empty.");
            }

            try
            {
                _mediaPlayer.Stop();  // before starting a new stream the currently running stream has to be stopped

                var media = new Media(_mediaPlayer._libVLC, streamUrl.Url, FromType.FromLocation);
                _mediaPlayer.Play(media);

                return Ok("Playing stream: " + streamUrl.Url);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("stop")]
        public IActionResult Stop()
        {
            try
            {
                _mediaPlayer.Stop();
                return Ok("Stopped playing.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("volume/up")]
        public IActionResult VolumeUp()
        {
            try
            {
                int currentVolume = _mediaPlayer.Volume;
                int newVolume = Math.Min(100, currentVolume + 10);
                _mediaPlayer.Volume = newVolume;
                return Ok($"Volume increased to {newVolume}%.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error increasing volume: {ex.Message}");
            }
        }

        [HttpGet("volume/down")]
        public IActionResult VolumeDown()
        {
            try
            {
                int currentVolume = _mediaPlayer.Volume;
                int newVolume = Math.Max(0, currentVolume - 10);
                _mediaPlayer.Volume = newVolume;
                return Ok($"Volume decreased to {newVolume}%.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error decreasing volume: {ex.Message}");
            }
        }

        [HttpGet("volume/mute")]
        public IActionResult VolumeMute()
        {
            try
            {
                _mediaPlayer.ToggleMute();
                return Ok($"Volume mute toggeled.");

                /*
                bool muted = _mediaPlayer.ToggleMute();
                if (muted)
                {
                    return Ok($"Volume muted.");
                }
                else
                {
                    return Ok($"Volume restored.");
                }
                */
            }
            catch (Exception ex)
            {
                return BadRequest($"Error toggling mute: {ex.Message}");
            }
        }

        [HttpPost("equalizer")]
        public IActionResult SetEqualizer([FromBody] EqualizerSettings settings)
        {
            if (settings == null)
            {
                return BadRequest("Settings cannot be null.");
            }

            try
            {
                var equalizer = new Equalizer(settings.Preset);
                _mediaPlayer.SetEqualizer(equalizer);

                return Ok($"Equalizer preset {settings.Preset} applied.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("set_equalizer_by_list")]
        public IActionResult SetEqualizerByList([FromBody] List<float> gains)
        {
            // Zusammenfassung: SetAmp
            //     Set a new amplification value for a particular equalizer frequency band. The
            //     new equalizer settings are subsequently applied to a media player by invoking
            //     MediaPlayer::setEqualizer(). The supplied amplification value will be clamped
            //     to the -20.0 to +20.0 range. LibVLC 2.2.0 or later
            //
            // Parameter:
            //   amp:
            //     amplification value (-20.0 to 20.0 Hz)
            //
            //   band:
            //     index, counting from zero, of the frequency band to set
            if (gains == null || gains.Count != _equalizer.BandCount)
            {
                return BadRequest("Ungültige Eingabe für Equalizer-Verstärkungen.");
            }

            for (uint band = 0; band < _equalizer.BandCount; band++)
            {
                _equalizer.SetAmp(gains[(int)band], band);
            }
            _mediaPlayer.SetEqualizer(_equalizer);  // update equalizer of the player

            return Ok("Equalizer-Einstellungen erfolgreich aktualisiert.");
        }

        [HttpGet("get_equalizer_state")]
        public IActionResult GetEqualizerState()
        {
            var equalizerState = new List<float>();

            for (uint band = 0; band < _equalizer.BandCount; band++)
            {
                equalizerState.Add(_equalizer.Amp(band));
            }
            return Ok(equalizerState);
        }
    }
}


