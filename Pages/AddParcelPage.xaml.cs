using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

public partial class AddParcelPage : ContentPage
{
    private readonly ParcelDatabase _parcelDatabase;
    private readonly string _username;

    public AddParcelPage(ParcelDatabase parcelDatabase, string username)
    {
        InitializeComponent();
        _parcelDatabase = parcelDatabase;
        _username = username;
    }

    private async void OnSaveParcelClicked(object sender, EventArgs e)
    {
        string parcelCode = ParcelCodeEntry.Text?.Trim() ?? "";
        string status = StatusEntry.Text?.Trim() ?? "";
        string location = LocationEntry.Text?.Trim() ?? "";
        string collectionCode = CollectionCodeEntry.Text?.Trim() ?? "";
        string pickupDeadline = PickupDeadlineEntry.Text?.Trim() ?? "";

        if (string.IsNullOrWhiteSpace(parcelCode) ||
            string.IsNullOrWhiteSpace(status) ||
            string.IsNullOrWhiteSpace(location) ||
            string.IsNullOrWhiteSpace(collectionCode) ||
            string.IsNullOrWhiteSpace(pickupDeadline))
        {
            await DisplayAlertAsync("Warning", "Please fill in all parcel details.", "OK");
            return;
        }

        var existingParcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(parcelCode, _username);

        if (existingParcel != null)
        {
            await DisplayAlertAsync("Warning", "This parcel code already exists for this user.", "OK");
            return;
        }

        var parcel = new Parcel
        {
            Username = _username,
            ParcelCode = parcelCode,
            Status = status,
            Location = location,
            CollectionCode = collectionCode,
            PickupDeadline = pickupDeadline
        };

        await _parcelDatabase.SaveParcelAsync(parcel);

        await DisplayAlertAsync("Success", "Parcel saved successfully.", "OK");
        await Navigation.PopAsync();
    }
}