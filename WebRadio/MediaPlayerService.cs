using LibVLCSharp.Shared;

namespace WebRadio
{
    public class MediaPlayerService
    {
        public MediaPlayer MediaPlayer { get; private set; }
        public LibVLC LibVLC { get; private set; }
        private int? _previousVolume;

        public MediaPlayerService(MediaPlayer mediaPlayer, LibVLC libVLC)
        {
            MediaPlayer = mediaPlayer;
            LibVLC = libVLC;
        }

        public void Stop()
        {
            MediaPlayer.Stop();
        }

        public void Play(Media media)
        {
            MediaPlayer.Play(media);
        }

        public int Volume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        public void ToggleMute()
        {
            if (_previousVolume.HasValue)
            {
                Volume = _previousVolume.Value;
                _previousVolume = null;
            }
            else
            {
                _previousVolume = Volume;
                Volume = 0;
            }
        }
    }
}