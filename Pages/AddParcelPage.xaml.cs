using parcel_station1.Data;
using parcel_station1.Models;

namespace parcel_station1.Pages;

// AddParcelPage allows the current user to add a new parcel record.
public partial class AddParcelPage : ContentPage
{
    // Database object used to save and retrieve parcel data.
    private readonly ParcelDatabase _parcelDatabase;

    // Stores the currently logged-in username so the parcel can be linked to that user.
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

        // Validate that all parcel details have been entered.
        if (string.IsNullOrWhiteSpace(parcelCode) ||
            string.IsNullOrWhiteSpace(status) ||
            string.IsNullOrWhiteSpace(location) ||
            string.IsNullOrWhiteSpace(collectionCode) ||
            string.IsNullOrWhiteSpace(pickupDeadline))
        {
            await DisplayAlertAsync("Warning", "Please fill in all parcel details.", "OK");
            return;
        }

        // Prevent duplicate parcel codes for the same user.
        var existingParcel = await _parcelDatabase.GetParcelByCodeAndUsernameAsync(parcelCode, _username);

        if (existingParcel != null)
        {
            await DisplayAlertAsync("Warning", "This parcel code already exists for this user.", "OK");
            return;
        }

        // Create a new parcel object using the entered values.
        var parcel = new Parcel
        {
            Username = _username,
            ParcelCode = parcelCode,
            Status = status,
            Location = location,
            CollectionCode = collectionCode,
            PickupDeadline = pickupDeadline
        };

        // Save the parcel into the local SQLite database.
        await _parcelDatabase.SaveParcelAsync(parcel);

        await DisplayAlertAsync("Success", "Parcel saved successfully.", "OK");

        // Return to the previous page so the dashboard can refresh its parcel data.
        await Navigation.PopAsync();
    }
}