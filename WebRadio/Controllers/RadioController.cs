using LibVLCSharp.Shared;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebRadio.Controllers
{
    public class StreamUrl
    {
        public string Url { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class VLCController : ControllerBase
    {
        private readonly MediaPlayerService _mediaPlayer;

        public VLCController(MediaPlayerService mediaPlayerService)
        {
            Core.Initialize();
            _mediaPlayer = mediaPlayerService;
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
                // Stoppen Sie den aktuellen Stream, bevor Sie den neuen Stream abspielen.
                _mediaPlayer.Stop();

                var media = new Media(_mediaPlayer.LibVLC, streamUrl.Url, FromType.FromLocation);
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
                //int currentVolume = _mediaPlayer.Volume;
                //int newVolume = Math.Max(0, currentVolume - 10);
                _mediaPlayer.Volume = 0;
                return Ok($"Volume muted.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error muting volume: {ex.Message}");
            }
        }
    }
}


