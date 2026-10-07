using Plugin.Maui.Audio;

namespace DigitDetective;

public class AudioService
{
    private const string SoundPrefKey = "SoundOn";

    private IAudioPlayer? _musicPlayer;
    private IAudioPlayer? _oneShotPlayer;

    public bool IsSoundOn
    {
        get => Preferences.Default.Get(SoundPrefKey, true);
        set => Preferences.Default.Set(SoundPrefKey, value);
    }

    public async Task PlayOnceAsync(string fileName)
    {
        if (!IsSoundOn) return;

        try
        {
            Stream stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            _oneShotPlayer = AudioManager.Current.CreatePlayer(stream);
            _oneShotPlayer.Play();
        }
        catch (Exception) { }
    }

    public async Task SetupLoopingMusicAsync(string fileName)
    {
        try
        {
            Stream stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            _musicPlayer = AudioManager.Current.CreatePlayer(stream);
            _musicPlayer.Loop = true;

            if (IsSoundOn)
                _musicPlayer.Play();
        }
        catch (Exception) { }
    }

    public void SetMusicOn(bool on)
    {
        IsSoundOn = on;

        if (_musicPlayer is null) return;

        if (on)
            _musicPlayer.Play();
        else
            _musicPlayer.Pause();
    }

    public void PauseMusic() => _musicPlayer?.Pause();

    public void ResumeMusicIfOn()
    {
        if (IsSoundOn) _musicPlayer?.Play();
    }
}