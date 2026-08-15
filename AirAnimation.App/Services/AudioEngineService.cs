using System;
using System.IO;
using System.Windows.Media;

namespace AirAnimation.App.Services;

public class AudioEngineService
{
    private readonly MediaPlayer _planePlayer = new();
    private readonly MediaPlayer _rainPlayer = new();
    private readonly MediaPlayer _windPlayer = new();

    private bool _isEnabled;
    private bool _isPlaying;

    public AudioEngineService()
    {
        var soundDir = Path.Combine(AppContext.BaseDirectory, "Resources", "Sound");
        
        LoadLoopingSound(_planePlayer, Path.Combine(soundDir, "airplane.mp3"));
        LoadLoopingSound(_rainPlayer, Path.Combine(soundDir, "rain.mp3"));
        LoadLoopingSound(_windPlayer, Path.Combine(soundDir, "wind.mp3")); // Used for snow/clouds
        
        SetPlaneVolume(1.0);
        SetRainVolume(0);
        SetWindVolume(0, 0);
    }

    private void LoadLoopingSound(MediaPlayer player, string path)
    {
        if (File.Exists(path))
        {
            player.Open(new Uri(path, UriKind.Absolute));
            player.MediaEnded += (s, e) =>
            {
                player.Position = TimeSpan.Zero;
                player.Play();
            };
        }
    }

    public void EnableSounds(bool enable)
    {
        _isEnabled = enable;
        UpdatePlayback();
    }

    public void SetAnimationPlaying(bool isPlaying)
    {
        _isPlaying = isPlaying;
        UpdatePlayback();
    }

    private void UpdatePlayback()
    {
        if (_isEnabled && _isPlaying)
        {
            _planePlayer.Play();
            _rainPlayer.Play();
            _windPlayer.Play();
        }
        else
        {
            _planePlayer.Pause();
            _rainPlayer.Pause();
            _windPlayer.Pause();
        }
    }

    public void SetPlaneVolume(double volume) => _planePlayer.Volume = volume;
    public void SetRainVolume(double intensity) => _rainPlayer.Volume = intensity * 0.8;
    
    public void PlayLightningStrike(double intensity)
    {
        if (!_isEnabled || !_isPlaying || intensity <= 0) return;
        
        var path = Path.Combine(AppContext.BaseDirectory, "Resources", "Sound", "thunder.mp3");
        if (!File.Exists(path)) return;
        
        var player = new MediaPlayer();
        player.Open(new Uri(path, UriKind.Absolute));
        player.Volume = intensity * 0.9;
        
        player.MediaEnded += (s, e) =>
        {
            player.Close();
        };
        
        player.Play();
    }
    
    // Wind is used for snow and clouds ambient
    public void SetWindVolume(double snowIntensity, double cloudOpacity)
    {
        _windPlayer.Volume = Math.Min(1.0, (snowIntensity * 0.8) + (cloudOpacity * 0.3));
    }
}
