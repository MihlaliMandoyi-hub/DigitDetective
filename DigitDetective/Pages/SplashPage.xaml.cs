// Group X | Surname, FirstName, StudentNumber | Surname, FirstName, StudentNumber | ...
using Plugin.Maui.Audio;

namespace DigitDetective;

public partial class SplashPage : ContentPage
{
    private readonly Random _rand = new();
    private readonly Color[] _palette =
    {
        Color.FromArgb("#FFD23F"),
        Color.FromArgb("#2EE6C5"),
        Color.FromArgb("#FF5D73"),
        Colors.White
    };

    private readonly AudioService _audio;
    private IDispatcherTimer _spawnTimer;

    public SplashPage(AudioService audio)
    {
        InitializeComponent();
        _audio = audio;

        _spawnTimer = Dispatcher.CreateTimer();
        _spawnTimer.Interval = TimeSpan.FromMilliseconds(180);
        _spawnTimer.Tick += (s, e) => _ = FloatDigitAsync();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await Task.Delay(250);
        _spawnTimer.Start();
        _ = _audio.PlayOnceAsync("chime.wav");

        await Task.WhenAll(
            TitleLabel.FadeTo(1, 700),
            TitleLabel.ScaleTo(1, 700, Easing.SpringOut));

        await StartButton.FadeTo(1, 500);
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _spawnTimer.Stop();
    }

    private async Task FloatDigitAsync()
    {
        double w = FloatingLayer.Width;
        double h = FloatingLayer.Height;
        if (w <= 0 || h <= 0 || FloatingLayer.Children.Count > 40) return;

        var digit = new Label
        {
            Text = _rand.Next(0, 10).ToString(),
            FontSize = _rand.Next(24, 64),
            FontAttributes = FontAttributes.Bold,
            TextColor = _palette[_rand.Next(_palette.Length)],
            Opacity = 0
        };

        AbsoluteLayout.SetLayoutBounds(digit,
            new Rect(_rand.NextDouble() * (w - 40), h,
                     AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
        FloatingLayer.Children.Add(digit);

        uint duration = (uint)_rand.Next(4000, 7000);

        async Task FadeInOut()
        {
            await digit.FadeTo(0.6, duration / 4);
            await digit.FadeTo(0, duration * 3 / 4);
        }

        await Task.WhenAll(
            digit.TranslateTo(0, -(h + 80), duration, Easing.Linear),
            digit.RotateTo(_rand.Next(-70, 70), duration),
            FadeInOut());

        FloatingLayer.Children.Remove(digit);
    }

    private async void OnStartClicked(object? sender, EventArgs e)
    {
        _spawnTimer.Stop();
        await Shell.Current.GoToAsync("//MainPage");
    }
}