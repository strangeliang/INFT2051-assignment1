using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using parcel_station1.Data;
using parcel_station1.Models;
using parcel_station1.Services;

namespace parcel_station1.Pages;

public partial class SearchPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
    private readonly string _username;
    private List<HistoryPreviewItem> _historyPreviewItems = new();

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
        await LoadParcelCountsAsync();
        await LoadHistoryPreviewAsync();
        await LoadLatestParcelSummaryAsync();
    }

    private async void OnSearchClicked(object sender, EventArgs e)
    {
        string code = ParcelCodeEntry.Text?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(code))
        {
            await ShowTopNotification("Please enter parcel code.", "#F59E0B");
            return;
        }

        var parcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(code, _username);

        if (parcel == null)
        {
            await ShowTopNotification("Parcel not found.", "#EF4444");
            return;
        }

        await ShowTopNotification("Parcel found successfully!", "#22C55E");

        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
        }
        catch
        {
        }

        BeepService.PlaySuccessBeep();

        await Task.Delay(250);
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

    private async void OnHistoryClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new HistoryPage(_parcelDatabase, _username));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlertAsync("Log out", "Are you sure you want to log out?", "Yes", "No");

        if (!confirm)
            return;

        await Navigation.PopAsync();
    }

    private async Task ShowTopNotification(string message, string backgroundColor)
    {
        TopNotificationLabel.Text = message;
        TopNotification.Background = new SolidColorBrush(Color.FromArgb(backgroundColor));
        TopNotification.IsVisible = true;

        TopNotification.TranslationY = -120;
        await TopNotification.TranslateToAsync(0, 0, 250, Easing.CubicOut);

        await Task.Delay(1800);

        await TopNotification.TranslateToAsync(0, -120, 250, Easing.CubicIn);
        TopNotification.IsVisible = false;
    }

    private async Task LoadParcelCountsAsync()
    {
        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        int pending = parcels.Count(p =>
            string.Equals(p.Status, "Pending", StringComparison.OrdinalIgnoreCase));

        int ready = parcels.Count(p =>
            string.Equals(p.Status, "Ready for Collection", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(p.Status, "Ready", StringComparison.OrdinalIgnoreCase));

        int collected = parcels.Count(p =>
            string.Equals(p.Status, "Collected", StringComparison.OrdinalIgnoreCase));

        PendingCountLabel.Text = pending.ToString();
        ReadyCountLabel.Text = ready.ToString();
        CollectedCountLabel.Text = collected.ToString();
    }

    private async Task LoadHistoryPreviewAsync()
    {
        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        var recentParcels = parcels
            .AsEnumerable()
            .Reverse()
            .Take(3)
            .ToList();

        _historyPreviewItems.Clear();

        foreach (var parcel in recentParcels)
        {
            _historyPreviewItems.Add(new HistoryPreviewItem
            {
                ParcelCode = $"PS{parcel.ParcelCode}",
                Status = parcel.Status ?? string.Empty,
                StatusColor = GetPreviewStatusColor(parcel.Status),
                Subtitle = GetHistorySubtitle(parcel)
            });
        }

        HistoryPreviewCollectionView.ItemsSource = null;
        HistoryPreviewCollectionView.ItemsSource = _historyPreviewItems;

        HistoryEmptyLabel.IsVisible = _historyPreviewItems.Count == 0;
        HistoryPreviewCollectionView.IsVisible = _historyPreviewItems.Count > 0;
    }

    private async Task LoadLatestParcelSummaryAsync()
    {
        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        var latestParcel = parcels
            .AsEnumerable()
            .Reverse()
            .FirstOrDefault();

        if (latestParcel == null)
        {
            LatestParcelCodeLabel.Text = "No parcels yet";
            LatestParcelStatusLabel.Text = "No recent parcel activity";
            LatestParcelTimeLabel.Text = "--";
            return;
        }

        LatestParcelCodeLabel.Text = $"PS{latestParcel.ParcelCode}";
        LatestParcelStatusLabel.Text = GetHistorySubtitle(latestParcel);
        LatestParcelTimeLabel.Text = GetLatestTimeText(latestParcel);
    }

    private string GetPreviewStatusColor(string? status)
    {
        if (string.IsNullOrWhiteSpace(status))
            return "#6B7280";

        return status.Trim().ToLower() switch
        {
            "pending" => "#A16207",
            "ready" => "#4F46E5",
            "ready for collection" => "#4F46E5",
            "collected" => "#15803D",
            _ => "#6B7280"
        };
    }

    private string GetHistorySubtitle(Parcel parcel)
    {
        if (string.IsNullOrWhiteSpace(parcel.Status))
            return "No status available";

        return parcel.Status.Trim().ToLower() switch
        {
            "collected" => $"Collected - {parcel.PickupDeadline}",
            "ready" => $"Ready for pickup - {parcel.Location}",
            "ready for collection" => $"Ready for pickup - {parcel.Location}",
            "pending" => $"Awaiting processing - {parcel.Location}",
            _ => parcel.Location ?? "Parcel updated"
        };
    }

    private string GetLatestTimeText(Parcel parcel)
    {
        if (string.IsNullOrWhiteSpace(parcel.Status))
            return "Now";

        return parcel.Status.Trim().ToLower() switch
        {
            "pending" => "Pending",
            "ready" => "Ready",
            "ready for collection" => "Ready",
            "collected" => "Done",
            _ => "Now"
        };
    }
}

public class HistoryPreviewItem
{
    public string ParcelCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = "#6B7280";
    public string Subtitle { get; set; } = string.Empty;
}