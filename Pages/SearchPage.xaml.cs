using Microsoft.Maui.Devices;
using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class SearchPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
    private readonly string _username;

    public SearchPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
        _username = username;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        WelcomeLabel.Text = $"Welcome back, {_username}";

        await _parcelDatabase.InitAsync();

        // 先把公共测试数据关掉，避免所有用户共用
        // await SeedTestParcelsAsync();

        await LoadParcelCountsAsync();
    }

    // =========================
    // Button events
    // =========================

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string code = ParcelCodeEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(code))
        {
            await ShowTopNotification("Please enter parcel code.", "#F59E0B");
            return;
        }

        // 只查当前用户自己的包裹
        var parcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(code, _username);

        if (parcel == null)
        {
            await ShowTopNotification("Parcel not found.", "#EF4444");
            return;
        }

        await ShowTopNotification("Parcel found successfully!", "#22C55E");

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(400));
        }
        catch
        {
        }

        await Task.Delay(300);
        await Navigation.PushAsync(new ResultPage(parcel));
    }

    private async void OnAddParcelClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AddParcelPage(_parcelDatabase, _username));
    }

    private async void OnScanQrClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ScanPage(_parcelDatabase, _username));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Log out", "Are you sure you want to log out?", "Yes", "No");

        if (!confirm)
            return;

        await Navigation.PopAsync();
    }

    // =========================
    // Helper methods
    // =========================

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

    // 如果以后还想保留演示数据，可以再用这个方法
    private async Task SeedTestParcelsAsync()
    {
        var allParcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        if (allParcels.Count > 0)
            return;

        await _parcelDatabase.SaveParcelAsync(new Parcel
        {
            Username = _username,
            ParcelCode = "1234",
            Status = "Ready for Collection",
            Location = "Locker A-12",
            CollectionCode = "483920",
            PickupDeadline = "12 March 2026"
        });

        await _parcelDatabase.SaveParcelAsync(new Parcel
        {
            Username = _username,
            ParcelCode = "5678",
            Status = "Pending",
            Location = "Locker B-03",
            CollectionCode = "715204",
            PickupDeadline = "15 March 2026"
        });

        await _parcelDatabase.SaveParcelAsync(new Parcel
        {
            Username = _username,
            ParcelCode = "9999",
            Status = "Collected",
            Location = "Locker C-07",
            CollectionCode = "000000",
            PickupDeadline = "Completed"
        });
    }

    private async Task LoadParcelCountsAsync()
    {
        // 只统计当前用户自己的包裹
        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        int pending = parcels.Count(p => p.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase));
        int ready = parcels.Count(p => p.Status.Equals("Ready for Collection", StringComparison.OrdinalIgnoreCase)
                                    || p.Status.Equals("Ready", StringComparison.OrdinalIgnoreCase));
        int collected = parcels.Count(p => p.Status.Equals("Collected", StringComparison.OrdinalIgnoreCase));

        PendingCountLabel.Text = pending.ToString();
        ReadyCountLabel.Text = ready.ToString();
        CollectedCountLabel.Text = collected.ToString();
    }
}