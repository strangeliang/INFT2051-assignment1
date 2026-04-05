using Microsoft.Maui.Devices;

namespace parcel_station1.Pages;

public partial class SearchPage : ContentPage
{
    public SearchPage()
    {
        InitializeComponent();
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string code = ParcelCodeEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(code))
        {
            await ShowTopNotification("Please enter parcel code.", "#F59E0B");
            return;
        }

        await ShowTopNotification("Parcel is ready for pickup!", "#22C55E");

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(400));
        }
        catch
        {
        }

        await Task.Delay(300);
        await Navigation.PushAsync(new ResultPage());
    }

    private async Task ShowTopNotification(string message, string backgroundColor)
    {
        TopNotificationLabel.Text = message;
        TopNotification.BackgroundColor = Color.FromArgb(backgroundColor);
        TopNotification.IsVisible = true;

        TopNotification.TranslationY = -120;
        await TopNotification.TranslateTo(0, 0, 250, Easing.CubicOut);

        await Task.Delay(1800);

        await TopNotification.TranslateTo(0, -120, 250, Easing.CubicIn);
        TopNotification.IsVisible = false;
    }
}