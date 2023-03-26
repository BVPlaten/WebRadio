using LibVLCSharp.Shared;

namespace WebRadio
{
    public class MediaPlayerService
    {
        public MediaPlayer _mediaPlayer { get; private set; }
        public LibVLC _libVLC { get; private set; }
        private int? _previousVolume;

        public MediaPlayerService(MediaPlayer mediaPlayer, LibVLC libVLC)
        {
            _mediaPlayer = mediaPlayer;
            _libVLC = libVLC;
        }

        public void Stop()
        {
            _mediaPlayer.Stop();
        }

        public void Play(Media media)
        {
            _mediaPlayer.Play(media);
        }

        public int Volume
        {
            get { return _mediaPlayer.Volume; }
            set { _mediaPlayer.Volume = value; }
        }

        public bool ToggleMute()
        {
            _mediaPlayer.ToggleMute();
            return true;
            /*
            if (_previousVolume.HasValue)
            {
                Volume = _previousVolume.Value;
                _previousVolume = null;
                return false;
            }
            else
            {
                _previousVolume = Volume;
                Volume = 0;
                return true;
            }
            */
        }

        public void SetEqualizer(Equalizer equalizer)
        {
            _mediaPlayer.SetEqualizer(equalizer);
        }

        public void GetEqualizer()
        {
            // todo  equalizer is not available in the media player after the equalizer has been set
            throw new NotImplementedException();
        }

    }
}