using System.Collections.ObjectModel;
using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class HistoryPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
    private readonly string _username;

    public ObservableCollection<ParcelHistoryItem> ParcelHistory { get; set; } = new();

    public HistoryPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
        _username = username;

        HistoryCollectionView.ItemsSource = ParcelHistory;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        HistoryWelcomeLabel.Text = $"Parcel history for {_username}";

        await _parcelDatabase.InitAsync();
        await LoadHistoryAsync();
    }

    private async Task LoadHistoryAsync()
    {
        ParcelHistory.Clear();

        var parcels = await _parcelDatabase.GetParcelsByUsernameAsync(_username);

        var orderedParcels = parcels.ToList();
        orderedParcels.Reverse();

        foreach (var parcel in orderedParcels)
        {
            ParcelHistory.Add(new ParcelHistoryItem
            {
                ParcelCode = $"Parcel Code: {parcel.ParcelCode}",
                Status = parcel.Status ?? "",
                StatusColor = GetStatusColor(parcel.Status),
                LocationDisplay = $"Location: {parcel.Location}",
                PickupDeadlineDisplay = $"Deadline: {parcel.PickupDeadline}",
                CollectionCodeDisplay = $"Collection Code: {parcel.CollectionCode}"
            });
        }

        EmptyStateFrame.IsVisible = ParcelHistory.Count == 0;
        HistoryCollectionView.IsVisible = ParcelHistory.Count > 0;
    }

    private string GetStatusColor(string? status)
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

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}

public class ParcelHistoryItem
{
    public string ParcelCode { get; set; } = "";
    public string Status { get; set; } = "";
    public string StatusColor { get; set; } = "#6B7280";
    public string LocationDisplay { get; set; } = "";
    public string PickupDeadlineDisplay { get; set; } = "";
    public string CollectionCodeDisplay { get; set; } = "";
}